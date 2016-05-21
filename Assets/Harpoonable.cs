using UnityEngine;
using System.Collections;

public class Harpoonable : MonoBehaviour {

	private GameObject barrier;

	void Start () {
		barrier = GetComponent<Joint2D> ().connectedBody.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnJointBreak2D (Joint2D brokenJoint) {
		Destroy (barrier);
	}
}
