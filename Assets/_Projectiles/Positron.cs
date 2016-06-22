using UnityEngine;
using System.Collections;

public class Positron : Projectile {

	public static float amps = 2f;
	public static float targetingDelta = .25f;

	public float launchTime;
	public float intervalTime = 3f;

	public Vector3 directionTowardTarget = Vector3.zero;
	public float speed = 5f;
	public float distanceAway = 0f;
	public Transform circuitHolder;
	public Transform currentTarget;
	public Transform previousTarget;
	public Vector3 previousStop;
	public Vector3 nextStop;
	//public Circuit circuit;

	void Awake () {
		base.OnAwake ();
	}

	void Start () {
		launchTime = Time.time;
		base.OnStart ();
	}
		
	void Update () {
		if (currentTarget) {
			directionTowardTarget = currentTarget.position - previousTarget.position;
			distanceAway = Vector3.Distance (transform.position, currentTarget.position);
			directionTowardTarget.Normalize ();
			distanceAway -= speed * Time.deltaTime;
			transform.position = currentTarget.position - directionTowardTarget * distanceAway;
		} else {
			Die ();
		}

		TargetProximityCheck ();
	}

	void OnTriggerEnter2D (Collider2D other) {		
		base.TriggerEnter2D (other);
	}

	public override void Die () {
		previousTarget = null;
		currentTarget = null;
		gameObject.SetActive (false);
	}

	void OnBecameInvisible () {
		Die ();
	}

	void TargetProximityCheck () {
		if (currentTarget) {
			if (Mathf.Abs (transform.position.x - currentTarget.position.x) < targetingDelta &&
				Mathf.Abs (transform.position.y - currentTarget.position.y) < targetingDelta &&
				Mathf.Abs (transform.position.z - currentTarget.position.z) < targetingDelta) {
				FindNewTargetOrDie ();
			}
		} else {
			Die ();
		}
	}

	void FindNewTargetOrDie () {
		Bird birdie = currentTarget.GetComponent<Bird> ();
		if (birdie != null) {
			Harpoon birdsHarp = birdie.harp;
			if (birdsHarp != null) {
				GameObject harpsHarpooned = birdsHarp.GetHarpooned ();
				if (harpsHarpooned != null) {
					previousTarget = birdie.transform;
					currentTarget = harpsHarpooned.transform;
					distanceAway = Vector3.Distance (transform.position, currentTarget.position);
				} else {
					previousTarget = birdie.transform;
					currentTarget = birdsHarp.transform;
					distanceAway = Vector3.Distance (transform.position, currentTarget.position);
				}
			} else {
				Die ();
			}
		} else {
			Die ();
		}
	}
}
