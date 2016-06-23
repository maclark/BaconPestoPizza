using UnityEngine;
using System.Collections;

public class Bubble : MonoBehaviour {

	public Player p;

	private GameManager gm;

	void Awake () {
		gm = GameObject.FindObjectOfType<GameManager> ();
	}

	void Update () {
		transform.position = gm.ClampToScreen (transform.position, gm.screenClampBuffer);
	}

	void OnCollisionEnter2D (Collision2D coll) {
		if (coll.transform.name == "BigBird") {
			p.BoardBigBird ();
			gm.RemoveAlliedTransform (transform);
			Destroy (gameObject);
		}
	}
}
