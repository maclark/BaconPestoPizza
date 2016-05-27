using UnityEngine;
using System.Collections;

public class Shotgun : Weapon {

	public int barrelAmount = 5;
	public float scatter = .1f;
	public float spread = 1f;
	public float spreadOffset = 1f;
	public float cockDelay = 0f;

	public Shotgun (Holster hol) : base (hol) {
		automatic = false;
		bulletSpeed = 1500f;
		reloadSpeed = 1f;
		clipSize = 6;
		roundsLeftInClip = 6;
	}

	public override void  Fire (Vector3 dir) {
		if (roundsLeftInClip > 0) {
			for (int i = 0; i < barrelAmount; i++) {
				Bullet bull = hol.p.GetComponent<ObjectPooler> ().GetPooledObject ().GetComponent<Bullet> ();
				bull.gameObject.SetActive (true);
				bull.accelerationMagnitude = bulletSpeed;

				float x = Random.Range (0f, scatter);
				float y = Random.Range (0f, scatter);
				dir.x += x;
				dir.y += y;

				float u = Random.Range (-spread, spread);
				float v = Random.Range (-spread, spread);
				Vector3 s = hol.p.transform.position + new Vector3 (u, v, 0f) + dir * spreadOffset;
				bull.Fire (s, dir);
			}
			readyToFire = false;
			hol.p.Invoke ("CockShotgun", cockDelay);
			roundsLeftInClip--;
		}

		if (roundsLeftInClip <= 0) {
			hol.p.StartCoroutine (hol.p.Reload ());
		}
	}
}
