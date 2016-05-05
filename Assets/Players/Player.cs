﻿using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public int hp = 100;
	public float moveForce = 20f;
	public float releaseBoost = 60f;
	public string LSHorizontal = "LS_Horizontal_P1";
	public string LSVertical = "LS_Vertical_P1";
	public string RSHorizontal = "RS_Horizontal_P1";
	public string RSVertical = "RS_Vertical_P1";
	public string fireButton = "RT_P1";
	public string interactButton = "B_P1";
	public bool docked = false;

	private Vector2 direction = Vector2.zero;
	private Vector2 aim = Vector2.zero;
	private Rigidbody2D rb;
	private GameManager gm;
	private BigBird bigBird;
	private ObjectPooler objectPooler;

	// Use this for initialization
	void Awake() {
		rb = GetComponent<Rigidbody2D> ();
		gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager> ();
		bigBird = GameObject.FindObjectOfType<BigBird>() as BigBird;
		objectPooler = GetComponent<ObjectPooler> ();

	}

	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

		if (docked) {
			HandleDockedInput ();
		} else {
			HandleFlyingInput ();
		}

	}

	void FixedUpdate () {
		if (!docked) {
			rb.AddForce (direction * moveForce);
		}
	}

	void OnTriggerEnter2D ( Collider2D other) {
		if (other.name == "BigBird") {
			if (!docked) {
				Dock ();
			}
		}
		else if (other.tag == "EnemyBullet") {
			TakeDamage (other.GetComponent<Bullet> ().damage);
			Destroy (other.gameObject);
		}
		else if (other.tag == "Enemy") {
			print ("hit enemy");
			TakeDamage (other.GetComponent<Enemy> ().kamikazeDamage);
			Destroy (other.gameObject);
		}
	}

	void HandleFlyingInput () {
		direction = new Vector2( Input.GetAxis(LSHorizontal), Input.GetAxis(LSVertical));
		Vector2 rightStick = new Vector2( Input.GetAxis(RSHorizontal), Input.GetAxis(RSVertical));

		if (rightStick != Vector2.zero) {
			aim = rightStick;
		} 
		else if (aim == Vector2.zero) {
			if (direction != Vector2.zero) {
				aim = direction;
			}
		}

		if (Input.GetAxis (fireButton) > 0) {
			FireBullet ();
		}
	}

	void HandleDockedInput () {
		if (Input.GetButtonDown (interactButton)) {
			Undock ();
		}
	}

	public void FireBullet() {
		GameObject bullet = objectPooler.GetPooledObject();
		bullet.SetActive (true);
		bullet.GetComponent<Bullet> ().Fire (transform, aim);
	}

	void TakeDamage( int dam) {
		hp -= dam;
		if (hp <= 0) {
			Die ();
		}
	}

	void Die() {
		Destroy (gameObject);
		gm.RemoveAlliedTransform (transform);
	}
			
	private bool Dock () {
		Vector3 dockPosition = bigBird.GetNearestOpenDock(transform.position);
		if (dockPosition != Vector3.zero) {
			transform.Translate (dockPosition - transform.position);
			docked = true;
			transform.parent = bigBird.transform;
			rb.Sleep ();
			GetComponent<BoxCollider2D> ().enabled = false;
			return true;
		} else return false;
	}

	private void Undock () {
		docked = false;
		transform.parent = null;
		rb.WakeUp ();
		Vector3 releaseDirection = transform.position - bigBird.transform.position;
		releaseDirection.Normalize ();
		rb.AddForce (releaseDirection * releaseBoost);
		Invoke( "EnableCollider", .5f);
	}

	void EnableCollider () {
		GetComponent<BoxCollider2D> ().enabled = true;
	}
}
