using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Circuit {

	public float amps;
	public float singleBatteryAmps = 5f;
	public bool hasBattery = false;
	public List<Bird> birds = new List<Bird> ();
	public List<Positron> posis = new List<Positron> ();

	public Circuit () {
	}
		

	public void AttemptCircuit (Bird birdie) {
		birds.Add (birdie);
		if (!hasBattery && birdie.hasBattery) {
			hasBattery = true;
			amps += singleBatteryAmps;
		}
		//first check if harpooned
		if (birdie.harp) {
			if (birdie.harp.GetHarpooned ()) {
				if (birdie.harp.GetHarpooned ().GetComponent<Bird> ()) {
					Bird birdiesBird = birdie.harp.GetHarpooned ().GetComponent<Bird> ();
					if (birdiesBird == birds [0]) {
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
			//b.harp.circuit = this;
			if (b.hasBattery) {
				b.harp.InvokeRepeating ("SendPositron", 0f, 1 / amps);
			}
		}


	}

	public void BreakCircuit () {
		foreach (Bird b in birds) {
			b.circuit = null;
			//b.harp.circuit = null;
			b.harp.CancelInvoke ();
		}

		List<Positron> damnedPosis = new List<Positron> ();
		foreach (Positron posi in posis) {
			damnedPosis.Add (posi);
		}
		foreach (Positron posi in damnedPosis) {
			posi.Die ();
		}
	}
}
