using UnityEngine;
using System.Collections;

public class WeakSpot : MonoBehaviour {
	private float modifier = 20f;
	private Hunter hunt;

	void Awake () {
		hunt = GetComponentInParent<Hunter> ();
		modifier = hunt.weakSpotModifier;
	}
		
	public void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "PlayerBullet") {
			hunt.TakeDamage (Mathf.RoundToInt (other.GetComponent<Projectile> ().damage * modifier), Color.red);
			other.GetComponent<Bullet> ().Die ();
		}
	}
}
