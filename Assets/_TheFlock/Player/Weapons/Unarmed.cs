using UnityEngine;
using System.Collections;

public class Unarmed : Weapon {


	public Unarmed (Holster hol) : base (hol) {
		name = "Unarmed";
	}

	public override void  Fire (Vector3 dir) {
	}
}