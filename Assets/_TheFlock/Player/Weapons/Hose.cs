using UnityEngine;
using System.Collections;

public class Hose : Weapon {

	public int barrelAmount = 1;
	public float scatter = .1f;
	public float spread = .15f;
	public float offset = .5f;

	public Hose (Holster hol) : base (hol) {
		name = "Hose";
		projectileSpeed = 100f;
		reloadSpeed = 1f;
		fireRate = .001f;
		clipSize = 1;
		roundsLeftInClip = 1;
		damage = 1;
	}

	public override void  Fire (Vector3 dir) {
		for (int i = 0; i < barrelAmount; i++) {
			SprayDrop drop = gm.dropPooler.GetPooledObject ().GetComponent<SprayDrop> ();
			drop.gameObject.SetActive (true);
			drop.forceMag = projectileSpeed;

			float x = Random.Range (0f, scatter);
			float y = Random.Range (0f, scatter);
			dir.x += x;
			dir.y += y;
			dir.Normalize ();

			float u = Random.Range (-spread, spread);
			float v = Random.Range (-spread, spread);
			Vector3 s = hol.p.transform.position + new Vector3 (u, v, 0f) + dir * offset;
			drop.Fire (s, dir);
			gm.bbm.waterTank.DecreaseResource (drop.sprayGulp);
		}
	}
}
