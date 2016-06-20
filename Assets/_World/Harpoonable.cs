using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Harpoonable : MonoBehaviour {

	//public Harpoon harp;
	public RelRB rrb;
	public Vector2 relPos;

	private GameManager gm;
	private List<Harpoon> otherHarps = new List<Harpoon> ();


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
		gm = GameObject.FindObjectOfType<GameManager> ();
		relPos = transform.localPosition;
	}

	public Rigidbody2D RiBo () {
		if (transform.GetComponent<SolarPanel> ()) {
			return GetComponent<Rigidbody2D> ();
		}

		if (transform.parent) {
			if (transform.parent.GetComponent<Rigidbody2D> ()) {
				return transform.parent.GetComponent<Rigidbody2D> ();
			}
		} 

		if (GetComponent<Rigidbody2D> ()) {
			return GetComponent<Rigidbody2D> ();
		} 

		return null;
	}

	public void SetRelRB () {
		if (transform.parent.GetComponent<Rigidbody2D> ()) {
			rrb = new RelRB (transform.parent.GetComponent<Rigidbody2D> (), transform.localPosition);
		} else {
			rrb = null;
		}
	}

	public void BreakLoose () {
		List<Harpoon> toRemove = new List<Harpoon> ();
		foreach (Harpoon har in otherHarps) {
			toRemove.Add (har);
		}

		foreach (Harpoon har in toRemove) {
			har.SetGripping (false);
		}
	}

	//receives message
	public void BeenHarpooned (Harpoon h) {
		otherHarps.Add (h);
	}

	//receives message
	public void HarpoonReleased (Harpoon h) {
		otherHarps.Remove (h);
	}

	public void IsPierced () {
	}

	public void SetSortingLayer (string layerName) {
		if (transform.parent) {
			if (transform.parent == gm.bigBird.transform) {
				return;
			}
		}
		if (GetComponent<SpriteRenderer> ()) {
			GetComponent<SpriteRenderer> ().sortingLayerName = layerName;
		} else if (GetComponentInChildren<SpriteRenderer> ()) {
			GetComponentInChildren<SpriteRenderer> ().sortingLayerName = layerName;
		}
	}

	public float GetDirectionalMass (Vector3 pullerPosition) {
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

	public List<Harpoon> GetOtherHarps () {
		return otherHarps;
	}
}
