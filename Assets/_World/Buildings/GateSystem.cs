using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GateSystem : MonoBehaviour {

	public GateOperator[] gateOps;
	public List<Transform> gates = new List<Transform> ();
	public List<Color> gateColors = new List<Color> ();

	void Start () {
		SetColors ();
		ShuffleColors ();
		ShuffleGates ();
		AssignGatesAndColors ();
	}

	void SetColors () {
		gateColors.Add (Color.red);
		gateColors.Add (Color.blue);
		gateColors.Add (Color.green);
		gateColors.Add (Color.yellow);
		gateColors.Add (Color.cyan);
		gateColors.Add (Color.magenta);
		gateColors.Add (Color.black);
		gateColors.Add (Color.white);
	}

	void ShuffleColors () {
		for (int i = 0; i < gateColors.Count; i++) {
			Color temp = gateColors [i];
			int randomIndex = Random.Range(i, gateColors.Count);
			gateColors [i] = gateColors [randomIndex];
			gateColors [randomIndex] = temp;
		}
	}

	void ShuffleGates () {
		for (int i = 0; i < gates.Count; i++) {
			Transform temp = gates [i];
			int randomIndex = Random.Range(i, gates.Count);
			gates [i] = gates [randomIndex];
			gates [randomIndex] = temp;
		}
	}

	void AssignGatesAndColors () {
		for (int i = 0; i < gateOps.Length; i++) {
			gateOps [i].SetGateAndColor (gates [i], gateColors [i]);
		}
	}
}
