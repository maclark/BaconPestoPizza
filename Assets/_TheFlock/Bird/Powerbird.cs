using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Powerbird : MonoBehaviour {


	public Transform thePowerbird;

	private int flashIndex = 0;
	private Transform[] powerers;


	void Update () {
		//transform.position = CalculateCenterOfReigners ();
	}
		

	public void SetPowerers (Transform[] poweringBirds) {
		powerers = new Transform [poweringBirds.Length];
		for (int i = 0; i < powerers.Length; i++) {
			powerers [i] = poweringBirds [i];
			Bird birdie = powerers [i].GetComponent<Bird> ();
			birdie.p.ReignPowerbird (this);
			birdie.harp.SetGripping (false);
			birdie.harp.SetGripping (true);
			birdie.harp.HarpoonObject (thePowerbird.gameObject);
		}
	}

	public void Unpower () {
		if (thePowerbird) {
			thePowerbird.SendMessage ("Unpowered");
		}

		for (int i = 0; i < powerers.Length; i++) {
			Bird birdie = powerers [i].GetComponent<Bird> ();
			birdie.p.UnreignPowerbird ();
			birdie.harp.SetGripping (false);
			birdie.harp.SetRecalling (true);
		}
		Destroy (gameObject);
	}

	Vector3 CalculateCenterOfReigners () {
		float x = 0;
		float y = 0;
		for (int i = 0; i < powerers.Length; i++) {
			x += powerers [i].transform.position.x;
			y += powerers [i].transform.position.y;
		}
		x = x / powerers.Length;
		y = y / powerers.Length;
		return new Vector3 (x, y, 0);
	}

	bool CheckIfPowerer (Transform other) {
		for (int i = 0; i < powerers.Length; i++) {
			if (powerers [i] == other) {
				return true;
			}
		}
		return false;
	}

	public void Exhausted () {
		Unpower ();
	}

	public void FlashColors () {
		thePowerbird.GetComponentInChildren<SpriteRenderer> ().color = powerers [flashIndex].GetComponentInChildren<SpriteRenderer> ().color;
		flashIndex = (flashIndex + 1) % powerers.Length;
	}

	public Transform[] GetPowerers () {
		return powerers;
	}

	public void HarpoonReleased (Harpoon h) {
		Exhausted ();
	}
}
