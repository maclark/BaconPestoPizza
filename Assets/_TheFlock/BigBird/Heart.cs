using UnityEngine;
using System.Collections;

public class Heart : MonoBehaviour {

	private GameManager gm;

	void Awake () {
		gm = GameObject.FindObjectOfType<GameManager> ();
	}

	void OnBecameInvisible () {
		gm.bbm.bigBirdIndicator.gameObject.SetActive (true);
	}

	void OnBecameVisible () {
		gm.bbm.bigBirdIndicator.transform.position = gm.bigBird.transform.position;
		gm.bbm.bigBirdIndicator.gameObject.SetActive (false);
	}
}
