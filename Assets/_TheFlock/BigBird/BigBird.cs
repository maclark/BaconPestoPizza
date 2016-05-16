using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BigBird : MonoBehaviour {

	public float accelerationMagnitude = 75f;
	public int hp = 1000;
	public float thresholdToTurnBigBird = .2f;
	public bool turning { get; set; }
	public float rotateSpeed = .3f;
	public float halfSecFill = 5f;
	public int halfSecRepair = 5;

	private Rigidbody2D rb;
	private Component[] dockTransforms;
	private Quaternion targetRotation = Quaternion.identity;
	private bool engineOn = false;
	private Bird birdGettingRepairs = null;
	private Bird birdGettingGas = null;
	private Thruster[] thrusters;
	private List<Bird> dockedBirds = new List<Bird> ();
	private List<Bird> gasLine = new List<Bird> ();
	private List<Bird> repairLine = new List<Bird> ();


	void Awake() {
		rb = GetComponent<Rigidbody2D> ();
	}

	void Start () {
		thrusters = GetComponentsInChildren<Thruster> ();
	}

	void FixedUpdate () {
		if (engineOn) {
			rb.AddForce (transform.up * accelerationMagnitude * rb.mass);
		}

		if (turning) {
			transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "EnemyBullet") {
			if (Random.Range (0, 1f) > .5) {
				other.GetComponent<Bullet>().Die();
			}
		} 
		else if (other.tag == "Enemy") {
			TakeDamage (other.GetComponent<Enemy> ().kamikazeDamage);
			Destroy (other.gameObject);
		}
		else if (other.name == "Web") {
			TurnEngineOn ();
			accelerationMagnitude = 10 * accelerationMagnitude;
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
		if (hp <= 0) {
			Die ();
		}
	}

	void Die() {
		//gm.RemoveAlliedTransform (transform);
		GetComponent<SpriteRenderer> ().color = Color.red;
		print ("big bird dead, need to recursively call children's Die()");
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


	public void DockBird (Bird b) {
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

	public void UndockBird (Bird b) {
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
				gasLine.Remove (repairLine [repairLine.Count - 1]);
				StartCoroutine (RepairBird (nextInLine));
			} else yield break;
		}

		birdGettingRepairs = b;
		b.health += halfSecRepair;
		yield return new WaitForSeconds (.5f);
		if (b.health > b.maxHealth) {
			b.health = b.maxHealth;
			if (repairLine.Count > 0) {
				Bird nextInLine = repairLine [repairLine.Count - 1];
				gasLine.Remove (repairLine [repairLine.Count - 1]);
				StartCoroutine (RepairBird (nextInLine));
			} else {
				birdGettingRepairs = null;
			}
		} else {
			StartCoroutine (RepairBird (birdGettingRepairs));
		}
	}

	IEnumerator FillBirdTank (Bird b) {
		if (b == null) {
			yield break;
		}
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
			}
		} else {
			StartCoroutine (FillBirdTank (birdGettingGas));
		}
	}
}
