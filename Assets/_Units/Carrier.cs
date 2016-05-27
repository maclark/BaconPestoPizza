using UnityEngine;
using System.Collections;

public class Carrier : Unit {

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
		GameObject enemyObj = Instantiate (enemyPrefab, GetComponentInChildren<Dock> ().transform.position, Quaternion.identity) as GameObject;
		enemyObj.transform.parent = this.transform.parent;
	}

	protected void TakeDamage( int dam) {
		hp -= dam;
		if (hp <= 0) {
			Die ();
		}
	}

	protected void Die() {
		CancelInvoke ();
		Destroy (gameObject);
	}

	protected void Webbed () {
		print("enemy carrier webbed!!!");
	}
}
