using UnityEngine;
using System.Collections;

public class Egg : Item {

	public float layTime;
	public float gestationTime;
	public GameObject hatchlingPrefab;
	public bool inCoop = false;
	public Color momsColor;
	public Bird mom;

	private Coop coo;
	private bool dead = false;

	void Awake () {
		base.OnAwake ();
	}
		
	void Start () {
		coo = gm.bigBird.GetComponentInChildren<Coop> ();
		layTime = 0f;
	}
	
	void Update () {
		layTime += Time.deltaTime;
		if (layTime > gestationTime) {
			Hatch ();
		}
	}

	void OnTriggerStay2D (Collider2D other) {
		//if (other.tag == "Player") {
		//print ("egg touching");
		//other.transform.parent.GetComponentInChildren<Player> ().itemTouching = transform;
		//}
		base.TriggerStay2D (other);
	}

	public void OnTriggerExit2D (Collider2D other) {
		/*if (other.tag == "Player") {
			print ("egg untouching");

			Player otherP = other.transform.parent.GetComponentInChildren<Player> ();
			if (otherP.itemTouching) {
				if (otherP.itemTouching == transform) {
					otherP.itemTouching = null;
				}
			}
		}*/
	}

	void OnCollisionEnter2D (Collision2D coll) {
		base.CollisionEnter2D (coll);
	}

	void OnCollisionExit2D (Collision2D coll) {
		base.CollisionExit2D (coll);
	}

	public void Hatch () {
		GameObject newPup = Instantiate (hatchlingPrefab, transform.position, Quaternion.identity) as GameObject;
		newPup.GetComponent<Hatchling> ().mom = mom;
		newPup.transform.parent = transform.parent;
		newPup.transform.rotation = transform.rotation;
		if (!mom.docked) {
			newPup.GetComponent<Hatchling> ().SetColor (momsColor);
		} else {
			newPup.GetComponent<Hatchling> ().SetColor (Color.grey);
			newPup.GetComponent<Collider2D> ().enabled = false;
		}


		mom.follower = newPup.transform;
		if (inCoop) {
			coo.RemoveOccupant (transform);
			coo.AddOccupant (newPup.transform);
		}
		Die ();
	}

	public override void Drop (Player p, string sortLayerName, int sortOrder, bool droppedItem=false, bool canDrop=true) {
		PlayerInput.State pState = p.GetComponent<PlayerInput> ().state;
		if (pState == PlayerInput.State.IN_COOP) {
			if (!coo.full) {
				coo.AddOccupant (this.transform);
				this.transform.parent = coo.transform;
				gameObject.layer = LayerMask.NameToLayer ("OnBoard");
				GetComponent<Collider2D> ().isTrigger = false;
				GetComponent<Rigidbody2D> ().isKinematic = false;
				GetComponent<Rigidbody2D> ().drag = 0f;
				transform.parent = null;
				droppedItem = true;
			}
		} else if (pState == PlayerInput.State.DOCKED) {
			Dock dock = p.GetComponent<PlayerInput> ().station.GetComponent<Dock> ();
			if (!dock.item) {
				transform.position = dock.transform.position;
				transform.parent = dock.transform;
				dock.item = transform;
				GetComponent<Collider2D> ().isTrigger = true;
				GetComponent<Rigidbody2D> ().isKinematic = true;
				sortOrder += 2;
				droppedItem = true;
			}
		} /*else if (pState == PlayerInput.State.ON_FOOT) {
			gameObject.layer = LayerMask.NameToLayer ("Crossover");
			GetComponent<Collider2D> ().isTrigger = false;
			GetComponent<Rigidbody2D> ().isKinematic = false;
			GetComponent<Rigidbody2D> ().drag = 5f;
			sortOrder++;
			transform.parent = null;
			alreadyDroppedItem = true;
		} else if (pState == PlayerInput.State.IN_HOLD || pState == PlayerInput.State.ON_PLATFORM) {
			GetComponent<Collider2D> ().isTrigger = true;
			GetComponent<Rigidbody2D> ().isKinematic = true;
			transform.parent = gm.bigBird.hold.transform;
			alreadyDroppedItem = true;
		} else if (pState == PlayerInput.State.ON_PLATFORM) {

		}
		*/
		base.Drop (p, sortLayerName, sortOrder, droppedItem, canDrop);
	}

	public void Die () {
		if (dead) {
			return;
		}
		dead = true;

		if (mom.follower == transform) {
			mom.follower = null;
		}

		if (mom.GetDead () || mom.damaged) {
			transform.parent = null;
			GetComponent<Rigidbody2D> ().isKinematic = false;
			GetComponent<Rigidbody2D> ().velocity = mom.GetComponent<Rigidbody2D> ().velocity;
			sr.color = new Color (0f, 0f, 0f, .5f);
			sr.sortingLayerName = "Buildings";
			GetComponent<Rigidbody2D> ().AddTorque (Random.Range (-mom.deathTorque, mom.deathTorque));
			CancelInvoke ();
			gameObject.layer = LayerMask.NameToLayer ("Dead");
			this.enabled = false;
		} else {
			Destroy (gameObject);
		}
	}
}