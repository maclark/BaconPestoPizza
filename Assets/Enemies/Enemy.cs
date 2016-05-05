using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {


	public Vector3 target = Vector3.zero;
	public float moveForce = 80f;
	public int hp = 100;
	public int kamikazeDamage = 200;
	public float fireRate = 1f;
	public float attackRange = 6f;

	private Rigidbody2D rb;
	private GameManager gm;
	private ObjectPooler objectPooler;

	// Use this for initialization
	void Awake(){
		gm = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
		rb = GetComponent<Rigidbody2D> ();
		objectPooler = GetComponent<ObjectPooler> ();
	}

	void Start () {
		SetNearestTarget ();
		InvokeRepeating ("FireBullet", fireRate, fireRate);
	}

	void FixedUpdate () {
		SetNearestTarget ();
		Chase ();
	}

	void OnTriggerEnter2D ( Collider2D other) {
		if (other.tag == "PlayerBullet") {
			TakeDamage (other.GetComponent<Bullet> ().damage);
			other.GetComponent<Bullet> ().Die ();
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
		Vector3 closestTarget = Vector3.zero;
		List<Transform> targetTransforms = gm.GetAlliedTransforms();
		foreach (Transform t in targetTransforms) {
			float d = Vector3.Distance( t.position, transform.position);
			if (d < closestTargetDistance) {
				closestTarget = t.position;
				closestTargetDistance = d;
				targetSet = true;
			}
		}
		target = closestTarget;
		return targetSet;
	}

	void Chase () {
		Vector3 direction = target - transform.position;
		if (Vector3.Magnitude (direction) > attackRange) {  
			direction.Normalize ();
			rb.AddForce (direction * moveForce);
		}
	}

	public void FireBullet() {
		GameObject bullet = objectPooler.GetPooledObject();
		bullet.SetActive (true);
		bullet.GetComponent<Bullet> ().Fire (transform, target - transform.position);
	}

	void Kamikaze() {
	}
}
