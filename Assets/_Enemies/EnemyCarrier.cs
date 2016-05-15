using UnityEngine;
using System.Collections;

public class EnemyCarrier : MonoBehaviour {

	public float moveForceMagnitude = 75f;
	public int hp = 1000;
	public float spawnRate = .5f;
	public GameObject enemyPrefab;

	private Component[] dockTransforms;

	void Start () {
		InvokeRepeating ("SpawnEnemy", 0f, spawnRate);
	}


	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "PlayerBullet") {
			TakeDamage (other.GetComponent<Bullet> ().damage);
			other.GetComponent<Bullet> ().Die ();
		}
	}

	void SpawnEnemy () {
		GameObject enemyObj = Instantiate (enemyPrefab, GetComponentInChildren<Dock> ().transform.position, Quaternion.identity) as GameObject;
		enemyObj.transform.parent = this.transform.parent;
	}

	void TakeDamage( int dam) {
		hp -= dam;
		if (hp <= 0) {
			Die ();
		}
	}

	void Die() {
		CancelInvoke ();
		Destroy (gameObject);
	}

	void Webbed () {
		print("enemy carrier webbed!!!");
	}

}
