﻿using UnityEngine;
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
	public int gold = 0;
	public LandingPad nearestPad = null;

	private GameManager gm;
	private Rigidbody2D rb;
	private Component[] dockTransforms;
	private Quaternion targetRotation = Quaternion.identity;
	private bool engineOn = false;
	private bool landed = false;
	private Bird birdGettingRepairs = null;
	private Bird birdGettingGas = null;
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
	}

	void Start () {
	}

	void FixedUpdate () {
		if (engineOn) {
			rb.AddForce (transform.up * accelerationMagnitude * rb.mass);
		}

		if (turning && !landed) {
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
		else if (other.name == "LandingPad") {
			nearestPad = other.GetComponent<LandingPad> ();
		}
	}

	void OnCollisionEnter2D (Collision2D coll) {
		print (coll.gameObject.name);
		if (coll.gameObject.name == "Gold") {
			gold++;
			gm.goldText.text = gold.ToString ();
			Destroy (coll.gameObject);
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

	void TakeDamage( int dam) {
		hp -= dam;
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

		transform.parent = pad.transform;
		rb.velocity = Vector3.zero;
		rb.angularVelocity = 0f;
		rb.Sleep ();
	}

	public void LiftOff () {
		TurnEngineOn ();
		rb.WakeUp ();
		transform.parent = null;
		landed = false;
	}

	IEnumerator LandingApproach (LandingPad pad) {
		yield return new WaitForSeconds (1f);
		transform.position = pad.transform.position + pad.landingMarkOffset;
		transform.rotation = pad.transform.rotation;
	}

	public bool Landed {
		get {
			return landed;
		}
	}
}
