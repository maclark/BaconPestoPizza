using UnityEngine;
using System.Collections;

public class Bullet : Projectile {

	public int bulletDamage = 100;
	public Vector3 direction = Vector3.zero;

	void Awake () {
		base.OnAwake ();
	}

	void Start () {
		damage = bulletDamage;
	}

	public void SetDirection (Vector3 dir) {
		direction = dir;
		direction.Normalize ();
	}

	public override void Fire (Vector3 start, Vector2 aim) {
		transform.position = start;
		SetDirection (aim);
		rb.AddForce (direction * forceMag);
	}

	public override void Die () {
		gameObject.SetActive (false);
	}

	void OnBecameInvisible () {
		gameObject.SetActive (false);
	}
}
