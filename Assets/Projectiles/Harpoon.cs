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
	public float detachForceMag	= 100f;
	public float detachDelay = .3f;
	public GameObject harpooner = null;
	public GameObject harpooned = null;

	private Rigidbody2D rb;
	private Vector3[] tetherPositions = new Vector3[2];
	private bool recalling = false;

	void Awake () {
		rb = GetComponent<Rigidbody2D> ();
	}

	void Update () {
		DrawTether ();
	}

	void FixedUpdate () {
		if (recalling) {
			Vector3 detachDir = harpooner.transform.position - transform.position;
			detachDir.Normalize ();
			rb.AddForce (detachDir * detachForceMag);
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

		if (distance < minWidthTetherLength) {
			atMaxTether = false;
			lr.material.color = Color.blue;

		} else if (distance < tetherMaxLength + minWidthTetherLength) {
			atMaxTether = false;
			lr.material.color = Color.gray;
		}
		else {
			lr.material.color = Color.red;
			atMaxTether = true;
		} 
	}

	/*void CreateTether () {
		Vector3 instantiatePoint = (transform.position - harpooner.transform.position) / 2;
		tether = Instantiate (prefabChain, instantiatePoint, Quaternion.identity) as GameObject;
		Chain invisibleChain = tether.GetComponent<Chain> ();
		invisibleChain.A = harpooner;
		if (harpooned == null) {
			invisibleChain.B = gameObject;
		} else {
			invisibleChain.B = harpooned;
		}
		List<GameObject> prefabList = new List<GameObject> ();
		prefabList.Add (prefabChainLink);
		invisibleChain.getPrefabList = prefabList;
		atMaxTether = true;
	}

	void BreakTether () {
		Destroy (tether);
		atMaxTether = false;
	}*/

	public void SetHarpooner (GameObject harpoonThrower) {
		harpooner = harpoonThrower;
	}

	public void HarpoonObject (GameObject harpoonRecipient) {
		harpooned = harpoonRecipient;
		GetComponent<Rigidbody2D> ().Sleep ();
		GetComponent<BoxCollider2D> ().enabled = false;
		//after turning off rb and collider, move harp if on player
		if (harpoonRecipient.tag == "Player") {
			transform.position = harpooned.transform.position;
		}
		transform.parent = harpooned.transform;
		SpriteRenderer[] harpoonSprites = GetComponentsInChildren<SpriteRenderer> ();
		foreach (SpriteRenderer r in harpoonSprites) {
			r.enabled = false;
		}
	}

	public void Recall (bool overrideToggle=false) {
		if (harpooned != null) {
			harpooned = null;
			GetComponent<Rigidbody2D> ().WakeUp ();
			GetComponent<BoxCollider2D> ().enabled = false;
			transform.parent = null;
			SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer> ();
			foreach (SpriteRenderer r in renderers) {
				r.enabled = true;
				r.color = new Color (r.color.r, r.color.g, r.color.b, .5f);
			}
			Invoke ("TurnOn", detachDelay);
		}
		if (overrideToggle) {
			recalling = true;
		} else {
			recalling = !recalling;
		}
	}

	void TurnOn () {
		GetComponent<BoxCollider2D> ().enabled = true;
		SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer> ();
		foreach (SpriteRenderer r in renderers) {
			r.color = new Color (r.color.r, r.color.g, r.color.b, 1f);
		}
	}
}
