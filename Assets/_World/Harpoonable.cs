using UnityEngine;
using System.Collections;

public class Harpoonable : MonoBehaviour {

	public Harpoon harp;
	public bool isGold;

	private GameObject destroyableObject;
	private GameManager gm;
	private Rigidbody2D rb;

	void Start () {
		if (GetComponent<Joint2D> ()) {
			destroyableObject = GetComponent<Joint2D> ().connectedBody.gameObject;
		}

		gm = GameObject.FindObjectOfType<GameManager> ();
		rb = GetComponent<Rigidbody2D> ();
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
			harp.SetGripping (false);
			harp.SetRecalling (true);
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

	public float GetEffectiveMass (Vector3 whalePosition, Vector3 pullingDirection) {
		float totalMass = rb.mass;
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
