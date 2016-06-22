using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LandingPad : MonoBehaviour {

	public Vector3 disembarkPoint;
	public Vector3 landingMarkOffset;
	public List<Transform> rampParts = new List<Transform> ();
	public Transform boardingZone;
	public Transform occupant;
	public bool hasRamp = true;

	void Start () {
		disembarkPoint = boardingZone.position;
		if (hasRamp) {
			WithdrawRamp ();
		}
	}

	public void OnTriggerStay2D (Collider2D other) {
		if (other.tag == "BigBird") {
			other.GetComponentInParent<BigBird> ().nearestPad = this;
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (other.tag == "BigBird") {
			LandingPad np = other.GetComponentInParent<BigBird> ().nearestPad;
			if (this == np) {
				np = null;
			}
		}
	}

	public void ExtendRamp () {
		foreach (Transform t in rampParts) {
			t.gameObject.SetActive (true);
		}
	}

	public void WithdrawRamp () {
		foreach (Transform t in rampParts) {
			t.gameObject.SetActive (false);
		}
	}
}
