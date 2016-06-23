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
	public GameObject portalPrefab;
	public GameObject gateSpaceSaverPrefab;
	public GameObject invaderPrefab;
	public GameObject eliteInvaderPrefab;
	public GameObject invaderCarrierPrefab;
	public GameObject eliteCarrierPrefab;
	public GameObject pufferPrefab;
	public GameObject bigPufferPrefab;
	public GameObject hunterPairPrefab;
	public GameObject alakazamPrefab;
	public GameObject waterPrefab;

	public float radius = 1000;
	public float width;
	public float height;
	public int tetroAttempts;
	public int megaAttempts;
	public int destructibleSpots;
	public int destructibleAttsPerSpot;
	public int difficultyMod;
	public int invaderAttempts;
	public int eliteInvaderAttempts;
	public int invaderCarrierAttempts;
	public int eliteCarrierAttempts;
	public int pufferAttempts;
	public int bigPufferAttempts;
	public int hunterPairAttempts;
	public int alakazamAttempts;
	public int waterAttempts;
	public int shopAttempts;

	public Vector2 origin;
	public int columns;
	public int rows;
	public float roomWidth;
	public float roomHeight;


	public enum TetrominoBasicShape {L, LINE, S, SQUARE, T}
	public enum TetrominoShape {J, L, LINE, S, SQUARE, T, Z}
	public enum LittleTetrominoShape {littleJ, littleLINE, littleS, littleSQUARE, littleT}
	public enum MegaTetrominoShape {megaL, megaLINE, megaS, megaSQUARE, megaT}

	private GameObject spaceSaverPrefab;
	private GameManager gm;
	private Spelunky spelunkyGod;

	void Awake () {
		gm = GameObject.FindObjectOfType<GameManager> ();
	}


	void Start () {
		SpawnNewZone ();
	}
	
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			SpawnShopField (origin, columns * roomWidth, rows * roomHeight, shopAttempts);
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

	public void SpawnNewZone () {
		spelunkyGod = new Spelunky (origin, columns, rows, roomWidth, roomHeight, gateSpaceSaverPrefab, wallPrefab, 10f, portalPrefab);
		spelunkyGod.BuildCave ();
		spelunkyGod.SaveCaveExits ();
		spelunkyGod.PlacePortal ();
		SpawnCastleField (origin, columns * roomWidth, rows * roomHeight, shopAttempts);
		SpawnMegaTetrosOffTrail ();
		SpawnWaterField (origin, columns * roomWidth, rows * roomHeight, waterAttempts);
		SpawnRectangleField (origin, columns * roomWidth, rows * roomHeight, tetroAttempts);
		//spelunkyGod.OutlineCave ();
		SpawnShopField (origin, columns * roomWidth, rows * roomHeight, shopAttempts);
		SpawnEnemyField (origin, columns * roomWidth, rows * roomHeight);
		SpawnDestructibles (origin, columns * roomWidth, rows * roomHeight, destructibleSpots, destructibleAttsPerSpot);
		spelunkyGod.DestroySpaceSavers ();
		if (gm.zoneNumber > 0) {
			gm.StartCoroutine (gm.WarpBigBird (spelunkyGod.bottomExitVector));
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
		} else {
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

	void SpawnObject (GameObject obj, Transform parent) {
		BoxCollider2D[] colls = obj.GetComponentsInChildren<BoxCollider2D> ();
		if (colls.Length > 0) {
			if (CheckSpaceClear(colls, 0f)) {
				GameObject go = Instantiate(obj, transform.position, transform.rotation) as GameObject;
				go.transform.parent = parent;
			}
		}

		CircleCollider2D cc = obj.GetComponent<CircleCollider2D> ();
		if (cc) {
			if (CheckSpaceClear(cc, 0f)) {
				GameObject go = Instantiate(obj, transform.position, transform.rotation) as GameObject;
				go.transform.parent = gm.enemyContainer.transform;
			}
		}
	}

	void SpawnEnemies (Vector2 botLeft, float fieldWidth, float fieldHeight, int invAttempts, int eInvAttempts, int invCarAttempts, 
		int eCarAttempts, int puffAttempts, int bigPuffAttempts, int hunterAttempts, int alkAtts) {

		for (int i = 0; i < invAttempts; i++) {
			float x = Random.Range (botLeft.x, botLeft.x + fieldWidth);
			float y = Random.Range (botLeft.y, botLeft.y+ fieldHeight);
			transform.position = new Vector2 (x, y);
			SpawnObject (invaderPrefab, gm.enemyContainer.transform);
		}

		for (int i = 0; i < eInvAttempts; i++) {
			float x = Random.Range (botLeft.x, botLeft.x + fieldWidth);
			float y = Random.Range (botLeft.y, botLeft.y+ fieldHeight);
			transform.position = new Vector2 (x, y);
			SpawnObject (eliteInvaderPrefab, gm.enemyContainer.transform);
		}

		for (int i = 0; i < invCarAttempts; i++) {
			float x = Random.Range (botLeft.x, botLeft.x + fieldWidth);
			float y = Random.Range (botLeft.y, botLeft.y+ fieldHeight);
			transform.position = new Vector2 (x, y);
			SpawnObject (invaderCarrierPrefab, gm.enemyContainer.transform);
		}

		for (int i = 0; i < eCarAttempts; i++) {
			float x = Random.Range (botLeft.x, botLeft.x + fieldWidth);
			float y = Random.Range (botLeft.y, botLeft.y+ fieldHeight);
			transform.position = new Vector2 (x, y);
			SpawnObject (eliteCarrierPrefab, gm.enemyContainer.transform);
		}

		for (int i = 0; i < puffAttempts; i++) {
			float x = Random.Range (botLeft.x, botLeft.x + fieldWidth);
			float y = Random.Range (botLeft.y, botLeft.y+ fieldHeight);
			transform.position = new Vector2 (x, y);
			SpawnObject (pufferPrefab, gm.enemyContainer.transform);
		}

		for (int i = 0; i < bigPuffAttempts; i++) {
			float x = Random.Range (botLeft.x, botLeft.x + fieldWidth);
			float y = Random.Range (botLeft.y, botLeft.y+ fieldHeight);
			transform.position = new Vector2 (x, y);
			SpawnObject (bigPufferPrefab, gm.enemyContainer.transform);
		}

		for (int i = 0; i < hunterAttempts; i++) {
			float x = Random.Range (botLeft.x, botLeft.x + fieldWidth);
			float y = Random.Range (botLeft.y, botLeft.y+ fieldHeight);
			transform.position = new Vector2 (x, y);
			SpawnObject (hunterPairPrefab, gm.enemyContainer.transform);
		}

		for (int i = 0; i < alkAtts; i++) {
			float x = Random.Range (botLeft.x, botLeft.x + fieldWidth);
			float y = Random.Range (botLeft.y, botLeft.y+ fieldHeight);
			transform.position = new Vector2 (x, y);
			SpawnObject (alakazamPrefab, gm.enemyContainer.transform);
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
			float y = Random.Range (botLeft.y, botLeft.y + fieldHeight);
			transform.position = new Vector2 (x, y);
			SpawnTetromino (null);
		}
	}

	void SpawnShopField (Vector2 botLeft, float fieldWidth, float fieldHeight, int attempts) {
		for (int i = 0; i < attempts; i++) {
			float x = Random.Range (botLeft.x, botLeft.x + fieldWidth);
			float y = Random.Range (botLeft.y, botLeft.y + fieldHeight);
			transform.position = new Vector2 (x, y);
			BoxCollider2D[] colls = gm.shopPrefab.GetComponentInChildren<Shopkeeper> ().GetColliderChild ();
			if (CheckSpaceClear(colls, 0f)) {
				GameObject go = Instantiate (gm.shopPrefab, transform.position, transform.rotation) as GameObject;
				go.transform.parent = gm.buildingContainer.transform;

				//cap one shop per spawnshopfield call
				return;
			}
		}
	}

	void SpawnCastleField (Vector2 botLeft, float fieldWidth, float fieldHeight, int attempts) {
		int loops = 0;
		while (true) {
			loops++;
			if (loops > 300) {
				return;
			}
			float x = Random.Range (botLeft.x, botLeft.x + fieldWidth);
			float y = Random.Range (botLeft.y, botLeft.y + fieldHeight);
			transform.position = new Vector2 (x, y);
			BoxCollider2D[] colls = gm.castlePrefab.GetComponentInChildren<Castle> ().GetColliderChild ();
			if (CheckSpaceClear(colls, 0f)) {
				GameObject go = Instantiate (gm.castlePrefab, transform.position, transform.rotation) as GameObject;
				go.transform.parent = gm.buildingContainer.transform;

				//cap one shop per spawnshopfield call
				return;
			}
		}
	}

	void SpawnWaterField (Vector2 botLeft, float fieldWidth, float fieldHeight, int attempts) {
		for (int i = 0; i < attempts; i++) {
			float x = Random.Range (botLeft.x, botLeft.x + fieldWidth);
			float y = Random.Range (botLeft.y, botLeft.y + fieldHeight);
			transform.position = new Vector2 (x, y);
			GameObject instance = Instantiate (waterPrefab, transform.position, Quaternion.identity) as GameObject;
			instance.transform.parent = transform.parent;
		}
	}

	void SpawnEnemyField (Vector2 botLeft, float fieldWidth, float fieldHeight) {
		int iAtts = Mathf.RoundToInt (Mathf.Log (invaderAttempts * difficultyMod * (gm.zoneNumber + 1)));
		int eiAtts = Mathf.RoundToInt (Mathf.Log (eliteInvaderAttempts * difficultyMod * (gm.zoneNumber)));
		int icAtts = Mathf.RoundToInt (Mathf.Log (invaderCarrierAttempts * difficultyMod * (gm.zoneNumber + 1)));
		int ecAtts = Mathf.RoundToInt (Mathf.Log (eliteCarrierAttempts * difficultyMod * (gm.zoneNumber - 1)));
		int pufAtts = Mathf.RoundToInt (Mathf.Log (pufferAttempts * difficultyMod * (gm.zoneNumber + 1)));
		int bpufAtts = Mathf.RoundToInt (Mathf.Log (bigPufferAttempts * difficultyMod * (gm.zoneNumber)));
		int huntPairAtts = Mathf.RoundToInt (Mathf.Log (hunterPairAttempts * difficultyMod * (gm.zoneNumber - 2)));
		int alkAtts = Mathf.RoundToInt (Mathf.Log (alakazamAttempts * difficultyMod * (gm.zoneNumber + 1)));

		SpawnEnemies (botLeft, fieldWidth, fieldHeight, iAtts, eiAtts, icAtts, ecAtts, pufAtts, bpufAtts, huntPairAtts, alkAtts);
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

	public void SpawnMegaTetrosOffTrail () {
		foreach (Room r in spelunkyGod.rooms) {
			if (!r.onTrail) {
				for (int i = 0; i < megaAttempts; i++) {
					float x = Random.Range (r.botLeft.x, r.botLeft.x + roomWidth);
					float y = Random.Range (r.botLeft.y, r.botLeft.y + roomHeight);
					transform.position = new Vector2 (x, y);
					SpawnMegaTetromino (null);
				}
			}
		}
	}


}

