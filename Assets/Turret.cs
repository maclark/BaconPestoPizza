using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour {

	public bool rightCannon;

	void Rotate (Vector3 direction) {
		float newZ = Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg;
		transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y, newZ, transform.rotation.w);
		print ("Direction: " + direction + ", " + "z: " + transform.rotation.z);
		if (rightCannon) {
			if (transform.rotation.z > 15) {
				transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y, 15f, transform.rotation.w);
			}
			else if (transform.rotation.z < -120) {
				transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y, -120f, transform.rotation.w);
			}
		}
		else {
			if (transform.rotation.z < -15) {
				transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y, -15f, transform.rotation.w);
			}
			else if (transform.rotation.z > 120) {
				transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y, 120f, transform.rotation.w);
			}
		}
	}
}
