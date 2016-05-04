using UnityEngine;
using System.Collections;

public class BigBird : MonoBehaviour {

	public float moveForce = 75f;

	private Vector2 direction = Vector2.up;
	private Rigidbody2D rb;
	private Component[] dockTransforms;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		rb.AddForce (direction * moveForce);
	}

	public Vector3 GetNearestOpenDock (Vector3 dockingShipPos) {
		dockTransforms = GetComponentsInChildren<Transform> ();
		float closestDockDistance = 100f;
		Vector3 closestDock = Vector3.zero;
		foreach (Transform t in dockTransforms) {
			float d = Vector3.Distance( t.position, dockingShipPos);
			if (d < closestDockDistance) {
				closestDock = t.position;
				closestDockDistance = d;
			}
		}
		return closestDock;
	}

}
