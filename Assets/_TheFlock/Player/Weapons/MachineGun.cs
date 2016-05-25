﻿using UnityEngine;
using System.Collections;

public class MachineGun : Weapon {
	public MachineGun (Holster hol) : base (hol) {
		bulletSpeed = 1000f;
		fireRate = .15f;
		reloadSpeed = 3f;
		clipSize = 40;
		roundsLeftInClip = 40;
	}
}
