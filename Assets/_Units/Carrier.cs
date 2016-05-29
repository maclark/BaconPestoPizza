using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Carrier : Unit {

	public int maxInterceptors = 8;
	public List<Transform> interceptors = new List<Transform> ();
	public float moveForceMagnitude = 75f;
	public int hp = 1000;
	public float spawnRate = .5f;
	public GameObject enemyPrefab;

	private Component[] dockTransforms;

	public void OnStart () {
		InvokeRepeating ("SpawnEnemy", 0f, spawnRate);
	}

	public void TriggerEnter2D (Collider2D other) {
		if (other.tag == "PlayerBullet") {
			TakeDamage (other.GetComponent<Bullet> ().damage);
			other.GetComponent<Bullet> ().Die ();
		}
	}

	protected void SpawnEnemy () {
		if (interceptors.Count < maxInterceptors) {
			GameObject enemyObj = Instantiate (enemyPrefab, GetComponentInChildren<Dock> ().transform.position, Quaternion.identity) as GameObject;
			interceptors.Add (enemyObj.transform);
			enemyObj.transform.parent = transform;
		}

		if (interceptors.Count >= maxInterceptors) {
			CancelInvoke ();
		}
	}

	public void LoseInterceptor (Transform interceptor) {
		interceptors.Remove (interceptor);
		if (interceptors.Count < 2) {
			InvokeRepeating ("SpawnEnemy", 0f, spawnRate);
		}
	}

	protected void TakeDamage( int dam) {
		hp -= dam;
		if (hp <= 0) {
			Die ();
		}
	}

	public void Die() {
		CancelInvoke ();
		Destroy (gameObject);
	}

	protected void Webbed () {
		print("enemy carrier webbed!!!");
	}
}
