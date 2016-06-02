using UnityEngine;
using System.Collections;

public class InvaderCarrier : Carrier {

	void Awake () {
		base.OnAwake ();
	}
	void Start () {
		base.OnStart ();
	}

	void Update () {
		base.OnUpdate ();
	}

	void OnTriggerEnter2D (Collider2D other) {
		base.TriggerEnter2D (other);
	}

}
