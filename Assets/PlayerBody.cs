using UnityEngine;
using System.Collections;

public class PlayerBody : MonoBehaviour {

	public float forceMag;
	public Vector3 playerOffset;
	public LandingPad pad;

	[HideInInspector]
	public Vector2 direction;

	private GameManager gm;
	private Rigidbody2D rb;

	void Awake () {
		gm = GameObject.FindObjectOfType<GameManager> ();
		rb = GetComponent<Rigidbody2D> ();
	}

	void FixedUpdate () {
		rb.AddForce (direction * forceMag);
	}

	void OnTriggerEnter2D (Collider2D other) {
		print (other.name);
		if (other.name == "BoardingZone") {
			print ("hit zone");
			pad = other.transform.parent.GetComponent<LandingPad> ();
			if (pad) {
				print ("got pad");
			}
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		print (other.name);
		if (other.name == "BoardingZone") {
			pad = null;
		}
	}


	public void PressedA () {
		if (pad) {
			print ("pad exists");
			if (pad.occupant) {
				print ("pad occupant exists");
				if (pad.occupant == gm.bigBird.transform) {
					GetComponentInChildren<Player> ().SpiritAway (gm.bigBird.transform, PlayerInput.State.NEUTRAL);
				} 
			}
		} else {
		}
	}
}
