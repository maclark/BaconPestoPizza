using UnityEngine;
using System.Collections;

public class Invader : Flyer {
	public float skew = 2f;

	void Awake() {
		base.OnAwake ();
	}

	void Start () {
		base.OnStart ();
	}

	void Update () {
		base.OnUpdate ();
	}

	void FixedUpdate () {
		base.OnFixedUpdate ();
	}

	void OnTriggerEnter2D ( Collider2D other) {
		base.TriggerEnter2D (other);
	}

	void OnBecameVisible () {
		base.BecameVisible ();
	}

	void OnBecameInvisible () {
		base.BecameInvisible ();
	}

	public override void FireBullet() {
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
		Vector2 skewVector = new Vector2 (Random.Range (-skew, skew), Random.Range (-skew, skew));
		Vector2 aim = target.position - transform.position;
		bullet.Fire (transform.position, aim + skewVector);	
	}
}
