using UnityEngine;
using System.Collections;

public class Hatchling : Item {

	public bool flying = false;
	public float flyTime = 0f;
	public float flyTimeRequired = 20f;
	public float assistsNeeded = 1;
	public Vector3 pupOffset;
	public GameObject birdPrefab;


	private int assists = 0;
	private bool isKiller = false;
	private bool isFlyer = false;

	void Awake () {
		base.OnAwake ();
	}
	void Start () {
		sr.color = Random.ColorHSV ();
		base.OnStart ();
	}
	
	void Update () {
		if (flying) {
			flyTime += Time.deltaTime;
			if (flyTime > flyTimeRequired) {
				isFlyer = true;
				if (isKiller) {
					Evolve ();
				}
			}
		}
	}

	public void OnTriggerEnter2D (Collider2D other) {
		/*if (other.tag == "Player") {
			print ("hatch touching: " + other.transform.name);
			other.transform.parent.GetComponentInChildren<Player> ().itemTouching = transform;
		} else */ if (other.tag == "EnemyBullet" || other.tag == "Enemy") {
			transform.parent.GetComponent<Bird> ().pup = null;
			Die ();
		}
	}

	public void OnTriggerExit2D (Collider2D other) {
		/*if (other.tag == "Player") {
			print ("hatch untouching: " + other.transform.name);
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

	void Evolve () {
		GameObject obj = Instantiate (birdPrefab, transform.position, Quaternion.identity) as GameObject;
		if (transform.parent) {
			obj.transform.parent = transform.parent;
		}
		obj.GetComponentInChildren<SpriteRenderer> ().color = sr.color;
		obj.GetComponent<Bird> ().color = sr.color;
		obj.GetComponent<Bird> ().colorSet = true;
		Destroy (gameObject);
	}

	public void Die () {
		Destroy (gameObject);
	}

	public void IncrementAssists () {
		print ("pup got an assist");
		assists++;
		if (assists > assistsNeeded) {
			isKiller = true;
			if (isFlyer) {
				Evolve ();
			}
		}
	}

	public void UndockFromBigBird (Bird b) {
		gameObject.layer = LayerMask.NameToLayer ("Flyers");
		GetComponent<Rigidbody2D> ().isKinematic = true;
		GetComponent<Collider2D> ().isTrigger = true;
		transform.rotation = Quaternion.identity;
		transform.position = b.transform.position + pupOffset;
		transform.parent = b.transform;
		flying = true;
	}

	public override void Drop (Player p, string sortLayerName, int sortOrder) {
		bool droppedItem = false;
		PlayerInput.State pState = p.GetComponent<PlayerInput> ().state;
		if (pState == PlayerInput.State.IN_COOP) {
			Coop coo = gm.bigBird.GetComponentInChildren<Coop> ();
			if (!coo.full) {
				coo.AddOccupant (this.transform);
				this.transform.parent = coo.transform;
				gameObject.layer = LayerMask.NameToLayer ("OnBoard");
				GetComponent<Collider2D> ().isTrigger = false;
				GetComponent<Rigidbody2D> ().isKinematic = false;
				GetComponent<Rigidbody2D> ().drag = 0f;
				sortOrder++;
				droppedItem = true;
			}
		} else if (pState == PlayerInput.State.ON_FOOT) {
			gameObject.layer = LayerMask.NameToLayer ("Pedestrians");
			GetComponent<Collider2D> ().isTrigger = false;
			GetComponent<Rigidbody2D> ().isKinematic = false;
			GetComponent<Rigidbody2D> ().drag = 5f;
			sortOrder++;
			droppedItem = true;
		} else if (pState == PlayerInput.State.DOCKED) {
			Dock dock = p.GetComponent<PlayerInput>().station.GetComponent<Dock> ();
			if (!dock.item) {
				GetComponent<Rigidbody2D> ().isKinematic = false;
				transform.position = dock.transform.position;
				transform.parent = dock.transform;
				dock.item = transform;
				GetComponent<Collider2D> ().enabled = true;
				sortOrder += 2;
				droppedItem = true;
			}
		}
		if (droppedItem) {
			base.Drop (p, sortLayerName, sortOrder); 
		}
	}
}
