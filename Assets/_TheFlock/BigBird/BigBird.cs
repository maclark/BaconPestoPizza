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
	private SpriteRenderer sr;
	private Component[] dockTransforms;
	private Quaternion targetRotation = Quaternion.identity;
	private Color color;
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
		sr = GetComponent<SpriteRenderer> ();
		pump = GetComponentInChildren<Pump> ();
		medkit = GetComponentInChildren<Medkit> ();
		hold = GetComponentInChildren<CargoHold> ();
		thrusters = GetComponentsInChildren<Thruster> ();

		color = sr.color;
	}

	void Start () {
		gm.AddAlliedTransform (transform);
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

		if (bbm.absorbing) {
			bbm.Absorb ();
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
		} 


	}

	void OnCollisionStay2D (Collision2D coll) {
		if (coll.gameObject.GetComponent<Item> ()) {
			hold.Load (coll.transform);
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
			if (k.bird && k.bird.rider == null) {
				float d = Vector3.Distance (k.transform.position, playerPos);
				if (d < closestDockDistance) {
					closestDock = k;
					closestDockDistance = d;
				}
			}
		}
		return closestDock.bird;
	}

	public Bird GetUnboardedBird () {
		dockTransforms = GetComponentsInChildren<Dock> ();
		foreach (Dock k in dockTransforms) {
			if (k.bird) {
				if (!k.bird.rider) {
					return k.bird;
				}
			}
		}
		return null;
	}


	public void TakeDamage (int dam) {
		if (bbm.shieldUp) {
			bbm.energyTank.DecreaseResource (dam);
			StartCoroutine (Flash (.1f, Color.yellow));
			if (bbm.energyTank.empty) {
				bbm.SolarShieldDown ();
			}
		}
	}

	public IEnumerator Flash (float flashLength, Color c) {
		if (sr.color == c) {
			yield break;
		}
		sr.color = c;
		yield return new WaitForSeconds (flashLength);
		sr.color = color;
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
		bbm.GasOut (halfSecFill);
		if (bbm.waterTank.empty) {
			yield break;
		}
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
		if (nearestPad.hasRamp) {
			nearestPad.ExtendRamp ();
		}
		LandingApproach ();
		nearestPad.occupant = this.transform;	
	}

	public void LiftOff () {
		gm.AddAlliedTransform (transform);
		Camera.main.GetComponent <CameraFollow> ().followBigBird = true;
		landing = false;
		rb.isKinematic = false;
		TurnEngineOn ();
		if (nearestPad.hasRamp) {
			nearestPad.WithdrawRamp ();
		}
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
		if (Mathf.Abs (transform.position.x - nearestPad.transform.position.x) < landingDelta &&
			Mathf.Abs (transform.position.y - nearestPad.transform.position.y) < landingDelta &&
			Mathf.Abs (transform.position.z - nearestPad.transform.position.z) < landingDelta) 
		{
			landing = false;
			landed = true;
			gm.RemoveAlliedTransform (transform);
			Camera.main.GetComponent <CameraFollow> ().followBigBird = false;
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
