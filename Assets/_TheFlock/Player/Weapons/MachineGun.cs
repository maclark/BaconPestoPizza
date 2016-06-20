using UnityEngine;
using System.Collections;

public class MachineGun : Weapon {
	public MachineGun (Holster hol) : base (hol) {
		name = "Machine Gun";
		projectileSpeed = 100f;
		fireRate = .15f;
		reloadSpeed = 3f;
		clipSize = 40;
		roundsLeftInClip = 40;
	}
}
