using UnityEngine;
using System.Collections;

public class LoadingPlatform : Station {
	public CargoHold hold;

	void Awake () {
		base.OnAwake ();
	}

	void Start () {
		base.SetStationType (Station.StationType.LOADING_PLATFORM);
	}

	public override void HandleInput () {
		if (Input.GetButtonDown (pi.bCircleButton)) {
			Abandon ();
			return;
		}

		/*
		if (!pi.sr.enabled) {
			pi.sr.enabled = true;
			gm.bigBird.hold.xSelector = 1;
			gm.bigBird.hold.ySelector = -1;
			pi.sh.verticalStickInUse = false;
			pi.sh.horizontalStickInUse = false;
			gm.bigBird.hold.Occupy (true);
		}
		*/

		if (Input.GetButtonDown (pi.xSquareButton)) {
			gm.bigBird.hold.PressedAOnPlatform (user);
		}

		if (Input.GetButtonDown (pi.aCrossButton)) {
			if (gm.bigBird.hold.platformCargo) {
				gm.bigBird.hold.Dump (gm.bigBird.hold.platformCargo.transform);
			}
		}

		Vector3 dir = new Vector3 (Input.GetAxis (pi.LSHorizontal), Input.GetAxis (pi.LSVertical), 0);
		float upness = Vector3.Dot (dir, gm.bigBird.transform.up);
		float overness = Vector3.Dot (dir, gm.bigBird.transform.right);
		if (Mathf.Abs (upness) > Mathf.Abs (overness)) {
			//move up/down relative to player
			if (upness > 0) {
				pi.LStickInUse = true;
				pi.timeOfLastStickUse = Time.time;
				pi.realStation = hold;
				pi.realSelectedStation = hold;

				gm.bigBird.hold.SelectorStep (user.transform, 0, 1);
				hold.Man (user);
				Abandon ();
			}
		}
	}

	public override void Man (Player p) {
		print ("manning platform");
		user = p;
		pi = user.GetComponent<PlayerInput> ();
		pi.realStation = this;
		pi.realSelectedStation = pi.realStation;
		pi.state = PlayerInput.State.ON_PLATFORM;
	}


	public override void Abandon () {
		print ("abandoning platform");
		gm.bigBird.hold.Occupy (false);
		pi.state = PlayerInput.State.CHANGING_STATIONS;
		pi.realStation = null;
		user = null;
		pi = null;
	}

	public override void MakeAvailable () {
		print ("making available platform");
		GetComponent<Collider2D> ().enabled = true;
	}

	public override void MakeUnavailable () {
		print ("making unavailable hold");
		GetComponent<Collider2D> ().enabled = false;
	}
}
