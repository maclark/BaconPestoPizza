using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurretStation : Station {
	public GameObject prefabProjectile;
	public float cooldown = 1f;

	private bool ready = true;
	private Vector3 aim;
	private List<Torpedo> torps = new List<Torpedo> ();

	void Awake () {
		base.OnAwake ();
	}

	void Start () {
		base.SetStationType (Station.StationType.TURRET);
	}

	public override void HandleInput () {
		print ("handling input turretstation");
		if (Input.GetButtonDown (pi.bCircleButton)) {
			Abandon ();
			return;
		}

		Rotate (new Vector3 (Input.GetAxis (pi.LSHorizontal), Input.GetAxis (pi.LSVertical), 0f));
		if (Input.GetAxis (pi.rightTrigger) < 0) {
			RightTrigger ();
		}

		if (Input.GetButtonDown (pi.aCrossButton)) {
			PressedA ();
		}
	}

	public override void Man (Player p) {
		print ("manning turretstation");
		user = p;
		pi = user.GetComponent<PlayerInput> ();
		pi.state = PlayerInput.State.ON_TURRET;
		GetComponentInChildren<SpriteRenderer> ().color = p.color;
	}


	public override void Abandon () {
		print ("abandoning turretstation");
		GetComponentInChildren<SpriteRenderer> ().color = Color.black;	
		pi.state = PlayerInput.State.CHANGING_STATIONS;
		pi.realStation = null;
		user = null;
		pi = null;
	}

	public override void MakeAvailable () {
		print ("making available turretstation");

		GetComponent<Collider2D> ().enabled = true;
	}

	public override void MakeUnavailable () {
		print ("making unavailable turretstation");
		GetComponent<Collider2D> ().enabled = false;
	}

	public void RightTrigger () {
		if (ready) {
			Fire ();
		}
	}

	public void PressedA () {
		if (torps.Count > 0) {
			torps [0].Detonate ();
		}
	}

	public void Rotate (Vector3 direction) {
		aim = direction;
		Quaternion targetRotation = Quaternion.LookRotation (new Vector3 (0,0,1), direction);
		transform.rotation = targetRotation;

	}

	public void Fire () {
		GameObject obj = Instantiate (prefabProjectile, transform.position, Quaternion.LookRotation(transform.forward, -transform.right)) as GameObject;
		Torpedo torp = obj.GetComponent<Torpedo> ();
		torp.t = this;
		torps.Add (torp);
		if (aim == Vector3.zero) {
			//depends on base rotation of object... TODO
			//aim = transform.up;
			aim = transform.up;
		}
		torp.Fire (transform.position, aim);
		ready = false;
		Invoke ("ResetCooldown", cooldown);
	}

	void ResetCooldown () {
		ready = true;
	}

	public void RemoveTorp (Torpedo torp) {
		torps.Remove (torp);
	}

}
