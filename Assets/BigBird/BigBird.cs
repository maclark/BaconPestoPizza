using UnityEngine;
using System.Collections;

public class BigBird : MonoBehaviour {

	public float moveForce = 75f;
	public int hp = 1000;
	public float rotateSpeed = .1f;
	public bool engineOn = false;

	private GameManager gm;
	private Rigidbody2D rb;
	private Component[] dockTransforms;
	private Quaternion targetRotation = Quaternion.identity;

	// Use this for initialization
	void Awake() {
		gm = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
		rb = GetComponent<Rigidbody2D> ();

	}

	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate () {
		if (engineOn) {
			rb.AddForce (transform.up * moveForce);
		}

		if (turning) {
			transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
		}

		print (rb.velocity);
	}

	public Vector3 GetNearestOpenDock (Vector3 dockingShipPos) {
		dockTransforms = GetComponentsInChildren<Dock> ();
		float closestDockDistance = 10000f;
		Vector3 closestDock = Vector3.zero;
		foreach (Dock k in dockTransforms) {
			float d = Vector3.Distance( k.transform.position, dockingShipPos);
			if (d < closestDockDistance) {
				closestDock = k.transform.position;
				closestDockDistance = d;
			}
		}
		return closestDock;
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "EnemyBullet") {
			if (Random.Range (0, 1f) > .5) {
				other.GetComponent<Bullet>().Die();
			}
		} else if (other.tag == "Enemy") {
			TakeDamage (other.GetComponent<Enemy> ().kamikazeDamage);
			Destroy (other.gameObject);
		}
	}

	void TakeDamage( int dam) {
		hp -= dam;
		if (hp <= 0) {
			Die ();
		}
	}

	void Die() {
		gm.RemoveAlliedTransform (transform);
		Destroy (gameObject);
	}

	public void SetTargetRotationZAngle (float zAngle) {
		targetRotation.eulerAngles = new Vector3( 0, 0, zAngle);
	}

	public bool turning { get; set; }

}
