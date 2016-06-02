using UnityEngine;
using System.Collections;

public class Pump : MonoBehaviour {
	public bool update = true;

	private GameManager gm;
	private LineRenderer lr;
	private bool reset = false;
	private bool moveToBird = false;
	private Vector3 birdPos = Vector3.zero;
	private Vector3[] positions = new Vector3[2];
	private float yOffset;
	private float xOffset;

	void Start () {
		gm = GameObject.FindObjectOfType<GameManager> ();
		lr = GetComponent<LineRenderer> ();	
		lr.sortingLayerName = "BigBird";
		lr.sortingOrder = 2;
		xOffset = transform.localPosition.x;
		yOffset = transform.localPosition.y;
	}
	
	void Update () {
		//TODO this has a lot of needless functions that i made during debugging and could be streamlined.
		//also, this script could be enabled/disabled when bigbird.gasline.count > 0.
		//would have to adjust how the birdGettingGas variable works however.
		if (moveToBird) {
			transform.position = birdPos;
			moveToBird = false;
		}
		else if (reset) {
			Vector3 rightOffset = gm.bigBird.transform.right * xOffset * gm.bigBird.transform.localScale.x;
			Vector3 upOffset = gm.bigBird.transform.up * yOffset * gm.bigBird.transform.localScale.y;
			transform.position = gm.bigBird.transform.position + rightOffset + upOffset;
			reset = false;
		}
		DrawPumpHose ();
	}

	void DrawPumpHose () {
		Vector3 upOffset = gm.bigBird.transform.up * yOffset * gm.bigBird.transform.localScale.y;
		Vector3 rightOffset = gm.bigBird.transform.right * xOffset * gm.bigBird.transform.localScale.x;
		positions [0] = transform.position;
		positions [1] = gm.bigBird.transform.position + rightOffset + upOffset;
		lr.SetPositions (positions);
	}

	public void ResetHose () {
		reset = true;
	}

	public void MoveToBirdPos (Vector3 pos) {
		moveToBird = true;
		birdPos = pos;
	}
}
