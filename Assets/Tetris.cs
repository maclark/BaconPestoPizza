using UnityEngine;
using System.Collections;

public class Tetris : MonoBehaviour {
	public float buffer = 10f;
	public GameObject J;
	public GameObject L;
	public GameObject LINE;
	public GameObject S;
	public GameObject SQUARE;
	public GameObject T;
	public GameObject Z;

	public float radius = 1000;
	public float width;
	public float height;
	public int attempts;

	public enum TetrominoBasicShape {L, LINE, S, SQUARE, T}
	public enum TetrominoShape {J, L, LINE, S, SQUARE, T, Z}

	// Use this for initialization
	void Start () {
		SpawnRectangleField (new Vector2 (-width / 2, 0), width, height, attempts);
		//SpawnCircleField (Vector2.zero, radius, attempts);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			SpawnTetromino (null);
		} else if (Input.GetKeyDown (KeyCode.L)) {
			SpawnTetromino (L);
		} else if (Input.GetKeyDown (KeyCode.I)) {
			SpawnTetromino (LINE);
		} else if (Input.GetKeyDown (KeyCode.S)) {
			SpawnTetromino (S);
		} else if (Input.GetKeyDown (KeyCode.Q)) {
			SpawnTetromino (SQUARE);
		} else if (Input.GetKeyDown (KeyCode.T)) {
			SpawnTetromino (T);
		} 
	}

	public void SpawnTetromino (GameObject tetro) {
		if (!tetro) {
			tetro = GetRandomTermino ();
		}
		ChangeColor (tetro.transform, Color.clear, true);
		tetro.transform.rotation = GetRandomTetrominoRotation ();
		BoxCollider2D[] colliders = tetro.GetComponentsInChildren<BoxCollider2D> ();
		if (CheckSpaceClear (colliders, buffer)) {
			GameObject instance = Instantiate (tetro, transform.position, tetro.transform.rotation) as GameObject;
		}
	}

	public GameObject GetRandomTermino () {
		TetrominoBasicShape tt = (TetrominoBasicShape)Random.Range (0, 5);

		switch (tt) {
		case TetrominoBasicShape.L:
			GameObject JL = Random.Range (0, 2) > 0 ? L : J;
			return JL;
			break;
		case TetrominoBasicShape.LINE:
			return LINE;
			break;
		case TetrominoBasicShape.S:
			GameObject SZ = Random.Range (0, 2) > 0 ? S : Z;
			return SZ;
			break;
		case TetrominoBasicShape.SQUARE:
			return SQUARE;
			break;
		case TetrominoBasicShape.T:
			return T;
			break;
		default:
			return null;
			break;
		}
	}

	void ChangeColor (Transform obj, Color c, bool randomColor=false) {
		Color newColor;

		if (randomColor) {
			newColor = Random.ColorHSV ();
		} else {
			newColor = c;
		}

		SpriteRenderer[] srs = obj.GetComponentsInChildren<SpriteRenderer> ();
		for (int i = 0; i < srs.Length; i++) {
			srs [i].color = newColor;
		}
	}



	public bool CheckSpaceClear(BoxCollider2D[] possibleColliders, float buff) {
		for (int i = 0; i < possibleColliders.Length; i++) {
			BoxCollider2D col = possibleColliders [i];
			Vector2 boxSize = new Vector2 (col.transform.localScale.x + buff * 2, col.transform.localScale.y + buff * 2);
			Vector2 spawnPosition = transform.position + col.transform.position;
			RaycastHit2D hit = Physics2D.BoxCast (spawnPosition, boxSize, col.transform.parent.rotation.eulerAngles.z, Vector2.zero, 0f); 
			if (hit.transform != null) {
				return false;
			}
		}
		return true;
	}

	public void SpawnRectangleField (Vector2 botLeft, float width, float height, int attempts) {
		for (int i = 0; i < attempts; i++) {
			float x = Random.Range (botLeft.x, botLeft.x + width);
			float y = Random.Range (botLeft.y, botLeft.y+ height);
			transform.position = new Vector2 (x, y);
			SpawnTetromino (null);
		}
	}

	public void SpawnCircleField (Vector2 origin, float radius, float circleAttempts) {
		for (int i = 0; i < circleAttempts; i++) {
			float theta = Random.Range (0, 360f);
			float r = Random.Range (0f, radius);
			transform.position = origin + GetVectorFromAngleAndMag (theta, r);
			SpawnTetromino (null);
		}
	}

	private Vector2 GetVectorFromAngleAndMag (float theta, float mag) {
		float x = Mathf.Cos (theta);
		float y = Mathf.Sin (theta);
		return new Vector2 (x * mag, y * mag);
	}

	public Quaternion GetRandomTetrominoRotation () {
		float zRot = Random.Range (0f, 360f);
		return Quaternion.Euler (0, 0, zRot);	
	}
}
