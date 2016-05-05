using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D ( Collider2D other) {
		print ("collided");
		if (other.tag == "Bullet") {
			Die ();
			print ("collided with bullet");
			Destroy (other.gameObject);
		}
	}

	void Die() {
		Destroy (gameObject);
	}
}
