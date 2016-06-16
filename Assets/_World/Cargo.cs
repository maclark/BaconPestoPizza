using UnityEngine;
using System.Collections;

public class Cargo : MonoBehaviour {
	public enum CargoType {POWERBIRD, GEM, SHIELD, GOLD, GREENS}
	public CargoType cargoType;
	public Sprite diamond;

	private SpriteRenderer sr;

	void Awake () {
		sr = GetComponentInChildren<SpriteRenderer> ();
	}

	void OnCollisionEnter2D (Collision2D coll) {
		if (coll.transform.tag == "Bird") {
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
	}

	public void RandomType () {
		cargoType = (CargoType) Random.Range (0, (int)CargoType.GREENS);

		switch (cargoType) 
		{
		case CargoType.POWERBIRD:
			sr.color = Color.cyan;
			break;
		case CargoType.GEM:
			sr.color = Color.magenta;
			sr.sprite = diamond;
			break;
		case CargoType.SHIELD:
			sr.color = Color.blue;
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
