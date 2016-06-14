using UnityEngine;
using System.Collections;

public class Egg : MonoBehaviour {

	public float layTime;
	public float gestationTime;
	public GameObject hatchlingPrefab;
	public bool inCoop = false;

	//private GameManager gm;

	void Awake () {
		//gm = GameObject.FindObjectOfType<GameManager> ();
	}
		
	void Start () {
		layTime = 0f;
		print ("egg start");
	}
	
	void Update () {
		if (inCoop) {
			layTime += Time.deltaTime;
		}
		if (layTime > gestationTime) {
			Hatch ();
		}
	}

	public void OnTriggerEnter2D (Collider2D other) {
		print ("egg is touching: " + other.tag);

		if (inCoop) {
			print ("egg is touching: " + other.tag);

			if (other.tag == "Player") {
				print ("Egg is now itemTouching");
				other.GetComponentInChildren<Player> ().itemTouching = transform;
			}
		}
	}

	public void OnTriggerExit2D (Collider2D other) {
		if (inCoop) {
			if (other.tag == "Player") {
				Player otherP = other.GetComponentInChildren<Player> ();
				if (otherP.itemTouching) {
					if (otherP.itemTouching == transform) {
						otherP.itemTouching = null;
						print ("Egg is no longer itemTouching");
					}
				}
			}
		}
	}

	public void Hatch () {
		if (inCoop) {
			print ("hatching in coop");

			GameObject obj = Instantiate (hatchlingPrefab, transform.position, Quaternion.identity) as GameObject;
			if (transform.parent) {
				obj.transform.parent = transform.parent;
			}
		} else {
			print ("hatching not in coop");
		}
		Destroy (gameObject);
	}
}
