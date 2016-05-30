using UnityEngine;
using System.Collections;

public class President : MonoBehaviour {

	public Vector3 velocity;
	public Transform target;
	private GameManager gm;

	void Awake () {
		gm = GameObject.FindObjectOfType<GameManager> ();
	}
	// Use this for initialization
	void Start () {
		target = gm.GetNextAppointment ();
		SetCamera ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (target) {
			if (transform.position == target.position) {
				target = null;
				ReachedTarget ();
			} else {
				transform.position = Vector3.MoveTowards (transform.position, target.position, velocity.magnitude * Time.deltaTime);
			}
		} else {
			transform.position += velocity * Time.deltaTime;
		}
	}
	void SetCamera () {
		Camera.main.GetComponent<CameraFollow> ().offsetDir = DirectionOfTravel ();
	}

	public Vector3 DirectionOfTravel () {
		if (target) {
			Vector3 direction = target.position - transform.position;
			direction.Normalize ();
			return direction;
		} else
			return Vector3.zero;
	}

	void ReachedTarget () {
		target = gm.GetNextAppointment ();
		SetCamera ();
	}
}
