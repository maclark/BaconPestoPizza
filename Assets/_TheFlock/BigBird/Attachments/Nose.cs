using UnityEngine;
using System.Collections;

public class Nose : MonoBehaviour {

	void Update () {
		transform.rotation = transform.parent.rotation;
	}
}
