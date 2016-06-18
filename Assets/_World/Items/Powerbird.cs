using UnityEngine;
using System.Collections;

public class Powerbird : Item {

	public float colorSwapSpeed = .5f;

	void Awake () {
		OnAwake ();
	}

	void Start () {
		InvokeRepeating ("RandomColor", colorSwapSpeed, colorSwapSpeed);
	}

	void OnCollisionEnter2D (Collision2D coll) {
		CollisionEnter2D (coll);
	}

	void OnCollisionExit2D (Collision2D coll) {
		CollisionExit2D (coll);
	}

	void America () {
		if (sr.color == Color.red) {
			sr.color = Color.white;
		} else if (sr.color == Color.white) {
			sr.color = Color.blue;
		} else if (sr.color == Color.blue) {
			sr.color = Color.red;
		}
	}

	void RandomColor () {
		sr.color = Random.ColorHSV ();
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
