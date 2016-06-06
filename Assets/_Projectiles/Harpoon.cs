using UnityEngine;
using System.Collections.Generic;

public class Harpoon : MonoBehaviour {

	public Vector3 tetherDirection = Vector3.zero;
	public float forceMag = 500f;
	public float recallMag = 100f;
	public float tetherLength;
	public float tautLength = 10f;
	public float stretchiness = .1f;
	public float k = 1f;
	public float maxTetherLength = 20f;

	public float minWidthTetherLength = 3;
	public float minTetherWidth = .05f;
	public float maxTetherWidth = .2f;
	public bool recalling = false;
	public bool gripping = true;
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
	private Vector3 tetherVector;

	void Awake () {
		rb = GetComponent<Rigidbody2D> ();
	}

	void Start () {
		lr = GetComponent<LineRenderer> ();
		lr.sortingLayerName = GetComponentInChildren<SpriteRenderer>().sortingLayerName;
	}

	void Update () {
		DrawTether ();
	}

	void FixedUpdate () {
		tetherVector = transform.position - harpooner.transform.position;
		tetherLength = tetherVector.magnitude;
		if (tetherLength > maxTetherLength) {
			SetGripping (false);
			Destroy (gameObject);
			Debug.Log ("snapped harp off");
		} else {
			HandleTether ();
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.name == "BirdTrigger") {
			Bird pBird = other.transform.parent.GetComponent<Bird> ();
			if (harpooner.name == pBird.name) {
				if (!pBird.aimingHarp && !pBird.swingingHarp && !pBird.throwingHarp) {
					pBird.hurledHarp = false;
					pBird.catchingHarp = true;
					Destroy (gameObject);
				}
			} else {
				//implement linking
				if (gripping) {
					HarpoonObject (pBird.gameObject);
					pBird.otherHarps.Add (this);
				}
			}
		}

		else if (other.tag == "Harpoonable") {
			if (gripping) {
				HarpoonObject (other.gameObject);
				other.GetComponent<Harpoonable> ().SetSortingLayer ("Birds");
			}
		}
	}

	public void Fire (Transform start, Vector2 aim) {
		transform.position = start.position;
		transform.rotation = start.rotation;
		aim.Normalize ();
		rb.AddForce (aim * forceMag);
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
		if (distance < tautLength) {
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

	public GameObject GetHarpooner () {
		return harpooner;
	}

	public GameObject GetHarpooned () {
		return harpooned;
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

	public void ToggleRecalling () {
		if (recalling) {
			SetRecalling (false);
		} else {
			SetRecalling (true);
		}	
	}

	public void SetRecalling (bool setRecalling) {
		recalling = setRecalling;
	}

	public void ToggleGripping () {
		if (gripping) {
			SetGripping (false);
		} else {
			SetGripping (true);
		}
	}

	public void SetGripping (bool setGripping) {
		if (setGripping != gripping) {
			SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer> ();
			if (gripping) {
				foreach (SpriteRenderer r in renderers) {
					r.color = new Color (r.color.r, r.color.g, r.color.b, .5f);
					lr.material.color = new Color (lr.material.color.r, lr.material.color.g, lr.material.color.b, .2f);
				}
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
				gripping = false;
			} else {
				foreach (SpriteRenderer r in renderers) {
					r.color = new Color (r.color.r, r.color.g, r.color.b, 1f);
				}
				lr.material.color = new Color (lr.material.color.r, lr.material.color.g, lr.material.color.b, 1f);
				gripping = true;
			}
		} 
		//else setGripping is the same gripping, do nothing
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

	void HandleRecalling () {
		Vector3 recallDir = harpooner.GetComponent<Bird> ().p.transform.position - transform.position;
		SetTautLength (recallDir.magnitude);
		recallDir.Normalize ();

		if (harpooned) {
			//TODO need to create a function AddEffectiveForcess or something for effective mass handling
			harpooned.GetComponent<Rigidbody2D> ().AddForce (recallDir * recallMag);
		} else {
			rb.AddForce (recallDir * recallMag);
		}
		//TODO would need to chceck if harpooner is harpooned and has an effective mass
		harpooner.GetComponent<Rigidbody2D> ().AddForce (-recallDir * recallMag);
	}

	void SetTautLength (float newLength) {
		//stretchiness should be some % set elsewhere of the tautlength
		tautLength = newLength;
		maxTetherLength = tautLength * (1 + stretchiness);
	}

	void HandleTether () {
		if (tetherLength > tautLength) {
			float x = tetherLength - tautLength;
			tetherDirection = transform.position - harpooner.transform.position;
			tetherDirection.Normalize ();
			Vector3 springForce = -k * x * tetherDirection;
			if (!harpooned) {
				rb.AddForce (springForce);
			} else {
				harpooned.GetComponent<Rigidbody2D> ().AddForce (springForce);
			}
			harpooner.GetComponent<Rigidbody2D> ().AddForce (-springForce);
		}

		if (recalling) {
			HandleRecalling ();
		}
	}
}
