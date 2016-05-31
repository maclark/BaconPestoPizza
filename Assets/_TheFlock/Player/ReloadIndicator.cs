using UnityEngine;
using System.Collections;

public class ReloadIndicator : MonoBehaviour {

	public Transform bar;
	public Transform slider;

	private float startTime;
	private float duration;

	public void StartReload (float reloadTime) {
		startTime = Time.time;
		duration = reloadTime;
		slider.position = bar.position + Vector3.left;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 leftSide = bar.position + Vector3.left;
		Vector3 rightSide = bar.position + Vector3.right;
		float t = (Time.time - startTime) / duration;
		slider.position = Vector3.Lerp (leftSide, rightSide, t); 
		if (t > 1) {
			gameObject.SetActive (false);
		}
	}

	void LateUpdate () {
		transform.rotation = Quaternion.identity;
	}

	public void SetColor (Color c) {
		bar.GetComponent<SpriteRenderer> ().color = c;
		slider.GetComponent<SpriteRenderer> ().color = c;
	}
}
