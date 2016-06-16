using UnityEngine;
using System.Collections;

public class BeltSwitch : MonoBehaviour {
	public Transform arrow;

	private ConveyorBelt[] belts;

	// Use this for initialization
	void Start () {
		belts = GetComponentsInChildren<ConveyorBelt> ();
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "Player") {
			SwitchBelts ();
		}
	}

	void SwitchBelts () {
		for (int i = 0; i < belts.Length; i++) {
			belts [i].SwitchDirection ();
		}

		Vector3 ea = arrow.eulerAngles;
		arrow.eulerAngles = new Vector3 (0, 0, ea.z + 180f);
	}
}
