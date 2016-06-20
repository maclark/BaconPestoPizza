using UnityEngine;
using System.Collections;

public class Rifle : Weapon {
	public Rifle (Holster hol) : base (hol) {
		name = "Rifle";
		projectileSpeed = 300f;
		fireRate = .3f;
		reloadSpeed = 2f;
		clipSize = 20;
		roundsLeftInClip = 20;
		damage = 400;
	}
}
