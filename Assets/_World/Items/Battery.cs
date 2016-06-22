using UnityEngine;
using System.Collections;

public class Battery : Item {

	public Transform holder;
	public bool held = false;
	public float radius = 2f;
	public float theta = 0f;
	public float radsPerSec = 2 * Mathf.PI;
	private Harpoonable hool;

	void Awake () {
		hool = GetComponent<Harpoonable> ();
		OnAwake ();
	}

	void Update () {
		if (held) {
			theta += radsPerSec * Time.deltaTime;
			float x = radius * Mathf.Cos (theta);
			float y = radius * Mathf.Sin (theta);
			transform.position = new Vector3 (holder.transform.position.x + x, holder.transform.position.y + y, 0f);
		}
	}

	void OnCollisionEnter2D (Collision2D coll) {
		if (!held) {
			if (coll.transform.tag == "Bird") {
				Bird birdie = coll.transform.GetComponent<Bird> ();
				if (!birdie.hasBattery) {
					PickedUpByBird (birdie);
				}
			}
			CollisionEnter2D (coll);
		}
	}

	void OnCollisionExit2D (Collision2D coll) {
		CollisionExit2D (coll);
	}

	public override void Drop (Player p, string sortLayerName, int sortOrder, bool droppedItem=false, bool canDrop=true) {
		base.Drop (p, sortLayerName, sortOrder, droppedItem, canDrop);
	}

	public void PickedUpByBird (Bird birdie) {
		birdie.hasBattery = true;
		holder = birdie.transform;
		hool.BreakLoose ();
		transform.parent = birdie.transform;
		transform.rotation = Quaternion.identity;
		GetComponent<Rigidbody2D> ().isKinematic = true;
		GetComponent<Collider2D> ().isTrigger = true;
		held = true;
	}

	public void DroppedUpByBird (Bird birdie) {
		birdie.hasBattery = false;
		holder = null;
		transform.parent = null;
		GetComponent<Rigidbody2D> ().isKinematic = false;
		GetComponent<Collider2D> ().isTrigger = false;
		Invoke ("Unheld", .5f);
	}

	void Unheld () {
		held = false;
	}
}
