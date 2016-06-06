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

	public SystemsHandler (List<Transform> cannonsR, List<Transform> cannonsL, GameObject ball) {
		rightCannons = cannonsR;
		leftCannons = cannonsL;
		gm = GameObject.FindObjectOfType<GameManager> ();
		bb = gm.bigBird;
		cannonballPrefab = ball;
	}

	public void FireBroadside (bool rightSide) {
		if (!cannonsReady) {
			return;
		}
			
		bb.StartCoroutine (ReadyCannons());
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
		cannonsReady = false;
	}

	IEnumerator FireCannon (Transform t, float fuseLength) {
		yield return new WaitForSeconds (fuseLength);
		//TODO object pooling
		GameObject obj = GameObject.Instantiate (cannonballPrefab, t.position, Quaternion.identity) as GameObject; 
		Cannonball ball = obj.GetComponent<Cannonball> ();
		ball.Fire (t.position, t.up);
	}

	IEnumerator ReadyCannons () {
		yield return new WaitForSeconds (cannonLoadTime);
		cannonsReady = true;
	}
}
