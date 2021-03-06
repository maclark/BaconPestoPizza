﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerWeapon : Weapon {

	public int barrelAmount = 1;
	public float scatter = .1f;
	public float spread = 1f;
	public float Offset = 1f;
	public List<Transform> poweringFlock;

	public PowerWeapon (Holster hol) : base (hol) {
		poweringFlock = hol.p.b.powerbird.GetPoweringBirds ();
		name = "PowerWeapon";
		projectileSpeed = 250f;
		reloadSpeed = 1f;
		fireRate = .001f;
		clipSize = 100;
		damage = 150;
	}

	public override void  Fire (Vector3 dir) {
		for (int i = 0; i < barrelAmount; i++) {
			Bullet bull = GetBullet ();
			bull.gameObject.SetActive (true);
			bull.forceMag = projectileSpeed;

			float x = Random.Range (0f, scatter);
			float y = Random.Range (0f, scatter);
			dir.x += x;
			dir.y += y;

			float u = Random.Range (-spread, spread);
			float v = Random.Range (-spread, spread);
			Vector3 s = hol.p.transform.position + new Vector3 (u, v, 0f) + dir * Offset;
			bull.Fire (s, dir);
			roundsLeftInClip--;
		}
	}

	public Bullet GetBullet () {
		int index = Random.Range (0, poweringFlock.Count);
		Bullet bull = poweringFlock [index].GetComponentInChildren<ObjectPooler> ().GetPooledObject ().GetComponent<Bullet> ();
		return bull;
	}

}
