using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room {
	public Vector2 botLeft;
	public float width;
	public float height;
	public float exitBuffer = 5f;
	public enum RoomExits {NONE, T, TB, TL, TR, TBL, TBR, TLR, TBLR, B, BL, BR, BLR, L, LR, R}
	public bool exitsSet = false;
	public bool zoneExit = false;
	public bool onTrail = false;

	private RoomExits exits;
	private List<Vector2> exitVectors;
	private List<RoomExits> allowedExits;
	private Spelunky cave;

	public Room (Vector2 bLeft, float w, float h, Spelunky c) {
		botLeft = bLeft;
		width = w;
		height = h;
		cave = c;
		exitVectors = new List<Vector2> ();
		allowedExits = new List<RoomExits> ();
	}

	public RoomExits GetExits () {
		return exits;
	}

	/*
	public void SetExits (RoomExits re) {
		exits = re;
		exitsSet = true;
	}

	public void SetExits (int i) {
		exits = allowedExits [i];
		exitsSet = true;
	}
*/

	public void PickExits (bool zoneTopRoom=false, bool movingRightRoom=false, bool movingLeftRoom=false, bool movingDownRoom=false) {
		int max = allowedExits.Count;
		if (!onTrail) {
			AddAllAllowedExits ();
			max = (int)RoomExits.R + 1;
		}
		if (max == 0) {
			return;
		}
		int index = Random.Range (0, max);
		exits = allowedExits [index];
		ReserveExits (zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
		exitsSet = true;
	}

	public List<Vector2> GetExitVectors () {
		return exitVectors;
	}

	public void AddAllowedExit (RoomExits re) {
		allowedExits.Add (re);
	}

	public void AddAllAllowedExits () {
		allowedExits.Add (RoomExits.NONE);
		allowedExits.Add (RoomExits.T);
		allowedExits.Add (RoomExits.TB);
		allowedExits.Add (RoomExits.TL);
		allowedExits.Add (RoomExits.TR);
		allowedExits.Add (RoomExits.TBL);
		allowedExits.Add (RoomExits.TBR);
		allowedExits.Add (RoomExits.TLR);
		allowedExits.Add (RoomExits.TBLR);
		allowedExits.Add (RoomExits.B);
		allowedExits.Add (RoomExits.BL);
		allowedExits.Add (RoomExits.BR);
		allowedExits.Add (RoomExits.BLR);
		allowedExits.Add (RoomExits.L);
		allowedExits.Add (RoomExits.LR);
		allowedExits.Add (RoomExits.R);
	}

	public void RemoveAllowedExit (RoomExits re) {
		allowedExits.Remove (re);
	}

	public void ReserveExits (bool zoneTopRoom, bool movingRightRoom, bool movingLeftRoom, bool movingDownRoom) {
		switch (exits) {
		case RoomExits.NONE: 
			break;
		case RoomExits.T:
			ReserveAnExit ("T", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			break;
		case RoomExits.TB: 
			ReserveAnExit ("T", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			ReserveAnExit ("B", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			break; 
		case RoomExits.TL:
			ReserveAnExit ("T", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			ReserveAnExit ("L", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			break;
		case RoomExits.TR:
			ReserveAnExit ("T", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			ReserveAnExit ("R", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			break; 
		case RoomExits.TBL:
			ReserveAnExit ("T", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			ReserveAnExit ("B", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			ReserveAnExit ("L", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			break;
		case RoomExits.TBR:
			ReserveAnExit ("T", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			ReserveAnExit ("B", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			ReserveAnExit ("R", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			break;
		case RoomExits.TLR:
			ReserveAnExit ("T", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			ReserveAnExit ("L", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			ReserveAnExit ("R", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			break;
		case RoomExits.TBLR:
			ReserveAnExit ("T", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			ReserveAnExit ("B", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			ReserveAnExit ("L", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			ReserveAnExit ("R", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			break;
		case RoomExits.B:
			ReserveAnExit ("B", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			break;
		case RoomExits.BL:
			ReserveAnExit ("B", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			ReserveAnExit ("L", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			break;
		case RoomExits.BR:
			ReserveAnExit ("B", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			ReserveAnExit ("R", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			break;
		case RoomExits.BLR:
			ReserveAnExit ("B", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			ReserveAnExit ("L", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			ReserveAnExit ("R", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			break;
		case RoomExits.L:
			ReserveAnExit ("L", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			break;
		case RoomExits.LR:
			ReserveAnExit ("L", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			ReserveAnExit ("R", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			break;
		case RoomExits.R:
			ReserveAnExit ("R", zoneTopRoom, movingRightRoom, movingLeftRoom, movingDownRoom);
			break;
		default:
			Debug.LogError ("not known RoomExit");
			break;
		}
	}

	void ReserveAnExit (string side, bool zoneTopRoom, bool movingRightRoom, bool movingLeftRoom, bool movingDownRoom) {
		Vector2 ex;
		float x;
		float y;
		switch (side) {
		case "T":
			x = Random.Range (botLeft.x + exitBuffer, botLeft.x + width - exitBuffer);
			y = botLeft.y + height;
			if (zoneTopRoom) {
				cave.caveExitVectors.Add (new Vector2 (x, y));
			}
			break;
		case "B":
			x = Random.Range (botLeft.x + exitBuffer, botLeft.x + width - exitBuffer);
			y = botLeft.y;
			if (movingDownRoom) {
				cave.caveExitVectors.Add (new Vector2 (x, y));
			}
			break;
		case "L":
			x = botLeft.x;
			y = Random.Range (botLeft.y, botLeft.y + height);
			if (movingLeftRoom) {
				cave.caveExitVectors.Add (new Vector2 (x, y));
			}
			break;
		case "R":
			x = botLeft.x + width;
			y = Random.Range (botLeft.y, botLeft.y + height);
			if (movingRightRoom) {
				cave.caveExitVectors.Add (new Vector2 (x, y));
			}
			break;
		default:
			Debug.LogError ("not known side");
			x = 0;
			y = 0;
			break;
		}
		ex = new Vector2 (x, y);
		exitVectors.Add (ex);
	}
}
