using UnityEngine;
using System.Collections.Generic;

public class Cannon : MonoBehaviour {

	public GameObject prefabProjectile;
	public float cooldown = 1f;

	private bool ready = true;
	private Vector3 aim;
	private List<Cannonball> balls;

	void Start () {
		balls = new List<Cannonball> ();
	}

	public void RightTrigger () {
		if (ready) {
			Fire ();
		}
	}

	public void PressedA () {
		if (balls.Count > 0) {
			balls [0].Detonate ();
		}
	}

	public void Rotate (Vector3 direction) {
		aim = direction;
		Quaternion targetRotation = Quaternion.LookRotation (new Vector3 (0,0,1), direction);
		transform.rotation = targetRotation;

	}

	public void Fire () {
		GameObject obj = Instantiate (prefabProjectile, transform.position, transform.rotation) as GameObject;
		Cannonball ball = obj.GetComponent<Cannonball> ();
		ball.t = this;
		balls.Add (ball);
		if (aim == Vector3.zero) {
			//depends on base rotation of object... TODO
			//aim = transform.up;
		}
		ball.Fire (transform.position, aim);
		ready = false;
		Invoke ("ResetCooldown", cooldown);
	}

	void ResetCooldown () {
		ready = true;
	}

	public void RemoveBall (Cannonball b) {
		balls.Remove (b);
	}
}


