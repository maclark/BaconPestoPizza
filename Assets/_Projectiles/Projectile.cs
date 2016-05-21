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

	public virtual void Fire (Transform start, Vector2 aim) {
		transform.position = start.position;
		transform.rotation = start.rotation;
		aim.Normalize ();
		rb.AddForce (aim * accel * rb.mass);
	}

	public virtual void Die () {
	}
}
