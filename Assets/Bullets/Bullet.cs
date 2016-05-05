﻿using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public Vector2 direction = Vector2.zero;
	public float moveForce = 100f;
	public int damage = 100;

	private Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		rb.AddForce ( direction * moveForce);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetDirection (Vector2 dir) {
		direction = dir;
		direction.Normalize ();
	}
}
