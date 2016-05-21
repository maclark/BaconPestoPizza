using UnityEngine;
using System.Collections;

public class Explosion : Projectile {

	public float animLength = .5f;

	void Start () {
		damage = 100;
		Invoke ("Die", animLength);
		base.OnStart ();
	}

	override public void Die () {
		Destroy (gameObject);
	}
}
