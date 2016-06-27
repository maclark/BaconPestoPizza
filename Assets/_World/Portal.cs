using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour {

	public bool bigBirdWarped = false;
	public bool opened = false;
	private GameManager gm;


	void Awake () {
		gm = GameObject.FindObjectOfType<GameManager> ();
		gm.currentPortal = this;
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.name == "BigBirdColliders" || other.tag == "BigBird") {
			if (!bigBirdWarped) {
				if (gm.bbm.friend || opened) {
					gm.SpawnNewZone ();
					Destroy (gm.bbm.friend.gameObject);
					gm.bbm.friend = null;
					bigBirdWarped = true;
				}
			}
		}
	}
}
