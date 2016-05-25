using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	public int damage;
	protected float accel;
	protected Rigidbody2D rb;

	protected virtual void OnAwake () {
		rb = GetComponent<Rigidbody2D> ();
	}

	protected virtual void OnStart () {
	}
	
	protected virtual void OnUpdate () {
	}

	public virtual void Fire (Vector3 start, Vector2 aim) {
		transform.position = start;
		aim.Normalize ();
		rb.AddForce (aim * accel * rb.mass);
	}

	public virtual void Die () {
	}
}
