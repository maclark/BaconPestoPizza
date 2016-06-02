using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Flyer : Unit {

	public float forceMag = 40f;
	public int hp = 100;
	public int kamikazeDamage = 200;
	public float fireRate = 1f;
	public float attackRange = 6f;

	protected Rigidbody2D rb;
	protected GameManager gm;
	protected Transform target;
	protected bool startFiring = false;
	protected bool stopFiring = false;

	// Use this for initialization
	protected override void OnAwake() {
		gm = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
		rb = GetComponent<Rigidbody2D> ();
		base.OnAwake ();
	}

	protected override void OnStart () {
		SetNearestTarget ();
	}

	protected override void OnUpdate () {
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
	}

	public void OnFixedUpdate () {
		Chase ();
	}

	public void TriggerEnter2D ( Collider2D other) {
		if (other.tag == "PlayerBullet") {
			TakeDamage (other.GetComponent<Projectile> ().damage);
			other.GetComponent<Bullet> ().Die ();
		}
		else if (other.tag == "Explosion") {
			TakeDamage (other.GetComponent<Projectile> ().damage);
		}
	}

	public void BecameVisible () {
		startFiring = true;
	}

	public void BecameInvisible () {
		stopFiring = true;
	}

	protected virtual void TakeDamage( int dam) {
		hp -= dam;
		if (hp <= 0) {
			Die ();
		} else
			StartCoroutine (base.FlashRed (0.1f));
	}

	public virtual void Die() {
		if (transform.parent) {
			if (transform.parent.GetComponent<Carrier> ()) {
				transform.parent.GetComponent<Carrier> ().LoseInterceptor (transform);
			}
		}
		CancelInvoke ();
		Destroy (gameObject);
	}

	protected bool SetNearestTarget() {
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

	protected virtual void Chase () {
		Vector3 direction = target.position - transform.position;
		if (Vector3.Magnitude (direction) > attackRange) {  
			direction.Normalize ();
			rb.AddForce (direction * forceMag);
		}
	}

	public virtual void FireBullet() {
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
		bullet.Fire (transform.position, target.position - transform.position);	
	}

	protected void Kamikaze() {
	}


}
