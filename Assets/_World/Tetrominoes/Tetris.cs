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
	public GameObject megaL;
	public GameObject megaLINE;
	public GameObject megaS;
	public GameObject megaSQUARE;
	public GameObject megaT;	
	public GameObject littleJ;
	public GameObject littleLINE;
	public GameObject littleS;
	public GameObject littleSQUARE;
	public GameObject littleT;
	public GameObject wallPrefab;
	public GameObject gatePrefab;
	public GameObject gateSpaceSaverPrefab;
	public GameObject invaderPrefab;
	public GameObject invaderCarrierPrefab;
	public GameObject pufferPrefab;
	public GameObject hunterPairPrefab;
	public GameObject alakazamPrefab;

	public float radius = 1000;
	public float width;
	public float height;
	public int tetroAttempts;
	public int megaAttempts;
	public int destrutibleSpots;
	public int destrutibleAttsPerSpot;
	public int difficultyMod = 2;
	public int invaderAttempts;
	public int invaderCarrierAttempts;
	public int pufferAttempts;
	public int hunterPairAttempts;
	public int alakazamAttempts;

	public Vector2 origin;
	public int columns;
	public int rows;
	public float roomWidth;
	public float roomHeight;


	public enum TetrominoBasicShape {L, LINE, S, SQUARE, T}
	public enum TetrominoShape {J, L, LINE, S, SQUARE, T, Z}
	public enum LittleTetrominoShape {littleJ, littleLINE, littleS, littleSQUARE, littleT}
	public enum MegaTetrominoShape {megaL, megaLINE, megaS, megaSQUARE, megaT}

	private float gateWidth;
	private GameObject gateSpaceSaver;
	private GameManager gm;
	private Spelunky spelunkyGod;

	void Awake () {
		gateWidth = gatePrefab.transform.localScale.x; 
		gm = GameObject.FindObjectOfType<GameManager> ();
	}


	// Use this for initialization
	void Start () {
		spelunkyGod = new Spelunky (origin, columns, rows, roomWidth, roomHeight, gateSpaceSaverPrefab, wallPrefab, 10f, this);
		spelunkyGod.BuildCave ();
		spelunkyGod.SaveCaveExits ();
		spelunkyGod.FillOffTrail ();
		SpawnRectangleField (origin, columns * roomWidth, rows * roomHeight, tetroAttempts);
		spelunkyGod.OutlineCave ();

		//spawn rectangle field
		//LayTetros ();
		//LayDestructibles ();
		//LayEnemies ();

		//MakeGateSpaceSaver ();
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

	public GameObject GetRandomMegaTetromino () {
		MegaTetrominoShape mtt = (MegaTetrominoShape)Random.Range (0, 5);

		switch (mtt) {
		case MegaTetrominoShape.megaL:
			return megaL;
			//break;
		case MegaTetrominoShape.megaLINE:
			return megaLINE;
			//break;
		case MegaTetrominoShape.megaS:
			return megaS;
			//break;
		case MegaTetrominoShape.megaSQUARE:
			return megaSQUARE;
			//break;
		case MegaTetrominoShape.megaT:
			return megaT;
			//break;
		default:
			return null;
			//break;
		}
	}

	public GameObject GetRandomTetromino () {
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

	public GameObject GetRandomLittleTetromino () {
		LittleTetrominoShape ltt = (LittleTetrominoShape)Random.Range (0, 5);

		switch (ltt) {
		case LittleTetrominoShape.littleJ:
			return littleJ;
			//break;
		case LittleTetrominoShape.littleLINE:
			return littleLINE;
			//break;
		case LittleTetrominoShape.littleS:
			return littleS;
			//break;
		case LittleTetrominoShape.littleSQUARE:
			return littleSQUARE;
			//break;
		case LittleTetrominoShape.littleT:
			return littleT;
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
			if (col.transform.parent) {
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
			tetro = GetRandomTetromino ();
		}
		ChangeColor (tetro.transform, Color.clear, true);
		tetro.transform.rotation = GetRandomTetrominoRotation ();
		BoxCollider2D[] colliders = tetro.GetComponentsInChildren<BoxCollider2D> ();
		if (colliders == null) {
			print ("this tetro has no colliders");
		}
		if (CheckSpaceClear (colliders, buffer)) {
			GameObject instance = Instantiate (tetro, transform.position, tetro.transform.rotation) as GameObject;
			instance.transform.parent = transform.parent;
		}
	}

	public void SpawnMegaTetromino (GameObject megaTetro) {
		if (!megaTetro) {
			megaTetro = GetRandomMegaTetromino ();
		}
		ChangeColor (megaTetro.transform, Color.clear, true);
		megaTetro.transform.rotation = GetRandomTetrominoRotation ();
		BoxCollider2D[] colliders = megaTetro.GetComponentsInChildren<BoxCollider2D> ();
		if (CheckSpaceClear (colliders, buffer)) {
			GameObject instance = Instantiate (megaTetro, transform.position, megaTetro.transform.rotation) as GameObject;
			instance.transform.parent = transform.parent;
		} else {
			Debug.Log ("space not clear for mega");
		}
	}

	public void SpawnDestructible (GameObject littleTetro) {
		if (!littleTetro) {
			littleTetro = GetRandomLittleTetromino ();
		}
		ChangeColor (littleTetro.transform, Color.clear, true);
		littleTetro.transform.rotation = GetRandomTetrominoRotation ();
		BoxCollider2D[] colliders = littleTetro.GetComponentsInChildren<BoxCollider2D> ();
		if (CheckSpaceClear (colliders, buffer)) {
			GameObject instance = Instantiate (littleTetro, transform.position, littleTetro.transform.rotation) as GameObject;
			instance.transform.parent = transform.parent;
		}
	}

	void SpawnGameObject (GameObject obj) {
		BoxCollider2D[] colls = obj.GetComponentsInChildren<BoxCollider2D> ();
		if (colls.Length > 0) {
			if (CheckSpaceClear(colls, 0f)) {
				Instantiate(obj, transform.position, transform.rotation);
			}
		}

		CircleCollider2D cc = obj.GetComponent<CircleCollider2D> ();
		if (cc) {
			if (CheckSpaceClear(cc, 0f)) {
				Instantiate(obj, transform.position, transform.rotation);
			}
		}
	}

	void SpawnEnemies (Vector2 botLeft, float fieldWidth, float fieldHeight, int invAttempts, int invCarAttempts, int puffAttempts, int hunterAttempts, int alkAtts) {
		for (int i = 0; i < invAttempts; i++) {
			float x = Random.Range (botLeft.x, botLeft.x + fieldWidth);
			float y = Random.Range (botLeft.y, botLeft.y+ fieldHeight);
			transform.position = new Vector2 (x, y);
			SpawnGameObject (invaderPrefab);
		}

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

		for (int i = 0; i < hunterAttempts; i++) {
			float x = Random.Range (botLeft.x, botLeft.x + fieldWidth);
			float y = Random.Range (botLeft.y, botLeft.y+ fieldHeight);
			transform.position = new Vector2 (x, y);
			SpawnGameObject (hunterPairPrefab);
		}

		for (int i = 0; i < alkAtts; i++) {
			float x = Random.Range (botLeft.x, botLeft.x + fieldWidth);
			float y = Random.Range (botLeft.y, botLeft.y+ fieldHeight);
			transform.position = new Vector2 (x, y);
			SpawnGameObject (alakazamPrefab);
		}
	}

	public void SpawnRectangleField (Vector2 botLeft, float fieldWidth, float fieldHeight, int attempts) {
		//Spawn tetros, avoiding space saver
		//move space saver 
		//spawn gate
		//spawn destructibles
		//spawn enemies

		for (int i = 0; i < attempts; i++) {
			float x = Random.Range (botLeft.x, botLeft.x + fieldWidth);
			float y = Random.Range (botLeft.y, botLeft.y+ fieldHeight);
			transform.position = new Vector2 (x, y);
			SpawnTetromino (null);
		}
	
		int iAtts = Mathf.RoundToInt (Mathf.Log (invaderAttempts * difficultyMod * (gm.gatesBroken + 1)));
		int icAtts = Mathf.RoundToInt (Mathf.Log (invaderCarrierAttempts * difficultyMod * (gm.gatesBroken - 1)));
		int pufAtts = Mathf.RoundToInt (Mathf.Log (pufferAttempts * difficultyMod * (gm.gatesBroken + 1)));
		int huntPairAtts = Mathf.RoundToInt (Mathf.Log (hunterPairAttempts * difficultyMod * (gm.gatesBroken + 1)));
		int alkAtts = Mathf.RoundToInt (Mathf.Log (alakazamAttempts * difficultyMod * (gm.gatesBroken + 1)));

		SpawnDestructibles (botLeft, fieldWidth, fieldHeight, destrutibleSpots, destrutibleAttsPerSpot);

		SpawnEnemies (botLeft, fieldWidth, fieldHeight, iAtts, icAtts, pufAtts, huntPairAtts, alkAtts);
	}

	public void SpawnObstacleField (Vector2 botLeft, float fieldWidth, float fieldHeight) {
		
	}

	public void SpawnCircleField (Vector2 origin, float radius, float circleAttempts) {
		for (int i = 0; i < circleAttempts; i++) {
			float theta = Random.Range (0, 360f);
			float r = Random.Range (0f, radius);
			transform.position = origin + GetVectorFromAngleAndMag (theta, r);
			SpawnTetromino (null);
		}
	}

	void SpawnDestructibles (Vector2 botLeft, float fieldWidth, float fieldHeight, int spots, int attemptsPerSpot) {
		for (int i = 0; i < spots; i++) {
			for (int j = 0; j < attemptsPerSpot; j++) {
				float x = Random.Range (botLeft.x, botLeft.x + fieldWidth);
				float y = Random.Range (botLeft.y, botLeft.y + fieldHeight);
				transform.position = new Vector2 (x, y);
				SpawnDestructible (null);
			}
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
	public int roomsFilled = 0;
	public void FillRoom (Room r) {
		if (r.onTrail) {
			roomsFilled++;
			SpawnRectangleField (r.botLeft, r.width, r.height, Mathf.RoundToInt (tetroAttempts / (columns * rows)));
		} else {
			SpawnObstacleField (r.botLeft, r.width, r.height);			
		}
	}
}

