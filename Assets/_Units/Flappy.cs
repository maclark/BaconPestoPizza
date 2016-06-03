using UnityEngine;
using System.Collections;

public class Flappy : Flyer {

	public float lookDelay = 2f;

	void Awake () {
		attackRange = 0;
		base.OnAwake ();
	}
	// TODO use giant player collider to wake up enemies
	//maybe use SendMessage to a WakeUp fx
	
	void Update () {
		if (target != null) {
			base.Chase ();
		}
	}

	void OnCollision2D (Collision2D coll) {
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "Player") {
			if (target != null) {
				float pDist = Vector3.Distance (other.transform.position, transform.position);
				float tDist = Vector3.Distance (target.transform.position, transform.position);
				if (pDist < tDist) {
					target = other.transform;
				}
			} else {
				target = other.transform;
			}
		} else if (other.tag == "PlayerBullet") {
			TakeDamage (other.GetComponent<Projectile> ().damage);
			other.GetComponent<Bullet> ().Die ();
		}
		else if (other.tag == "Explosion") {
			TakeDamage (other.GetComponent<Projectile> ().damage);
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (target != null) {
			if (other.transform == target.transform) {
				target = null;
			}
		}
	}
}
