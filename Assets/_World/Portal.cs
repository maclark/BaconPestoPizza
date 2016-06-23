using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour {

	public bool bigBirdWarped = false;
	private GameManager gm;


	void Awake () {
		gm = GameObject.FindObjectOfType<GameManager> ();
		gm.currentPortal = this;
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.name == "BigBirdColliders" || other.tag == "BigBird") {
			if (gm.bbm.friend && !bigBirdWarped) {
				gm.SpawnNewZone ();
				Destroy (gm.bbm.friend.gameObject);
				gm.bbm.friend = null;
				bigBirdWarped = true;
			} else if (!bigBirdWarped) {
				gm.SpawnNewZone ();
				gm.bbm.friend = null;
				bigBirdWarped = true;
			}
		}
	}
}
