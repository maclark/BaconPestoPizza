using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public float moveForce = 20f;

	private Vector2 direction = Vector2.zero;
	private Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		direction = new Vector2( Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
	}

	void FixedUpdate () {
		rb.AddForce (direction * moveForce);
	}
}
