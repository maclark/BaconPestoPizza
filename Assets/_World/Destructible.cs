using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Destructible : MonoBehaviour {

	public int hp = 500;
	public float birthRange = 5f;
	public float epxlosionMag = 100f;
	public float chanceForCargo = .33f;
	public List<GameObject> babies;
	public GameObject cargo;

	void OnTriggerEnter2D (Collider2D other) {
		Projectile proj = other.GetComponent<Projectile> ();
		if (proj) {
			TakeDamage (proj.damage);
			proj.Die ();
		}
	}

	void TakeDamage (int damage) {
		hp -= damage;
		if (hp < 0) {
			Die ();
		}
	}

	void Die () {
		Harpoonable hpl = GetComponent<Harpoonable> ();
		if (hpl) {
			hpl.Detach ();
		}
		if (babies != null) {
			foreach (GameObject obj in babies) {
				GiveBirth (obj);
			}
		}
		if (Random.Range (0f, 1f) < chanceForCargo) {
			GiveBirth (cargo);
		}
		Destroy (gameObject);
	}

	void GiveBirth (GameObject baby) {
		float x = transform.position.x + Random.Range (-birthRange, birthRange);
		float y = transform.position.y + Random.Range (-birthRange, birthRange);
		Vector3 birthplace = new Vector3 (x, y, 0);
		GameObject obj = Instantiate (baby, birthplace, Quaternion.identity) as GameObject;
		if (transform.parent) {
			obj.transform.parent = transform.parent;
		}
		Explode (obj.transform);
	}

	void Explode (Transform t) {
		Vector3 v = t.position - transform.position;
		float r = v.magnitude;
		v.Normalize ();
		t.GetComponent<Rigidbody2D> ().AddForce (v * epxlosionMag / r);
	}
}
