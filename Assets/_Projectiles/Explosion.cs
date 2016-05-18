using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	public float animLength = .5f;

	void Start () {
		Invoke ("Die", animLength);
	}

	void Die () {
		Destroy (gameObject);
	}
}
