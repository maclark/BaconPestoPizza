using UnityEngine;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour {

	public float smoothTime = 0.3f;
	public Vector3 offsetDir = Vector3.zero;
	public float offsetMag = 10f;

	private bool followGoal = true;
	private Vector3 velocity = Vector3.zero;
	private GameManager gm;

	void Awake () {
		gm = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
	}

	void FixedUpdate () {
		if (followGoal) {
			List<Transform> targetTransforms = gm.GetAlliedTransforms ();
			float x = 0f;
			float y = 0f;
			foreach (Transform t in targetTransforms) {
				x += t.position.x;
				y += t.position.y;
			}

			x = x / targetTransforms.Count;
			y = y / targetTransforms.Count;

			Vector3 goalPos = new Vector3 (x + offsetDir.x * offsetMag, y + offsetDir.y * offsetMag, transform.position.z);
			transform.position = Vector3.SmoothDamp (transform.position, goalPos, ref velocity, smoothTime);
		}
	}

	public void SetToRoom (Vector3 botLeft, float width, float height) {
		Camera.main.orthographicSize = height / 2;
		transform.position = new Vector3 (botLeft.x + width / 2, botLeft.y + height / 2, 0);
		followGoal = false;
	}
}