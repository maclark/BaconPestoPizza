using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Circuit {

	public float amps = 5f;
	public bool hasBattery = false;
	public Bird batteryBird;
	public List<Bird> birds = new List<Bird> ();

	public Circuit () {
	}

	public void AttemptCircuit (Bird birdie) {
		birds.Add (birdie);
		if (birdie.hasBattery) {
			batteryBird = birdie;
			hasBattery = true;
		}
		//first check if harpooned
		if (birdie.harp) {
			if (birdie.harp.GetHarpooned ()) {
				if (birdie.harp.GetHarpooned ().GetComponent<Bird> ()) {
					Bird birdiesBird = birdie.harp.GetHarpooned ().GetComponent<Bird> ();
					if (birdiesBird == birds [0]) {
						Debug.Log ("completed circuit, but maybe no battery");
						if (hasBattery) {
							CompleteCircuit ();
						}
					} else {
						AttemptCircuit (birdiesBird);
					}
				}
			}
		}
	}


	void CompleteCircuit () {
		foreach (Bird b in birds) {
			b.circuit = this;
			if (b == batteryBird) {
				b.harp.InvokeRepeating ("SendPositrons", 0f, 1 / amps);
			}
		}
	}

	public void BreakCircuit () {
		foreach (Bird b in birds) {
			b.circuit = null;
		}
	}
}
