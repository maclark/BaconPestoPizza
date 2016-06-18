using UnityEngine;
using System.Collections;

public class Vault : MonoBehaviour {

	public Shopkeeper keeper;

	void OnCollisionEnter2D (Collision2D coll) {
		Item c = coll.transform.GetComponent<Item> ();
		if (c) {
			keeper.BuyFromPlayer (c.itemType);
			Destroy (c.gameObject);
		}
	}
}
