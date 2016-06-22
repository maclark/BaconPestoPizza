using UnityEngine;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour {
	public float minSize = 14f;
	public float maxSize = 24f;
	public float smoothTime = 0.3f;
	public float smoothSizeTime = 0.1f;
	public float offsetMag = 10f;
	public float screenIncreaseMod = 2f;
	public Vector3 offsetDir = Vector3.zero;

	private bool followGoal = true;
	private Vector3 velocity = Vector3.zero;
	private float currentVelocity = 0f;
	private GameManager gm;

	void Awake () {
		gm = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
	}

	void FixedUpdate () {
		if (followGoal) {
			List<Transform> targetTransforms = gm.GetAlliedTransforms ();
			float xSum = 0f;
			float ySum = 0f;
			float highestY = -Mathf.Infinity;
			float lowestY = Mathf.Infinity;
			foreach (Transform t in targetTransforms) {
				if (t.position.y > highestY) {
					highestY = t.position.y;
				} else if (t.position.y < lowestY) {
					lowestY = t.position.y;
				}

				xSum += t.position.x;
				ySum += t.position.y;

			}

			float xAvg = xSum / targetTransforms.Count;
			float yAvg = ySum / targetTransforms.Count;

			Vector3 goalPos = new Vector3 (xAvg + offsetDir.x * offsetMag, yAvg + offsetDir.y * offsetMag, transform.position.z);
			//goalPos = Vector3.ClampMagnitude
			transform.position = Vector3.SmoothDamp (transform.position, goalPos, ref velocity, smoothTime);

			float size = screenIncreaseMod * (highestY - lowestY) / 2;
			//print ("screenIncreaseMod * (highestY - lowestY) / 2 : (screenIncreaseMod * " + highestY + " - " + lowestY + ") / 2 = " + size);
			Camera.main.orthographicSize = Mathf.SmoothDamp (Camera.main.orthographicSize, size, ref currentVelocity, smoothSizeTime);
			Camera.main.orthographicSize = Mathf.Clamp (Camera.main.orthographicSize, minSize, maxSize);
		}
	}

	public void SetToRoom (Vector3 botLeft, float width, float height) {
		Camera.main.orthographicSize = height / 2;
		transform.position = new Vector3 (botLeft.x + width / 2, botLeft.y + height / 2, 0);
		followGoal = false;
	}
}