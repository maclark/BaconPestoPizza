using UnityEngine;
using System.Collections;

public class PlayerBody : MonoBehaviour {

	public float forceMag;
	public Vector3 playerOffset;
	public GameObject trigger;
	public LandingPad pad;

	[HideInInspector]
	public Vector2 direction;

	private GameManager gm;
	private Rigidbody2D rb;
	private Player p;

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
		} else if (other.GetComponent<Item> ()) {
			p.itemTouching = other.transform;
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (other.name == "BoardingZone") {
			pad = null;
		} else if (other.name == "CargoPlatform") {
			pad = null;
		} else if (other.GetComponent<Item> ()) {
			if (p.itemTouching == other.transform) {
				p.itemTouching = null;
			}
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

	public void SetPlayer (Player player) {
		p = player;
	}

}
