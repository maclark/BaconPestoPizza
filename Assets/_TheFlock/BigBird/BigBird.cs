using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BigBird : MonoBehaviour {

	public float forceMag = 75f;
	public int turn = 0;
	public float turnSpeed = 200f;
	public float landingSpeed = 1f;
	public float landingRotate = 1f;
	public float landingDelta = .1f;
	public float halfSecFill = 5f;
	public LandingPad nearestPad = null;
	public CargoHold hold;
	public GameObject cannonballPrefab;
	public List<Transform> rightCans;
	public List<Transform> leftCans;



	public bool landing = false;



	private GameManager gm;
	private BigBirdManager bbm;
	private Rigidbody2D rb;
	private Component[] dockTransforms;
	private Quaternion targetRotation = Quaternion.identity;
	private bool engineOn = false;
	private bool landed = false;

	private Bird birdGettingGas = null;
	private Transform target = null;
	private Pump pump;
	private Medkit medkit;
	private Thruster[] thrusters;
	private List<Bird> dockedBirds = new List<Bird> ();
	private List<Bird> gasLine = new List<Bird> ();


	void Awake() {
		gm = GameObject.FindObjectOfType<GameManager> ();
		bbm = GetComponent<BigBirdManager> ();
		rb = GetComponent<Rigidbody2D> ();
		pump = GetComponentInChildren<Pump> ();
		medkit = GetComponentInChildren<Medkit> ();
		hold = GetComponentInChildren<CargoHold> ();
		thrusters = GetComponentsInChildren<Thruster> ();
	}

	void Update () {
		if (landing) {
			LandingApproach ();
		}
	}

	void FixedUpdate () {
		if (engineOn) {
			if (target) {
				Quaternion r = Quaternion.LookRotation (Vector3.forward, target.position - transform.position);
				transform.rotation = Quaternion.RotateTowards (transform.rotation, r, turnSpeed);	
				//TODO make this torque based to use physics engine collisions
				//rb.AddTorque (rotateSpeed * 1000);
				if (transform.position == target.position) {
					target = null;
					TurnEngineOff ();
				}
			}
			bbm.Sweat ();
			rb.AddForce (transform.up * forceMag);
		}

		if (turn != 0 && !landed) {
			rb.AddTorque (turnSpeed * turn);
		}

		if (bbm.drinking) {
			bbm.Drink ();
		}

		SetCamera ();

	}

	void OnTriggerEnter2D (Collider2D other) {
		if (target) {
			if (other.transform == target.transform) {
				target = null;
				TurnEngineOff ();
				SetCamera ();
			}
		}
		if (other.tag == "EnemyBullet") {
			if (Random.Range (0, 1f) > .5) {
				TakeDamage (other.GetComponent<Projectile> ().damage);
				other.GetComponent<Projectile> ().Die ();
			}
		} 
		else if (other.tag == "Enemy") {
			//TakeDamage (other.GetComponent<Flyer> ().kamikazeDamage);
			//other.gameObject.GetComponent<Flyer> ().Die ();
		}
		else if (other.name == "Web") {
			TurnEngineOn ();
			forceMag = 10 * forceMag;
		}

	}

	void OnCollisionEnter2D (Collision2D coll) {
		if (target) {
			if (coll.transform == target.transform) {
				target = null;
				TurnEngineOff ();
				SetCamera ();
			}
		}

		if (coll.transform.tag == "FlyerEnemy") {
			TakeDamage (coll.transform.GetComponent<Flyer> ().kamikazeDamage);
			coll.transform.gameObject.GetComponent<Flyer> ().Die ();
		} else if (coll.gameObject.tag == "Harpoonable") {
			if (coll.gameObject.GetComponent<Item> ()) {
				hold.Load (coll.transform);
			}
		}
	}



	/// <summary>
	/// MAKE SURE TO CHEK IF RETURED NULL DOCK. Probably should use bool return type with an out dock variable.
	/// </summary>
	/// <returns>The nearest open dock.</returns>
	/// <param name="dockingShipPos">Docking ship position.</param>
	public Dock GetNearestOpenDock (Vector3 dockingShipPos) {
		dockTransforms = GetComponentsInChildren<Dock> ();
		float closestDockDistance = 10000f;
		Dock closestDock = null;
		foreach (Dock k in dockTransforms) {
			if (!k.bird) {
				float d = Vector3.Distance (k.transform.position, dockingShipPos);
				if (d < closestDockDistance) {
					closestDock = k;
					closestDockDistance = d;
				}
			}
		}
		return closestDock;
	}

	/// <summary>
	/// Gets the nearest docked and unboarded bird. CHECK IF NULL.
	/// </summary>
	/// <returns>The nearest docked and unboarded bird.</returns>
	/// <param name="playerPos">Player position.</param>
	public Bird GetNearestFreeBird (Vector3 playerPos) {
		dockTransforms = GetComponentsInChildren<Dock> ();
		float closestDockDistance = 10000f;
		Dock closestDock = null;
		foreach (Dock k in dockTransforms) {
			if (k.bird && k.bird.p == null) {
				float d = Vector3.Distance (k.transform.position, playerPos);
				if (d < closestDockDistance) {
					closestDock = k;
					closestDockDistance = d;
				}
			}
		}
		return closestDock.bird;
	}

	public void TakeDamage( int dam) {
		bbm.hp -= dam;
		bbm.energyTank.SetResource (bbm.hp);
		if (bbm.hp <= 0) {
			Die ();
		}
	}

	void Die() {
		//gm.RemoveAlliedTransform (transform);
		GetComponent<SpriteRenderer> ().color = Color.red;
		//TODO print ("big bird dead, need to recursively call children's Die()");
	}

	public void SetTargetRotationZAngle (float zAngle) {
		targetRotation.eulerAngles = new Vector3( 0, 0, zAngle);
	}

	public void TurnEngineOn () {
		engineOn = true;
		foreach (Thruster thrust in thrusters) {
			thrust.GetComponent<SpriteRenderer> ().enabled = true;
		}
		SetCamera ();
	}

	public void TurnEngineOff () {
		engineOn = false;
		foreach (Thruster thrust in thrusters) {
			thrust.GetComponent<SpriteRenderer> ().enabled = false;
		}
		SetCamera ();
	}

	public bool GetEngineOn () {
		return engineOn;
	}


	public void AddToDockedBirds (Bird b) {
		dockedBirds.Add (b);

		if (birdGettingGas == null) {
			StartCoroutine (FillBirdTank (b));
		} else {
			gasLine.Add (b);
		}

		medkit.FindHurtBird ();
	}

	public void RemoveFromDockedBirds (Bird b) {
		dockedBirds.Remove (b);
		if (birdGettingGas == b) {
			birdGettingGas = null;
		}
		if (gasLine.Contains (b)) {
			gasLine.Remove (b);
		}
	}

	IEnumerator FillBirdTank (Bird b) {
		if (b == null) {
			pump.RecoilHose ();
			yield break;
		}
		pump.MoveToBirdPos (b.transform.position);
		birdGettingGas = b;
		b.gas += halfSecFill;
		b.ranOutOfGas = false;
		b.forceMag = b.hydratedForceMag;
		yield return new WaitForSeconds (.5f);
		if (b.gas > b.fullTank) {
			b.gas = b.fullTank;
			if (gasLine.Count > 0) {
				Bird nextInLine = gasLine [gasLine.Count - 1];
				gasLine.Remove (gasLine [gasLine.Count - 1]);
				StartCoroutine (FillBirdTank (nextInLine));
			} else {
				birdGettingGas = null;
				pump.RecoilHose ();
			}
		} else {
			StartCoroutine (FillBirdTank (birdGettingGas));
		}
	}

	public void BigDock () {
		TurnEngineOff ();
		landing = true;
		nearestPad.ExtendRamp ();
		LandingApproach ();
		nearestPad.occupant = this.transform;	
	}

	public void LiftOff () {
		landing = false;
		rb.isKinematic = false;
		TurnEngineOn ();
		nearestPad.WithdrawRamp ();
		transform.parent = null;
		nearestPad.occupant = null;
		landed = false;
	}

	public void LandingApproach () {
		if (!nearestPad) {
			Debug.Log ("landing on null pad");
			LiftOff ();
			return;
		}

		Vector3 translationDirection = nearestPad.transform.position - transform.position;
		translationDirection.Normalize ();
		transform.Translate (translationDirection * landingSpeed * Time.deltaTime);
		transform.rotation = Quaternion.RotateTowards (transform.rotation, nearestPad.transform.rotation, landingRotate);
		if (transform.position.x - nearestPad.transform.position.x < landingDelta &&
			transform.position.y - nearestPad.transform.position.y < landingDelta &&
			transform.position.z - nearestPad.transform.position.z < landingDelta) 
		{
			landing = false;
			landed = true;
			transform.parent = nearestPad.transform;
			transform.rotation = nearestPad.transform.rotation;
			rb.velocity = Vector3.zero;
			rb.angularVelocity = 0f;
			rb.isKinematic = true;
		}
	}

	public bool Landed {
		get {
			return landed;
		}
	}

	public void SetTarget (NavPointer nav) {
		if (nav.target) {
			if (nav.target == transform) {
				target = null;
				TurnEngineOff ();
				SetCamera ();
				return;
			}

			if (nav.target.parent) {
				if (nav.target.parent.transform == transform) {
					target = null;
					TurnEngineOff ();
					SetCamera ();
					return;
				}
			}

			target = nav.target;
			TurnEngineOn ();
			SetCamera ();
		} else {
			gm.invisibleTarget.transform.position = nav.transform.position;
			target = gm.invisibleTarget.transform;
			TurnEngineOn ();
			SetCamera ();
		}
	}

	void SetCamera () {
		Camera.main.GetComponent<CameraFollow> ().offsetDir = transform.up;//DirectionOfTravel ();
	}

	public Vector3 DirectionOfTravel () {
		Vector3 dot;
		if (target) {
			dot = target.position - transform.position;
			dot.Normalize ();
		} else if (engineOn) {
			dot = transform.up;
		} else
			dot = Vector3.zero;
	
		return dot;
	}
}
