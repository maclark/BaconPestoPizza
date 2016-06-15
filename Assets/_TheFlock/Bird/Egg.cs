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
		if (inCoop) {
			if (other.tag == "Player") {
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
					}
				}
			}
		}
	}

	public void Hatch () {
		if (inCoop) {
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
