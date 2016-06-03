﻿using UnityEngine;
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
	public GameObject wallPrefab;
	public GameObject gatePrefab;
	public GameObject gateSpaceSaverPrefab;
	public GameObject invaderCarrierPrefab;
	public GameObject pufferPrefab;

	public float radius = 1000;
	public float width;
	public float height;
	public int attempts;
	public int invaderCarrierAttempts;
	public int pufferAttempts;

	public enum TetrominoBasicShape {L, LINE, S, SQUARE, T}
	public enum TetrominoShape {J, L, LINE, S, SQUARE, T, Z}

	private float gateWidth;
	private GameObject gateSpaceSaver;

	void Awake () {
		gateWidth = gatePrefab.transform.localScale.x; 
	}


	// Use this for initialization
	void Start () {
		MakeGateSpaceSaver ();
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

	public GameObject GetRandomTermino () {
		TetrominoBasicShape tt = (TetrominoBasicShape)Random.Range (0, 5);

		switch (tt) {
		case TetrominoBasicShape.L:
			GameObject JL = Random.Range (0, 2) > 0 ? L : J;
			return JL;
			//break;
		case TetrominoBasicShape.LINE:
			return LINE;
			//break;
		case TetrominoBasicShape.S:
			GameObject SZ = Random.Range (0, 2) > 0 ? S : Z;
			return SZ;
			//break;
		case TetrominoBasicShape.SQUARE:
			return SQUARE;
			//break;
		case TetrominoBasicShape.T:
			return T;
			//break;
		default:
			return null;
			//break;
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

	public bool CheckSpaceClear(CircleCollider2D possibleCollider, float buff) {
		Vector2 spawnPosition = transform.position + possibleCollider.transform.position;
		RaycastHit2D hit = Physics2D.CircleCast (spawnPosition, possibleCollider.radius, Vector2.zero); 
		if (hit.transform != null) {
				return false;
		}
		return true;
	}

	public bool CheckSpaceClear(BoxCollider2D[] possibleColliders, float buff) {
		for (int i = 0; i < possibleColliders.Length; i++) {
			BoxCollider2D col = possibleColliders [i];
			Vector2 boxSize = new Vector2 (col.transform.localScale.x + buff * 2, col.transform.localScale.y + buff * 2);
			Vector2 spawnPosition = transform.position + col.transform.position;
			float angle = 0f;
			if (collider.transform.parent) {
				angle = col.transform.parent.rotation.eulerAngles.z;
			}
			RaycastHit2D hit = Physics2D.BoxCast (spawnPosition, boxSize, angle, Vector2.zero, 0f); 
			if (hit.transform != null) {
				return false;
			}
		}
		return true;
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
			instance.transform.parent = transform.parent;
		}
	}


	void SpawnGameObject (GameObject obj) {
		BoxCollider2D[] colls = obj.GetComponentsInChildren<BoxCollider2D> ();
		if (colls.Length > 0) {
			if (CheckSpaceClear(colls, 0f)) {
				GameObject instance = Instantiate(obj, transform.position, transform.rotation) as GameObject;
			}
		}

		CircleCollider2D cc = obj.GetComponent<CircleCollider2D> ();
		if (cc) {
			if (CheckSpaceClear(cc, 0f)) {
				GameObject instance = Instantiate(obj, transform.position, transform.rotation) as GameObject;
			}
		}
	}

	void SpawnEnemies (Vector2 botLeft, float fieldWidth, float fieldHeight, int invCarAttempts, int puffAttempts) {
		for (int i = 0; i < invCarAttempts; i++) {
			float x = Random.Range (botLeft.x, botLeft.x + fieldWidth);
			float y = Random.Range (botLeft.y, botLeft.y+ fieldHeight);
			transform.position = new Vector2 (x, y);
			SpawnGameObject (invaderCarrierPrefab);
		}

		for (int i = 0; i < puffAttempts; i++) {
			float x = Random.Range (botLeft.x, botLeft.x + fieldWidth);
			float y = Random.Range (botLeft.y, botLeft.y+ fieldHeight);
			transform.position = new Vector2 (x, y);
			SpawnGameObject (pufferPrefab);
		}
	}

	public void SpawnRectangleField (Vector2 botLeft, float fieldWidth, float fieldHeight, int attempts) {
		for (int i = 0; i < attempts; i++) {
			float x = Random.Range (botLeft.x, botLeft.x + fieldWidth);
			float y = Random.Range (botLeft.y, botLeft.y+ fieldHeight);
			transform.position = new Vector2 (x, y);
			SpawnTetromino (null);
		}

		MoveGateSpaceSaver ();
		Spawn3Sides (botLeft, fieldWidth, fieldHeight);
		SpawnEnemies (botLeft, fieldWidth, fieldHeight, invaderCarrierAttempts, pufferAttempts);
	}

	public void SpawnCircleField (Vector2 origin, float radius, float circleAttempts) {
		for (int i = 0; i < circleAttempts; i++) {
			float theta = Random.Range (0, 360f);
			float r = Random.Range (0f, radius);
			transform.position = origin + GetVectorFromAngleAndMag (theta, r);
			SpawnTetromino (null);
		}
	}

	void Spawn3Sides (Vector2 botLeft, float fieldWidth, float fieldHeight) {
		Vector3 leftWallSpawn = new Vector3 (botLeft.x, botLeft.y + fieldHeight / 2, 0);
		GameObject leftWall = Instantiate (wallPrefab, leftWallSpawn, Quaternion.identity) as GameObject;
		leftWall.transform.localScale = new Vector3 (5f, fieldHeight, 0);
		leftWall.transform.parent = transform.parent;

		Vector3 rightWallSpawn = new Vector3 (botLeft.x + fieldWidth, botLeft.y + fieldHeight / 2, 0);
		GameObject rightWall = Instantiate (wallPrefab, rightWallSpawn, Quaternion.identity) as GameObject;
		rightWall.transform.localScale = new Vector3 (5f, fieldHeight, 0);
		rightWall.transform.parent = transform.parent;

		Vector3 topLeftSpawn = new Vector3 (botLeft.x + (fieldWidth - gateWidth) / 4, botLeft.y + fieldHeight * 5 / 6);
		GameObject topLeftWall = Instantiate (wallPrefab, topLeftSpawn, Quaternion.identity) as GameObject;
		topLeftWall.transform.localScale = new Vector3 ((fieldWidth - gateWidth) / 2, 5f, 0);
		topLeftWall.transform.parent = transform.parent;

		Vector3 topRightSpawn = new Vector3 (botLeft.x + fieldWidth - (fieldWidth - gateWidth) / 4, botLeft.y + fieldHeight * 5 / 6);
		GameObject topRightWall = Instantiate (wallPrefab, topRightSpawn, Quaternion.identity) as GameObject;
		topRightWall.transform.localScale = new Vector3 ((fieldWidth - gateWidth) / 2, 5f, 0);
		topRightWall.transform.parent = transform.parent;

		Vector3 gateSpawn = new Vector3 (botLeft.x + fieldWidth / 2, botLeft.y + fieldHeight * 5 / 6, 0);
		GameObject gate = Instantiate (gatePrefab, gateSpawn, Quaternion.identity) as GameObject;
		gate.transform.parent = transform.parent;
	}

		
	//TODO this is temporaray
	void MakeGateSpaceSaver () {
		gateSpaceSaver = Instantiate (gateSpaceSaverPrefab, new Vector3 (0, height * 5 / 6, 0), Quaternion.identity) as GameObject;
		gateSpaceSaver.transform.localScale = new Vector2 (gateWidth, gateWidth);
		gateSpaceSaver.transform.parent = transform.parent;
	}
	void MoveGateSpaceSaver () {
		gateSpaceSaver.transform.position = new Vector3 (0, gateSpaceSaver.transform.position.y + height, 0);
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