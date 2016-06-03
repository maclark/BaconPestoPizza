using UnityEngine;
using System.Collections;

public class Weapon {
	public string name = "unnamed";
	public bool automatic = true;
	public bool cocked = true;
	public bool reloading = false;
	public float fireRate = .1f;
	public float bulletSpeed = 200f;
	public float reloadSpeed = 2f;
	public int clipSize = 60;
	public int roundsLeftInClip = 60;
	public int damage = 100;
	public bool firing = false;
	public Holster hol;

	public Weapon (Holster holster) {
		hol = holster;
	}

	public virtual void Fire (Vector3 dir) {
		if (roundsLeftInClip > 0 && !reloading) {
			Bullet bullet = hol.p.GetComponent<ObjectPooler>().GetPooledObject ().GetComponent<Bullet> ();
			bullet.gameObject.SetActive (true);
			bullet.forceMag = bulletSpeed;
			bullet.damage = damage;
			bullet.Fire (hol.p.transform.position, dir);
			roundsLeftInClip--;
		}

		if (roundsLeftInClip <= 0 && !reloading) {
			hol.p.StartCoroutine (Reload ());
		}
	}

	public virtual IEnumerator Reload () {
		reloading = true;
		hol.p.b.TurnOnReloadIndicator (reloadSpeed);
		yield return new WaitForSeconds (reloadSpeed);
		if (!hol.weaponChangedDuringReload) {
			roundsLeftInClip = clipSize;
		} else {
			hol.weaponChangedDuringReload = false;
		}
		reloading = false;
	}

	public void CockWeapon () {
		cocked = true;
	}
}
