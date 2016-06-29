/*using UnityEngine;
using System.Collections.Generic;

public class Turret : MonoBehaviour {

	public GameObject prefabProjectile;
	public float cooldown = 1f;

	private bool ready = true;
	private Vector3 aim;
	private List<Torpedo> torps;

	void Start () {
		torps = new List<Torpedo> ();
	}

	public void RightTrigger () {
		if (ready) {
			Fire ();
		}
	}

	public void PressedA () {
		if (torps.Count > 0) {
			torps [0].Detonate ();
		}
	}

	public void Rotate (Vector3 direction) {
		aim = direction;
		Quaternion targetRotation = Quaternion.LookRotation (new Vector3 (0,0,1), direction);
		transform.rotation = targetRotation;

	}

	public void Fire () {
		GameObject obj = Instantiate (prefabProjectile, transform.position, Quaternion.LookRotation(transform.forward, -transform.right)) as GameObject;
		Torpedo torp = obj.GetComponent<Torpedo> ();
		torp.t = this;
		torps.Add (torp);
		if (aim == Vector3.zero) {
			//depends on base rotation of object... TODO
			//aim = transform.up;
			aim = transform.up;
		}
		torp.Fire (transform.position, aim);
		ready = false;
		Invoke ("ResetCooldown", cooldown);
	}

	void ResetCooldown () {
		ready = true;
	}

	public void RemoveTorp (Torpedo torp) {
		torps.Remove (torp);
	}
}


*/