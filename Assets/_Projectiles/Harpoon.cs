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
	public float hoseWidth = .4f;
	public bool recalling = false;
	public bool gripping = true;
	public bool taut = false;
	public bool isWaterHose = false;
	public bool isSolarHose = false;
	public bool electrocuting = false;
	public OrbWeb web = null;
	//public List<Positron> posis = new List<Positron> ();
	//public Circuit circuit;
	public Powerbird powerbirdie = null;
	public GameObject webPrefab;
	public GameObject orbWebPrefab;
	public GameObject positronPrefab;

	private GameManager gm;
	private Rigidbody2D rb;
	private LineRenderer lr;
	private Vector3[] tetherPositions = new Vector3[2];
	private GameObject harpooner = null;
	private GameObject harpooned = null;
	private List<Bird> linkedBirds = new List<Bird> ();
	private Vector3 tetherVector;
	private int posis;
	private float fullTetherLength;
	private float fullTautLength;

	void Awake () {
		gm = GameObject.FindObjectOfType<GameManager> ();
		rb = GetComponent<Rigidbody2D> ();
	}

	void Start () {
		gameObject.name = "Harpoon";
		lr = GetComponent<LineRenderer> ();
		lr.sortingLayerName = GetComponentInChildren<SpriteRenderer>().sortingLayerName;
		lr.material.color = harpooner.GetComponent<Bird> ().rider.color;
		fullTetherLength = maxTetherLength;
		fullTautLength = tautLength;
		Bird harpB = harpooner.GetComponent<Bird> ();
		if (harpB.hasBattery) {
			Electrocute ();
		}
	}

	void Update () {
		DrawTether ();
		if (electrocuting) {
			if (!harpooner.GetComponent<Bird> ().hasBattery) {
				Ground ();
			}
		} else if (harpooner.GetComponent<Bird> ().hasBattery) {
			//Electrocute ();
		}
	}

	void FixedUpdate () {
		tetherVector = transform.position - harpooner.transform.position;
		tetherLength = tetherVector.magnitude;
		if (tetherLength > maxTetherLength + 1) {
			ClampHarp ();
			if (harpooned) {
				SetGripping (false);
			}
		} else if (tetherLength > maxTetherLength) {
			ClampHarp ();
		} 
		HandleTether ();
	}

	void OnTriggerEnter2D (Collider2D other) { 
		Bird pBird = other.transform.GetComponent<Bird> ();
		Harpoonable hool = other.GetComponent<Harpoonable> ();
		if (!hool) {
			return;
		} else if (!hool.enabled) {
			return;
		}

		if (!pBird) {
			if (other.transform.parent) {
				pBird = other.transform.parent.GetComponent<Bird> ();
			}
		}
		if (pBird) {
			if (harpooner.transform == pBird.transform) {
				if (!pBird.aimingHarp && !pBird.swingingHarp && !pBird.throwingHarp) {
					pBird.hurledHarp = false;
					pBird.catchingHarp = true;
					Destroy (gameObject);
				}
			} else {
				if (gripping) {
					HarpoonObject (pBird.gameObject);
					pBird.otherHarps.Add (this);
				}
			}
		} else if (gripping) {
			HarpoonObject (other.gameObject);
		}
	}

	public void Fire (Transform start, Vector2 aim) {
		transform.position = start.position;
		transform.rotation = start.rotation;
		//aim.Normalize ();
		rb.AddForce (aim * forceMag);
	}

	public void Die () {
		SetGripping (false);
		CancelInvoke ();
		Destroy (gameObject);
	}

	void DrawTether () {
		tetherPositions [0] = harpooner.GetComponent<Bird> ().rider.transform.position;

		if (harpooned != null) {
			tetherPositions [1] = harpooned.transform.position;
		} else {
			tetherPositions [1] = transform.position;
		}
		lr.SetPositions (tetherPositions);

		float distance = Vector3.Distance (tetherPositions[0], tetherPositions[1]);
		//for determining when to stop thinning the line renderered
		if (distance < tautLength) {
			taut = false;
		}
		else {
			//lr.material.color = Color.red;
			taut = true;
		} 

		float tetherWidth;
		if (isWaterHose || isSolarHose) {
			tetherWidth = hoseWidth;
		} else {
			tetherWidth = Mathf.Lerp (maxTetherWidth, minTetherWidth, (distance - minWidthTetherLength) / tautLength);
		}
		lr.SetWidth (tetherWidth, tetherWidth);
	}

	public void SetHarpooner (GameObject harpoonThrower) {
		harpooner = harpoonThrower;
	}

	public GameObject GetHarpooner () {
		return harpooner;
	}

	public GameObject GetHarpooned () {
		if (!harpooned) {
			return null;
		} else {
			return harpooned;
		}
	}

	public void HarpoonObject (GameObject harpoonRecipient) {
		harpooned = harpoonRecipient;
		GetComponent<Rigidbody2D> ().Sleep ();
		Collider2D coll = GetComponent<Collider2D> ();
		if (coll) {
			coll.enabled = false;
		}
		transform.position = harpooned.transform.position;
		transform.parent = harpooned.transform;

		if (harpoonRecipient.name == "WaterTank") {
			ChangeTetherToHose (true, false);
		} else if (harpoonRecipient.name == "EnergyTank") {
			ChangeTetherToHose (false, true);
		}

		harpoonRecipient.SendMessageUpwards ("BeenHarpooned", this, SendMessageOptions.DontRequireReceiver);
		harpoonRecipient.GetComponent<Harpoonable> ().SetSortingLayer ("Birds");
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
		if (setGripping == false) {
			//if (circuit != null) {
			//	circuit.BreakCircuit ();
			//}
		}

		if (isWaterHose || isSolarHose) {
			ChangeHoseToTether ();
		}

		if (setGripping != gripping) {
			SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer> ();
			if (gripping) {
				foreach (SpriteRenderer r in renderers) {
					r.color = Color.gray;
					r.color = new Color (r.color.r, r.color.g, r.color.b, .5f);
					lr.material.color = new Color (lr.material.color.r, lr.material.color.g, lr.material.color.b, .2f);
				}
				if (harpooned != null) {
					harpooned.SendMessageUpwards ("HarpoonReleased", this, SendMessageOptions.DontRequireReceiver);
					harpooned = null;
					transform.parent = null;
					GetComponent<Rigidbody2D> ().WakeUp ();
					GetComponent<BoxCollider2D> ().enabled = true;
				}
				gripping = false;
			} else {
				foreach (SpriteRenderer r in renderers) {
					r.color = Color.black;
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
		Vector3 recallDir = harpooner.GetComponent<Bird> ().rider.transform.position - transform.position;
		SetTautLength (recallDir.magnitude);
		recallDir.Normalize ();

		if (harpooned) {
			//TODO need to create a function AddEffectiveForcess or something for effective mass handling
			harpooned.GetComponent<Harpoonable> ().RiBo ().AddForce (recallDir * recallMag);
		} else {
			rb.AddForce (recallDir * recallMag);
		}
		//TODO would need to chceck if harpooner is harpooned and has an effective mass
		harpooner.GetComponent<Harpoonable> ().RiBo ().AddForce (-recallDir * recallMag);
	}

	void SetTautLength (float newLength) {
		if (isWaterHose || isSolarHose) {
			return;
		}
		//stretchiness should be some % set elsewhere of the tautlength
		tautLength = Mathf.Min (tautLength, newLength);
		maxTetherLength = Mathf.Min (maxTetherLength, tautLength + tautLength * stretchiness);
	}

	void HandleTether () {
		if (tetherLength > tautLength) {
			//print ("applying spring force");
			float x = tetherLength - tautLength;
			tetherDirection = transform.position - harpooner.transform.position;
			tetherDirection.Normalize ();
			Vector3 springForce = -k * x * tetherDirection;
			if (!harpooned) {
				rb.AddForce (springForce);
			} else {
				//harpooned.GetComponent<Harpoonable> ().RiBo ().AddForce (springForce);
				if (isWaterHose || isSolarHose) {
					harpooned.GetComponent<Harpoonable> ().GetComponentInParent<Rigidbody2D> ().AddForce (springForce);
				} else {
					harpooned.GetComponent<Harpoonable> ().GetComponent<Rigidbody2D> ().AddForce (springForce);
				}
			}
			//harpooner.GetComponent<Harpoonable> ().RiBo ().AddForce (-springForce);
			harpooner.GetComponent<Harpoonable> ().GetComponent<Rigidbody2D> ().AddForce (-springForce);
		}

		if (recalling) {
			HandleRecalling ();
		}
	}

	void TetherHarp () {
		//determine direction of tether and orthogonal axis to that
		Vector2 directionOfTether = transform.position - harpooner.transform.position;
		directionOfTether.Normalize ();
		Vector2 orthoDirectionOfTether = new Vector2 (directionOfTether.y, -directionOfTether.x);

		//find components of velocity along new axes
		Vector2 harpVel = rb.velocity;
		float harpVelTetherComponent = Vector2.Dot (harpVel, directionOfTether);

		float harpVelOrthoComponent = Vector2.Dot (harpVel, orthoDirectionOfTether);

		//if harpoon still moving away from harpooner, stop it
		if (harpVelTetherComponent > 0) {
			harpVelTetherComponent = 0;
		}

		//Add components of velocity to find resultant velocity vector of harpoon
		Vector2 resultantHarpVel = (directionOfTether * harpVelTetherComponent) + (orthoDirectionOfTether * harpVelOrthoComponent);
		rb.velocity = resultantHarpVel;
	}

	void ClampHarp () {
		//print ("applying clamp");
		float r = tetherVector.magnitude;
		float theta = Mathf.Atan2 (tetherVector.y, tetherVector.x);
		r = Mathf.Clamp (r, 0, maxTetherLength); 
		float x = r * Mathf.Cos (theta);
		float y = r * Mathf.Sin (theta);
		if (harpooned) {
			//TODO for relative mass adjustment. Current method just makes lighter mass move.
			/*
			float harpoonedMass = harpooned.GetComponent<Harpoonable> ().GetDirectionalMass (harpooner.transform.position);
			float harpoonerMass = harpooner.GetComponent<Harpoonable> ().GetDirectionalMass (harpooned.transform.position);
			float harpoonedPercentage = harpoonedMass / (harpoonedMass + harpoonerMass);
			float harpoonerPercentage = 1 - harpoonedMass;
			*/
			float harpoonedMass = harpooned.GetComponent<Harpoonable> ().GetDirectionalMass (harpooner.transform.position);
			float harpoonerMass = harpooner.GetComponent<Harpoonable> ().GetDirectionalMass (harpooned.transform.position);
			if (harpoonerMass >= harpoonedMass) {
				Vector3 newHarpoonedPos = new Vector3 (harpooner.transform.position.x + x, harpooner.transform.position.y + y, 0);
				harpooned.transform.position = newHarpoonedPos;
				transform.position = newHarpoonedPos;
			} else {
				Vector3 newHarpoonerPos = new Vector3 (harpooned.transform.position.x - x, harpooned.transform.position.y - y, 0);
				harpooner.transform.position = newHarpoonerPos;
			}
		}
	}

	void SendPositron () {
		if (gm.positronContainer.GetComponentsInChildren<Transform> ().Length < 100) {
			Positron posi = gm.positronPooler.GetPooledObject ().GetComponent<Positron> ();
			posi.gameObject.SetActive (true);
			posi.previousTarget = GetHarpooner ().GetComponent<Bird> ().rider.transform;
			posi.transform.position = posi.previousTarget.position;
			if (harpooned) {
				posi.currentTarget = harpooned.transform;
			} else {
				posi.currentTarget = transform;
			}
			posi.distanceAway = Vector3.Distance (transform.position, posi.currentTarget.position);
		}
		//posis.Add (posi);
	}

	/*void DestroyPositrons () {
		List<Positron> doomedPosis = new List<Positron> ();
		foreach (Positron p in posis) {
			doomedPosis.Add (p);
		}

		foreach (Positron p in doomedPosis) {
			doomedPosis.Die ();
		}
	}*/

	void ChangeTetherToHose (bool waterHose, bool solarHose) {
		if (waterHose) {
			isWaterHose = true;
			lr.material.color = Color.blue;
			Player p = harpooner.GetComponentInChildren<Player> ();
			p.Equip (new Hose (p.w.hol));
		} else if (solarHose) {
			Player p = harpooner.GetComponentInChildren<Player> ();
			p.Equip (new Unarmed (p.w.hol));
			SolarPanel sp = p.b.GetPanel ();
			if (sp) {
				sp.AdjustAbsorbing ();
			}
			isSolarHose = true;
			lr.material.color = Color.grey;
		}
		maxTetherLength = fullTetherLength * 2f;
		tautLength = fullTautLength * 2f;
	}

	void ChangeHoseToTether () {
		isWaterHose = false;
		isSolarHose = false;
		Player p = harpooner.GetComponentInChildren<Player> ();
		p.Unequip ();
		lr.material.color = harpooner.GetComponentInChildren<Bird> ().rider.color;
	}

	public void SetTetherColor (Color c) {
		lr.material.color = c;
	}

	public void Electrocute () {
		InvokeRepeating ("SendPositron", 0f, 1 / Positron.amps);
	}

	public void Ground () {
		CancelInvoke ();
		electrocuting = false;
	}
}
