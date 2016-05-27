using UnityEngine;
using System.Collections;

public class Invader : Flyer {

	void Awake() {
		base.OnAwake ();
	}

	void Start () {
		base.OnStart ();
	}

	void Update () {
		base.OnUpdate ();
	}

	void FixedUpdate () {
		base.OnFixedUpdate ();
	}

	void OnTriggerEnter2D ( Collider2D other) {
		base.TriggerEnter2D (other);
	}

	void OnBecameVisible () {
		base.BecameVisible ();
	}

	void OnBecameInvisible () {
		base.BecameInvisible ();
	}
}
