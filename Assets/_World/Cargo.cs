using UnityEngine;
using System.Collections;

public class Cargo : MonoBehaviour {
	public enum CargoType {POWERBIRD, AUTOTURRET, SHIELD, GOLD, GREENS}
	public CargoType cargoType;

	private SpriteRenderer sr;

	void Awake () {
		sr = GetComponentInChildren<SpriteRenderer> ();
		RandomType ();
	}

	void OnCollisionEnter2D (Collision2D coll) {
		print (coll.transform.tag);
		print ("collision");
		if (coll.transform.tag == "Player") {
			Bird birdie = coll.transform.GetComponent<Bird> ();
			if (cargoType == Cargo.CargoType.SHIELD) {
				if (!birdie.Shield.gameObject.activeSelf) {
					birdie.Shield.ActivateShield ();
					Destroy (gameObject);
				}
			} else if (cargoType == Cargo.CargoType.GREENS) {
				birdie.EatGreens ();
				Destroy (gameObject);
			}
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		print (other.transform.tag + " trigger");

	}

	public void RandomType () {
		cargoType = (CargoType) Random.Range (0, 5);

		switch (cargoType) 
		{
		case CargoType.POWERBIRD:
			sr.color = Color.cyan;
			break;
		case CargoType.AUTOTURRET:
			sr.color = Color.black;
			break;
		case CargoType.SHIELD:
			sr.color = Color.blue;
			break;
		case CargoType.GOLD:
			sr.color = Color.yellow;
			break;
		case CargoType.GREENS:
			sr.color = Color.green;
			break;
		default:
			break;

		}
	}

	public IEnumerator Dumped () {
		yield return new WaitForSeconds (.5f);
		GetComponent<BoxCollider2D> ().enabled = true;
	}
}
