﻿using UnityEngine;
using System.Collections;

public class Greens : Item {

	void Awake () {
		OnAwake ();
	}

	void OnCollisionEnter2D (Collision2D coll) {
		CollisionEnter2D (coll);
	}

	void OnCollisionExit2D (Collision2D coll) {
		CollisionExit2D (coll);
	}

	public override void Drop (Player p, string sortLayerName, int sortOrder) {
		bool droppedItem = false;
		PlayerInput.State pState = p.GetComponent<PlayerInput> ().state;
		if (pState == PlayerInput.State.IN_COOP) {
		} else if (pState == PlayerInput.State.ON_FOOT) {
			gameObject.layer = LayerMask.NameToLayer ("Pedestrians");
			GetComponent<Collider2D> ().isTrigger = false;
			GetComponent<Rigidbody2D> ().isKinematic = false;
			GetComponent<Rigidbody2D> ().drag = 5f;
			sortOrder++;
			transform.parent = null;
			droppedItem = true;

		} else if (pState == PlayerInput.State.DOCKED) {
		}
		if (droppedItem) {
			base.Drop (p, sortLayerName, sortOrder); 
		}
	}
}
