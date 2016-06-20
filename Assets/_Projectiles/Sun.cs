using UnityEngine;
using System.Collections;

public class Sun : Projectile {

	public Vector3 direction = Vector3.zero;
	public float maxLifeSpan = 60f;
	public float minLifeSpan = 0f;
	public float rotationOfLight = 3f;
	public float speedOfLight = 10f;
	private float lifeSpan;


	void Awake () {
		base.OnAwake ();
	}

	void Start () {
		lifeSpan = Random.Range (minLifeSpan, maxLifeSpan);
		base.OnStart ();
	}

	void Update () {
		//TODO lerp srhink scale over lifetime
	}

	public void SetDirection (Vector3 dir) {
		direction = dir;
		direction.Normalize ();
	}

	void OnTriggerEnter2D (Collider2D other) {
		//this will cause it to Die at walls
		//base.TriggerEnter2D (other);
	}

	public override void Fire (Vector3 start, Vector2 aim) {
		transform.position = start;
		Invoke ("Die", lifeSpan);
		SetDirection (aim);
		rb.AddForce (direction * speedOfLight);
		rb.AddTorque (rotationOfLight);
	}

	public override void Die () {
		gameObject.SetActive (false);
	}
}
