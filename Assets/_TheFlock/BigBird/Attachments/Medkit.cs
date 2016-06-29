using UnityEngine;
using System.Collections;

public class Medkit : MonoBehaviour {
	public bool withPatient;
	public int healPoints = 2;
	public float treatmentFrequency = .5f;


	private GameManager gm;
	private float xOffset;
	private float yOffset;

	void Awake () {
		gm = GameObject.FindObjectOfType<GameManager> ();
		xOffset = transform.localPosition.x;
		yOffset = transform.localPosition.y;
	}

	public void Reset () {
		Vector3 rightOffset = gm.bigBird.transform.right * xOffset * gm.bigBird.transform.localScale.x;
		Vector3 upOffset = gm.bigBird.transform.up * yOffset * gm.bigBird.transform.localScale.y;
		transform.position = gm.bigBird.transform.position + rightOffset + upOffset;
		GetComponent<SpriteRenderer> ().sortingOrder = 0;
	}


	public void FindHurtBird () {
		if (withPatient) {
			return;
		}
		Bird[] birds = gm.bigBird.GetComponentsInChildren<Bird> ();
		for (int i = 0; i < birds.Length; i++) {
			if (birds [i].health < birds [i].maxHealth) {
				StartCoroutine (HealPatient (birds [i]));
				return;
			}
		}
	}

	public IEnumerator HealPatient (Bird b) {
		transform.position = b.transform.position;
		GetComponent<SpriteRenderer> ().sortingOrder = 3;
		yield return new WaitForSeconds (treatmentFrequency);
		b.health += healPoints;
		if (b.health >= b.maxHealth) {
			withPatient = false;
			Reset ();
			FindHurtBird ();
		} else {
			StartCoroutine (HealPatient (b));
		}
	}
}
