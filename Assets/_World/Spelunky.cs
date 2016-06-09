using UnityEngine;
using System.Collections;

public class Spelunky {
	public Room[,] rooms;

	private int columns;
	private int rows;
	private float roomWidth;
	private float roomHeight;
	private Vector2 botLeft;
	private int[] downRooms;

	public Spelunky (Vector2 bottomLeft, int c, int r, float rWidth, float rHeight) {
		botLeft = bottomLeft;
		columns = c;
		rows = r;
		roomWidth = rWidth;
		roomHeight = rHeight;
		rooms = new Room[columns, rows];
		downRooms = new int[rows];
	}
		
	public void BuildCave () {
		for (int i = 0; i < columns; i++) {
			for (int j = 0; j < rows; j++) {
				Vector2 roomBotLeft = new Vector2 (botLeft.x + roomWidth * i, botLeft.y + roomHeight * j);
				rooms [i, j] = new Room (roomBotLeft, roomWidth, roomHeight);
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
			bool trailRoomRight = false;
			bool firstInRow = true;
			bool foundDownRoom = false;
			while (!foundDownRoom) {
				if (u < downRooms [v]) {
					//moving right
					AllowRightExits (rooms[u,v], firstInRow);
					rooms [u, v].onTrail = true;
					rooms [u, v].PickExits ();
					trailRoomRight = false;
					u++;
				} else if (u > downRooms [v]) {
					//moving left
					AllowLeftExits (rooms[u,v], firstInRow);
					rooms [u, v].onTrail = true;
					rooms [u, v].PickExits ();
					trailRoomRight = true;
					u--;
				} else if (u == downRooms [v]) {
					//set room with bottom exit
					AllowBottomExits(rooms[u,v], firstInRow, trailRoomRight);
					rooms [u, v].onTrail = true;
					rooms [u, v].PickExits ();
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

	void AllowRightExits (Room r, bool firstInRow) {
		r.AddAllowedExit (Room.RoomExits.LR);
		r.AddAllowedExit (Room.RoomExits.TLR);
		r.AddAllowedExit (Room.RoomExits.BLR);
		r.AddAllowedExit (Room.RoomExits.TBLR);
		if (firstInRow) {
			r.AddAllowedExit (Room.RoomExits.TR);
			r.AddAllowedExit (Room.RoomExits.TBR);
			r.RemoveAllowedExit (Room.RoomExits.BLR);
			r.RemoveAllowedExit (Room.RoomExits.LR);
		}
	}

	void AllowLeftExits (Room r, bool firstInRow) {
		r.AddAllowedExit (Room.RoomExits.LR);
		r.AddAllowedExit (Room.RoomExits.TLR);
		r.AddAllowedExit (Room.RoomExits.BLR);
		r.AddAllowedExit (Room.RoomExits.TBLR);
		if (firstInRow) {
			r.AddAllowedExit (Room.RoomExits.TL);
			r.AddAllowedExit (Room.RoomExits.TBL);
			r.RemoveAllowedExit (Room.RoomExits.LR);
			r.RemoveAllowedExit (Room.RoomExits.BLR);
		}
	}

	void AllowBottomExits (Room r, bool firstInRow, bool trailRoomRight) {
		if (firstInRow) {
			r.AddAllowedExit (Room.RoomExits.TB);
			r.AddAllowedExit (Room.RoomExits.TBL);
			r.AddAllowedExit (Room.RoomExits.TBR);
			r.AddAllowedExit (Room.RoomExits.TBLR);
		} else if (trailRoomRight) {
			r.AddAllowedExit (Room.RoomExits.BR);
			r.AddAllowedExit (Room.RoomExits.TBR);
			r.AddAllowedExit (Room.RoomExits.BLR);
			r.AddAllowedExit (Room.RoomExits.TBLR);
		} else {
			r.AddAllowedExit (Room.RoomExits.BL);
			r.AddAllowedExit (Room.RoomExits.TLR);
			r.AddAllowedExit (Room.RoomExits.BLR);
			r.AddAllowedExit (Room.RoomExits.TBLR);
		}
	}

	void DefineOtherRoomsExits () {
		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < columns; j++) {
				if (rooms [i, j].exitsSet == false) {
					rooms [i, j].PickExits ();
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

}
