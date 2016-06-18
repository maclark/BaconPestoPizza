using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Coop : MonoBehaviour {

	public bool full = false;
	public int capacity = 3;

	private List<Transform> occupants = new List<Transform> ();


	public bool AddOccupant (Transform t) {
		if (full)
			return false;

		occupants.Add (t);
		Egg e = t.GetComponent<Egg> ();
		if (e) {
			e.inCoop = true;
		}

		if (occupants.Count >= capacity) {
			full = true;
		}
		return true;
	}

	public void RemoveOccupant (Transform t) {
		Egg e = t.GetComponent<Egg> ();
		if (e) {
			e.inCoop = false;
		}
		full = false;
		occupants.Remove (t);	
	}
}


/*using UnityEngine;
using System.Collections;

public class Coop : MonoBehaviour {

	public bool full = false;
	public int capacity = 3;
	
	private float[] slotOffsets;
	private Transform[] occupants;

	void Awake () {
		
	}

	void Start () {
		slotOffsets = new float [capacity];
		float coopWidth = GetComponent<BoxCollider2D> ().size.x;
		float slotWidth = coopWidth / capacity;
		for (int i = 0; i < capacity; i++) {
			float x = (-coopWidth / 2) + slotWidth / 2 + i * slotWidth;
			slotOffsets [i] = x;
		}

		occupants = new Transform[capacity];
		for (int i = 0; i < occupants.Length; i++) {
			occupants [i] = null;
		}
	}

	public bool AddOccupant (Transform t) {
		if (full)
			return false;

		bool addedOccupant = false;

		for (int i = 0; i < occupants.Length; i++) {
			if (occupants [i] == null) {
				occupants [i] = t;
				print (slotOffsets [i] + " position.x " + transform.position.x);
				t.position = transform.position + transform.right * slotOffsets[i];
				addedOccupant = true;
				break;
			}
		}

		Egg e = t.GetComponent<Egg> ();
		if (e) {
			e.inCoop = true;
		}

		full = true;
		for (int i = 0; i < occupants.Length; i++) {
			if (occupants [i] == null) {
				full = false;
			}
		}

		return addedOccupant;
	}

	public void RemoveOccupant (Transform t) {
		for (int i = 0; i < occupants.Length; i++) {
			if (occupants [i] == t) {
				occupants [i] = null;
			}
		}
		Egg e = t.GetComponent<Egg> ();
		//Hatchling h = t.GetComponent<Hatchling> ();
		if (e) {
			e.inCoop = false;
		}
		full = false;
	}
}
*/