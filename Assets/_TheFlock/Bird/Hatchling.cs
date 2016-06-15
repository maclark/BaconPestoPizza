using UnityEngine;
using System.Collections;

public class Hatchling : MonoBehaviour {

	public bool flying = false;
	//TODO need to do kills required
	public float flyTime = 0f;
	public float flyTimeRequired = 20f;
	public float assistsNeeded = 1;
	public GameObject birdPrefab;


	private int assists = 0;
	private bool isKiller = false;
	private bool isFlyer = false;
	private SpriteRenderer sr;

	// Use this for initialization
	void Start () {
		sr = GetComponent<SpriteRenderer> ();
		sr.color = Random.ColorHSV ();
	}
	
	// Update is called once per frame
	void Update () {
		if (flying) {
			flyTime += Time.deltaTime;
			if (flyTime > flyTimeRequired) {
				isFlyer = true;
				if (isKiller) {
					Evolve ();
				}
			}
		}
	}

	public void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "Player") {
			other.GetComponentInChildren<Player> ().itemTouching = transform;
		}

		if (other.tag == "EnemyBullet" || other.tag == "Enemy") {
			transform.parent.GetComponent<Bird> ().pup = null;
			Die ();
		}
	}

	void Evolve () {
		GameObject obj = Instantiate (birdPrefab, transform.position, Quaternion.identity) as GameObject;
		if (transform.parent) {
			obj.transform.parent = transform.parent;
		}
		obj.GetComponentInChildren<SpriteRenderer> ().color = sr.color;
		obj.GetComponent<Bird> ().color = sr.color;
		obj.GetComponent<Bird> ().colorSet = true;
		Destroy (gameObject);
	}

	public void Die () {
		Destroy (gameObject);
	}

	public void IncrementAssists () {
		print ("pup got an assist");
		assists++;
		if (assists > assistsNeeded) {
			isKiller = true;
			if (isFlyer) {
				Evolve ();
			}
		}
	}
}
