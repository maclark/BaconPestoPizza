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

	public void Hatch () {
		if (inCoop) {
			print ("hatching in coop");

			GameObject obj = Instantiate (hatchlingPrefab, transform.position, Quaternion.identity) as GameObject;
			if (transform.parent) {
				obj.transform.parent = transform.parent;
			}
		} else {
			print ("hatching not in coop");

			GameObject obj = Instantiate (hatchlingPrefab, transform.position, Quaternion.identity) as GameObject;
			if (transform.parent) {
				obj.transform.parent = transform.parent;
			}
		}
		Destroy (gameObject);
	}
}
