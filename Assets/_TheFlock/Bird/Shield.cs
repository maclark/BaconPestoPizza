using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour {

	public void ActivateShield () {
		gameObject.SetActive (true);
	}

	public void DeactivateShield () {
		gameObject.SetActive (false);
	}

	public void SetColor (Color c) {
		GetComponent<SpriteRenderer> ().color = new Color(c.r, c.g, c.b, .5f);
	}

	void LateUpdate () {
		transform.rotation = Quaternion.identity;
	}
}
