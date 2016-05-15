using UnityEngine;
using System.Collections.Generic;

public class WhaleDynamics {

	public class AttachedMass {
		public Transform t; 
		public float m; //mass
		public float a; //accel
		public float theta; //angle
		public Vector3 v;

		public AttachedMass () {}
	}

	/* TODO When a harpooner is pulled and it is also harpooned,
	 * its harpoon tether stretches indefinitely, even though
	 * at max tether. When pulling on a harpooner, need to recursively
	 * check if it also harpooned and calculate effective mass on it.
	 * */


	public Transform w;
	public List<Transform> attachedMasses;
	public float a = 100f;
	public Vector3 pullingDirection;

	private float pullingMass;
	private List<AttachedMass> effectiveMasses;

	public WhaleDynamics (Transform whale, List<Transform> harpooners, float whaleAcceleration, Vector3 whaleDirection) {
		w = whale;
		attachedMasses = harpooners;
		a = whaleAcceleration;
		pullingDirection = whaleDirection;
		pullingMass = w.GetComponent<Rigidbody2D> ().mass;
		effectiveMasses = new List<AttachedMass> ();
	}

	//For when a moving body is harpooned by multiple harpooners
	public void CalculateWhaleDragging ()
	{
		//First, determine which masses are behind the orthogonal line to the pull direction
		effectiveMasses.Clear ();
		foreach (Transform t in attachedMasses) {
			AttachedMass am = new AttachedMass ();
			am.t = t;
			am.m = t.GetComponent<Rigidbody2D> ().mass;
			am.v = w.position - t.position;
			am.v.Normalize ();
			am.theta = Vector3.Angle (pullingDirection, am.v);
			if (am.theta < 90f) {
				am.theta = am.theta * Mathf.Deg2Rad;
				effectiveMasses.Add (am);
			}
		}

		//For effective masses, compute how much of their mass contributes to the drag
		float virtualMass = 0f;
		float af = 0f;
		foreach (AttachedMass am in effectiveMasses) {
			virtualMass += am.m * Mathf.Cos (am.theta);
		}
		//Use the result to calculate the net acceleration of the whale
		af = a * pullingMass / (pullingMass + virtualMass);

		//Apply that along the lines of tension to the effectiveMasses and the whale
		foreach (AttachedMass am in effectiveMasses) {
			am.a = Vector3.Dot (af * pullingDirection, am.v);
			am.t.GetComponent<Rigidbody2D> ().AddForce (am.a * am.v);
		}
		w.GetComponent<Rigidbody2D> ().AddForce (af * pullingDirection);
	}
}
