using UnityEngine;
using System.Collections;

public class Flame : MonoBehaviour {
	public FlammableCompartment compartment;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation = Quaternion.identity;
	}

	public void Die () {
		compartment.flames.Remove (this);
		gameObject.SetActive (false);
	}
}
