using UnityEngine;
using System.Collections;

public class TriangleDrawer : MonoBehaviour {

	public Transform[] ships = new Transform[3];

	private Vector3[] shipVertices = new Vector3[3];
	private Vector2[] shipVertices2D = new Vector2[3];
	private Mesh mesh;
	private EdgeCollider2D ec;

	void Start() {
		gameObject.AddComponent<MeshFilter>();
		gameObject.AddComponent<MeshRenderer>();
		ec = GetComponent<EdgeCollider2D>();
		mesh = GetComponent<MeshFilter>().mesh;
		mesh.Clear();

		OrderShipsClockwise ();
	 	shipVertices[0] = ships[0].position; 
		shipVertices[1] = ships[1].position; 
		shipVertices[2] = ships[2].position; 

		mesh.vertices = shipVertices;
		mesh.uv = new Vector2[] {new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1)};
		mesh.triangles = new int[] {0, 1, 2};
	}

	void Update () {
		OrderShipsClockwise ();
		shipVertices[0] = ships[0].position; 
		shipVertices[1] = ships[1].position; 
		shipVertices[2] = ships[2].position; 

		mesh.vertices = shipVertices;

		shipVertices2D[0] = new Vector2 (shipVertices[0].x, shipVertices[0].y); 
		shipVertices2D[1] = new Vector2 (shipVertices[1].x, shipVertices[1].y); 
		shipVertices2D[2] = new Vector2 (shipVertices[2].x, shipVertices[2].y); 
		ec.points = shipVertices2D;
	}

	/// <summary>
	/// Finds ship with highest y coordinate to begin triangle. Then compares angle of vectors to the other two ships.
	/// The ship with a larger angle vector is made the next triangle vertex and the remaining ship is the last vertex.
	/// </summary>
	void OrderShipsClockwise () {
		
		//start with highest point
		int highestIndex = -1;
		int nextIndex = -1;
		int lastIndex = -1;

		float highest = 0f;
		bool highestInitialized = false;
		for (int i = 0; i < ships.Length; i++) {
			if (!highestInitialized) {
				highest = ships [i].position.y;
				highestIndex = i;
				highestInitialized = true;
			}
			else {
				if (ships[i].position.y > highest) {
					highest = ships[i].position.y;
					highestIndex = i;

				}
			}
		}

		Vector2 vectorToNextShip = ships [(highestIndex + 1) % 3].position - ships [highestIndex].position;
		Vector2 vectorToLastShip = ships [(highestIndex + 2) % 3].position - ships [highestIndex].position;

		float angleToNextShip = Mathf.Atan2 (vectorToNextShip.y, vectorToNextShip.x) * Mathf.Rad2Deg;
		float angleToLastShip = Mathf.Atan2 (vectorToLastShip.y, vectorToLastShip.x) * Mathf.Rad2Deg;

		if (angleToNextShip > angleToLastShip) {
			nextIndex = (highestIndex + 1) % 3;
			lastIndex = (highestIndex + 2) % 3;
		} else {
			lastIndex = (highestIndex + 1) % 3;
			nextIndex = (highestIndex + 2) % 3;
		}

		Transform[] shipsTemp = new Transform[3];
		shipsTemp[0] = ships[0];
		shipsTemp[1] = ships[1];
		shipsTemp[2] = ships[2];
		ships[0] = shipsTemp[highestIndex];
		ships[1] = shipsTemp[nextIndex];
		ships[2] = shipsTemp[lastIndex];
	}
}