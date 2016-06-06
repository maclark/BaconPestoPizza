using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Snake : MonoBehaviour {
	public int initialLinks = 10;
	public List<SnakeLink> links = new List<SnakeLink> ();
	public Sprite square;
	public GameObject link;



	void Awake () {
		

	}

	// Use this for initialization
	void Start () {
		for (int i = 0; i < initialLinks; i++) {
			SpawnLink ();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void FixedUpdate () {
	}

	void CreateLink () {
		link = new GameObject ();
		link.AddComponent<SpriteRenderer> ();
		link.AddComponent<Rigidbody2D> ();
		link.AddComponent<BoxCollider2D> ();
		link.AddComponent<SnakeLink> ();
		link.SetActive (false);
	}

	void SpawnLink () {
		GameObject newLink = Instantiate (link, links[links.Count].transform.position, Quaternion.identity) as GameObject;
		links.Add (newLink.GetComponent<SnakeLink> ());
	}
}
