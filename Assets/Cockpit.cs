using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cockpit : Station {
	private SystemsHandler sysH;


	void Awake () {
		base.OnAwake ();
	}

	void Start () {
		base.SetStationType (Station.StationType.COCKPIT);
	}

	public override void HandleInput () {
		if (Input.GetButtonDown (pi.bCircleButton)) {
			Abandon ();
			return;
		}

		/*if (Input.GetButtonDown (xSquareButton)) {
			bigBird.turn = 0;
			state = State.NAVIGATING;
			return;
		} */

		if (gm.bigBird.Landed || gm.bigBird.landing) {
			if (Input.GetButtonDown (pi.xSquareButton)) {
				gm.bigBird.LiftOff ();
			}
			return;
		}

		if (Input.GetButtonDown (pi.xSquareButton)) {
			if (gm.bigBird.nearestPad) {
				gm.bigBird.BigDock ();
				return;
			}
		}

		gm.bigBird.turn = -Mathf.RoundToInt (Input.GetAxis (pi.LSHorizontal));

		if (Input.GetButtonDown (pi.aCrossButton)) {
			if (!gm.bigBird.GetEngineOn ()) {
				gm.bigBird.TurnEngineOn ();
			}
		} else if (Input.GetButtonUp (pi.aCrossButton)) {
			if (gm.bigBird.GetEngineOn ()) {
				gm.bigBird.TurnEngineOff ();
			}
		}

		if (Input.GetAxisRaw (pi.rightTrigger) < 0) {
			sysH.FireBroadside (true);
		}  else if (Input.GetAxisRaw (pi.leftTrigger) > 0) {
			sysH.FireBroadside (false);
		}
	}

	public override void Man (Player p) {
		print ("manning cockpitstation");
		user = p;
		pi = user.GetComponent<PlayerInput> ();
		pi.state = PlayerInput.State.PILOTING;
		pi.sr.sortingOrder = 0;
		sysH = pi.sysH;
		GetComponentInChildren<SpriteRenderer> ().color = new Color(.37f, .26f, .15f);
	}


	public override void Abandon () {
		print ("abandoning cockpitstation");
		pi.gm.bigBird.turn = 0;
		GetComponentInChildren<SpriteRenderer> ().color = Color.black;
		pi.state = PlayerInput.State.CHANGING_STATIONS;
		pi.sr.sortingOrder = 2;
		pi.realStation = null;
		user = null;
		pi = null;
	}

	public override void MakeAvailable () {
		print ("making available cockpitstation");
		GetComponent<Collider2D> ().enabled = true;
	}

	public override void MakeUnAvailable () {
		print ("making unavailable cockpitstation");
		GetComponent<Collider2D> ().enabled = false;
	}

}
