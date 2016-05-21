using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public Bird b = null;
	public bool navigating = false;

	private PlayerInput pi;

	void Awake () {
		pi = GetComponent<PlayerInput> ();
	}

	public void StartPlayer () {
		GetComponent<SpriteRenderer> ().enabled = true;
		pi.state = PlayerInput.State.neutral;
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

	public void UnboardBird (Transform newParent) {
		GetComponent<SpriteRenderer> ().enabled = true;
		transform.parent = newParent;
		b.p = null;
		b.color = Color.black;
		b.GetComponent<SpriteRenderer>().color = Color.black;
		b = null;
	}
}
