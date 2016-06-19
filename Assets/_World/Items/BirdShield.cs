using UnityEngine;
using System.Collections;

public class BirdShield : Item {

	private Harpoonable hool;

	void Awake () {
		hool = GetComponent<Harpoonable> ();
		OnAwake ();
	}
		

	void OnTriggerStay2D (Collider2D other) {
		base.TriggerStay2D (other);
	}

	void OnCollisionEnter2D (Collision2D coll) {
		if (coll.transform.tag == "Bird") {
			Bird birdie = coll.transform.GetComponent<Bird> ();
			if (!birdie.Shield.gameObject.activeSelf) {
				birdie.Shield.ActivateShield ();
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
		/*bool droppedItem = false;
		PlayerInput.State pState = p.GetComponent<PlayerInput> ().state;
		if (pState == PlayerInput.State.ON_FOOT) {
			gameObject.layer = LayerMask.NameToLayer ("Crossover");
			GetComponent<Collider2D> ().isTrigger = false;
			GetComponent<Rigidbody2D> ().isKinematic = false;
			GetComponent<Rigidbody2D> ().drag = 5f;
			sortOrder++;
			transform.parent = null;
			droppedItem = true;
		} else if (pState == PlayerInput.State.IN_HOLD || pState == PlayerInput.State.ON_PLATFORM) {
			GetComponent<Collider2D> ().isTrigger = true;
			GetComponent<Rigidbody2D> ().isKinematic = true;
			transform.parent = gm.bigBird.hold.transform;
			droppedItem = true;
		} else if (pState == PlayerInput.State.ON_PLATFORM) {
			
		}

		if (droppedItem) {
			base.Drop (p, sortLayerName, sortOrder); 
		}
		*/

		base.Drop (p, sortLayerName, sortOrder, droppedItem, canDrop);
	}
}