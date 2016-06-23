using UnityEngine;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour {
	public float widthThreshPercent = .9f;
	public float heightThreshPercent = .9f;
	public float incMod = 1.1f;
	public float decMod = .8f;
	public float minSize = 14f;
	public float maxSize = 24f;
	public float smoothTime = 0.3f;
	public float smoothSizeTime = 0.1f;
	public float offsetMag = 10f;
	public float screenIncreaseMod = 2f;
	public bool followBigBird = true;
	public Vector3 offsetDir = Vector3.zero;

	private bool followGoal = true;
	private Vector3 velocity = Vector3.zero;
	private float currentVelocity = 0f;
	private float sizeMod;
	private GameManager gm;
	private bool increaseSize;

	void Awake () {
		gm = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
	}

	void FixedUpdate () {
		if (followGoal) {
			List<Transform> targetTransforms = gm.GetAlliedTransforms ();
			float xSum = 0f;
			float ySum = 0f;
			increaseSize = false;
			foreach (Transform t in targetTransforms) {
				xSum += t.position.x;
				ySum += t.position.y;
				Vector3 screenPos = Camera.main.WorldToScreenPoint (t.position);
				if (screenPos.x > Screen.width * widthThreshPercent || screenPos.x < Screen.width * (1 - widthThreshPercent) ||
					screenPos.y > Screen.height * heightThreshPercent || screenPos.y < Screen.height * (1 - heightThreshPercent)) {
					increaseSize = true;
				}
			}
			float xAvg = xSum / targetTransforms.Count;
			float yAvg = ySum / targetTransforms.Count;


			Vector3 goalPos;
			if (followBigBird) {
				goalPos = new Vector3 (xAvg + offsetDir.x * offsetMag, yAvg + offsetDir.y * offsetMag, transform.position.z);
			} else {
				goalPos = new Vector3 (xAvg, yAvg, transform.position.z);
			}
			transform.position = Vector3.SmoothDamp (transform.position, goalPos, ref velocity, smoothTime);

			float currentSize = Camera.main.orthographicSize;
			if (increaseSize) {
				sizeMod = incMod;
			} else {
				sizeMod = decMod;
			}
			Camera.main.orthographicSize = Mathf.SmoothDamp (currentSize, currentSize * sizeMod, ref currentVelocity, smoothSizeTime);
			Camera.main.orthographicSize = Mathf.Clamp (Camera.main.orthographicSize, minSize, maxSize);
		}
	}

	public void SetToRoom (Vector3 botLeft, float width, float height) {
		Camera.main.orthographicSize = height / 2;
		transform.position = new Vector3 (botLeft.x + width / 2, botLeft.y + height / 2, 0);
		followGoal = false;
	}
}