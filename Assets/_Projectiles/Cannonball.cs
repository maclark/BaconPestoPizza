using UnityEngine;
using System.Collections.Generic;

public class Cannonball : Projectile {

	public float cannonballAccel;
	public float maxDetonation = 4f;
	public GameObject explosionPrefab;
	public Turret t;

	void Awake () {
		base.OnAwake ();
	}

	void Start () {
		//Invoke ("Detonate", maxDetonation);
		base.OnStart ();
	}
	
	void Update () {
		base.OnUpdate ();
	}

	void OnTriggerEnter2D (Collider2D other) {
		base.TriggerEnter2D (other);
	}

	public override void Fire (Vector3 start, Vector2 aim) {
		forceMag = cannonballAccel;
		base.Fire (start, aim);
	}

	public void Detonate () {
		t.RemoveBall (this);
		Instantiate (explosionPrefab, transform.position, transform.rotation);
		CancelInvoke ();
		Destroy (gameObject);
	}

	void OnBecameInsivible () {
		base.BecameInvisible ();
	}
}
