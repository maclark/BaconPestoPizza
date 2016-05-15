using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public Vector2 direction = Vector2.zero;
	public float accelerationMagnitude = 100f;
	public int damage = 100;

	private Rigidbody2D rb;

	void Awake () {
		rb = GetComponent<Rigidbody2D> ();
	}

	public void SetDirection (Vector2 dir) {
		direction = dir;
		direction.Normalize ();
	}

	public void Fire (Transform start, Vector2 aim) {
		transform.position = start.position;
		transform.rotation = start.rotation;
		SetDirection (aim);
		rb.AddForce (direction * accelerationMagnitude * rb.mass);
	}

	public void Die () {
		gameObject.SetActive (false);
	}

	void OnBecameInvisible () {
		gameObject.SetActive (false);
	}
}
