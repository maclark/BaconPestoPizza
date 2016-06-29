using UnityEngine;
using System.Collections;

public class Hatchling : Item {

	public float forceMag = 50f;
	public float flyTime = 0f;
	public float flyTimeRequired = 20f;
	public float assistsNeeded = 1;
	public float goodbyeTime = 2f;
	public bool flying = false;
	public bool goingHome = false;
	public Vector3 pupOffset;
	public GameObject birdPrefab;
	public Bird mom;

	private int assists = 0;
	public bool isKiller = false;
	public bool isFlyer = false;
	private bool dead = false;
	private Rigidbody2D rb;

	void Awake () {
		rb = GetComponent<Rigidbody2D> ();
		base.OnAwake ();
	}
	void Start () {
		
		base.OnStart ();
	}
	
	void Update () {
		if (goingHome) {
			Vector3 directionHome = gm.bigBird.transform.position - transform.position;
			directionHome.Normalize ();
			rb.AddForce (directionHome * forceMag);
		} else {
			if (!isFlyer && flying) {
				flyTime += Time.deltaTime;
				if (flyTime > flyTimeRequired) {
					isFlyer = true;
				}
			}

			if (CheckEvolutionRequirements ()) {
				goingHome = true;
				transform.parent = null;
				rb.isKinematic = false;
				GetComponent<Collider2D> ().isTrigger = false;
			}
		} 


	}

	public bool CheckEvolutionRequirements () {
		//Removing requirement to be killer
		isKiller = true;
		return (isKiller && isFlyer && gm.birds.Count < 8);
	}

	public void OnTriggerEnter2D (Collider2D other) {
		/*if (other.tag == "Player") {
			print ("hatch touching: " + other.transform.name);
			other.transform.parent.GetComponentInChildren<Player> ().itemTouching = transform;
		} else */ 
		if (other.tag == "EnemyBullet" || other.tag == "Enemy") {
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
		if (coll.transform.tag == "BigBird") {
			Evolve ();
		}
		base.CollisionEnter2D (coll);
	}

	void OnCollisionExit2D (Collision2D coll) {
		base.CollisionExit2D (coll);
	}

	public void Evolve () {
		if (mom.follower == transform) {
			mom.follower = null;
		}		
		GameObject obj = Instantiate (birdPrefab, transform.position, Quaternion.identity) as GameObject;
		obj.GetComponentInChildren<SpriteRenderer> ().color = mom.color;
		Bird grownUp = obj.GetComponent<Bird> ();
		grownUp.color = mom.color;
		grownUp.colorSet = true;
		grownUp.StartCoroutine ("TurnBlack", goodbyeTime);
		Destroy (gameObject);
	}
		
	public void IncrementAssists () {
		print ("pup got an assist");
		assists++;
		if (assists >= assistsNeeded) {
			isKiller = true;
		}
	}

	public override void Drop (Player p, string sortLayerName, int sortOrder, bool droppedItem=false, bool canDrop=true) {
		PlayerInput.State pState = p.GetComponent<PlayerInput> ().state;
		if (pState == PlayerInput.State.IN_COOP) {
			Coop coo = gm.bigBird.GetComponentInChildren<Coop> ();
			if (!coo.full) {
				coo.AddOccupant (this.transform);
				this.transform.parent = coo.transform;
				gameObject.layer = LayerMask.NameToLayer ("OnBoard");
				GetComponent<Collider2D> ().isTrigger = false;
				rb.isKinematic = false;
				rb.drag = 0f;
				droppedItem = true;
			} else {
				canDrop = false;
			}
		} else if (pState == PlayerInput.State.DOCKED) {
			Dock dock = p.GetComponent<PlayerInput> ().station.GetComponent<Dock> ();
			if (!dock.item) {
				GetComponent<Rigidbody2D> ().isKinematic = false;
				transform.position = dock.transform.position;
				transform.parent = dock.transform;
				dock.item = transform;
				GetComponent<Collider2D> ().enabled = true;
				sortOrder += 2;
				droppedItem = true;
			}
		} else if (pState == PlayerInput.State.IN_HOLD || pState == PlayerInput.State.ON_PLATFORM) {
			canDrop = false;
		}
			
		base.Drop (p, sortLayerName, sortOrder, droppedItem, canDrop);
	}

	public void SetColor (Color c) {
		sr.color = c;
	}

	public Color GetColor () {
		return sr.color;
	}

	public void Die () {
		if (dead) {
			return;
		}
		dead = true;

		if (mom.follower == transform) {
			mom.follower = null;
		}

		if (!mom.docked) {
			transform.parent = null;
			rb.constraints = RigidbodyConstraints2D.None;
			rb.isKinematic = false;
			rb.velocity = mom.GetComponent<Rigidbody2D> ().velocity;
			rb.AddTorque (Random.Range (-mom.deathTorque, mom.deathTorque));
			sr.color = new Color (mom.color.r, mom.color.g, mom.color.b, .25f);
			sr.sortingLayerName = "Buildings";
			CancelInvoke ();
			gameObject.layer = LayerMask.NameToLayer ("Dead");
			this.enabled = false;
		} else {
			Destroy (gameObject);
		}
	}

}
