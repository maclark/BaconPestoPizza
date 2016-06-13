using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour {

	public bool reachedByBigBird = false;
	private GameManager gm;

	void Awake () {
		gm = GameObject.FindObjectOfType<GameManager> ();
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.name == "BigBirdColliders") {
			if (!reachedByBigBird) {
				gm.SpawnNewZone ();
			}
			reachedByBigBird = true;
		}
	}
}
