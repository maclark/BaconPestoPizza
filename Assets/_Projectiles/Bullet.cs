using UnityEngine;
using System.Collections;

public class Bullet : Projectile {

	public Vector2 direction = Vector2.zero;
	public float accelerationMagnitude = 100f;
	public int damage = 100;

	void Awake () {
		base.OnAwake ();
	}

	public void SetDirection (Vector2 dir) {
		direction = dir;
		direction.Normalize ();
	}

	public override void Fire (Transform start, Vector2 aim) {
		transform.position = start.position;
		transform.rotation = start.rotation;
		SetDirection (aim);
		rb.AddForce (direction * accelerationMagnitude * rb.mass);
	}

	public override void Die () {
		gameObject.SetActive (false);
	}

	void OnBecameInvisible () {
		gameObject.SetActive (false);
	}
}
