using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BigBird : MonoBehaviour {

	public float forceMag = 75f;
	public int hp = 1000;
	public float waterTankCapacity = 10000f;
	public float sweatRate = 10f;
	public float drinkRate = 100f;
	public bool drinking = false;
	public int turn = 0;
	public float turnSpeed = 200f;
	public float halfSecFill = 5f;
	public int halfSecRepair = 5;
	public int gold = 0;
	public LandingPad nearestPad = null;
	public float landedScale = 15f;
	public CargoHold hold;
	public GameObject cannonballPrefab;
	public List<Transform> rightCans;
	public List<Transform> leftCans;

	private GameManager gm;
	private Rigidbody2D rb;
	private Component[] dockTransforms;
	private Quaternion targetRotation = Quaternion.identity;
	private bool engineOn = false;
	private bool landed = false;
	private ResourceBar waterTank;
	private ResourceBar energyTank;
	private float water;
	private Bird birdGettingRepairs = null;
	private Bird birdGettingGas = null;
	private Transform target = null;
	private Pump pump;
	private Medkit medkit;
	private WaterSource localWater;
	private Thruster[] thrusters;
	private List<Bird> dockedBirds = new List<Bird> ();
	private List<Bird> gasLine = new List<Bird> ();
	private List<Bird> repairLine = new List<Bird> ();


	void Awake() {
		gm = GameObject.FindObjectOfType<GameManager> ();
		rb = GetComponent<Rigidbody2D> ();
		pump = GetComponentInChildren<Pump> ();
		medkit = GetComponentInChildren<Medkit> ();
		hold = GetComponentInChildren<CargoHold> ();
		thrusters = GetComponentsInChildren<Thruster> ();
		waterTank = gm.GetBigBirdWaterTank ();
		energyTank = gm.GetBigBirdEnergyTank ();
	}

	void Start () {
		waterTank.capacity = waterTankCapacity;
		waterTank.SetResource (waterTankCapacity);
		energyTank.capacity = hp;
		waterTank.current = hp;
		energyTank.current = hp;
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
			Sweat ();
			rb.AddForce (transform.up * forceMag);
		}

		if (turn != 0 && !landed) {
			rb.AddTorque (turnSpeed * turn);

			if (engineOn) {
				//SetCamera ();
			}
		}

		if (drinking) {
			Drink ();
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
		else if (other.name == "LandingPad") {
			nearestPad = other.GetComponent<LandingPad> ();
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

		if (coll.transform.tag == "Enemy") {
			TakeDamage (coll.transform.GetComponent<Flyer> ().kamikazeDamage);
			coll.transform.gameObject.GetComponent<Flyer> ().Die ();
		} else if (coll.gameObject.tag == "Harpoonable") {
			if (coll.gameObject.GetComponent<Cargo> ()) {
				hold.Load (coll.transform);
			}
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (other.name == "LandingPad") {
			if (nearestPad == other.GetComponent<LandingPad> ()) {
				nearestPad = null;
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
		hp -= dam;
		energyTank.SetResource (hp);
		if (hp <= 0) {
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

		if (birdGettingRepairs == null) {
			if (b.health < b.maxHealth) {
				StartCoroutine (RepairBird (b));
			}
		} else {
			repairLine.Add (b);
		}
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

	IEnumerator RepairBird (Bird b) {
		if (b == null) {
			if (repairLine.Count > 0) {
				Bird nextInLine = repairLine [repairLine.Count - 1];
				repairLine.Remove (repairLine [repairLine.Count - 1]);
				StartCoroutine (RepairBird (nextInLine));
			} else {
				medkit.Reset ();
				yield break;
			}
		}

		birdGettingRepairs = b;
		medkit.transform.position = b.transform.position;
		yield return new WaitForSeconds (.5f);
		b.health += halfSecRepair;
		if (b.health > b.maxHealth) {
			b.health = b.maxHealth;
			if (repairLine.Count > 0) {
				Bird nextInLine = repairLine [repairLine.Count - 1];
				gasLine.Remove (repairLine [repairLine.Count - 1]);
				StartCoroutine (RepairBird (nextInLine));
			} else {
				birdGettingRepairs = null;
				medkit.Reset ();
			}
		} else {
			StartCoroutine (RepairBird (birdGettingRepairs));
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
		landed = true;
		TurnEngineOff ();
		rb.Sleep ();
		StartCoroutine (LandingApproach (nearestPad));
		nearestPad.occupant = this.transform;

		//rb.velocity = Vector3.zero;
		//rb.angularVelocity = 0f;
	
	}

	public void LiftOff () {
		TurnEngineOn ();
		transform.parent = null;
		nearestPad.occupant = null;
		landed = false;
	}

	IEnumerator LandingApproach (LandingPad pad) {
		yield return new WaitForSeconds (1f);
		//transform.localScale = new Vector3 (landedScale, landedScale, 1);
		transform.position = pad.transform.position;//+ pad.landingMarkOffset;
		transform.rotation = Quaternion.LookRotation (transform.forward, pad.transform.up);
		transform.parent = pad.transform;
		Player[] players = GetComponentsInChildren<Player> ();
		for (int i = 0; i < players.Length; i++) {
			if (players [i].isPlaying) {
				print (players [i].name + " is playing");
				players [i].Disembark (nearestPad.disembarkPoint);
			}
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

	public void AtWaterSource (WaterSource s) {
		drinking = true;
		localWater = s;
	}

	public void LeftWaterSource (WaterSource s) {
		if (localWater == s) {
			drinking = false;
			localWater = null;
		}
	}

	public void Drink () {
		if (waterTank.full || localWater.dry) {
			return;
		} else {
			float thisGulp;
			if (localWater.gallons < drinkRate) {
				thisGulp = localWater.gallons;
			} else {
				thisGulp = drinkRate;
			}
			waterTank.IncreaseResource (localWater.Gulp (thisGulp));
		}
	}

	public void Sweat () {
		waterTank.DecreaseResource (sweatRate);

		if (waterTank.empty) {
			Dehydrated ();
		}
	}

	void Dehydrated () {
		Debug.Log ("do dehydration");
	}
}
