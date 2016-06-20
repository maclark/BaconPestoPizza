using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlammableCompartment : MonoBehaviour {
	public float hp = 100f;
	public float burnDamage = 5f;
	public float projectileFireChance = .25f;
	public float spreadChance = .01f;
	public float extinguishChance = .05f;
	public List<Flame> flames = new List<Flame> (); 
	public Transform[] stations;

	private GameManager gm;

	// Use this for initialization
	void Awake () {
		gm = GameObject.FindObjectOfType<GameManager> ();
		
	}
	
	// Update is called once per frame
	void Update () {
		if (flames.Count > 0) {
			Burn ();
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "EnemyBullet") {
			StartFire (projectileFireChance);
			other.GetComponent<Projectile> ().Die ();
		} else if (flames.Count > 0) {
			if (other.GetComponent<SprayDrop> ()) {
				PutOutFire (extinguishChance, other);
			}
		}
	}

	void StartFire (float chanceForFire) {
		bool makeFire = Random.Range (0, 1f) < chanceForFire ? true : false;
		if (makeFire) {
			Flame f = gm.flamePooler.GetPooledObject ().GetComponent <Flame> ();
			f.gameObject.SetActive (true);	
			f.transform.position =  RandomCompartmentPosition ();
			f.transform.parent = gm.bigBird.transform;
			f.compartment = this;
			flames.Add (f);
		}
	}

	Vector3 RandomCompartmentPosition () {
		return transform.position;
	}

	void Burn () {
		hp -= flames.Count * burnDamage * Time.deltaTime;
		if (hp <= 0) {
			BurnedUp ();
		}
		/*for (int i = 0; i < flames.Count; i++) {
			StartFire (spreadChance);
		}*/
	}

	void BurnedUp () {
		List<Flame> toExtinguish = new List<Flame> ();
		foreach (Flame f in flames) {
			toExtinguish.Add (f);
		}
		foreach (Flame f in toExtinguish) {
			f.Die ();
		}

		for (int i = 0; i < stations.Length; i++) {
			stations [i].GetComponent<Collider2D> ().enabled = false;
		}
		GetComponentInChildren<SpriteRenderer> ().enabled = true;
	}

	void PutOutFire (float chanceToExtinguish, Collider2D other) {
		bool extinguish = Random.Range (0, 1f) < chanceToExtinguish ? true : false;
		if (extinguish) {
			flames [0].Die ();
			other.GetComponent<Projectile> ().Die ();
		}
	}
}
