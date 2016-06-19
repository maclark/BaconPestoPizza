using UnityEngine;
using System.Collections;

public class Ruby : Item {

	void Awake () {
		OnAwake ();
	}

	void OnTriggerStay2D (Collider2D other) {
		base.TriggerStay2D (other);
	}
		
	void OnCollisionEnter2D (Collision2D coll) {
		CollisionEnter2D (coll);
	}

	void OnCollisionExit2D (Collision2D coll) {
		CollisionExit2D (coll);
	}

	public override void Drop (Player p, string sortLayerName, int sortOrder, bool droppedItem=false, bool canDrop=true) {
		base.Drop (p, sortLayerName, sortOrder, droppedItem, canDrop);
	}
}
