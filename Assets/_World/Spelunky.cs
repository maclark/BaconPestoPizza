using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spelunky {
	public Room[,] rooms;
	public List<Vector2> caveExitVectors;

	private int columns;
	private int rows;
	private float roomWidth;
	private float roomHeight;
	private Vector2 botLeft;
	private int[] downRooms;
	private GameObject spaceSaverPrefab;
	private GameObject boundaryPrefab;
	private float exitWidth;
	private GameManager gm;
	private Vector3 bottomExitVector = Vector3.zero;
	private Vector3 topExitVector = Vector3.zero;
	private Tetris tetrisLord;


	public Spelunky (Vector2 bottomLeft, int c, int r, float rWidth, float rHeight, GameObject ss, GameObject wallPrefab, float exWidth, Tetris tetris) {
		botLeft = bottomLeft;
		columns = c;
		rows = r;
		roomWidth = rWidth;
		roomHeight = rHeight;
		spaceSaverPrefab = ss;
		boundaryPrefab = wallPrefab;
		exitWidth = exWidth;
		tetrisLord = tetris;

		caveExitVectors = new List<Vector2> ();
		rooms = new Room[columns, rows];
		downRooms = new int[rows];
		gm = GameObject.FindObjectOfType<GameManager> ();
	}
		
	public void BuildCave () {
		for (int i = 0; i < columns; i++) {
			for (int j = 0; j < rows; j++) {
				Vector2 roomBotLeft = new Vector2 (botLeft.x + roomWidth * i, botLeft.y + roomHeight * j);
				rooms [i, j] = new Room (roomBotLeft, roomWidth, roomHeight, this);
			}
		}

		for (int i = rows - 1; i >= 0; i--) {
			int col = Random.Range (0, columns);
			downRooms [i] = col;
		}
		BlazeRoomTrail ();
		Debug.Log ("finished room trail");
		DefineOtherRoomsExits ();
	}

	void BlazeRoomTrail () {
		int u = Random.Range (0, columns);
		//starting at top and going down
		for (int v = rows - 1; v >= 0; v--) {
			bool firstInRow = true;
			bool foundDownRoom = false;
			while (!foundDownRoom) {
				bool zoneTopRoom = false;
				bool movingRightRoom = false;
				bool movingLeftRoom = false;
				bool movingDownRoom = false;
				if (v == rows - 1 && firstInRow) {
					zoneTopRoom = true;
				}
				if (u < downRooms [v]) {
					//moving right
					AllowRightExits (rooms[u,v]);
					movingRightRoom = true;
					rooms [u, v].onTrail = true;
					rooms [u, v].PickExits (zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
					u++;
				} else if (u > downRooms [v]) {
					//moving left
					AllowLeftExits (rooms[u,v]);
					movingLeftRoom = true;
					rooms [u, v].onTrail = true;
					rooms [u, v].PickExits (zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
					u--;
				} else if (u == downRooms [v]) {
					//set room with bottom exit
					movingDownRoom = true;
					AllowBottomExits(rooms[u,v]);
					rooms [u, v].onTrail = true;
					rooms [u, v].PickExits (zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
					foundDownRoom = true;
				} else {
					Debug.LogError ("where is this room?");
				}

				if (firstInRow) {
					firstInRow = false;
				}
			}
		}
	}

	void AllowRightExits (Room r) {
		r.AddAllowedExit (Room.RoomExits.R);
		r.AddAllowedExit (Room.RoomExits.TR);
		r.AddAllowedExit (Room.RoomExits.LR);
		r.AddAllowedExit (Room.RoomExits.BR);
		r.AddAllowedExit (Room.RoomExits.TBR);
		r.AddAllowedExit (Room.RoomExits.TLR);
		r.AddAllowedExit (Room.RoomExits.BLR);
		r.AddAllowedExit (Room.RoomExits.TBLR);
	}

	void AllowLeftExits (Room r) {
		r.AddAllowedExit (Room.RoomExits.L);
		r.AddAllowedExit (Room.RoomExits.TL);
		r.AddAllowedExit (Room.RoomExits.LR);
		r.AddAllowedExit (Room.RoomExits.BL);
		r.AddAllowedExit (Room.RoomExits.TBL);
		r.AddAllowedExit (Room.RoomExits.TLR);
		r.AddAllowedExit (Room.RoomExits.BLR);
		r.AddAllowedExit (Room.RoomExits.TBLR);
	}

	void AllowBottomExits (Room r) {
		r.AddAllowedExit (Room.RoomExits.B);
		r.AddAllowedExit (Room.RoomExits.BL);
		r.AddAllowedExit (Room.RoomExits.BR);
		r.AddAllowedExit (Room.RoomExits.TB);
		r.AddAllowedExit (Room.RoomExits.TBL);
		r.AddAllowedExit (Room.RoomExits.TBR);
		r.AddAllowedExit (Room.RoomExits.BLR);
		r.AddAllowedExit (Room.RoomExits.TBLR);
	}

	void DefineOtherRoomsExits () {
		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < columns; j++) {
				if (rooms [i, j].exitsSet == false) {
					rooms [i, j].PickExits (false, false);
				}
			}
		}
	}

	public void PrintDetails () {
		for (int j = 0; j < rows; j++) {
			for (int i = 0; i < columns; i++) {
				if (rooms [i, j].onTrail) {
					Debug.Log ("rooms[" + i + ", " + j + "]");
				}
				//rooms [i, j].PrintDetails ();
			}
		}
	}

	public void OutlineCave () {
		SpawnBoundaries (botLeft, columns * roomWidth, rows * roomHeight);
		//LayBuildings ();
		//LayMegaTetros ();
	}

	public void SaveCaveExits () {
		foreach (Vector2 ev in caveExitVectors) {
			Vector3 spawn = new Vector3 (ev.x, ev.y, 0);
			GameObject exitSaver = GameObject.Instantiate (spaceSaverPrefab, spawn, Quaternion.identity) as GameObject;
			exitSaver.transform.localScale = new Vector2 (exitWidth, exitWidth);
			exitSaver.transform.parent = gm.exitContainer;

			if (bottomExitVector == Vector3.zero) {
				bottomExitVector = new Vector3 (ev.x, ev.y, 0);
			} else if (bottomExitVector.y > ev.y) {
				bottomExitVector = new Vector3 (ev.x, ev.y, 0);
			}

			if (topExitVector == Vector3.zero) {
				topExitVector = new Vector3 (ev.x, ev.y, 0);
			} else if (topExitVector.y < ev.y) {
				topExitVector = new Vector3 (ev.x, ev.y, 0);
			}
		}
	}

	void SpawnBoundaries (Vector2 botLeft, float boundaryWidth, float boundaryHeight) {
		Vector3 bottomLeftSpawn = new Vector3 ((botLeft.x + bottomExitVector.x - exitWidth / 2) / 2, botLeft.y, 0);
		GameObject bottomLeftWall = GameObject.Instantiate (boundaryPrefab, bottomLeftSpawn, Quaternion.identity) as GameObject;
		bottomLeftWall.transform.localScale = new Vector3 ((bottomExitVector.x - exitWidth / 2 - botLeft.x), 5f, 0);
		bottomLeftWall.transform.parent = gm.wallContainer;

		Vector3 bottomRightSpawn = new Vector3 ((botLeft.x + boundaryWidth + bottomExitVector.x + exitWidth / 2) / 2, botLeft.y, 0);
		GameObject bottomRightWall = GameObject.Instantiate (boundaryPrefab, bottomRightSpawn, Quaternion.identity) as GameObject;
		bottomRightWall.transform.localScale = new Vector3 (botLeft.x + boundaryWidth - (bottomExitVector.x + exitWidth / 2), 5f, 0);
		bottomRightWall.transform.parent = gm.wallContainer;

		Vector3 leftWallSpawn = new Vector3 (botLeft.x, botLeft.y + boundaryHeight / 2, 0);
		GameObject leftWall = GameObject.Instantiate (boundaryPrefab, leftWallSpawn, Quaternion.identity) as GameObject;
		leftWall.transform.localScale = new Vector3 (5f, boundaryHeight, 0);
		leftWall.transform.parent = gm.wallContainer;

		Vector3 rightWallSpawn = new Vector3 (botLeft.x + boundaryWidth, botLeft.y + boundaryHeight / 2, 0);
		GameObject rightWall = GameObject.Instantiate (boundaryPrefab, rightWallSpawn, Quaternion.identity) as GameObject;
		rightWall.transform.localScale = new Vector3 (5f, boundaryHeight, 0);
		rightWall.transform.parent = gm.wallContainer;

		Vector3 topLeftSpawn = new Vector3 ((botLeft.x + topExitVector.x - exitWidth / 2) / 2, botLeft.y + boundaryHeight);
		GameObject topLeftWall = GameObject.Instantiate (boundaryPrefab, topLeftSpawn, Quaternion.identity) as GameObject;
		topLeftWall.transform.localScale = new Vector3 ((topExitVector.x - exitWidth / 2 - botLeft.x), 5f, 0);
		topLeftWall.transform.parent = gm.wallContainer;

		Vector3 topRightSpawn = new Vector3 ((botLeft.x + boundaryWidth + topExitVector.x + exitWidth / 2) / 2, botLeft.y + boundaryHeight);
		GameObject topRightWall = GameObject.Instantiate (boundaryPrefab, topRightSpawn, Quaternion.identity) as GameObject;
		topRightWall.transform.localScale = new Vector3 (botLeft.x + boundaryWidth - (topExitVector.x + exitWidth / 2), 5f, 0);
		topRightWall.transform.parent = gm.wallContainer;
	}

	public void FillOffTrail () {
		foreach (Room r in rooms) {
			if (!r.onTrail) {
				for (int i = 0; i < tetrisLord.megaAttempts; i++) {
					float x = Random.Range (r.botLeft.x, r.botLeft.x + roomWidth);
					float y = Random.Range (r.botLeft.y, r.botLeft.y + roomHeight);
					tetrisLord.transform.position = new Vector2 (x, y);
					tetrisLord.SpawnMegaTetromino (null);
				}
			}
		}
	}

	public void MarkTrail () {
		GameObject trailMarkers = new GameObject ();
		foreach (Room r in rooms) {
			if (r.onTrail) {
				tetrisLord.transform.position = new Vector2 (r.botLeft.x + roomWidth / 2, r.botLeft.y + roomHeight / 2);
				tetrisLord.SpawnTetromino (null);
			}
		}
	}
}
