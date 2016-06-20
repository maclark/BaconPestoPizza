using UnityEngine;
using System.Collections;

public class SunMaker {
	public float sunFrequency = 20f;
	public float buffer = 5f;
	public float speedOfLight = 10f;
	public float rotationOfLight = 3f;

	public Vector3 sunlightDirection = Vector3.zero;
	public GameObject sunPrefab;

	private GameManager gm;


	public SunMaker () {
		gm = GameObject.FindObjectOfType<GameManager> ();
		if (sunlightDirection == Vector3.zero) {
			sunlightDirection = new Vector3 (1f, -1f, 0);
			sunlightDirection.Normalize ();
		}
	}

	public void MakeSunshine () {
		Sun sun = gm.sunPooler.GetPooledObject ().GetComponent<Sun> ();
		sun.gameObject.SetActive (true);
		sun.Fire (GetCameraBorderSpawnPosition (), sunlightDirection);
	}

	Vector3 GetCameraBorderSpawnPosition () {
		float halfWidth = Camera.main.orthographicSize * Screen.width / Screen.height;
		float halfHeight = Camera.main.orthographicSize;
		Vector2 botLeft = new Vector2 (Camera.main.transform.position.x - halfWidth, Camera.main.transform.position.y - halfHeight);
		bool spawnOnTop = Random.Range (0, 1f) > .5 ? true : false;
		float x;
		float y;
		if (spawnOnTop) {
			y = botLeft.y + 2 * halfHeight + buffer;
			x = Random.Range (botLeft.x, botLeft.x + 2 * halfWidth);
		} else {
			y = Random.Range (botLeft.y, botLeft.y + 2 * halfHeight);
			x = botLeft.x - buffer;
		}
		return new Vector3 (x, y, 0);
	}
}
