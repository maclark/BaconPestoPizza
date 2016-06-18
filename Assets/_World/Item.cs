using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

	public enum ItemType {RUBY, CANNONBALLS, TORPEDO,  POWERBIRD, EGG, BIRD_SHIELD, TON_WATER, SUN, GREENS}
	public ItemType itemType;
	public Sprite diamond;
	public Sprite birdhead;
	public Sprite egg;
	public Sprite greens;
	public Shopkeeper keeper;
	public bool forSale = false;
	public Transform displayCase;

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
		/*if (other.name == "CargoPlatform") {
			CargoHold ch = gm.bigBird.GetComponentInChildren<CargoHold> ();
			if (!ch.platformCargo) {
				ch.MoveToLoadingPlatform (transform);
			}
		}*/
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
		if (coll.transform.tag == "Bird") {
			Bird birdie = coll.transform.GetComponent<Bird> ();
			if (itemType == Item.ItemType.BIRD_SHIELD) {
				if (!birdie.Shield.gameObject.activeSelf) {
					birdie.Shield.ActivateShield ();
					Destroy (gameObject);
				}
			} else if (itemType == Item.ItemType.GREENS) {
				birdie.EatGreens ();
				Destroy (gameObject);
			}
		} else if (coll.transform.tag == "Player") {
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
		case ItemType.POWERBIRD:
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

	public virtual void Drop (Player p, string sortLayerName, int sortOrder) {
		GetComponent<Collider2D> ().enabled = true;
		GetComponentInChildren<SpriteRenderer> ().sortingLayerName = sortLayerName;
		GetComponentInChildren<SpriteRenderer> ().sortingOrder = sortOrder;
		p.itemHeld = null;
	}

	public void Sold () {
		displayCase = null;
		forSale = false;
	}
}
