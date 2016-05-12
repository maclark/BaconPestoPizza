using UnityEngine;
using System.Collections;

public class BigBird : MonoBehaviour {

	public float moveForceMagnitude = 75f;
	public int hp = 1000;
	public float engineOnRotateSpeed = .2f;
	public float engineOffRotateSpeed = .1f;
	public bool turning { get; set; }
	public float rotateSpeed { get; set; }

	private GameManager gm;
	private Rigidbody2D rb;
	private Component[] dockTransforms;
	private Quaternion targetRotation = Quaternion.identity;
	private bool engineOn = false;

	// Use this for initialization
	void Awake() {
		gm = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
		rb = GetComponent<Rigidbody2D> ();

	}

	void Start () {
		GetComponent<SpriteRenderer> ().color = Color.gray;
	}

	void FixedUpdate () {
		if (engineOn) {
			rb.AddForce (transform.up * moveForceMagnitude);
		}

		if (turning) {
			transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
		}
	}

	/// <summary>
	/// MAKE SURE TO CHEK IF RETURED NULL DOCK. Probably should use bool return type with an out dock variable.
	/// </summary>
	/// <returns>The nearest open dock.</returns>
	/// <param name="dockingShipPos">Docking ship position.</param>
	public Dock GetNearestOpenDock (Vector3 dockingShipPos) {
		dockTransforms = GetComponentsInChildren<Dock> ();
		float closestDockDistance = 10000f;
		Dock closestDock = null;
		foreach (Dock k in dockTransforms) {
			if (!k.occupied) {
				float d = Vector3.Distance (k.transform.position, dockingShipPos);
				if (d < closestDockDistance) {
					closestDock = k;
					closestDockDistance = d;
				}
			}
		}
		return closestDock;
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "EnemyBullet") {
			if (Random.Range (0, 1f) > .5) {
				other.GetComponent<Bullet>().Die();
			}
		} 
		else if (other.tag == "Enemy") {
			TakeDamage (other.GetComponent<Enemy> ().kamikazeDamage);
			Destroy (other.gameObject);
		}
		else if (other.name == "Web") {
			TurnEngineOn ();
			moveForceMagnitude = 10 * moveForceMagnitude;
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

	public void TurnEngineOn () {
		engineOn = true;
		GetComponent<SpriteRenderer> ().color = Color.white;
		rotateSpeed = engineOnRotateSpeed;
	}

	public void TurnEngineOff () {
		engineOn = false;
		GetComponent<SpriteRenderer> ().color = Color.gray;
		rotateSpeed = engineOffRotateSpeed;
	}

	public bool GetEngineOn () {
		return engineOn;
	}
}
