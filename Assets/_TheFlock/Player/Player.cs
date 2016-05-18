using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public Bird b = null;
	public bool navigating = false;

	private BigBird bigBird;

	void Awake () {
		bigBird = GameObject.FindObjectOfType<BigBird>() as BigBird;
	}

	public void StartPlayer () {
		Bird nearestBird = bigBird.GetNearestFreeBird (transform.position);
		if (nearestBird) {
			BoardBird (nearestBird);
			GetComponent<SpriteRenderer> ().enabled = false;
		}
	}

	public void BoardBird (Bird bird) {
		b = bird;
		b.p = this;
		b.color = GetComponent<SpriteRenderer> ().color;
		b.GetComponent<SpriteRenderer>().color = b.color;
		b.GetComponent<ObjectPooler> ().enabled = true;
		b.GetComponent<ObjectPooler> ().SetPooledObjectsColor (b.color);
		transform.position = b.transform.position;
		transform.parent = b.transform;
		GetComponent<SpriteRenderer> ().enabled = false;
	}
}
