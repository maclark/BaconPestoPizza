using UnityEngine;
using UnityEditor;
using System.Collections;

public class Player : MonoBehaviour {

	public int hp = 100;
	public float moveForce = 20f;
	public float fireRate = .1f;
	public float releaseBoost = 60f;
	public string LSHorizontal = "LS_Horizontal_P1";
	public string LSVertical = "LS_Vertical_P1";
	public string RSHorizontal = "RS_Horizontal_P1";
	public string RSVertical = "RS_Vertical_P1";
	public string rightTrigger = "RT_P1";
	public string aButton = "A_P1";
	public string interactButton = "B_P1";
	public string boostButton = "LB_P1";
	public bool docked = false;
	public float thresholdToTurnBigBird = .2f;
	public float flashGap = 1f;
	public float flashDuration = .15f;
	public float flashDampener = .1f;
	public float hitInvincibilityDuration = .25f;
	public float boostCooldown = 2f;

	private Vector2 direction = Vector2.zero;
	private Vector2 aim = Vector2.zero;
	private Rigidbody2D rb;
	private GameManager gm;
	private BigBird bigBird;
	private ObjectPooler objectPooler;
	private bool firing = false;
	private bool damaged = false;
	private bool invincible = false;
	private bool canBoost = true;
	private Color originalColor;

	void Awake() {
		rb = GetComponent<Rigidbody2D> ();
		gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager> ();
		bigBird = GameObject.FindObjectOfType<BigBird>() as BigBird;
		objectPooler = GetComponent<ObjectPooler> ();
		originalColor = GetComponent<SpriteRenderer> ().color;
	}
	
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
			if (!invincible) {
				TakeDamage (other.GetComponent<Bullet> ().damage);
				other.GetComponent<Bullet> ().Die ();
			}
		}
		else if (other.tag == "Enemy") {
			if (!invincible) {
				TakeDamage (other.GetComponent<Enemy> ().kamikazeDamage);
				Destroy (other.gameObject);
			}
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
			
		if (!firing && Input.GetAxis (rightTrigger) > 0) {
			firing = true;
			InvokeRepeating ("FireBullet", fireRate, fireRate);
		} else if (firing && Input.GetAxis (rightTrigger) <= 0) {
			firing = false;
			CancelInvoke ();
		}

		if (Input.GetButton (boostButton)) {
			if (canBoost) {
				StartCoroutine( Boost ());
			}
		}

		else if (Input.GetButton ("10_P1")) {
			print ("10_P1");
		}
		else if (Input.GetButton ("11_P1")) {
			print ("11_P1");
		}
		else if (Input.GetButton ("12_P1")) {
			print ("12_P1");
		}
		else if (Input.GetButton ("13_P1")) {
			print ("13_P1");
		}
		else if (Input.GetButton ("14_P1")) {
			print ("14_P1");
		}
		else if (Input.GetButton ("15_P1")) {
			print ("15_P1");
		}
	}

	void HandleDockedInput () {
		if (Input.GetButtonDown (interactButton)) {
			Undock ();
		} else {
			float x = Input.GetAxis (RSHorizontal);
			float y = Input.GetAxis(RSVertical); 

			if (Mathf.Sqrt (x * x + y * y) > thresholdToTurnBigBird) {
				float angle = Mathf.Atan2 (Input.GetAxis (RSHorizontal), Input.GetAxis (RSVertical)) * Mathf.Rad2Deg;
				bigBird.turning = true;
				bigBird.SetTargetRotationZAngle (-angle);
			} else
				bigBird.turning = false;
		}

		if (Input.GetButtonDown (aButton)) {
			bigBird.engineOn = !bigBird.engineOn;
		}
	}

	public void FireBullet() {
		Bullet bullet = objectPooler.GetPooledObject().GetComponent<Bullet>();
		bullet.gameObject.SetActive (true);
		bullet.Fire (transform, aim);
	}

	/// <summary>
	/// flag gap increases linearly
	/// flash duration is constant
	/// </summary>
	/// <param name="dam">Dam.</param> testing this
	void TakeDamage( int dam) {
		if (damaged) {
			//explodeee
			Die();
		} else {
			damaged = true;
			StartCoroutine (Invincibility (hitInvincibilityDuration));
			StartCoroutine (FlashDamage (Time.time));
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
			transform.parent = bigBird.transform;
			GetComponent<BoxCollider2D> ().enabled = false;
			GetComponent<SpriteRenderer> ().color = originalColor;
			rb.Sleep ();
			CancelInvoke ();
			firing = false;
			damaged = false;
			invincible = false;
			docked = true;
			return true;
		} else return false;
	}

	private void Undock () {
		docked = false;
		transform.parent = null;
		transform.rotation = Quaternion.identity;
		rb.WakeUp ();
		Vector3 releaseDirection = transform.position - bigBird.transform.position;
		releaseDirection.Normalize ();
		rb.AddForce (releaseDirection * releaseBoost);
		Invoke( "EnableCollider", .5f);
	}

	void EnableCollider () {
		GetComponent<BoxCollider2D> ().enabled = true;
	}

	IEnumerator FlashDamage (float timeAtDamage) {
		if (docked)
			yield break;
		
		Color startColor = GetComponent<SpriteRenderer> ().color;
		GetComponent<SpriteRenderer> ().color = new Color (1, 0, 0, Mathf.Max(startColor.a, .5f));
		//EditorApplication.Beep ();
		yield return new WaitForSeconds (flashDuration);
		GetComponent<SpriteRenderer> ().color = startColor;

		if (damaged) {
			float flashGapModifier = flashDampener * (Time.time - timeAtDamage);
			if (flashGapModifier > 1) {
				//explode
				Die();
				return true;
			}
			yield return new WaitForSeconds (Mathf.Max (0f, flashGap - flashGapModifier));
			StartCoroutine (FlashDamage (timeAtDamage));
		}
	}

	IEnumerator Invincibility (float duration) {
		invincible = true;
		SpriteRenderer sr = GetComponent<SpriteRenderer> ();
		Color startColor = sr.color;
		sr.color = new Color (startColor.r, startColor.g, startColor.b, .1f);
		yield return new WaitForSeconds (duration);
		sr.color = new Color (startColor.r, startColor.g, startColor.b, 1f);
		invincible = false;
	}

	IEnumerator Boost () {
		canBoost = false;
		rb.AddForce (direction * releaseBoost);
		yield return new WaitForSeconds (boostCooldown);
		canBoost = true;
	}

}
