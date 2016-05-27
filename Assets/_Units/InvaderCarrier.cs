using UnityEngine;
using System.Collections;

public class InvaderCarrier : Carrier {

	void Start () {
		base.OnStart ();
	}

	void OnTriggerEnter2D (Collider2D other) {
		base.TriggerEnter2D (other);
	}

}
