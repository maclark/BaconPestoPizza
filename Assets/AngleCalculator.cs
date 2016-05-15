using UnityEngine;
using System.Collections.Generic;

public class AngleCalculator : MonoBehaviour {

	public Transform m1;
	public List<Transform> attachedMasses;
	public float a = 100f;
	public Vector3 pullingDirection;

	//public Transform[] masses;
	//public float[] angles;
	//public Vector3[] vectors;
	//public float[] accelerations;

	private float pullingMass;
	private List<AttachedMass> massList;


	public class AttachedMass {
		public Transform t; 
		public float m; //mass
		public float a; //accel
		public float theta; //angle
		public Vector3 v;

		public AttachedMass () {
		}
	}

	void Start () {
		//pullingDirection = Vector3.right;
		pullingMass = m1.GetComponent<Rigidbody2D> ().mass;
		massList = new List<AttachedMass> ();
		print (Application.targetFrameRate);
	}

	void FixedUpdate ()
	{
		massList.Clear ();
		foreach (Transform t in attachedMasses) {
			AttachedMass am = new AttachedMass ();
			am.t = t;
			am.m = t.GetComponent<Rigidbody2D> ().mass;
			am.v = m1.position - t.position;
			am.v.Normalize ();
			am.theta = Vector3.Angle (pullingDirection, am.v);
			if (am.theta < 90f) {
				am.theta = am.theta * Mathf.Deg2Rad;
				massList.Add (am);
			}
		}

		//TODO check if masses.Count == 0
		float virtualMass = 0f;
		float af = 0f;
		foreach (AttachedMass am in massList) {
			virtualMass += am.m * Mathf.Cos (am.theta);
		}
		af = a * pullingMass / (pullingMass + virtualMass);

		foreach (AttachedMass am in massList) {
			am.a = Vector3.Dot (af * pullingDirection, am.v);
			am.t.GetComponent<Rigidbody2D> ().AddForce (am.a * am.v);
		}
		m1.GetComponent<Rigidbody2D> ().AddForce (af * pullingDirection);
	}
}



		/*
		 * 
	void Start () {
		masses = new Transform[2];
		angles = new float[2];
		vectors = new Vector3[2];
		accelerations = new float[2];
		masses[0] = m2;
		masses[1] = m3;

		m = m1.GetComponent<Rigidbody2D> ().mass;
	}

	// Update is called once per frame
	void FixedUpdate () {

		for (int i = 0; i < masses.Length; i++) {
			vectors [i] = masses [i].position - m1.position;
			float ang = Vector3.Angle (direction, vectors [i]);
			ang -= 90f;
			ang = ang * Mathf.Deg2Rad;
			angles [i] = ang;
		}

		float virtualMass = 0f;
		for (int i = 0; i < masses.Length; i++) {
			virtualMass += masses[i].GetComponent<Rigidbody2D>().mass * Mathf.Sin (angles[i]);
		}


		float af = a * m / (m + virtualMass);

		for (int i = 0; i < vectors.Length; i++) {
			Vector3 temp = vectors [i];
			temp = -1 * temp; 
			temp.Normalize ();
			vectors [i] = temp;
		}

		for (int i = 0; i < accelerations.Length; i++) {
			accelerations[i] = Vector3.Dot (af * direction, vectors[i]);
			masses [i].GetComponent<Rigidbody2D> ().AddForce (accelerations [i] * vectors [i]);
		}

		m1.GetComponent<Rigidbody2D> ().AddForce (af * direction);



		/*Vector3 v2 = m2.position - m1.position;
		Vector3 v3 = m3.position - m1.position;

		float angleV1V2 = Vector3.Angle (v1, v2);
		float theta = angleV1V2 - 90f;
		float angleV1V3 = Vector3.Angle (v1, v3);
		float phi = angleV1V3 - 90f;

		float thetaRad = theta * Mathf.Deg2Rad;
		float phiRad = phi * Mathf.Deg2Rad;

		float af = a * m / (m + n * Mathf.Sin (thetaRad) + o * Mathf.Sin (phiRad));

		float virualMass = n * Mathf.Sin (thetaRad) + o * Mathf.Sin (phiRad);

		v2 = -v2;
		v2.Normalize ();
		b = Vector3.Dot (af * Vector3.right, v2);
		v3 = -v3;
		v3.Normalize ();
		c = Vector3.Dot (af * Vector3.right, v3);

		m1.GetComponent<Rigidbody2D> ().AddForce (af * Vector3.right);
		m2.GetComponent<Rigidbody2D> ().AddForce (b * v2);
		m3.GetComponent<Rigidbody2D> ().AddForce (c * v3);	


	//b = (a m sin(phi))/(n (sin(theta) cos(phi)+cos(theta) sin(phi)))
//b = (m * a / n) * (Mathf.Sin (phi) / (Mathf.Sin (theta) * Mathf.Cos (phi) + Mathf.Cos (theta) * Mathf.Sin (phi)));

//c = (a m sin(theta))/(o (sin(theta) cos(phi)+cos(theta) sin(phi)))
//c = (m * a / o) * (Mathf.Sin (theta) / (Mathf.Sin (theta) * Mathf.Cos (phi) + Mathf.Cos (theta) * Mathf.Sin (phi)));

	}
}
*/


