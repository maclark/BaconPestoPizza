using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class Bird : MonoBehaviour {

	public Player p;
	public int health = 100;
	public int maxHealth = 100;
	public float accelerationMagnitude = 60f;

	public float boostAccel = 60f;
	public bool docked = false;
	public float flashGap = 1f;
	public float flashDuration = .15f;
	public float flashDampener = .1f;
	public float hitInvincibilityDuration = .25f;
	public float boostCooldown = 2f;

	public float fullTank = 60f;
	public float lowGasWarning = 10f;
	public float gas = 60f;
	public float gasPerSecond = 1f;
	public float gasPerBoost = 2f;
	public float gasLightDelay = .2f;
	public float harpHurlOffset = 1f;
	public float rollDuration = 1f;
	public float rerollDelay = 1f;
	public float rollSpeed = 1f;

	public GameObject harpoonPrefab;
	public GameObject gasLightPrefab;
	public List<Bird> linkedBirds;
	public List<Harpoon> otherHarps;

	[HideInInspector]
	public Color color;
	public bool damaged = false, invincible = false, canBoost = true, hasHarpoon = true, canRoll = true, rolling = false;
	[HideInInspector]
	public Vector2 direction = Vector2.zero, pullDir = Vector2.zero, orthoPullDir = Vector2.zero;
	[HideInInspector]
	public Harpoon harp;
	[HideInInspector]
	public Web web;

	private Rigidbody2D rb;
	private GameManager gm;
	private SpriteRenderer sr;
	private BigBird bigBird;
	private Dock dock = null;
	private GameObject gasLight;
	private float startScaleFactor;
	private float startAccelerationgMagnitude;
	private bool ranOutOfGas = false;
	private bool gasLightFlashing = false;

	void Awake() {
		rb = GetComponent<Rigidbody2D> ();
		gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager> ();
		sr = GetComponent<SpriteRenderer> ();
		bigBird = GameObject.FindObjectOfType<BigBird>() as BigBird;
		startScaleFactor = transform.localScale.x;
	}

	void Start () {
		color = sr.color;

		otherHarps = new List<Harpoon> ();
		linkedBirds = new List<Bird> ();

		startAccelerationgMagnitude = accelerationMagnitude;

		Dock ();
		gm.AddAlliedTransform (transform);
	}

	void Update () {
		if (!docked) {
			if (!ranOutOfGas) {
				gas -= gasPerSecond * Time.deltaTime;
				if (gas <= lowGasWarning) {
					if (!gasLightFlashing) {
						LowOnGas ();
					} else if (gas <= 0) {
						ranOutOfGas = true;
					}
				}
			}

			if (rolling) {
				BarrelRoll ();
			}
		}
	}

	void FixedUpdate () {
		if (!docked) {
			HandleFlying ();
		}



		//check if just boosted
		if (!canBoost) {
			accelerationMagnitude = startAccelerationgMagnitude;
		}	
	}


	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "EnemyBullet") {
			if (rolling) {
				BounceBullets (other);
			}
			else if (!invincible) {
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

	void OnCollisionEnter2D (Collision2D coll) {
		if (coll.gameObject.name == "BigBird") {
			Dock ();
		}
	}

	void OnCollisionExit2D (Collision2D coll) {
		if (coll.gameObject.name == "BigBird") {
		}
	}

	void HandleFlying () {
		if (direction == Vector2.zero) {
			return;
		}


		if (otherHarps.Count > 0) {
			HandleFlyingWhileHarpooned ();
		}
		else if (harp != null) {
			//Has launched harp, check if max tethered 
			if (!harp.atMaxTether) {
				//Harp not at max tether, bird not harpooned by anything, move normally
				rb.AddForce (direction * accelerationMagnitude * rb.mass);
			} 
			//Has launched harpoon and it is at max tether
			else {
				HandleHarpAtMaxTether ();
			}
		} 
		//Hasn't launched harp and not harpooned by anything, move normally
		else {
			rb.AddForce (direction * accelerationMagnitude * rb.mass);
		}
	}

	void HandleFlyingWhileHarpooned () {
		//Make a list of masses dragging on Bird
		List<Transform> draggers = new List<Transform>();
		foreach (Harpoon h in otherHarps) {
			if (h.atMaxTether) {
				draggers.Add (h.GetHarpooner ().transform);
			}
		}

		//Check for harp at max tether and if it is attached to anything
		if (harp != null) {
			if (harp.atMaxTether) {
				if (harp.GetHarpooned () != null) {
					draggers.Add (harp.GetHarpooned ().transform);
				} else {
					draggers.Add (harp.transform);
				} 
			}
		}

		//WhaleDynamics is for calculating effective drag masses and doing the physics
		if (draggers.Count > 0) {
			WhaleDynamics wd = new WhaleDynamics (transform, draggers, accelerationMagnitude, direction);
			wd.CalculateWhaleDragging ();
		} 
		//If harp and harpooners aren't at max tether, then fly normally
		else {
			rb.AddForce (direction * accelerationMagnitude * rb.mass);
		}
	}

	void HandleHarpAtMaxTether () {
		//Calculate and add force parallel to pull vector
		pullDir = transform.position - harp.transform.position;
		pullDir.Normalize ();
		float pullMagnitude = Vector3.Dot (direction * accelerationMagnitude * rb.mass, pullDir);
		if (pullMagnitude > 0) {
			PullHarp (pullMagnitude);
		}
		else {
			//Check if harp is moving away from bird
			if (harp.GetComponent<Rigidbody2D> ().velocity != Vector2.zero) {
				TetherHarp ();
			}
			//Ship is not pulling on harp, so it moves normally
			rb.AddForce (direction * accelerationMagnitude * rb.mass);
		} 
	}

	void PullHarp (float pullMag) {
		Rigidbody2D rbOther;
		float totalMass = rb.mass;
		if (harp.GetHarpooned () != null) {
			rbOther = harp.GetHarpooned ().GetComponent<Rigidbody2D> ();
			totalMass += rbOther.mass;
		} else {
			rbOther = harp.GetComponent<Rigidbody2D> ();
			totalMass += rbOther.mass;
		}

		//Caclulate and add force orthogonal to pull vector
		orthoPullDir = new Vector3 (-pullDir.y, pullDir.x, 0);
		float orthoPullMag = Vector3.Dot (direction * accelerationMagnitude * rb.mass, orthoPullDir);		

		rb.AddForce (pullDir * pullMag / totalMass);
		rbOther.AddForce (pullDir * pullMag / totalMass);
		rb.AddForce (orthoPullDir * orthoPullMag * rb.mass);
	}

	void TetherHarp () {
		//determine direction of tether and orthogonal axis to that
		Vector2 directionOfTether = harp.transform.position - transform.position;
		directionOfTether.Normalize ();
		Vector2 orthoDirectionOfTether = new Vector2 (directionOfTether.y, -directionOfTether.x);

		//find components of velocity along new axes
		Vector2 harpVel = harp.GetComponent<Rigidbody2D> ().velocity;
		float harpVelTetherComponent = Vector2.Dot (harpVel, directionOfTether);
		float harpVelOrthoComponent = Vector2.Dot (harpVel, orthoDirectionOfTether);

		//if harpoon still moving away from harpooner, stop it
		if (harpVelTetherComponent > 0) {
			harpVelTetherComponent = 0;
		}

		//Add components of velocity to find resultant velocity vector of harpoon
		Vector2 resultantHarpVel = (directionOfTether * harpVelTetherComponent) + (orthoDirectionOfTether * harpVelOrthoComponent);
		harp.GetComponent<Rigidbody2D> ().velocity = resultantHarpVel;
	}

	/// <summary>
	/// flag gap increases linearly
	/// flash duration is constant
	/// </summary>
	/// <param name="dam">Dam.</param> testing this
	void TakeDamage( int dam) {
		if (damaged) {
			//#TODO explodeee
			Die ();
		} else {
			health -= dam;
			if (health < 0) {
				health = 0;
			}
			damaged = true;
			StartCoroutine (HitInvincibility (hitInvincibilityDuration));
			StartCoroutine (FlashDamage (Time.time));
		}
	}

	void Die () {
		gm.RemoveAlliedTransform (transform);
		//instantiate bubble
		if (p != null) {
			p.Bubble (rb.velocity);
		}
		Destroy (gameObject);
	}
			
	public bool Dock () {
		dock = bigBird.GetNearestOpenDock (transform.position);
		if (!dock) {
			docked = false;
			return docked;
		}

		dock.bird = this;
		docked = true;
		dock.gameObject.layer = LayerMask.NameToLayer ("Stations");

		DisableColliders ();
		sr.color = color;
		sr.sortingLayerName = "BigBird";
		sr.sortingOrder = 1;
		rb.Sleep ();

		transform.position = dock.transform.position;
		transform.parent = dock.transform;

		damaged = false;
		invincible = false;
		gasLightFlashing = false;
		ranOutOfGas = false;
		Destroy (gasLight);
		CancelInvoke ();

		if (rolling) {
			transform.rotation = Quaternion.identity;
			rolling = false;
			canRoll = true;
		}

		if (p) {
			dock.GetComponent<BoxCollider2D> ().enabled = false;
			p.GetComponent<PlayerInput> ().station = dock.transform;
			p.GetComponent<PlayerInput> ().state = PlayerInput.State.DOCKED;
			p.GetComponent<PlayerInput> ().CancelInvoke ();
			p.w.firing = false;
		}

		if (dock.transform.parent == bigBird.transform) {
			bigBird.AddToDockedBirds (this);
			//close
		}

		if (otherHarps.Count > 0) {
			foreach (Harpoon h in otherHarps) {
				h.DetachAndRecall (true);
			}
			otherHarps.Clear ();
		}

		if (harp) {
			harp.DetachAndRecall ();
			Destroy (harp.gameObject);
			hasHarpoon = true;
		}

		return docked;
	}

	public void Undock () {
		if (health < maxHealth) {
			Debug.Log ("Can't undock unhealthy bird.");
			return;
		}
		if (dock.transform.parent == bigBird.transform) {
			bigBird.RemoveFromDockedBirds (this);
		}

		dock.GetComponent<BoxCollider2D> ().enabled = true;
		dock.gameObject.layer = LayerMask.NameToLayer ("Default");
		docked = false;
		dock.bird = null;
		dock = null;

		transform.parent = null;
		transform.localScale = new Vector3 (startScaleFactor, startScaleFactor, 1);
		transform.rotation = Quaternion.identity;
		rb.WakeUp ();
		Vector3 releaseDirection = transform.position - bigBird.transform.position;
		releaseDirection.Normalize ();
		rb.AddForce (releaseDirection * boostAccel * rb.mass);
		Invoke ("EnableColliders", .5f);
		sr.sortingLayerName = "Birds";
		sr.sortingOrder = 0;
	}

	void EnableColliders () {
		Collider2D[] colls = GetComponentsInChildren<Collider2D> (true);
		foreach (Collider2D coll in colls) {
			coll.enabled = true;
		}
	}

	void DisableColliders () {
		Collider2D[] colls = GetComponentsInChildren<Collider2D> ();
		foreach (Collider2D coll in colls) {
			coll.enabled = false;
		}
	}

	IEnumerator FlashDamage (float timeAtDamage) {

		Color startColor = sr.color;
		sr.color = new Color (1, 0, 0, Mathf.Max(startColor.a, .5f));
		yield return new WaitForSeconds (flashDuration);
		if (docked) {
			yield break;
		}
		sr.color = startColor;

		if (damaged) {
			float flashGapModifier = flashDampener * (Time.time - timeAtDamage);
			if (flashGapModifier > 1) {
				//explode
				Die ();
				yield break;
			}
			yield return new WaitForSeconds (Mathf.Max (0f, flashGap - flashGapModifier));
			if (docked) {
				yield break;
			}
			StartCoroutine (FlashDamage (timeAtDamage));
		}
	}



	/// <summary>
	/// Hurls the harpoon.
	/// </summary>
	public void HurlHarpoon () {
		if (hasHarpoon) {
			//Offset harp so it doesn't immediately collide with ship
			p.aim.Normalize ();
			GameObject harpoon = Instantiate (harpoonPrefab, p.transform.position + p.aim * harpHurlOffset, transform.rotation) as GameObject;
			harpoon.name = "Harpoon";
			harp = harpoon.GetComponent<Harpoon> ();
			harp.SetHarpooner (gameObject);
			harp.Fire (harp.transform, p.aim);
			hasHarpoon = false;
		}
	}

	public void DetachHarpoon () {
		if (harp) {
			harp.DetachAndRecall ();
		}
	}

	IEnumerator HitInvincibility (float duration) {	
		invincible = true;
		Color startColor = sr.color;
		sr.color = new Color (startColor.r, startColor.g, startColor.b, .1f);
		p.GetComponent<SpriteRenderer> ().color = sr.color;
		yield return new WaitForSeconds (duration);
		if (p) { 
			p.GetComponent<SpriteRenderer> ().color = startColor;
			sr.color = startColor;

		} else {
			sr.color = Color.black;
		}
		invincible = false;
	}

	public IEnumerator Boost () {
		canBoost = false;
		gas -= gasPerBoost;
		accelerationMagnitude = boostAccel;
		yield return new WaitForSeconds (boostCooldown);
		canBoost = true;
	}

	public void BarrelRoll () {
		transform.Rotate (rollSpeed * Time.deltaTime, 0, 0);
	}

	public IEnumerator EndRoll () {
		yield return new WaitForSeconds (rollDuration);
		//if about to burn up (die) this invoke can fail
		if (!docked) {
			transform.rotation = Quaternion.identity;
			rolling = false;
			yield return new WaitForSeconds (rerollDelay);
			canRoll = true;
		}
	}

	void LowOnGas () {
		Vector3 position = new Vector3 (transform.position.x, transform.position.y + 1.5f * GetComponent<BoxCollider2D> ().size.y, 0f);
		gasLight = Instantiate (gasLightPrefab, position, transform.rotation) as GameObject;
		gasLight.transform.parent = transform;
		gasLightFlashing = true;
		StartCoroutine (FlashGasLight());
	}

	IEnumerator FlashGasLight () {
		if (gasLight) {
			if (ranOutOfGas) {
				gasLight.GetComponent<SpriteRenderer> ().enabled = true;
				gasLightFlashing = false;
			} else if (gasLight.GetComponent<SpriteRenderer> ().enabled == true) {
				gasLight.GetComponent<SpriteRenderer> ().enabled = false;
				yield return new WaitForSeconds (gasLightDelay / 2);
			} else if (gasLight.GetComponent<SpriteRenderer> ().enabled == false) {
				gasLight.GetComponent<SpriteRenderer> ().enabled = true;
				yield return new WaitForSeconds (gasLightDelay);
			}
		}

		if (gasLight && gasLightFlashing) {
			StartCoroutine (FlashGasLight ());
		}
	}

	public void RemoveHarp (Harpoon harpy) {
		otherHarps.Remove (harpy);
	}

	public Dock GetDock () {
		return dock;
	}

	private void BounceBullets (Collider2D other) {
		//Since we hit the bullets twice during the roll, it works if one frame we stop the bullets
		//and the next frame we Fire them Away. Kinda inelegant.
		Vector3 away = other.transform.position - transform.position;
		away.Normalize ();
		if (other.GetComponent<Rigidbody2D> ().velocity == Vector2.zero) {
			other.GetComponent<Bullet> ().Fire (other.transform.position, away);
		} else {
			other.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		}
	}
}
