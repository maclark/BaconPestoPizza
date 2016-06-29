using UnityEngine;
using System.Collections.Generic;

public class Torpedo : Projectile {

	public float torpedoAccel;
	public float maxDetonation = 4f;
	public GameObject explosionPrefab;
	public TurretStation t;

	void Awake () {
		base.OnAwake ();
	}

	void Start () {
		Invoke ("Detonate", maxDetonation);
		base.OnStart ();
	}

	void Update () {
		base.OnUpdate ();
	}

	void OnTriggerEnter2D (Collider2D other) {
		base.TriggerEnter2D (other);
	}

	public override void Fire (Vector3 start, Vector2 aim) {
		forceMag = torpedoAccel;
		base.Fire (start, aim);
	}

	public override void Die () {
		Detonate ();
	}

	public void Detonate () {
		t.RemoveTorp (this);
		Instantiate (explosionPrefab, transform.position, transform.rotation);
		CancelInvoke ();
		base.Die ();
	}

	void OnBecameInsivible () {
		base.BecameInvisible ();
	}
}
