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
			Projectile pro = other.GetComponent<Projectile> ();
			hunt.attacker = pro.owner;
			hunt.TakeDamage (Mathf.RoundToInt (pro.damage * modifier), Color.red);
			pro.Die ();
		}
	}
}
