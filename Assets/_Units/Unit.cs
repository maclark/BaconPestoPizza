using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour {
	protected SpriteRenderer sr;
	public float wakeUpRange = 300f;

	protected virtual void OnAwake () {
		sr = GetComponent<SpriteRenderer> ();
	}
	protected virtual void OnStart () {
	}

	protected virtual void OnUpdate () {
	}

	public IEnumerator FlashRed (float flashLength) {
		if (sr.color == Color.red) {
			yield break;
		}
		Color ogColor = sr.color;
		sr.color = Color.red;
		yield return new WaitForSeconds (flashLength);
		sr.color = ogColor;
	}

	public virtual void PlayerIsNear (Transform p) {
		if (Vector2.Distance (p.position, transform.position) < wakeUpRange) {
			gameObject.SetActive (true);
		}
	}

	public virtual void PlayerIsFar (Transform p) {
		if (Vector2.Distance (p.position, transform.position) > wakeUpRange) {
			gameObject.SetActive (false);
		}
	}
}
