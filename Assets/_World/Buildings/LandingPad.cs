using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LandingPad : MonoBehaviour {

	public Vector3 disembarkPoint;
	public Vector3 landingMarkOffset;
	public List<Transform> rampParts = new List<Transform> ();
	public Transform boardingZone;
	public Transform occupant;

	void Start () {
		disembarkPoint = boardingZone.position;
		WithdrawRamp ();
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
