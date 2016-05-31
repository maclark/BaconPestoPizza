using UnityEngine;
using System.Collections.Generic;

public class Harpoon : MonoBehaviour {

	public Vector2 direction = Vector2.zero;
	public float forceMag = 500f;
	public float minTetherWidth = .05f;
	public float maxTetherWidth = .2f;
	public float minWidthTetherLength = 3;
	public float tautLength = 10f;
	public float maxTetherLength = 30f;
	public bool taut = false;
	public float recallMag	= 100f;
	public float detachDelay = .3f;
	public bool recalling = false;
	public GameObject webPrefab;

	private Rigidbody2D rb;
	private LineRenderer lr;
	private Vector3[] tetherPositions = new Vector3[2];
	private GameObject harpooner = null;
	private GameObject harpooned = null;

	void Awake () {
		rb = GetComponent<Rigidbody2D> ();
	}

	void Start () {
		lr = GetComponent<LineRenderer> ();
		lr.sortingLayerName = GetComponentInChildren<SpriteRenderer>().sortingLayerName;
	}

	void Update () {
		DrawTether ();
		if (harpooned != null) {
			CheckTetherCollision ();
		}

		if (Vector3.Distance (transform.position, harpooner.transform.position) > maxTetherLength) {
			DetachAndRecall (true);
		}
	}

	void FixedUpdate () {
		if (recalling) {
			Vector3 detachDir = harpooner.GetComponent<Bird> ().p.transform.position - transform.position;
			detachDir.Normalize ();
			rb.AddForce (detachDir * recallMag);
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.name == "BirdTrigger") {
			Bird pBird = other.transform.parent.GetComponent<Bird> ();
			if (harpooner.name == pBird.name) {
				pBird.hasHarpoon = true;
				Destroy (gameObject);
			} else {
				//implement linking
				if (!recalling) {
					HarpoonObject (pBird.gameObject);
					pBird.otherHarps.Add (this);
				}
			}
		}

		else if (other.tag == "Harpoonable") {
			if (!recalling) {
				HarpoonObject (other.gameObject);
				other.GetComponent<SpriteRenderer> ().sortingLayerName = "Birds";
			}
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
		rb.AddForce (direction * forceMag);
	}

	public void Die () {
		Destroy (gameObject);
	}

	void DrawTether () {
		tetherPositions [0] = harpooner.GetComponent<Bird> ().p.transform.position;

		if (harpooned != null) {
			tetherPositions [1] = harpooned.transform.position;
		} else {
			tetherPositions [1] = transform.position;
		}
		lr.SetPositions (tetherPositions);

		float distance = Vector3.Distance (tetherPositions[0], tetherPositions[1]);
		float tetherWidth = Mathf.Lerp (maxTetherWidth, minTetherWidth, (distance - minWidthTetherLength )/ tautLength);
		lr.SetWidth (tetherWidth, tetherWidth);

		//for determining when to stop thinning the line renderered
		if (distance < minWidthTetherLength) {
			taut = false;
			lr.material.color = harpooner.GetComponent<Bird> ().color;
		} 
		else if (distance < tautLength + minWidthTetherLength) {
			taut = false;
			lr.material.color = harpooner.GetComponent<Bird> ().color;
		}
		else {
			lr.material.color = Color.red;
			taut = true;
		} 
	}

	public void SetHarpooner (GameObject harpoonThrower) {
		harpooner = harpoonThrower;
	}

	public void HarpoonObject (GameObject harpoonRecipient) {
		harpooned = harpoonRecipient;
		GetComponent<Rigidbody2D> ().Sleep ();
		GetComponent<BoxCollider2D> ().enabled = false;
		transform.position = harpooned.transform.position;
		transform.parent = harpooned.transform;

		if (harpoonRecipient.tag == "Player") {
			AttemptWeb ();
		} else if (harpoonRecipient.tag == "Harpoonable") {
			harpoonRecipient.GetComponent<Harpoonable> ().harp = this;
		}

		harpoonRecipient.SendMessage ("BeenHarpooned", null, SendMessageOptions.DontRequireReceiver);
	}

	public void DetachAndRecall (bool overrideToggle=false) {

		if (harpooned != null) {
			if (harpooned.tag == "Player") {
				harpooned.GetComponent<Bird> ().RemoveHarp (this);
			} else if (harpooned.tag == "Harpoonable") {
				harpooned.SendMessage ("HarpoonReleased", null, SendMessageOptions.DontRequireReceiver);
			}
			harpooned = null;
			transform.parent = null;
			GetComponent<Rigidbody2D> ().WakeUp ();
			GetComponent<BoxCollider2D> ().enabled = true;
		}


		SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer> ();
		if (overrideToggle) {
			recalling = true;
		} else {
			recalling = !recalling;
			if (!recalling) {
				foreach (SpriteRenderer r in renderers) {
					r.color = new Color (r.color.r, r.color.g, r.color.b, 1f);
				}
			} else {
				foreach (SpriteRenderer r in renderers) {
					r.color = new Color (r.color.r, r.color.g, r.color.b, .5f);
				}
			}
		}
	}


	//#TODO what happens when objects cross a tether?
	void CheckTetherCollision () {
		RaycastHit2D[] hits = Physics2D.LinecastAll (transform.position, harpooner.GetComponent<Bird> ().p.transform.position);
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
			if (harpooned.GetComponent<Bird> ().harp.harpooned) {
				if (harpooned.GetComponent<Bird> ().harp.harpooned.GetComponent<Bird> ().harp) {
					if (harpooned.GetComponent<Bird> ().harp.harpooned.GetComponent<Bird> ().harp.harpooned == harpooner) {
						ThrowWeb (harpooner.GetComponent<Bird> (), harpooned.GetComponent<Bird> (), harpooned.GetComponent<Bird> ().harp.harpooned.GetComponent<Bird> ());
					}
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

	public GameObject GetHarpooner () {
		return harpooner;
	}

	public GameObject GetHarpooned () {
		return harpooned;
	}
}
