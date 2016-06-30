using UnityEngine;
using System.Collections;

public class Dock : Station {
	public Bird bird = null;
	public Kanga kanga = null;
	public bool locked = false;
	public Transform item;


	void Awake () {
		base.OnAwake ();
	}

	void Start () {
		base.SetStationType (Station.StationType.DOCK);
	}

	public override void HandleInput () {
		if (Input.GetButtonDown (pi.bCircleButton)) {
			Abandon ();
			return;
		}

		if (Input.GetButtonDown (pi.xSquareButton)) {
			if (item) {
				user.itemTouching = item;
				user.PickUpItem ();
			}  else if (!item && user.itemHeld) {
				user.DropItem ();
			}
		}

		if (user.b == null && bird) {
			user.BoardBird (bird);
		}

		if (Input.GetButtonDown (pi.aCrossButton)) {
			Launch ();
		}
	}

	public override void Man (Player p) {
		print ("manning dock");
		user = p;
		pi = user.GetComponent<PlayerInput> ();
		pi.state = PlayerInput.State.DOCKED;
		GetComponentInChildren<SpriteRenderer> ().color = user.color;
	}


	public override void Abandon () {
		print ("abandoning dock");
		if (user.b) {
			user.Debird (gm.bigBird.transform);
		}	
		pi.state = PlayerInput.State.CHANGING_STATIONS;
		pi.realStation = null;
		user = null;
		pi = null;
	}

	public void Launch () {
		user.b.UndockFromBigBird ();
		pi.state = PlayerInput.State.FLYING;
		pi.realStation = null;
		user = null;
		pi = null;
		bird = null;	
	}

	public override void MakeAvailable () {
		print ("making available dock");

		GetComponent<Collider2D> ().enabled = true;
	}

	public override void MakeUnavailable () {
		print ("making unavailable dock");
		GetComponent<Collider2D> ().enabled = false;
	}


}
