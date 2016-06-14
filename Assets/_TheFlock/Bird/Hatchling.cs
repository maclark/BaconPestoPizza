using UnityEngine;
using System.Collections;

public class Hatchling : MonoBehaviour {

	public bool flying = false;
	public float flyTime = 0f;
	public float flyTimeRequired = 20f;
	public GameObject birdPrefab;

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
				Mature ();
			}
		}
	}

	public void OnTriggerEnter2D (Collider2D other) {
		print ("hatchling is touching: " + other.tag);
		if (other.tag == "Player") {
			print ("hatchling is now itemTouching");
			other.GetComponentInChildren<Player> ().itemTouching = transform;
		}
	}

	void Mature () {
		GameObject obj = Instantiate (birdPrefab, transform.position, Quaternion.identity) as GameObject;
		if (transform.parent) {
			obj.transform.parent = transform.parent;
		}
		obj.GetComponentInChildren<SpriteRenderer> ().color = sr.color;
		obj.GetComponent<Bird> ().color = sr.color;
		Destroy (gameObject);
	}
}
