using UnityEngine;
using System.Collections;

public class Harpoonable : MonoBehaviour {

	public Harpoon harp;
	public bool isGold;

	private GameObject barrier;

	void Start () {
		if (GetComponent<Joint2D> ()) {
			barrier = GetComponent<Joint2D> ().connectedBody.gameObject;
		}
	}

	void OnJointBreak2D (Joint2D brokenJoint) {
		GetComponent<BoxCollider2D> ().isTrigger = false;
		Destroy (barrier);
	}

	public void Die () {
		if (harp) {
			harp.DetachAndRecall ();
		}
		Destroy (gameObject);
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
}
