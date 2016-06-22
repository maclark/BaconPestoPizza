using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Flyer : Unit {

	public float forceMag = 40f;
	public int kamikazeDamage = 200;
	public float fireRate = 1f;
	public float attackRange = 6f;
	public float bulletForce = 60f;
	public Carrier mother;

	protected Rigidbody2D rb;
	protected GameManager gm;
	protected Transform target;
	protected bool startAttacking = false;
	protected bool stopFiring = false;

	// Use this for initialization
	protected override void OnAwake() {
		gm = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
		rb = GetComponent<Rigidbody2D> ();
		base.OnAwake ();
	}

	protected override void OnStart () {
		//TODO maybe should just track all nearest players all the time?
		InvokeRepeating ("SetNearestTarget", 2f, 2f);
		//SetNearestTarget ();
	}

	protected override void OnUpdate () {
		if (target == null) {
			SetNearestTarget ();
		}

		if (startAttacking) {
			InvokeRepeating ("FireBullet", fireRate, fireRate);
			startAttacking = false;
		}

		if (stopFiring) {
			CancelInvoke ();
			stopFiring = false;
		}
	}

	public void OnFixedUpdate () {
		Chase ();
	}

	public void TriggerEnter2D (Collider2D other) {
		if (other.tag == "PlayerBullet") {
			Projectile pro = other.GetComponent<Projectile> ();
			attacker = pro.owner;
			TakeDamage (pro.damage, Color.red);
			pro.Die ();
		}
		else if (other.tag == "Explosion") {
			TakeDamage (other.GetComponent<Projectile> ().damage, Color.red);
		}
	}

	public void BecameVisible () {
		startAttacking = true;
	}

	public void BecameInvisible () {
		stopFiring = true;
	}

	public override void TakeDamage (int dam, Color c) {
		base.TakeDamage (dam, c);
	}

	public override void Die() {
		if (mother) {
			mother.LoseInterceptor (transform);
		}
		CancelInvoke ();
		base.Die ();
	}

	protected virtual bool SetNearestTarget() {
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
		if (!target) {
			SetNearestTarget ();
		}
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
