using UnityEngine;
using System.Collections;

public class BigBird : MonoBehaviour {

	public float moveForce = 75f;
	public int hp = 1000;

	private Vector2 direction = Vector2.up;
	private GameManager gm;
	private Rigidbody2D rb;
	private Component[] dockTransforms;

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
		rb.AddForce (direction * moveForce);
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
			TakeDamage (other.GetComponent<Bullet> ().damage);
			Destroy (other.gameObject);
		} else if (other.tag == "Enemy") {
			print ("hit enemy");
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
}
