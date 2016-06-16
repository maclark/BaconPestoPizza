using UnityEngine;
using System.Collections;

public class Vault : MonoBehaviour {

	public Shopkeeper keeper;

	void OnCollisionEnter2D (Collision2D coll) {
		Cargo c = coll.transform.GetComponent<Cargo> ();
		if (c) {
			keeper.BuyFromPlayer (c.cargoType);
			Destroy (c.gameObject);
		}
	}
}
