using UnityEngine;
using System.Collections;

public class Harpoonable : MonoBehaviour {

	public Harpoon harp;
	public RelRB rrb;
	public Vector2 relPos;

	private GameObject destroyableObject;
	private GameManager gm;

	public class RelRB {
		Rigidbody2D parentRB;
		Vector2 relativePos;

		public RelRB (Rigidbody2D rb, Vector2 relPos) {
			parentRB = rb;
			relativePos = relPos;
		}

		public void AddForceToParent (Vector3 force) {
			parentRB.AddForceAtPosition (force, relativePos);
		}
	}

	void Start () {
		if (GetComponent<Joint2D> ()) {
			destroyableObject = GetComponent<Joint2D> ().connectedBody.gameObject;
		}

		gm = GameObject.FindObjectOfType<GameManager> ();
		relPos = transform.localPosition;
	}

	void OnJointBreak2D (Joint2D brokenJoint) {
		GetComponent<BoxCollider2D> ().isTrigger = false;
		transform.parent = null;
		transform.localScale = new Vector3 (1, 1, 1);
		gm.BrokeGate ();
		Destroy (destroyableObject);
	}

	public Rigidbody2D RiBo () {
		Rigidbody2D rb;
		if (GetComponent<Rigidbody2D> ()) {
			rb = GetComponent<Rigidbody2D> ();
		} else if (transform.parent.GetComponent<Rigidbody2D> ()) {
			rb = transform.parent.GetComponent<Rigidbody2D> ();
		} else {
			rb = null;
		}
		return rb;
	}

	public void SetRelRB () {
		if (transform.parent.GetComponent<Rigidbody2D> ()) {
			rrb = new RelRB (transform.parent.GetComponent<Rigidbody2D> (), transform.localPosition);
		} else {
			rrb = null;
		}
	}

	public void Detach () {
		if (harp) {
			harp.SetGripping (false);
		}
	}
	public void BeenHarpooned (Harpoon h) {
		harp = h;
	}
	public void HarpoonReleased () {
		harp = null;
	}

	public void IsPierced () {
	}

	public void SetSortingLayer (string layerName) {
		if (GetComponent<SpriteRenderer> ()) {
			GetComponent<SpriteRenderer> ().sortingLayerName = layerName;
		} else if (GetComponentInChildren<SpriteRenderer> ()) {
			GetComponentInChildren<SpriteRenderer> ().sortingLayerName = layerName;
		}
	}

	public float GetEffectiveMass (Vector3 whalePosition, Vector3 pullingDirection) {
		float totalMass = RiBo ().mass;
		/*foreach (Harpoon h in otherHarps) {
			float m = h.GetHarpooner ().GetComponent<Bird> ().GetEffectiveMass ();
			Vector3 v = whalePosition - transform.position;
			float theta = Vector3.Angle (pullingDirection, v);
			if (theta < 90f) {
				theta = theta * Mathf.Deg2Rad;
				totalMass += m * Mathf.Cos (theta);
			}
		}*/
		return totalMass;
	}
}
