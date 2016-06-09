using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room {
	public Vector2 botLeft;
	public float width;
	public float height;
	public enum RoomExits {NONE, T, TB, TL, TR, TBL, TBR, TLR, TBLR, B, BL, BR, BLR, L, LR, R}
	public bool exitsSet = false;
	public bool zoneExit = false;
	public bool onTrail = false;

	private RoomExits exits;
	private List<Vector2> exitVectors;
	private List<RoomExits> allowedExits;

	public Room (Vector2 bLeft, float w, float h) {
		botLeft = bLeft;
		width = w;
		height = h;
		exitVectors = new List<Vector2> ();
		allowedExits = new List<RoomExits> ();
	}

	public void PrintDetails () {
		if (onTrail) {
		} else {
		}
	}

	public RoomExits GetExits () {
		return exits;
	}

	public void SetExits (RoomExits re) {
		exits = re;
		exitsSet = true;
	}

	public void SetExits (int i) {
		exits = allowedExits [i];
		exitsSet = true;
	}


	public void PickExits () {
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
		ReserveExits ();
		exitsSet = true;
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

	public void ReserveExits () {
		switch (exits) {
		case RoomExits.NONE: 
			break;
		case RoomExits.T:
			ReserveAnExit ("T");
			break;
		case RoomExits.TB: 
			ReserveAnExit ("T");
			ReserveAnExit ("B");
			break; 
		case RoomExits.TL:
			ReserveAnExit ("T");
			ReserveAnExit ("L");
			break;
		case RoomExits.TR:
			ReserveAnExit ("T");
			ReserveAnExit ("R");
			break; 
		case RoomExits.TBL:
			ReserveAnExit ("T");
			ReserveAnExit ("B");
			ReserveAnExit ("L");
			break;
		case RoomExits.TBR:
			ReserveAnExit ("T");
			ReserveAnExit ("B");
			ReserveAnExit ("R");
			break;
		case RoomExits.TLR:
			ReserveAnExit ("T");
			ReserveAnExit ("L");
			ReserveAnExit ("R");
			break;
		case RoomExits.TBLR:
			ReserveAnExit ("T");
			ReserveAnExit ("B");
			ReserveAnExit ("L");
			ReserveAnExit ("R");
			break;
		case RoomExits.B:
			ReserveAnExit ("B");
			break;
		case RoomExits.BL:
			ReserveAnExit ("B");
			ReserveAnExit ("L");
			break;
		case RoomExits.BR:
			ReserveAnExit ("B");
			ReserveAnExit ("R");
			break;
		case RoomExits.BLR:
			ReserveAnExit ("B");
			ReserveAnExit ("L");
			ReserveAnExit ("R");
			break;
		case RoomExits.L:
			ReserveAnExit ("L");
			break;
		case RoomExits.LR:
			ReserveAnExit ("L");
			ReserveAnExit ("R");
			break;
		case RoomExits.R:
			ReserveAnExit ("R");
			break;
		default:
			Debug.LogError ("not known RoomExit");
			break;
		}
	}

	void ReserveAnExit (string side) {
		Vector2 ex;
		float x;
		float y;
		switch (side) {
		case "T":
			x = Random.Range (botLeft.x, botLeft.x + width);
			y = botLeft.y + height;
			break;
		case "B":
			x = Random.Range (botLeft.x, botLeft.x + width);
			y = botLeft.y;
			break;
		case "L":
			x = botLeft.x;
			y = Random.Range (botLeft.y, botLeft.y + height);
			break;
		case "R":
			x = botLeft.x + width;
			y = Random.Range (botLeft.y, botLeft.y + height);
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
