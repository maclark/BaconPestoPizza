using UnityEngine;
using System.Collections;

public class NeonerBody : MonoBehaviour {

	public float forceMag;
	public Vector3 playerOffset;
	public GameObject trigger;
	public LandingPad pad;
	public Vector2 direction;

	private GameManager gm;
	private Rigidbody2D rb;
	//private Neoner neoner;

	void Awake () {
		gm = GameObject.FindObjectOfType<GameManager> ();
		rb = GetComponent<Rigidbody2D> ();
	}

	void FixedUpdate () {
		rb.AddForce (direction * forceMag);
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.name == "BoardingZone") {
			pad = other.transform.parent.GetComponent<LandingPad> ();
		} else if (other.name == "CargoPlatform") {
			pad = other.GetComponentInParent<BigBird> ().nearestPad;
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (other.name == "BoardingZone") {
			pad = null;
		} else if (other.name == "CargoPlatform") {
			pad = null;
		}
	}

	public void PressedB () {
		if (pad) {
			if (pad.occupant) {
				if (pad.occupant == gm.bigBird.transform) {
					GetComponentInChildren<Player> ().SpiritAway (gm.bigBird.transform, PlayerInput.State.CHANGING_STATIONS);
				} 
			}
		}
	}

	public void SetNeoner (Neoner n) {
		//neoner = n;
	}

}
