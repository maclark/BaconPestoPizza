using UnityEngine;
using System.Collections;

public class SprayDrop : Projectile {
	public float extinguishChance = .1f;
	public float sprayGulp = 1f;
	public float maxLifeSpan = 1f;
	public float minLifeSpan = 0f;
	public Vector3 direction = Vector3.zero;

	private float lifeSpan;

	void Awake () {
		base.OnAwake ();
	}

	void Start () {
		lifeSpan = Random.Range (minLifeSpan, maxLifeSpan);
		base.OnStart ();
	}
		
	public void SetDirection (Vector3 dir) {
		direction = dir;
		direction.Normalize ();
	}

	void OnTriggerEnter2D (Collider2D other) {
		base.TriggerEnter2D (other);
	}

	public override void Fire (Vector3 start, Vector2 aim) {
		transform.position = start;
		Invoke ("Die", lifeSpan);
		SetDirection (aim);
		rb.AddForce (direction * forceMag);
	}

	public override void Die () {
		gameObject.SetActive (false);
	}
}
