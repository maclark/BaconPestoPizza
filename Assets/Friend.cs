using UnityEngine;
using System.Collections;

public class Friend : Item {
	public Sprite[] sprites;
	public Vector3 startPosition;
	public GameObject kangaPrefab;
	public Transform[] castleWalls;


	void Awake () {
		OnAwake ();
	}

	void Start () {
		startPosition = transform.position;
		sr.sprite = sprites [Random.Range (0, sprites.Length)];
		GetComponent<Rigidbody2D> ().isKinematic = true;
		GetComponent<Collider2D> ().isTrigger = false;
		GetComponent<SpriteRenderer> ().sortingLayerName = "Buildings";
		GetComponent<SpriteRenderer> ().sortingOrder = 2;
		gameObject.layer = LayerMask.NameToLayer ("Crossover");
	}

	void OnTriggerStay2D (Collider2D other) {
		base.TriggerStay2D (other);
	}

	void OnCollisionEnter2D (Collision2D coll) {
		CollisionEnter2D (coll);
	}

	void OnCollisionExit2D (Collision2D coll) {
		CollisionExit2D (coll);
	}

	public override void Drop (Player p, string sortLayerName, int sortOrder, bool droppedItem=false, bool canDrop=true) {
		PlayerInput.State pState = p.GetComponent<PlayerInput> ().state;
		if (pState == PlayerInput.State.IN_HOLD) {
			gm.bbm.friend = this;
			GetComponent<Collider2D> ().isTrigger = true;
			GetComponent<Rigidbody2D> ().isKinematic = true;
			droppedItem = true;
			GetComponentInChildren<SpriteRenderer> ().sortingLayerName = sortLayerName;
			GetComponentInChildren<SpriteRenderer> ().sortingOrder = 1;
			return;
		} 
		base.Drop (p, sortLayerName, sortOrder, droppedItem, canDrop);
	}

	public void Rescued () {
		Invoke ("SpawnKangas", 10f);
	}

	void SpawnKangas () {
		for (int i = 0; i < gm.GetAlliedTransforms ().Count + gm.zoneNumber; i++) {
			Instantiate (kangaPrefab, startPosition, Quaternion.identity);
		}
		Invoke ("LowerWalls", 10f);
	}

	void LowerWalls () {
		for (int i = 0; i < castleWalls.Length; i++) {
			Destroy (castleWalls [i].gameObject);
		}
	}
}
