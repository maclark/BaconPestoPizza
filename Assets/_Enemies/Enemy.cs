using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {

	public float accelerationMagnitude = 40f;
	public int hp = 100;
	public int kamikazeDamage = 200;
	public float fireRate = 1f;
	public float attackRange = 6f;

	private Rigidbody2D rb;
	private GameManager gm;
	private Transform target;
	private bool startFiring = false;
	private bool stopFiring = false;

	// Use this for initialization
	void Awake(){
		gm = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
		rb = GetComponent<Rigidbody2D> ();
	}

	void Start () {
		SetNearestTarget ();
	}

	void FixedUpdate () {
		if (target == null) {
			SetNearestTarget ();
		}

		if (startFiring) {
			InvokeRepeating ("FireBullet", fireRate, fireRate);
			startFiring = false;
		}
	
		if (stopFiring) {
			CancelInvoke ();
			stopFiring = false;
		}

		Chase ();
	}

	void OnTriggerEnter2D ( Collider2D other) {
		if (other.tag == "PlayerBullet") {
			TakeDamage (other.GetComponent<Projectile> ().damage);
			other.GetComponent<Bullet> ().Die ();
		}
		else if (other.tag == "Explosion") {
			TakeDamage (other.GetComponent<Projectile> ().damage);
		}
	}

	void TakeDamage( int dam) {
		hp -= dam;
		if (hp <= 0) {
			Die ();
		}
	}

	void Die() {
		CancelInvoke ();
		Destroy (gameObject);
	}

	bool SetNearestTarget() {
		bool targetSet = false;
		float closestTargetDistance = 10000f;
		Transform closestTarget = null;
		List<Transform> targetTransforms = gm.GetAlliedTransforms();
		if (targetTransforms.Count == 0) {
			return targetSet;
		}

		foreach (Transform t in targetTransforms) {
			float d = Vector3.Distance( t.position, transform.position);
			if (d < closestTargetDistance) {
				closestTarget = t;
				closestTargetDistance = d;
				targetSet = true;
			}
		}
		target = closestTarget;
		return targetSet;
	}

	void Chase () {
		Vector3 direction = target.position - transform.position;
		if (Vector3.Magnitude (direction) > attackRange) {  
			direction.Normalize ();
			rb.AddForce (direction * accelerationMagnitude * rb.mass);
		}
	}

	public void FireBullet() {
		if (target == null) {
			SetNearestTarget ();
			if (target == null) {
				CancelInvoke ();
				stopFiring = true;
				return;
			}
		}
		Bullet bullet = gm.GetComponent<ObjectPooler> ().GetPooledObject ().GetComponent<Bullet> ();
		bullet.gameObject.SetActive (true);
		bullet.Fire (transform, target.position - transform.position);	
	}

	void Kamikaze() {
	}

	void OnBecameVisible () {
		startFiring = true;
	}

	void OnBecameInvisible () {
		stopFiring = true;
	}
}
