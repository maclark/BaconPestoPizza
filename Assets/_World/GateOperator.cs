using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GateOperator : MonoBehaviour {
	public Transform gate;
	public bool pickRandomColor = false;
	public bool pickRandomGate = false;
	public bool pressureGate = false;
	public Kanga kanga;

	private List<Transform> occupants = new List<Transform> ();
	private Vector3 startPosition;
	private Color color;

	void Start () {
		if (!pickRandomGate) {
			startPosition = gate.transform.position;
		}

		if (!pickRandomColor) {
			color = GetComponent<SpriteRenderer> ().color;
			SetColors ();
		}
	}

	void Update () {
		if (occupants.Count > 0) {
			RaiseGate ();
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "Player") {
			occupants.Add (other.transform);
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (pressureGate) {
			if (other.tag == "Player") {
				occupants.Remove (other.transform);
				if (occupants.Count == 0) {
					LowerGate ();
				}
			}
		}
	}

	public void SetGateAndColor (Transform g, Color c) {
		gate = g;
		color = c;
		SetColors ();
		startPosition = gate.transform.position;
	}

	void RaiseGate () {
		float x = startPosition.x;
		float y = startPosition.y + transform.localScale.y * 2;
		gate.position = new Vector3 (x, y, 0);
		if (kanga) {
			kanga.gameObject.SetActive (true);
		}
	}

	void LowerGate () {
		gate.position = startPosition;
	}
		
	void SetColors () {
		GetComponent<SpriteRenderer> ().color = color;
		gate.GetComponent<SpriteRenderer> ().color = color;
	}
}
