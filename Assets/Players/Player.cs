using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public Bird b = null;
	public bool navigating = false;

	private BigBird bigBird;

	void Awake () {
		bigBird = GameObject.FindObjectOfType<BigBird>() as BigBird;
	}

	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void StartPlayer () {
		Bird nearestBird = bigBird.GetNearestDockedBird (transform.position);
		if (nearestBird) {
			BoardBird (nearestBird);
			GetComponent<SpriteRenderer> ().enabled = false;
		}
	}

	void BoardBird (Bird bird) {
		b = bird;
		b.p = this;
		transform.position = b.transform.position;
		transform.parent = b.transform;
		GetComponent<SpriteRenderer> ().enabled = false;
	}
}
