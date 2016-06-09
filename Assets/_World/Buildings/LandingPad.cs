using UnityEngine;
using System.Collections;

public class LandingPad : MonoBehaviour {

	public Vector3 disembarkPoint;
	public Vector3 landingMarkOffset;
	public Transform occupant;

	void Start () {
		disembarkPoint = transform.parent.transform.position;
	}

}
