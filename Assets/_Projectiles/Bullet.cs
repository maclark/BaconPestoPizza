using UnityEngine;
using System.Collections;

public class Bullet : Projectile {

	public int bulletDamage = 100;
	public Vector2 direction = Vector2.zero;
	public float accelerationMagnitude = 100f;

	void Awake () {
		base.OnAwake ();
	}

	void Start () {
		damage = bulletDamage;
	}

	public void SetDirection (Vector2 dir) {
		direction = dir;
		direction.Normalize ();
	}

	public override void Fire (Vector3 start, Vector2 aim) {
		transform.position = start;
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
