using UnityEngine;
using System.Collections;

public class Medkit : MonoBehaviour {

	private GameManager gm;
	private float xOffset;
	private float yOffset;

	void Awake () {
		gm = GameObject.FindObjectOfType<GameManager> ();
		xOffset = transform.localPosition.x;
		yOffset = transform.localPosition.y;
	}

	public void Reset () {
		Vector3 rightOffset = gm.bigBird.transform.right * xOffset * gm.bigBird.transform.localScale.x;
		Vector3 upOffset = gm.bigBird.transform.up * yOffset * gm.bigBird.transform.localScale.y;
		transform.position = gm.bigBird.transform.position + rightOffset + upOffset;
	}
}
