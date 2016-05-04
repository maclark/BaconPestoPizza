using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public Vector2 direction = Vector2.zero;
	public float moveForce = 100f;

	private Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		FireBullet ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetDirection (Vector2 dir) {
		direction = dir;
		direction.Normalize ();
	}

	public void FireBullet() {
		rb.AddForce ( direction * moveForce);
	}
}
