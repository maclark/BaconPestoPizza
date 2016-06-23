using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Destructible : MonoBehaviour {

	public int hp = 500;
	public float birthRange = 5f;
	public float epxlosionMag = 100f;
	public float chanceForRuby = .05f;
	public float chanceForGreens = .1f;
	public float chanceForShield = .15f;
	public float chanceForEaglehead = .02f;
	public List<GameObject> babies;
	public GameObject ruby;
	public GameObject greens;
	public GameObject shield;
	public GameObject eaglehead;

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
		Harpoonable[] hpls = GetComponentsInChildren<Harpoonable> ();
		for (int i = 0; i < hpls.Length; i++) {
			hpls [i].BreakLoose ();
		}

		if (babies != null) {
			foreach (GameObject obj in babies) {
				GiveBirth (obj);
			}
		}
		if (Random.Range (0f, 1f) < chanceForRuby) {
			GiveBirth (ruby);
		}
		if (Random.Range (0f, 1f) < chanceForGreens) {
			GiveBirth (greens);
		}
		if (Random.Range (0f, 1f) < chanceForShield) {
			GiveBirth (shield);
		}
		if (Random.Range (0f, 1f) < chanceForEaglehead) {
			GiveBirth (eaglehead);
		}
		Destroy (gameObject);
	}

	void GiveBirth (GameObject baby) {
		float x = transform.position.x + Random.Range (-birthRange, birthRange);
		float y = transform.position.y + Random.Range (-birthRange, birthRange);
		Vector3 birthplace = new Vector3 (x, y, 0);
		GameObject obj = Instantiate (baby, birthplace, Quaternion.identity) as GameObject;
		obj.GetComponent<Collider2D> ().isTrigger = false;
		obj.GetComponent<Rigidbody2D> ().isKinematic = false;
		if (obj.GetComponent<Harpoonable> ()) {
			obj.GetComponent<Harpoonable> ().enabled = true;
		}
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
