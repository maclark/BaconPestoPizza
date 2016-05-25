using UnityEngine;
using System.Collections;

public class Weapon {
	public bool automatic = true;
	public bool readyToFire = true;
	public bool reloading = false;
	public float fireRate = .1f;
	public float bulletSpeed = 1000f;
	public float reloadSpeed = 2f;
	public int clipSize = 60;
	public int roundsLeftInClip = 60;
	public bool firing = false;
	public Holster hol;

	public Weapon (Holster holster) {
		hol = holster;
	}

	public virtual void Fire (Vector3 dir) {
		if (roundsLeftInClip > 0 && !reloading) {
			Bullet bullet = hol.p.GetComponent<ObjectPooler>().GetPooledObject ().GetComponent<Bullet> ();
			bullet.gameObject.SetActive (true);
			bullet.accelerationMagnitude = bulletSpeed;
			bullet.Fire (hol.p.transform.position, dir);
			roundsLeftInClip--;
		}

		if (roundsLeftInClip <= 0) {
			hol.p.StartCoroutine (hol.p.Reload ());
		}
	}
}
