using UnityEngine;
using System.Collections;

public class Puffer : Flyer {

	public int spawnNum = 6;
	public GameObject flappyPrefab;
	public float explosionPower = 50.0F;

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

	public override void Die () {
		for (int i = 0; i < spawnNum; i++) {
			CircleCollider2D cc = GetComponent<CircleCollider2D> ();
			float x = Random.Range (transform.position.x - cc.radius, transform.position.x + cc.radius);
			float y = Random.Range (transform.position.y - cc.radius, transform.position.y + cc.radius);
			Vector3 spawnPoint = new Vector3 (x, y, 0);
			GameObject flap = Instantiate (flappyPrefab, spawnPoint, Quaternion.identity) as GameObject;
			flap.transform.parent = transform.parent;
			Vector3 direction = flap.transform.position - transform.position;
			direction.Normalize ();
			flap.GetComponent<Rigidbody2D>().AddForce (direction * explosionPower * 10);
		}

		base.Die ();
	}
}
