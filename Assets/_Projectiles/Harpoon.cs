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
	public float recallMag	= 100f;
	public float detachDelay = .3f;
	public bool recalling = false;
	public bool taut = false;
	public OrbWeb web = null;
	public GameObject webPrefab;
	public GameObject orbWebPrefab;

	private Rigidbody2D rb;
	private LineRenderer lr;
	private Vector3[] tetherPositions = new Vector3[2];
	private GameObject harpooner = null;
	private GameObject harpooned = null;
	private List<Bird> linkedBirds = new List<Bird> ();

	void Awake () {
		rb = GetComponent<Rigidbody2D> ();
	}

	void Start () {
		lr = GetComponent<LineRenderer> ();
		lr.sortingLayerName = GetComponentInChildren<SpriteRenderer>().sortingLayerName;
	}

	void Update () {
		DrawTether ();
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
				other.GetComponent<Harpoonable> ().SetSortingLayer ("Birds");
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
			lr.material.color = harpooner.GetComponent<Bird> ().p.color;
		} 
		else if (distance < tautLength + minWidthTetherLength) {
			taut = false;
			lr.material.color = harpooner.GetComponent<Bird> ().p.color;
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
			linkedBirds.Clear ();
			AttemptWeb (harpooner.GetComponent<Bird> ());
		} else if (harpoonRecipient.tag == "Harpoonable") {
			harpoonRecipient.GetComponent<Harpoonable> ().harp = this;
		}

		harpoonRecipient.SendMessage ("BeenHarpooned", null, SendMessageOptions.DontRequireReceiver);
	}

	public void DetachAndRecall (bool forceRecall=false) {
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
			if (web) {
				web.Break ();
				//must return here, because other this call will untoggle recalling bool
				return;
			}
		}


		SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer> ();
		if (forceRecall) {
			recalling = true;
		} else {
			recalling = !recalling;
		}

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
		
	void AttemptWeb (Bird birdie) {
		linkedBirds.Add (birdie);
		//first check if harpooned
		if (birdie.harp) {
			if (birdie.harp.harpooned) {
				if (birdie.harp.harpooned.GetComponent<Bird> ()) {
					Bird birdiesBird = birdie.harp.harpooned.GetComponent<Bird> ();
					if (birdiesBird == linkedBirds [0]) {
						MakeWeb ();
					} else {
						AttemptWeb (birdiesBird);
					}
				}
			}
		}
	}

	void MakeWeb () {
		//TODO special case for only 2 bids in web
		GameObject webObj = Instantiate (orbWebPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		OrbWeb newWeb = webObj.GetComponent<OrbWeb> ();
		Transform[] linkedBirdsTransforms = new Transform [linkedBirds.Count];
		for (int i = 0; i < linkedBirds.Count; i++) {
			linkedBirdsTransforms [i] = linkedBirds [i].transform;
		}
		newWeb.SetWebbers (linkedBirdsTransforms);
	}

	public GameObject GetHarpooner () {
		return harpooner;
	}

	public GameObject GetHarpooned () {
		return harpooned;
	}
}
