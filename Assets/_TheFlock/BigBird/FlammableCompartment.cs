using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlammableCompartment : MonoBehaviour {
	public float hp = 100f;
	public float burnDamage = 5f;
	public float projectileFireChance = .25f;
	public float spreadChance = .01f;
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
			SprayDrop sd = other.GetComponent<SprayDrop> ();
				if (sd) {
				PutOutFire (sd.extinguishChance);
				other.GetComponent<Projectile> ().Die ();
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
		float halfWidth = GetComponent<Collider2D> ().bounds.size.x / 2;
		float halfHeight= GetComponent<Collider2D> ().bounds.size.y / 2;
		float x = Random.Range (transform.position.x - halfWidth, transform.position.x + halfWidth);
		float y = Random.Range (transform.position.y - halfHeight, transform.position.y + halfHeight);
		return new Vector3 (x, y, 0);
	}

	void Burn () {
		hp -= flames.Count * burnDamage * Time.deltaTime;
		if (hp <= 0) {
			print ("BURNED UP: " + transform.name);
			//BurnedUp ();
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
		GetComponent <Collider2D> ().enabled = false;
		GetComponentInChildren<SpriteRenderer> ().enabled = true;
	}

	void PutOutFire (float chanceToExtinguish) {
		bool extinguish = Random.Range (0, 1f) < chanceToExtinguish ? true : false;
		if (extinguish) {
			flames [0].Die ();
		}
	}
}
