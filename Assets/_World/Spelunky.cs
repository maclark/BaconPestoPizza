﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spelunky {
	public Room[,] rooms;
	public List<Vector2> caveExitVectors;
	public Vector3 topExitVector = Vector3.zero;
	public Vector3 bottomExitVector = Vector3.zero;

	private int columns;
	private int rows;
	private float roomWidth;
	private float roomHeight;
	private Vector2 botLeft;
	private int[] downRooms;
	private GameObject spaceSaverPrefab;
	private GameObject boundaryPrefab;
	private GameObject portalPrefab;
	private float exitWidth;
	private GameManager gm;


	public Spelunky (Vector2 bottomLeft, int c, int r, float rWidth, float rHeight, GameObject ss, GameObject wallPrefab, float exWidth, GameObject portPrefab) {
		botLeft = bottomLeft;
		columns = c;
		rows = r;
		roomWidth = rWidth;
		roomHeight = rHeight;
		spaceSaverPrefab = ss;
		boundaryPrefab = wallPrefab;
		portalPrefab = portPrefab;
		exitWidth = exWidth;

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
					AllowRightExits (rooms[u,v], zoneTopRoom);
					movingRightRoom = true;
					rooms [u, v].onTrail = true;
					rooms [u, v].PickExits (zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
					u++;
				} else if (u > downRooms [v]) {
					//moving left
					AllowLeftExits (rooms[u,v], zoneTopRoom);
					movingLeftRoom = true;
					rooms [u, v].onTrail = true;
					rooms [u, v].PickExits (zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
					u--;
				} else if (u == downRooms [v]) {
					//set room with bottom exit
					movingDownRoom = true;
					AllowBottomExits(rooms[u,v], zoneTopRoom);
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

	void AllowRightExits (Room r, bool zoneTopRoom) {
		r.AddAllowedExit (Room.RoomExits.R);
		r.AddAllowedExit (Room.RoomExits.TR);
		r.AddAllowedExit (Room.RoomExits.LR);
		r.AddAllowedExit (Room.RoomExits.BR);
		r.AddAllowedExit (Room.RoomExits.TBR);
		r.AddAllowedExit (Room.RoomExits.TLR);
		r.AddAllowedExit (Room.RoomExits.BLR);
		r.AddAllowedExit (Room.RoomExits.TBLR);
		if (zoneTopRoom) {
			r.RemoveAllowedExit (Room.RoomExits.R);
			r.RemoveAllowedExit (Room.RoomExits.LR);
			r.RemoveAllowedExit (Room.RoomExits.BR);
			r.RemoveAllowedExit (Room.RoomExits.BL);
			r.RemoveAllowedExit (Room.RoomExits.BLR);
		}}

	void AllowLeftExits (Room r, bool zoneTopRoom) {
		r.AddAllowedExit (Room.RoomExits.L);
		r.AddAllowedExit (Room.RoomExits.TL);
		r.AddAllowedExit (Room.RoomExits.LR);
		r.AddAllowedExit (Room.RoomExits.BL);
		r.AddAllowedExit (Room.RoomExits.TBL);
		r.AddAllowedExit (Room.RoomExits.TLR);
		r.AddAllowedExit (Room.RoomExits.BLR);
		r.AddAllowedExit (Room.RoomExits.TBLR);
		if (zoneTopRoom) {
			r.RemoveAllowedExit (Room.RoomExits.L);
			r.RemoveAllowedExit (Room.RoomExits.TL);
			r.RemoveAllowedExit (Room.RoomExits.LR);
			r.RemoveAllowedExit (Room.RoomExits.BL);
			r.RemoveAllowedExit (Room.RoomExits.BLR);
		}
	}

	void AllowBottomExits (Room r, bool zoneTopRoom) {
		r.AddAllowedExit (Room.RoomExits.B);
		r.AddAllowedExit (Room.RoomExits.BL);
		r.AddAllowedExit (Room.RoomExits.BR);
		r.AddAllowedExit (Room.RoomExits.TB);
		r.AddAllowedExit (Room.RoomExits.TBL);
		r.AddAllowedExit (Room.RoomExits.TBR);
		r.AddAllowedExit (Room.RoomExits.BLR);
		r.AddAllowedExit (Room.RoomExits.TBLR);
		if (zoneTopRoom) {
			r.RemoveAllowedExit (Room.RoomExits.B);
			r.RemoveAllowedExit (Room.RoomExits.BL);
			r.RemoveAllowedExit (Room.RoomExits.BR);
			r.RemoveAllowedExit (Room.RoomExits.BL);
			r.RemoveAllowedExit (Room.RoomExits.BLR);
		}}

	void DefineOtherRoomsExits () {
		for (int i = 0; i < columns; i++) {
			for (int j = 0; j < rows; j++) {
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
	}

	public void SaveCaveExits () {
		foreach (Vector2 ev in caveExitVectors) {
			Vector3 spawn = new Vector3 (ev.x, ev.y, 0);
			GameObject exitSaver = GameObject.Instantiate (spaceSaverPrefab, spawn, Quaternion.identity) as GameObject;
			exitSaver.transform.localScale = new Vector2 (exitWidth, exitWidth);
			exitSaver.transform.parent = gm.exitContainer.transform;

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

	public void DestroySpaceSavers () {
		//This grabs the parent transfrom too, which doesn't have a boxcollider on it.
		Transform[] spaceSavers = gm.exitContainer.GetComponentsInChildren<Transform> ();
		for (int i = 0; i < spaceSavers.Length; i++) {
			if (spaceSavers [i].position == topExitVector) {
				spaceSavers [i].name = "TopExit";
				spaceSavers [i].GetComponent<BoxCollider2D> ().isTrigger = true;
			} else if (spaceSavers [i].position == bottomExitVector) {
				spaceSavers [i].name = "BottomExit";
				spaceSavers [i].GetComponent<BoxCollider2D> ().isTrigger = true;
			} else if (spaceSavers [i].GetComponent<BoxCollider2D> ()) {
				spaceSavers [i].GetComponent<BoxCollider2D> ().isTrigger = true;
			}
		}
	}


	void SpawnBoundaries (Vector2 botLeft, float boundaryWidth, float boundaryHeight) {
		Vector3 bottomLeftSpawn = new Vector3 ((botLeft.x + bottomExitVector.x - exitWidth / 2) / 2, botLeft.y, 0);
		GameObject bottomLeftWall = GameObject.Instantiate (boundaryPrefab, bottomLeftSpawn, Quaternion.identity) as GameObject;
		bottomLeftWall.transform.localScale = new Vector3 ((bottomExitVector.x - exitWidth / 2 - botLeft.x), 5f, 0);
		bottomLeftWall.transform.parent = gm.boundaryContainer.transform;

		Vector3 bottomRightSpawn = new Vector3 ((botLeft.x + boundaryWidth + bottomExitVector.x + exitWidth / 2) / 2, botLeft.y, 0);
		GameObject bottomRightWall = GameObject.Instantiate (boundaryPrefab, bottomRightSpawn, Quaternion.identity) as GameObject;
		bottomRightWall.transform.localScale = new Vector3 (botLeft.x + boundaryWidth - (bottomExitVector.x + exitWidth / 2), 5f, 0);
		bottomRightWall.transform.parent = gm.boundaryContainer.transform;

		Vector3 leftWallSpawn = new Vector3 (botLeft.x, botLeft.y + boundaryHeight / 2, 0);
		GameObject leftWall = GameObject.Instantiate (boundaryPrefab, leftWallSpawn, Quaternion.identity) as GameObject;
		leftWall.transform.localScale = new Vector3 (5f, boundaryHeight, 0);
		leftWall.transform.parent = gm.boundaryContainer.transform;

		Vector3 rightWallSpawn = new Vector3 (botLeft.x + boundaryWidth, botLeft.y + boundaryHeight / 2, 0);
		GameObject rightWall = GameObject.Instantiate (boundaryPrefab, rightWallSpawn, Quaternion.identity) as GameObject;
		rightWall.transform.localScale = new Vector3 (5f, boundaryHeight, 0);
		rightWall.transform.parent = gm.boundaryContainer.transform;

		Vector3 topLeftSpawn = new Vector3 ((botLeft.x + topExitVector.x - exitWidth / 2) / 2, botLeft.y + boundaryHeight);
		GameObject topLeftWall = GameObject.Instantiate (boundaryPrefab, topLeftSpawn, Quaternion.identity) as GameObject;
		topLeftWall.transform.localScale = new Vector3 ((topExitVector.x - exitWidth / 2 - botLeft.x), 5f, 0);
		topLeftWall.transform.parent = gm.boundaryContainer.transform;

		Vector3 topRightSpawn = new Vector3 ((botLeft.x + boundaryWidth + topExitVector.x + exitWidth / 2) / 2, botLeft.y + boundaryHeight);
		GameObject topRightWall = GameObject.Instantiate (boundaryPrefab, topRightSpawn, Quaternion.identity) as GameObject;
		topRightWall.transform.localScale = new Vector3 (botLeft.x + boundaryWidth - (topExitVector.x + exitWidth / 2), 5f, 0);
		topRightWall.transform.parent = gm.boundaryContainer.transform;
	}

	public void PlacePortal () {
		if (gm.nextPortal) {
			gm.lastPortalPosition = gm.nextPortal.transform.position;
		}
		Vector3 spawnPoint = caveExitVectors [Random.Range (1, caveExitVectors.Count)];
		gm.nextPortal = GameObject.Instantiate (portalPrefab, spawnPoint, Quaternion.identity) as GameObject;
		gm.nextPortal.transform.parent = gm.boundaryContainer.transform;
	}

	public void PlaceTunnel () {
		if (gm.lastPortalPosition != Vector3.zero) {
			Vector3 endOfPortalTail = gm.lastPortalPosition + gm.endOfPortalTailOffset;
			float distance = Vector3.Distance (bottomExitVector, endOfPortalTail);
			Vector3 spawnPoint = (bottomExitVector + endOfPortalTail) / 2;
			Quaternion tunnelRot = Quaternion.LookRotation (gm.transform.forward, bottomExitVector - endOfPortalTail);
			GameObject tunnel = GameObject.Instantiate (gm.tunnelPrefab, spawnPoint, tunnelRot) as GameObject;
			tunnel.transform.localScale = new Vector3 (tunnel.transform.localScale.x, distance, 1);
			tunnel.transform.parent = gm.boundaryContainer.transform;
		}
	}
}
