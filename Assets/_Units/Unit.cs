﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour {
	public int hp;
	public float wakeUpRange = 300f;
	public Transform attacker;
	public Color color;

	protected SpriteRenderer sr;

	protected virtual void OnAwake () {
		
		sr = GetComponent<SpriteRenderer> ();
		if (!sr) {
			sr = GetComponentInChildren<SpriteRenderer> ();
		}
		color = sr.color;
	}

	protected virtual void OnStart () {
	}

	protected virtual void OnUpdate () {
	}

	public virtual void TakeDamage (int dam, Color c) {
		hp -= dam;
		if (hp <= 0) {
			Die ();
		} else
			StartCoroutine (Flash (.1f, c));
	}

	public virtual void Die () {
		if (attacker) {
			attacker.SendMessage ("Killed", transform, SendMessageOptions.DontRequireReceiver);
		}
		Destroy (gameObject);
	}

	public IEnumerator Flash (float flashLength, Color c) {
		if (sr.color == c) {
			yield break;
		}
		sr.color = c;
		yield return new WaitForSeconds (flashLength);
		sr.color = color;
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
