using UnityEngine;
using System.Collections;

public class Harpoonable : MonoBehaviour {

	public Harpoon harp;
	public bool isGold;

	private GameObject destroyableObject;
	private GameManager gm;

	void Start () {
		if (GetComponent<Joint2D> ()) {
			destroyableObject = GetComponent<Joint2D> ().connectedBody.gameObject;
		}

		gm = GameObject.FindObjectOfType<GameManager> ();
	}

	void OnJointBreak2D (Joint2D brokenJoint) {
		GetComponent<BoxCollider2D> ().isTrigger = false;
		transform.parent = null;
		transform.localScale = new Vector3 (1, 1, 1);
		gm.BrokeGate ();
		Destroy (destroyableObject);
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
