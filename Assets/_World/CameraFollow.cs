using UnityEngine;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour {

	public float smoothTime = 0.3f;

	private Vector3 velocity = Vector3.zero;
	private GameManager gm;

	void Awake () {
		gm = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
	}

	void FixedUpdate () {
		List<Transform> targetTransforms = gm.GetAlliedTransforms ();
		float x = 0f;
		float y = 0f;
		foreach (Transform t in targetTransforms) {
			x += t.position.x;
			y += t.position.y;
		}

		x = x / targetTransforms.Count;
		y = y / targetTransforms.Count;

		Vector3 goalPos = new Vector3 (x, y, transform.position.z);
		transform.position = Vector3.SmoothDamp (transform.position, goalPos, ref velocity, smoothTime);
	}
}