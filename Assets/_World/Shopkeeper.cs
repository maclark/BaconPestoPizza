using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Shopkeeper : MonoBehaviour {

	public int cannonballsPrice = 6;
	public int torpedoPrice = 3;
	public int powerbirdPrice = 60;
	public int birdShieldPrice = 30;
	public int unitOfEnergyPrice = 10;
	public int tonOfWaterPrice = 10;
	public int greensPrice = 60;
	public int rubyPrice = 20;
	public int eggPrice = 100;
	public float patrolForce;
	public float patrolTime = 3f;
	public float patrolPause = 2f;
	public bool patrolling = true;
	public Vector3 patrolDirection;
	public GameObject colliderChild;
	public List<GameObject> possibleItems;
	public List<Transform> displays;

	private GameManager gm;

	void Awake () {
		gm = GameObject.FindObjectOfType<GameManager> ();
	}

	void Start () {
		patrolDirection = transform.right;
		StartCoroutine ("PausePatrol");
		SetUpDisplays ();
	}

	void FixedUpdate () {
		if (patrolling) {
			Patrol();
		}
	}
	public BoxCollider2D[] GetColliderChild () {
		BoxCollider2D[] colls = new BoxCollider2D[1];
		colls [0] = colliderChild.GetComponent<BoxCollider2D> ();
		return colls;
	}

	public bool SellToPlayer (Item it) {
		bool soldItem = false;

		switch (it.itemType) {
		case Item.ItemType.RUBY:
			if (gm.bbm.PayShop (rubyPrice)) {
				soldItem = true;
			}
			break;
		case Item.ItemType.CANNONBALLS:
			if (!gm.bbm.cannonballsMaxed) {
				if (gm.bbm.PayShop (cannonballsPrice)) {
					gm.bbm.cannonballs += 3;
					soldItem = true;
				}
			}
			break;
		case Item.ItemType.TORPEDO:
			if (!gm.bbm.torpedoesMaxed) {
				if (gm.bbm.PayShop (torpedoPrice)) {
					gm.bbm.torpedoes++;
					soldItem = true;
				}
			}
			break;
		case Item.ItemType.POWERBIRD:
			if (gm.bbm.PayShop (powerbirdPrice)) {
				soldItem = true;
			}
			break;
		case Item.ItemType.BIRD_SHIELD:
			if (gm.bbm.PayShop (birdShieldPrice)) {
				soldItem = true;
			}
			break;
		case Item.ItemType.TON_WATER:
			if (!gm.GetBigBirdWaterTank ().full) {
				if (gm.bbm.PayShop (tonOfWaterPrice)) {
					gm.bbm.waterTank.IncreaseResource (2000);
					soldItem = true;
				}
			}
			break;
		case Item.ItemType.SUN:
			if (!gm.GetBigBirdEnergyTank ().full) {
				if (gm.bbm.PayShop (unitOfEnergyPrice)) {
					gm.bbm.energyTank.IncreaseResource (2000);
					soldItem = true;
				}
			}
			break;
		case Item.ItemType.GREENS:
			if (!gm.bigBird.hold.GetFull ()) {
				if (gm.bbm.PayShop (greensPrice)) {
					soldItem = true;
				}
			}
			break;
		default:
			break;
		}

		if (soldItem) {
			it.Sold ();
		}
		return soldItem;
	}


	public void BuyFromPlayer (Item.ItemType gt) {
		switch (gt) {
		case Item.ItemType.RUBY:
			gm.bbm.Collect (rubyPrice / 2);
			break;
		case Item.ItemType.CANNONBALLS:
			gm.bbm.Collect (cannonballsPrice / 2);
			break;
		case Item.ItemType.TORPEDO:
			gm.bbm.Collect (torpedoPrice / 2);
			break;
		case Item.ItemType.POWERBIRD:
			gm.bbm.Collect (powerbirdPrice / 2);
			break;
		case Item.ItemType.EGG:
			gm.bbm.Collect (eggPrice / 2);
			break;
		case Item.ItemType.BIRD_SHIELD:
			gm.bbm.Collect (birdShieldPrice / 2);
			break;
		case Item.ItemType.TON_WATER:
			gm.bbm.Collect (tonOfWaterPrice / 2);
			break;
		case Item.ItemType.SUN:
			gm.bbm.Collect (unitOfEnergyPrice / 2);
			break;
		case Item.ItemType.GREENS:
			gm.bbm.Collect (greensPrice / 2);
			break;
		default:
			break;
		}
	}

	public void PutOnSale (Item it, Transform dispCase) {
		it.displayCase = dispCase;
		it.transform.position = it.displayCase.position;
		it.GetComponent<Rigidbody2D> ().isKinematic = true;
		it.GetComponent<Collider2D> ().isTrigger = false;
		it.GetComponent<SpriteRenderer> ().sortingLayerName = "Buildings";
		it.GetComponent<SpriteRenderer> ().sortingOrder = 2;
		it.gameObject.layer = LayerMask.NameToLayer ("Crossover");
		it.forSale = true;
		it.keeper = this;
	}


	void SetUpDisplays () {
		foreach (Transform t in displays) {
			Item it = GetRandomItem ();
			PutOnSale (it, t);
		}
	}

	Item GetRandomItem () {
		int index = Random.Range (0, possibleItems.Count);
		GameObject obj = Instantiate (possibleItems [index]) as GameObject;
		Item it = obj.GetComponent<Item> ();
		return it;
	}


	public void Patrol () {
		GetComponent<Rigidbody2D> ().AddForce (patrolDirection * patrolForce);
	}

	public IEnumerator PausePatrol () {
		patrolling = false;
		yield return new WaitForSeconds (patrolPause);
		PickPatrolDirection ();
		patrolling = true;
		yield return new WaitForSeconds (patrolTime);
		StartCoroutine ("PausePatrol");
	}

	void PickPatrolDirection () {
		patrolDirection = -patrolDirection;
		patrolDirection.Normalize ();
	}
}
