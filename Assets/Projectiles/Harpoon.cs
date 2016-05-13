using UnityEngine;
using System.Collections.Generic;

public class Harpoon : MonoBehaviour {

	public Vector2 direction = Vector2.zero;
	public float moveForceMagnitude = 500f;
	public float minWidth = .05f;
	public float maxWidth = .2f;
	public float minWidthTetherLength = 3;
	public float tetherMaxLength = 10f;
	public bool atMaxTether = false;
	public float recallForceMag	= 100f;
	public float detachDelay = .3f;
	public float recallVelocity = 10f;
	public bool recalling = false;
	public GameObject webPrefab;
	public GameObject harpooner = null;
	public GameObject harpooned = null;

	private Rigidbody2D rb;
	private Vector3[] tetherPositions = new Vector3[2];

	void Awake () {
		rb = GetComponent<Rigidbody2D> ();
	}

	void Start () {
		//#TODO make tehter appear over ships?
		//GetComponent<LineRenderer> ().sortingLayerID = GetComponentInChildren<SpriteRenderer> ().sortingLayerID;
		//GetComponent<LineRenderer> ().sortingOrder = GetComponentInChildren<SpriteRenderer> ().sortingOrder;

	}
	void Update () {
		DrawTether ();
		if (harpooned != null) {
			CheckTetherCollision ();
		}
	}

	void FixedUpdate () {
		if (recalling) {
			Vector3 detachDir = harpooner.transform.position - transform.position;
			detachDir.Normalize ();
			rb.AddForce (detachDir * recallForceMag);
		}
	}


	public void SetDirection (Vector2 dir) {
		direction = dir;
		direction.Normalize ();
	}

	public void Fire (Transform start, Vector2 aim) {
		transform.position = start.position;
		transform.rotation = start.rotation;
		SetDirection (aim);
		rb.AddForce (direction * moveForceMagnitude);
	}

	public void Die () {
		Destroy (gameObject);
	}

	void DrawTether () {

		LineRenderer lr = GetComponent<LineRenderer> ();

		tetherPositions [0] = harpooner.transform.position;
		if (harpooned != null) {
			tetherPositions [1] = harpooned.transform.position;
		} else {
			tetherPositions [1] = transform.position;
		}
		lr.SetPositions (tetherPositions);

		float distance = Vector3.Distance (tetherPositions[0], tetherPositions[1]);
		float tetherWidth = Mathf.Lerp (maxWidth, minWidth, (distance - minWidthTetherLength )/ tetherMaxLength);
		lr.SetWidth (tetherWidth, tetherWidth);

		//for determining when to stop thinning the line renderered
		if (distance < minWidthTetherLength) {
			atMaxTether = false;
			lr.material.color = harpooner.GetComponent<Bird> ().color;
			//lr.material.color = Color.blue;
		} 
		else if (distance < tetherMaxLength + minWidthTetherLength) {
			atMaxTether = false;
			lr.material.color = harpooner.GetComponent<Bird> ().color;
		}
		else {
			lr.material.color = Color.red;
			atMaxTether = true;
		} 
	}

	public void SetHarpooner (GameObject harpoonThrower) {
		harpooner = harpoonThrower;
	}

	public void HarpoonObject (GameObject harpoonRecipient) {
		harpooned = harpoonRecipient;
		GetComponent<Rigidbody2D> ().Sleep ();
		GetComponent<BoxCollider2D> ().enabled = false;
		//after turning off rb and collider, move harp if on player
		//#TODO maybe leave sprites on and don't move harpoon? could be cool. plus, when harpooning bigbird or random things, don't want to move anchor...?
		if (harpoonRecipient.tag == "Player") {
			transform.position = harpooned.transform.position;
			AttemptWeb ();
		}
		transform.parent = harpooned.transform;
		/*SpriteRenderer[] harpoonSprites = GetComponentsInChildren<SpriteRenderer> ();
		foreach (SpriteRenderer r in harpoonSprites) {
			r.enabled = false;
		}*/
	}

	public void DetachAndRecall (bool overrideToggle=false) {
		SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer> ();
		foreach (SpriteRenderer r in renderers) {
			//r.enabled = true;
			r.color = new Color (r.color.r, r.color.g, r.color.b, .5f);
		}

		if (harpooned != null) {
			if (harpooned.tag == "Player") {
				print ("BreakWeb ();");
			}
			harpooned = null;
			transform.parent = null;
			GetComponent<Rigidbody2D> ().WakeUp ();
			GetComponent<BoxCollider2D> ().enabled = true;
		}

		if (overrideToggle) {
			recalling = true;
		} else {
			recalling = !recalling;
			if (!recalling) {
				foreach (SpriteRenderer r in renderers) {
					r.color = new Color (r.color.r, r.color.g, r.color.b, 1f);
				}
			}
		}
	}


	//#TODO what happens when objects cross a tether?
	void CheckTetherCollision () {
		RaycastHit2D[] hits = Physics2D.LinecastAll (transform.position, harpooner.transform.position);
		for (int i = 0; i < hits.Length; i++) {
			Transform t = hits [i].transform;
			if (t.name != name && t.name != harpooner.name && t.name != harpooned.name) {
				if (t.tag == "Player") {
				} else if (t.tag == "Enemy") {
				} else if (t.tag == "BigBird") {
				}
			}
		}

		if (hits.Length == 0) {
			//print ("hitting nothing");
		}
		else if (hits.Length == 1) {
			//print (hits [0].transform.name);
		}
		else if (hits.Length == 2) {
			//print (hits [0].transform.name + " " + hits[1].transform.name);
		}
		else if (hits.Length == 3) {
			//print (hits [0].transform.name + " " + hits[1].transform.name + " " + hits[2].transform.name);
		}
	}

	void AttemptWeb () {
		if (harpooned.GetComponent<Bird> ().harp) {
			if (harpooned.GetComponent<Bird> ().harp.harpooned.GetComponent<Bird> ().harp) {
				if (harpooned.GetComponent<Bird> ().harp.harpooned.GetComponent<Bird> ().harp.harpooned == harpooner) {
					ThrowWeb (harpooner.GetComponent<Bird> (), harpooned.GetComponent<Bird> (), harpooned.GetComponent<Bird> ().harp.harpooned.GetComponent<Bird> ());
				}
			}
		}
	}

	void ThrowWeb (Bird b1, Bird b2, Bird b3) {
		GameObject webObject = Instantiate (webPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		Web web = webObject.GetComponent<Web> ();
		web.Add (b1.transform, b2.transform, b3.transform);
		b1.web = web;
		b2.web = web;
		b3.web = web;
	}
}
