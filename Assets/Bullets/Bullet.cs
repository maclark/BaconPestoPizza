using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public Vector2 direction = Vector2.zero;
	public float moveForce = 100f;
	public int damage = 100;
	public float bulletDuration = 1f;

	private Rigidbody2D rb;

	void Awake () {
		rb = GetComponent<Rigidbody2D> ();
	}

	public void SetDirection (Vector2 dir) {
		direction = dir;
		direction.Normalize ();
	}

	void Unenable () {
		gameObject.SetActive (false);
	}

	public void Fire (Transform start, Vector2 aim) {
		transform.position = start.position;
		transform.rotation = start.rotation;
		SetDirection (aim);
		rb.AddForce ( direction * moveForce);
		Invoke ("Unenable", bulletDuration);
	}

	public void Die () {
		Unenable ();
	}
}
