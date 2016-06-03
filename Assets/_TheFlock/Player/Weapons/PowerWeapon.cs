using UnityEngine;
using System.Collections;

public class PowerWeapon : Weapon {

	public int barrelAmount = 1;
	public float scatter = .1f;
	public float spread = 1f;
	public float spreadOffset = 1f;

	public PowerWeapon (Holster hol) : base (hol) {
		name = "Power Weapon";
		bulletSpeed = 250f;
		reloadSpeed = 1f;
		fireRate = .001f;
		clipSize = 100;
		roundsLeftInClip = 100;
		damage = 150;
	}

	public override void  Fire (Vector3 dir) {
		if (roundsLeftInClip > 0) {
			for (int i = 0; i < barrelAmount; i++) {
				Bullet bull = hol.p.GetComponent<ObjectPooler> ().GetPooledObject ().GetComponent<Bullet> ();
				bull.gameObject.SetActive (true);
				bull.forceMag = bulletSpeed;

				float x = Random.Range (0f, scatter);
				float y = Random.Range (0f, scatter);
				dir.x += x;
				dir.y += y;

				float u = Random.Range (-spread, spread);
				float v = Random.Range (-spread, spread);
				Vector3 s = hol.p.transform.position + new Vector3 (u, v, 0f) + dir * spreadOffset;
				bull.Fire (s, dir);
				roundsLeftInClip--;
			}
		} else {
			Debug.Log ("unweb");
			hol.p.b.webTrap.Consumed ();
		}
	}
}
