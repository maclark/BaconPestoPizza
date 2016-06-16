using UnityEngine;
using System.Collections;

public class ConveyorBelt : MonoBehaviour {

	public bool cwCorner = false;
	public bool ccwCorner = false;

	// Update is called once per frame
	void Update () {
	
	}

	public void SwitchDirection () {
		Transform[] trans = GetComponentsInChildren<Transform> ();
		for (int i = 0; i < trans.Length; i++) {
			Vector3 ea = trans [i].eulerAngles;
			trans [i].eulerAngles = new Vector3 (0, 0, ea.z + 180f); 
		}

		Vector3 eaBelt = transform.eulerAngles;
		if (ccwCorner) {
			transform.eulerAngles = new Vector3 (0, 0, eaBelt.z + 90f); 
			ccwCorner = false;
			cwCorner = true;
		} else if (cwCorner) {
			transform.eulerAngles = new Vector3 (0, 0, eaBelt.z - 90f); 
			ccwCorner = true;
			cwCorner = false;
		} else {
			transform.eulerAngles = new Vector3 (0, 0, eaBelt.z - 180f);
		}

		GetComponent<AreaEffector2D> ().forceMagnitude = -GetComponent<AreaEffector2D> ().forceMagnitude;
	}
}
