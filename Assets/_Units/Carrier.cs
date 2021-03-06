﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Carrier : Unit {

	public int maxInterceptors = 8;
	public List<Transform> interceptors = new List<Transform> ();
	public float moveForceMagnitude = 75f;
	public float spawnRate = .5f;
	public GameObject enemyPrefab;

	private Component[] dockTransforms;
	private bool spawning = false;
	private bool hitExplosion;

	protected override void OnAwake() {
		base.OnAwake ();
	}

	protected override void OnStart () {
		InvokeRepeating ("SpawnInterceptor", 0f, spawnRate);
	}

	protected override void OnUpdate () {
		hitExplosion = false;
		base.OnUpdate ();
	}

	public void TriggerEnter2D (Collider2D other) {
		if (other.tag == "PlayerBullet") {
			Projectile pro = other.GetComponent<Projectile> ();
			attacker = pro.owner;
			TakeDamage (other.GetComponent<Projectile> ().damage, Color.red);
			other.GetComponent<Projectile> ().Die ();
		} else if (other.tag == "Explosion") {
			if (hitExplosion)
				return;
			hitExplosion = true;
			TakeDamage (other.GetComponent<Projectile> ().damage, Color.red);
		}
	}

	protected void SpawnInterceptor () {
		spawning = true;
		if (interceptors.Count < maxInterceptors) {
			GameObject enemyObj = Instantiate (enemyPrefab, GetComponentInChildren<Dock> ().transform.position, Quaternion.identity) as GameObject;
			if (transform.parent) {
				enemyObj.transform.parent = transform.parent;
			}
			enemyObj.GetComponent <Flyer> ().mother = this;
			interceptors.Add (enemyObj.transform);
		}

		if (interceptors.Count >= maxInterceptors) {
			spawning = false;
			CancelInvoke ();
		}
	}

	public void LoseInterceptor (Transform interceptor) {
		interceptors.Remove (interceptor);
		if (interceptors.Count < 2 && !spawning) {
			InvokeRepeating ("SpawnInterceptor", 0f, spawnRate);
		}
	}

	public override void TakeDamage (int dam, Color c) {
		base.TakeDamage (dam, c);
	}

	public override void Die() {
		CancelInvoke ();
		base.Die ();
	}

	protected void Webbed () {
		print("enemy carrier webbed!!!");
	}
}
