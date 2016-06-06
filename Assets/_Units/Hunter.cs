using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hunter : Flyer {
	public float skew = 2f;
	public float detectionRange = 200f;
	public float rotateSpeed = 1f;
	public float searchCooldown = 3f;
	public float reloadTime = 3f;
	public int clipSize = 8;
	public float weakSpotModifier = 20f;
	public float meleeRange = 50f;
	public int meleeDamage = 500;
	public float meleeForceMag = 3000f;
	public float meleeSkew = 0f;
	public float meleeChargeUp = .5f;
	public float meleeRecovery = 2f;
	public float meleeVelThresh = 8000f;
	public float angerFactor = 0.7f;

	public Transform mate;

	private WeakSpot weakSpot;
	private float lastSearch;
	private int clip;
	private bool inMeleeRange = false;
	private bool reloading = false;
	private bool meleeing = false;

	void Awake() {
		base.OnAwake ();
	}

	void Start () {
		clip = clipSize;
		base.OnStart ();
	}

	void Update () {
		if (Time.time - lastSearch > searchCooldown) {
			SetNearestTarget ();
		}

		if (startAttacking && target != null) {
			inMeleeRange = Vector3.Distance (transform.position, target.position) < meleeRange ? true : false;
			if (inMeleeRange && !meleeing) {
				StartCoroutine (Melee ());
			} else if (!reloading && !meleeing) {
				InvokeRepeating ("FireBullet", fireRate, fireRate);
				startAttacking = false;
			}
		}

		if (stopFiring) {
			CancelInvoke ();
			stopFiring = false;
		}
	}

	void FixedUpdate () {
		FaceTarget ();
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "PlayerBullet") {
			TakeDamage (other.GetComponent<Projectile> ().damage, Color.gray);
			other.GetComponent<Projectile> ().Die ();
		}
		else if (other.tag == "Explosion") {
			TakeDamage (Mathf.RoundToInt (other.GetComponent<Projectile> ().damage / 4), Color.gray);
		}
	}

	void OnCollisionEnter2D (Collision2D coll) {
		if (coll.transform.tag == "Player") {
			coll.transform.GetComponent<Bird> ().TakeOneShotKill ();
		} else if (rb.velocity.sqrMagnitude > meleeVelThresh) {
			if (coll.transform.tag == "BigBird") {
				coll.transform.GetComponent<BigBird> ().TakeDamage (meleeDamage);
			}
			if (coll.transform.tag == "Enemy") {
				coll.transform.GetComponent<Unit> ().TakeDamage (meleeDamage, Color.red);
			}
		}
	}

	void OnBecameVisible () {
		base.BecameVisible ();
	}

	void OnBecameInvisible () {
		base.BecameInvisible ();
	}

	public override void Die () {
		if (mate) {
			mate.GetComponent<Hunter> ().Anger ();
		}
		base.Die ();
	}

	public override void FireBullet() {
		if (clip <= 0) {
			CancelInvoke ();
			StartCoroutine (Reload ());
			return;
		}

		Bullet bullet = gm.GetComponent<ObjectPooler> ().GetPooledObject ().GetComponent<Bullet> ();
		bullet.gameObject.SetActive (true);
		bullet.forceMag = bulletForce;
		bullet.Fire (transform.position, -transform.up);	
		clip--;
	}

	protected override bool SetNearestTarget() {
		lastSearch = Time.time;
		bool targetSet = false;
		float closestTargetDistance = detectionRange;
		Transform closestTarget = null;
		List<Transform> targetTransforms = gm.GetAlliedTransforms();

		foreach (Transform t in targetTransforms) {
			float d = Vector3.Distance (t.position, transform.position);
			if (d < closestTargetDistance) {
				closestTarget = t;
				closestTargetDistance = d;
				targetSet = true;
			}
		}
		target = closestTarget;
		return targetSet;
	}

	public override void TakeDamage (int dam, Color c) {
		base.TakeDamage (dam, c);
	}

	IEnumerator Reload () {
		reloading = true;
		yield return new WaitForSeconds (reloadTime);
		clip = clipSize;
		reloading = false;
		startAttacking = true;
	}

	IEnumerator Melee () {
		meleeing = true;
		yield return new WaitForSeconds (meleeChargeUp);
		Vector3 meleeSkewVector = new Vector3 (Random.Range (-meleeSkew, meleeSkew), Random.Range (-meleeSkew, meleeSkew), 0f);
		rb.AddForce ((-transform.up + meleeSkewVector) * meleeForceMag);
		yield return new WaitForSeconds (meleeRecovery);
		meleeing = false;
	}

	void FaceTarget () {
		if (target) {
			Quaternion targetRot = Quaternion.LookRotation (transform.forward, transform.position - target.position);
			transform.rotation = Quaternion.RotateTowards (transform.rotation, targetRot, rotateSpeed);
		}
	}

	public void Anger () {
		searchCooldown *= angerFactor;
		meleeRecovery *= angerFactor;
	}

}
