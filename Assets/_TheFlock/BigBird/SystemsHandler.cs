using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SystemsHandler {
	public float cannonLoadTime = 2f;
	public float maxFuseTime = .3f;
	public bool cannonsReady = true;
	public GameObject cannonballPrefab;
	public List<Transform> rightCannons;
	public List<Transform> leftCannons;

	private GameManager gm;
	private BigBird bb;

	public SystemsHandler () {
		gm = GameObject.FindObjectOfType<GameManager> ();
		bb = gm.bigBird;
	}


	public void FireBroadside (bool rightSide) {
		bb.Invoke ("ReadyCannons", cannonLoadTime);

		if (rightSide) {
			foreach (Transform t in rightCannons) {
				float fuse = Random.Range (0, maxFuseTime);
				bb.StartCoroutine (FireCannon (t, fuse));
			}
		} else {
			foreach (Transform t in leftCannons) {
				float fuse = Random.Range (0, maxFuseTime);
				bb.StartCoroutine (FireCannon (t, fuse));
			}
		}
	}

	IEnumerator FireCannon (Transform t, float fuseLength) {
		yield return new WaitForSeconds (fuseLength);
		//TODO object pooling
		GameObject obj = Instantiate (cannonballPrefab, t.position, Quaternion.identity) as GameObject; 
	}

	void ReadyCannons () {
		cannonsReady = true;
	}
}
