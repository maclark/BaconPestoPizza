﻿using UnityEngine;
using System.Collections;

public class Greens : Item {

	void Awake () {
		OnAwake ();
	}

	void OnCollisionEnter2D (Collision2D coll) {
		if (coll.transform.tag == "Bird") {
			Bird birdie = coll.transform.GetComponent<Bird> ();
			if (!birdie.inHeat && !birdie.pregnant && !birdie.follower) {
				birdie.EatGreens ();
				Destroy (gameObject);
			}
		}
		CollisionEnter2D (coll);
	}

	void OnCollisionExit2D (Collision2D coll) {
		CollisionExit2D (coll);
	}

	public override void Drop (Player p, string sortLayerName, int sortOrder, bool droppedItem=false, bool canDrop=true) {
		base.Drop (p, sortLayerName, sortOrder, droppedItem, canDrop);
	}
}
