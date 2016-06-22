using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

	public enum ItemType {BATTERY, EAGLEHEAD, BIRD_SHIELD, RUBY, CANNONBALLS, TORPEDO, EGG, TON_WATER, SUN, GREENS}
	public ItemType itemType;
	public Sprite diamond;
	public Sprite birdhead;
	public Sprite egg;
	public Sprite greens;
	public Shopkeeper keeper;
	public Transform displayCase;
	public bool forSale = false;

	protected GameManager gm;
	protected SpriteRenderer sr;


	void Awake () {
		OnAwake ();
	}

	protected void OnStart () {
	}

	protected void OnAwake () {
		gm = GameObject.FindObjectOfType<GameManager> ();
		sr = GetComponentInChildren<SpriteRenderer> ();
	}

	void OnTriggerStay2D (Collider2D other) {
		TriggerStay2D (other);
	}


	protected void TriggerStay2D (Collider2D other) {
	}

	protected void TriggerExit2D (Collider2D other) {

	}

	void OnCollisionEnter2D (Collision2D coll) {
		CollisionEnter2D (coll);
	}

	void OnCollisionExit2D (Collision2D coll) {
		CollisionExit2D (coll);
	}

	protected void CollisionEnter2D (Collision2D coll) {
		if (coll.transform.tag == "Player") {
			coll.transform.GetComponentInChildren<Player> ().itemTouching = transform;
		}
	}

	protected void CollisionExit2D (Collision2D coll) {
		if (coll.transform.tag == "Player") {
			Player otherP = coll.transform.GetComponentInChildren<Player> ();
			if (otherP.itemTouching) {
				if (otherP.itemTouching == transform) {
					otherP.itemTouching = null;
				}
			}
		}
	}
		

	public void RandomType () {
		itemType = (ItemType) Random.Range (0, (int)ItemType.GREENS);
		switch (itemType) 
		{
		case ItemType.EAGLEHEAD:
			sr.color = Color.cyan;
			break;
		case ItemType.RUBY:
			sr.color = Color.red;
			sr.sprite = diamond;
			break;
		case ItemType.BIRD_SHIELD:
			sr.color = Color.blue;
			break;
		case ItemType.GREENS:
			sr.color = Color.green;
			break;
		default:
			break;
		}
	}

	public virtual IEnumerator EnableColliders () {
		yield return new WaitForSeconds (.5f);
		Collider2D col = GetComponent<Collider2D> ();
		col.enabled = true;
		col.isTrigger = false;
	}

	public virtual void Drop (Player p, string sortLayerName, int sortOrder, bool droppedItem=false, bool canDrop=true) {
		if (!canDrop) {
			return;
		}

		if (!droppedItem) {
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
				sortOrder = 1;
				transform.parent = gm.bigBird.hold.transform;
				droppedItem = true;
			}
		}

		if (droppedItem) {
			GetComponent<Collider2D> ().enabled = true;
			GetComponentInChildren<SpriteRenderer> ().sortingLayerName = sortLayerName;
			GetComponentInChildren<SpriteRenderer> ().sortingOrder = sortOrder;
			p.itemHeld = null;
		}
	}

	public void Sold () {
		displayCase = null;
		forSale = false;
	}
}
