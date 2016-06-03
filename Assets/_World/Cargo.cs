using UnityEngine;
using System.Collections;

public class Cargo : MonoBehaviour {
	public enum CargoType {POWERBIRD, AUTOTURRET, SHIELD, GOLD}
	public CargoType cargoType;

	private SpriteRenderer sr;

	void Awake () {
		sr = GetComponentInChildren<SpriteRenderer> ();
		RandomType ();
	}

	void OnCollision2D (Collision2D coll) {
	}

	void OnTriggerEnter2D (Collider2D other) {
	}

	public void RandomType () {
		cargoType = (CargoType) Random.Range (0, 4);

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
		default:
			break;

		}
	}

	public IEnumerator Dumped () {
		yield return new WaitForSeconds (.5f);
		GetComponent<BoxCollider2D> ().enabled = true;
	}
}
