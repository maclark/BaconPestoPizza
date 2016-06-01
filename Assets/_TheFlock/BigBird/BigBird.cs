using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BigBird : MonoBehaviour {

	public float forceMag = 75f;
	public int hp = 1000;
	public float thresholdToTurnBigBird = .2f;
	public bool turning { get; set; }
	public float rotateSpeed = .3f;
	public float halfSecFill = 5f;
	public int halfSecRepair = 5;
	public int gold = 0;
	public LandingPad nearestPad = null;
	public float landedScale = 15f;

	private GameManager gm;
	private Rigidbody2D rb;
	private Component[] dockTransforms;
	private Quaternion targetRotation = Quaternion.identity;
	private float startScale;
	private bool engineOn = false;
	private bool landed = false;
	private HealthBar healthBar;
	private Bird birdGettingRepairs = null;
	private Bird birdGettingGas = null;
	private Transform target = null;
	private Pump pump;
	private Medkit medkit;
	private Thruster[] thrusters;
	private List<Bird> dockedBirds = new List<Bird> ();
	private List<Bird> gasLine = new List<Bird> ();
	private List<Bird> repairLine = new List<Bird> ();

	void Awake() {
		gm = GameObject.FindObjectOfType<GameManager> ();
		rb = GetComponent<Rigidbody2D> ();
		pump = GetComponentInChildren<Pump> ();
		medkit = GetComponentInChildren<Medkit> ();
		thrusters = GetComponentsInChildren<Thruster> ();
		startScale = transform.localScale.x;
		healthBar = gm.GetBigBirdHealthBar ();
	}

	void Start () {
		healthBar.max = hp;
		healthBar.current = hp;
	}

	void FixedUpdate () {
		if (engineOn) {
			if (target) {
				Quaternion r = Quaternion.LookRotation (Vector3.forward, target.position - transform.position);
				transform.rotation = Quaternion.RotateTowards (transform.rotation, r, rotateSpeed);	
				//TODO make this torque based to use physics engine collisions
				//rb.AddTorque (rotateSpeed * 1000);
				if (transform.position == target.position) {
					target = null;
					TurnEngineOff ();
				}
			}
			rb.AddForce (transform.up * forceMag);
		}


		if (turning && !landed) {
			transform.rotation = Quaternion.RotateTowards (transform.rotation, targetRotation, rotateSpeed);
			rb.angularVelocity = 0f;
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (target) {
			if (other.transform == target.transform) {
				target = null;
				TurnEngineOff ();
			}
		}

		if (other.tag == "EnemyBullet") {
			if (Random.Range (0, 1f) > .5) {
				TakeDamage (other.GetComponent<Bullet> ().damage);
				other.GetComponent<Bullet> ().Die ();
			}
		} 
		else if (other.tag == "Enemy") {
			TakeDamage (other.GetComponent<Flyer> ().kamikazeDamage);
			other.gameObject.GetComponent<Flyer> ().Die ();
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
				print ("collided  with target");
				target = null;
				TurnEngineOff ();
			}
		}

		if (coll.gameObject.tag == "Harpoonable") {
			if (coll.gameObject.GetComponent<Harpoonable> ().isGold) {
				gold++;
				gm.goldText.text = gold.ToString ();
				coll.transform.GetComponent<Harpoonable> ().Die ();
			}
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (other.name == "LandingPad") {
			if (nearestPad == other.GetComponent<LandingPad> ()) {
				nearestPad = null;
				print ("turning off bc of landing pad? lol");
				TurnEngineOff ();
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

	void TakeDamage( int dam) {
		hp -= dam;
		healthBar.AdjustHealth (hp);
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
	}

	public void TurnEngineOff () {
		engineOn = false;
		foreach (Thruster thrust in thrusters) {
			thrust.GetComponent<SpriteRenderer> ().enabled = false;
		}
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
				medkit.transform.position = transform.position;
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
				medkit.transform.position = transform.position;
			}
		} else {
			StartCoroutine (RepairBird (birdGettingRepairs));
		}
	}

	IEnumerator FillBirdTank (Bird b) {
		if (b == null) {
			pump.ResetHose ();
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
				pump.ResetHose ();
			}
		} else {
			StartCoroutine (FillBirdTank (birdGettingGas));
		}
	}

	public void SetDown (LandingPad pad) {
		landed = true;
		TurnEngineOff ();

		StartCoroutine (LandingApproach (pad));

		rb.velocity = Vector3.zero;
		rb.angularVelocity = 0f;
		rb.Sleep ();
	}

	public void LiftOff () {
		TurnEngineOn ();
		transform.parent = null;
		transform.localScale = new Vector3 (startScale, startScale, 1);
		rb.WakeUp ();
		landed = false;
	}

	IEnumerator LandingApproach (LandingPad pad) {
		yield return new WaitForSeconds (1f);
		transform.localScale = new Vector3 (landedScale, landedScale, 1);
		transform.position = pad.transform.position + pad.landingMarkOffset;
		transform.rotation = pad.transform.rotation;
		transform.parent = pad.transform;
	}

	public bool Landed {
		get {
			return landed;
		}
	}

	public void SetTarget (NavPointer nav) {
		if (nav.target) {
			if (nav.target != transform) {
				target = nav.target;
			} else
				target = null;
		} else {
			gm.invisibleTarget.transform.position = nav.transform.position;
			target = gm.invisibleTarget.transform;
		}
		TurnEngineOn ();
	}
}
