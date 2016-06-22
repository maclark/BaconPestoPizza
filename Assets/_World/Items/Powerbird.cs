using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Powerbird : MonoBehaviour {

	public float initialCharge = 10f;
	public float drainRate = 1f;
	public float powerPerBirdPerc = .5f;
	public Transform thePowerbird;
	public Sprite thunderbird;
	public Material bandMaterial;
	public float forceMag;
	public float k;
	public float startBandwidth;
	public float endBandWidth;
	public float mass;
	public float drag;
	public float dragPerBird;
	public float percentLostPerHit = .5f;

	private GameManager gm;
	private int flashIndex = 0;
	private List<Transform> theFlock = new List<Transform> ();
	private ResourceBar chargeTank;

	void Awake () {
		gm = GameObject.FindObjectOfType<GameManager> ();
	}

	void Start () {
		DeterminePoweringBirds ();
		AddTethers ();
		initialCharge = initialCharge * (1 + powerPerBirdPerc * theFlock.Count);
		chargeTank = GetComponentInChildren<ResourceBar> ();
		chargeTank.capacity = initialCharge;
		chargeTank.SetResource (initialCharge);
		drag = dragPerBird * theFlock.Count - 1;
		thePowerbird.GetComponent <Bird>().Powered (this);
	}

	void Update () {
		DeterminePoweringBirds ();
		DrawTethers ();
		DrainCharge ();
	}

	void FixedUpdate () {
		if (theFlock.Count > 1) {
			Vector3 cop = CalculateCenterOfPower ();
			Vector3 directionToCop = cop - transform.position;
			directionToCop.Normalize ();
			forceMag = k * Vector3.Distance (transform.position, cop);
			thePowerbird.GetComponent<Rigidbody2D> ().AddForce (directionToCop * forceMag);
		}
	}

	public void DeterminePoweringBirds () {
		theFlock.Clear ();
		foreach (Transform t in gm.GetAlliedTransforms ()) {
			Player p = t.GetComponent <Player> ();
			if (p) {
				if (p.b) {
					theFlock.Add (p.b.transform);
				}
			}
		}
	}

	public void AddTethers () {
		foreach (Transform t in theFlock) {
			if (t != thePowerbird.transform) {
				Bird birdie = t.GetComponent<Bird> ();
				birdie.gameObject.AddComponent<LineRenderer> ();
				LineRenderer lr = birdie.GetComponent<LineRenderer> ();
				lr.SetWidth (endBandWidth, startBandwidth);
				lr.material = bandMaterial;
				Color c = birdie.rider.color;
				c = new Color (c.r, c.g, c.b, .5f);
				lr.SetColors (c, c);
			}
		}
	}

	public void DrawTethers () {
		foreach (Transform t in theFlock) {
			if (t != thePowerbird.transform) {
				LineRenderer lr = t.GetComponent<Bird> ().GetComponent<LineRenderer> ();
				Vector3[] positions = new Vector3[2];
				positions [0] = transform.position;
				positions [1] = t.position;
				lr.SetPositions (positions);
			}
		}
	}

	public void RemoveTethers () {
		foreach (Transform t in theFlock) {
			Destroy (t.GetComponent<LineRenderer> ());
		}
	}

	public void Unpower () {
		if (thePowerbird) {
			thePowerbird.SendMessage ("Unpowered");
		}
		RemoveTethers ();
		Destroy (gameObject);
	}

	Vector3 CalculateCenterOfPower () {
		float x = 0;
		float y = 0;
		foreach (Transform t in theFlock) {
			if (t != thePowerbird.transform) {
				x += t.position.x;
				y += t.position.y;
			}
		}
		x = x / (theFlock.Count - 1);
		y = y / (theFlock.Count - 1);
		return new Vector3 (x, y, 0);
	}

	public void Exhausted () {
		Unpower ();
	}

	public void FlashColors () {
		Color c = theFlock [flashIndex].GetComponentInChildren<Player> ().color;
		thePowerbird.GetComponentInChildren<SpriteRenderer> ().color = c;
		GetComponentInChildren <SpriteRenderer> ().color = c;
		flashIndex = (flashIndex + 1) % theFlock.Count;
	}

	public List<Transform> GetPoweringBirds () {
		return theFlock;
	}

	public void DrainCharge () {
		chargeTank.DecreaseResource (drainRate * Time.deltaTime);
		if (chargeTank.empty) {
			Exhausted ();
		}
	}

	public void TakeDamage () {
		chargeTank.DecreaseResource (chargeTank.current * percentLostPerHit);
	}
}
