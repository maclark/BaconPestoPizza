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
		//Hatchling h = t.GetComponent<Hatchling> ();
		if (e) {
			e.inCoop = false;
		}
		full = false;
		occupants.Remove (t);	
	}
}
