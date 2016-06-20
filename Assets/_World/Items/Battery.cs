using UnityEngine;
using System.Collections;

public class Battery : Item {

	private Harpoonable hool;


	void Awake () {
		hool = GetComponent<Harpoonable> ();
		OnAwake ();
	}

	void OnCollisionEnter2D (Collision2D coll) {
		if (coll.transform.tag == "Bird") {
			Bird birdie = coll.transform.GetComponent<Bird> ();
			if (!birdie.hasBattery) {
				birdie.hasBattery = true;
				hool.BreakLoose ();
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
