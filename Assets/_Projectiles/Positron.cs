using UnityEngine;
using System.Collections;

public class Positron : Projectile {

	public float launchTime;
	public float intervalTime = 3f;

	public Vector3 direction = Vector3.zero;
	public float speed = 5f;
	public float distance = 0f;
	public Transform circuitHolder;
	public Transform harpoonedTransform;
	public Vector3 previousStop;
	public Vector3 nextStop;
	public Circuit circuit;

	void Awake () {
		base.OnAwake ();
	}

	void Start () {
		launchTime = Time.time;
		base.OnStart ();
	}

	void NewOldUpdate () {
		transform.position = Vector3.Lerp (circuitHolder.position, harpoonedTransform.position, (Time.time - launchTime) / intervalTime);
	}

	void Update () {
		previousStop = circuitHolder.transform.position;
		nextStop = harpoonedTransform.transform.position;
		direction = nextStop - previousStop;
		direction.Normalize ();
		distance += speed * Time.deltaTime;
		transform.position = previousStop + direction * distance;
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "Bird") {
			Bird b = other.GetComponent<Bird> ();
			if (b.hasBattery && b.p.transform != circuitHolder.transform) {
				Die ();	
			} else if (b.transform == harpoonedTransform) {
				FinishLength ();
			}
		}
		base.TriggerEnter2D (other);
	}

	public void FinishLength () {
		circuitHolder = harpoonedTransform.GetComponent<Bird> ().p.transform;
		harpoonedTransform = harpoonedTransform.GetComponent<Bird> ().harp.GetHarpooned ().transform;
		distance = 0f;
	}

	public override void Die () {
		distance = 0f;
		circuitHolder = null;
		harpoonedTransform = null;
		circuit.posis.Remove (this);
		gameObject.SetActive (false);
	}

	void OnBecameInvisible () {
		Die ();//gameObject.SetActive (false);
	}
}
