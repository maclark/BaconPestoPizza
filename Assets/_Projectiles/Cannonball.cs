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
		Invoke ("Detonate", maxDetonation);
		base.OnStart ();
	}
	
	void Update () {
		base.OnUpdate ();
	}

	public override void Fire (Transform start, Vector2 aim) {
		accel = cannonballAccel;
		base.Fire (start, aim);
	}

	public void Detonate () {
		t.RemoveBall (this);
		Instantiate (explosionPrefab, transform.position, transform.rotation);
		CancelInvoke ();
		Destroy (gameObject);
	}
}
