using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Kanga : Flyer {
	public float skew = 2f;
	public Neoner rider;
	public int health;
	public int maxHealth;
	public float forceMag;
	public float hydratedForceMag;
	public float boostForceMag;
	public float flashGap = 1f;
	public float flashDuration = .2f;
	public float damageDuration = 10f;
	public float flashDampener = .1f;
	public float hitInvincibilityDuration = .25f;
	public float boostCooldown = 2f;
	public float dockRayLength = 8f;
	public float undockColliderDelay = .2f;

	public float harpHurlOffset = 1f;
	public float swingSpeed = 3f;
	public float windUpTime = 1f;
	public float aimThetaModifier = 1f;
	public float earlyReleaseAngleRad = Mathf.PI / 4;
	public float rollDuration = 1f;
	public float rerollDelay = 1f;
	public float rollSpeed = 1f;
	public float eggLayDelay = 3f;
	public float deathTorque = 25f;

	public bool docked = false;
	public bool harpLoaded = false;
	public bool aimingHarp = false;
	public bool swingingHarp = false;
	public bool throwingHarp = false;
	public bool catchingHarp = false;
	public bool colorSet = false;

	public Transform body;
	public GameObject harpoonPrefab;
	public Sprite kanga;
	public List<Harpoon> otherHarps = new List<Harpoon> ();

	public Color color;
	public bool damaged = false, invincible = false, canBoost = true, hurledHarp = false, canRoll = true, rolling = false;
	public Vector2 direction = Vector2.zero, pullDir = Vector2.zero, orthoPullDir = Vector2.zero;
	public Harpoon harp;

	private Rigidbody2D rb;
	private GameManager gm;
	private SpriteRenderer sr;
	private Dock dock = null;
	private Shield shield;
	private ReloadIndicator reloadIndicator;
	private Harpoon loadedHarp;
	private Vector3 releaseHarpPosition;
	private Vector3 hurlAim;
	private float swingStartTime;
	private float theta;
	private float releaseTheta;
	private bool swingStarted = false;
	private bool dead = false;


	void Awake() {
		rb = GetComponent<Rigidbody2D> ();
		gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager> ();
		sr = body.GetComponent<SpriteRenderer> ();
		shield = GetComponentInChildren<Shield> (true);
		base.OnAwake ();
	}

	void Start () {
		if (!colorSet) {
			color = Color.magenta;
			colorSet = true;
		}
		kanga = sr.sprite;
		base.OnStart ();
	}

	void Update () {
		if (aimingHarp) {
			AimHarp ();
		} else if (swingingHarp) {
			SwingHarp ();
		}

		if (rolling) {
			AileronRoll ();
		}
		base.OnUpdate ();
	}

	void FixedUpdate () {
		if (!docked) {
			HandleFlying ();
		}

		//check if just boosted
		if (!canBoost) {
			forceMag = hydratedForceMag;
		}	
		base.OnFixedUpdate ();
	}

	void OnTriggerEnter2D (Collider2D other) {
		base.TriggerEnter2D (other);
	}

	void OnBecameVisible () {
		base.BecameVisible ();
	}

	void OnBecameInvisible () {
		base.BecameInvisible ();
	}

	public override void FireBullet() {
		if (target == null) {
			SetNearestTarget ();
			if (target == null) {
				CancelInvoke ();
				stopFiring = true;
				return;
			}
		}
		Bullet bullet = gm.GetComponent<ObjectPooler> ().GetPooledObject ().GetComponent<Bullet> ();
		bullet.gameObject.SetActive (true);
		Vector2 skewVector = new Vector2 (Random.Range (-skew, skew), Random.Range (-skew, skew));
		Vector2 aim = target.position - transform.position;
		bullet.Fire (transform.position, aim + skewVector);	
	}

	public override void Die ()
	{
		base.Die ();
	}


	void FixedUpdate () {
		
	}


	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "PlayerBullet") {
			if (rolling) {
				BounceBullets (other);
			} else if (!invincible) {
				if (shield.gameObject.activeSelf) {
					shield.DeactivateShield ();
					StartCoroutine (HitInvincibility (hitInvincibilityDuration));
					other.GetComponent<Bullet> ().Die ();
				} else { 
					TakeDamage (other.GetComponent<Bullet> ().damage);
					other.GetComponent<Bullet> ().Die ();
				}
			}
		} else if (other.tag == "Enemy") {
			if (!invincible) {
				if (shield.gameObject.activeSelf) {
					shield.DeactivateShield ();
					StartCoroutine (HitInvincibility (hitInvincibilityDuration));
					other.GetComponent<Flyer> ().Die ();
				} else { 
					TakeDamage (other.GetComponent<Flyer> ().kamikazeDamage);
					other.GetComponent<Flyer> ().Die ();
				}
			}
		}
	}

	void OnCollisionEnter2D (Collision2D coll) {		
		if (coll.collider.gameObject.name == "DockableColliders") {
			DockOnHeavyHippo ();
		}
	}

	void OnCollisionExit2D (Collision2D coll) {

	}

	void HandleFlying () {
		if (direction == Vector2.zero) {
			if (harp != null) {
				if (harp.taut) {
					HandleHarpTaut ();
					return;
				}
			}
		}

		if (otherHarps.Count > 0) {
			HandleFlyingWhileHarpooned ();
		}
		else if (harp != null) {
			//Has launched harp, check if max tethered 
			if (!harp.taut) {
				//Harp not at max tether, bird not harpooned by anything, move normally
				rb.AddForce (direction * forceMag);
			} 
			//Has launched harpoon and it is at max tether
			else {
				HandleHarpTaut ();
			}
		} 
		//Hasn't launched harp and not harpooned by anything, move normally
		else {
			rb.AddForce (direction * forceMag);
		}
	}

	void HandleFlyingWhileHarpooned () {
		//Make a list of masses dragging on Bird
		List<Transform> draggers = new List<Transform>();
		foreach (Harpoon h in otherHarps) {
			if (h.taut) {
				draggers.Add (h.GetHarpooner ().transform);
			}
		}

		//Check for harp at max tether and if it is attached to anything
		if (harp != null) {
			if (harp.taut) {
				if (harp.GetHarpooned () != null) {
					draggers.Add (harp.GetHarpooned ().transform);
				} else {
					draggers.Add (harp.transform);
				} 
			}
		}

		//WhaleDynamics is for calculating effective drag masses and doing the physics
		if (draggers.Count > 0) {
			WhaleDynamics wd = new WhaleDynamics (transform, draggers, forceMag, direction);
			wd.CalculateWhaleDragging ();
		} 
		//If harp and harpooners aren't at max tether, then fly normally
		else {
			rb.AddForce (direction * forceMag);
		}
	}

	void HandleHarpTaut () {
		//Calculate and add force parallel to pull vector
		pullDir = transform.position - harp.transform.position;
		pullDir.Normalize ();
		float pullMagnitude = Vector3.Dot (direction * forceMag, pullDir);
		if (pullMagnitude > 0) {
			PullHarp (pullMagnitude);
		}
		else {
			//Check if harp is moving away from bird
			if (harp.GetComponent<Rigidbody2D> ().velocity != Vector2.zero) {
				//TetherHarp ();
			}
			//Ship is not pulling on harp, so it moves normally
			rb.AddForce (direction * forceMag);
		} 
	}

	void PullHarp (float pullMag) {
		Rigidbody2D rbOther;
		float totalMass = rb.mass;
		if (harp.GetHarpooned () != null) {
			rbOther = harp.GetHarpooned ().GetComponent<Harpoonable> ().RiBo ();
			totalMass += rbOther.mass;
		} else {
			rbOther = harp.GetComponent<Rigidbody2D> ();
			totalMass += rbOther.mass;
		}

		//Caclulate and add force orthogonal to pull vector
		orthoPullDir = new Vector3 (-pullDir.y, pullDir.x, 0);
		float orthoPullMag = Vector3.Dot (direction * forceMag * rb.mass, orthoPullDir);		

		rb.AddForce (pullDir * pullMag / totalMass);
		rbOther.AddForce (pullDir * pullMag / totalMass);
		rb.AddForce (orthoPullDir * orthoPullMag);
	}

	/// <summary>
	/// flag gap increases linearly
	/// flash duration is constant
	/// </summary>
	/// <param name="dam">Dam.</param> testing this
	public void TakeDamage (int dam) {
		if (damaged) {
			//#TODO explodeee
			Die ();
		} else {
			health = 0;
			damaged = true;
			StartCoroutine (HitInvincibility (hitInvincibilityDuration));
			StartCoroutine (Flash (Time.time));
		}
	}

	public void TakeOneShotKill () {
		Die ();
	}

	void Die () {
		if (dead) {
			return;
		}
		dead = true;

		gm.RemoveAlliedTransform (transform);
		gm.birds.Remove (this);
		DetachOtherHarps ();
		if (harp) {
			harp.Die ();
		}

		if (rider) {
			rider.Bubble (rb.velocity);
		}

		rb.drag = .5f;
		rb.constraints = RigidbodyConstraints2D.None;
		rb.AddTorque (Random.Range (-deathTorque, deathTorque));
		sr.color = new Color (color.r, color.g, color.b, .25f);
		sr.sortingLayerName = "Buildings";
		CancelInvoke ();
		gameObject.layer = LayerMask.NameToLayer ("Dead");
		this.enabled = false;
	}

	public bool DockOnHeavyHippo (Dock d) {
		
		dock = d;

		if (!dock) {
			docked = false;
			return docked;
		}

		if (rolling) {
			transform.rotation = Quaternion.identity;
			rolling = false;
			canRoll = true;
		}

		if (harp) {
			harp.SetGripping (false);
			harp.SetRecalling (true);
			Destroy (harp.gameObject);
			hurledHarp = false;
		}

		DetachOtherHarps ();
		transform.position = dock.transform.position;
		transform.rotation = dock.transform.rotation;
		transform.parent = dock.transform;
		DisableColliders ();
		sr.color = color;
		sr.sortingLayerName = "BigBird";
		sr.sortingOrder = 1;
		damaged = false;
		invincible = false;
		CancelInvoke ();
		rb.Sleep ();
		dock.bird = this;
		dock.gameObject.layer = LayerMask.NameToLayer ("Stations");
		docked = true;

		if (rider) {
			rider.Dock (dock);
		}
			
		return docked;
	}

	public void Undock () {
		if (health < maxHealth) {
			Debug.Log ("Can't undock unhealthy bird.");
			return;
		}

		Vector3 releaseDirection = transform.position - dock.transform.position;
		releaseDirection.Normalize ();
		RaycastHit2D hit = Physics2D.Raycast (dock.transform.position, releaseDirection, dockRayLength, releaseMask);
		if (hit.transform != null) {
			print ("undock blocked by " + hit.transform.name);
			return;
		}

		dock.GetComponent<BoxCollider2D> ().enabled = true;
		docked = false;
		dock.bird = null;
		dock = null;

		transform.parent = null;
		transform.rotation = Quaternion.identity;
		rb.WakeUp ();

		rb.AddForce (releaseDirection * boostForceMag);
		Invoke ("EnableColliders", undockColliderDelay);
		sr.sortingLayerName = "Birds";
		sr.sortingOrder = 0;
		rider.Undock ();
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

	IEnumerator Flash (float timeAtDamage) {
		if (Time.time - timeAtDamage > damageDuration) {
			Die ();
			yield break;
		}
		Color startColor = sr.color;
		sr.color = new Color (1, 0, 0, Mathf.Max(startColor.a, .5f));
		yield return new WaitForSeconds (flashDuration);
		if (docked || dead) {
			yield break;
		}
		sr.color = startColor;
		yield return new WaitForSeconds (flashGap);
		if (docked || dead) {
			yield break;
		}
		StartCoroutine (Flash (timeAtDamage));
	}

	IEnumerator FlashDamage (float timeAtDamage) {
		Color startColor = sr.color;
		sr.color = new Color (1, 0, 0, Mathf.Max(startColor.a, .5f));
		yield return new WaitForSeconds (flashDuration);
		if (docked || dead) {
			yield break;
		}
		sr.color = startColor;

		if (damaged) {
			float flashGapModifier = flashDampener * (Time.time - timeAtDamage);
			if (flashGapModifier > 1) {
				//explode
				print ("death time: " + Time.time);
				Die ();
				yield break;
			}
			yield return new WaitForSeconds (Mathf.Max (0f, flashGap - flashGapModifier));
			if (docked || dead) {
				yield break;
			}
			StartCoroutine (FlashDamage (timeAtDamage));
		}
	}

	IEnumerator HitInvincibility (float duration) {	
		invincible = true;
		Color startColor = sr.color;
		sr.color = new Color (startColor.r, startColor.g, startColor.b, .1f);
		rider.GetComponent<SpriteRenderer> ().color = sr.color;
		yield return new WaitForSeconds (duration);
		if (rider) { 
			rider.GetComponent<SpriteRenderer> ().color = Color.white;
		} 

		sr.color = startColor;
		invincible = false;
	}

	public IEnumerator Boost () {
		canBoost = false;
		forceMag = boostForceMag;
		yield return new WaitForSeconds (boostCooldown);
		canBoost = true;
	}

	public void AileronRoll () {
		DetachOtherHarps ();
		body.Rotate (rollSpeed * Time.deltaTime, 0, 0);
	}

	public IEnumerator EndRoll () {
		yield return new WaitForSeconds (rollDuration);
		//if about to burn up (die) this invoke can fail
		if (!docked) {
			body.transform.rotation = Quaternion.identity;
			rolling = false;
			yield return new WaitForSeconds (rerollDelay);
			canRoll = true;
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


	public Shield Shield {
		get {
			return shield;
		}
	}

	public ReloadIndicator ReloadIndicator {
		get {
			return reloadIndicator;
		}
	}

	public void TurnOnReloadIndicator (float reloadTime) {
		reloadIndicator.gameObject.SetActive (true);
		reloadIndicator.StartReload (reloadTime);
	}

	public void TurnOffReloadIndicator () {
		reloadIndicator.gameObject.SetActive (false);
	}

	public void DetachOtherHarps () {
		if (otherHarps.Count > 0) {
			List<Harpoon> harpsToDetach = new List<Harpoon> ();
			foreach (Harpoon h in otherHarps) {
				harpsToDetach.Add (h);
			}

			foreach (Harpoon h in harpsToDetach) {
				h.SetGripping (false);
				h.SetRecalling (true);
			}
		}
	}


	public void HurlHarpoon () {
		if (!hurledHarp) {
			//p.aim.Normalize ();
			harp.transform.parent = null;

			////Offset harp so it doesn't immediately collide with ship
			releaseHarpPosition = rider.transform.position + rider.aim * harpHurlOffset;

			//simple launch (not swinging or winding up)
			harp.transform.position = releaseHarpPosition;
			harp.Fire (harp.transform, rider.aim);
			harp.GetComponent<BoxCollider2D> ().enabled = true;

			hurledHarp = true;
			aimingHarp = false;
			harpLoaded = false;
		}

	}

	public void SwingHurlHarpoon () {
		if (!hurledHarp && !swingingHarp) {
			//p.aim.Normalize ();
			harp.transform.parent = null;

			////Offset harp so it doesn't immediately collide with ship
			releaseHarpPosition = rider.transform.position + rider.aim * harpHurlOffset;

			//simple launch (not swinging or winding up)
			swingingHarp = true;
			harp.transform.position = releaseHarpPosition;
			//harp.Fire (harp.transform, hurlAim);
			//harp.GetComponent<BoxCollider2D> ().enabled = true;

			hurledHarp = true;
			aimingHarp = false;
			harpLoaded = false;

			//windingUp = true;
			//releaseHarpPosition = harp.transform.position;
			//throwingHarp = true;
		}
	}

	public void LoadHarp () {
		rider.w.firing = false;
		rider.CancelInvoke ();
		rider.GetComponent<PlayerInput> ().CancelInvoke ();


		GameObject harpoon = Instantiate (harpoonPrefab, rider.transform.position + rider.aim, transform.rotation) as GameObject;
		harpoon.name = "Harpoon";
		harp = harpoon.GetComponent<Harpoon> ();
		harp.SetHarpooner (gameObject);
		harp.GetComponent<BoxCollider2D> ().enabled = false;
		harp.GetComponent<Rigidbody2D> ().Sleep ();
		harp.transform.parent = transform;

		aimingHarp = true;
		harpLoaded = true;
	}

	public void UnloadHarp () {
		hurledHarp = false;
		catchingHarp = true;
		aimingHarp = false;
		harpLoaded = false;
		Destroy (harp.gameObject);
	}

	public void AimHarp () {
		harp.transform.position = rider.transform.position + rider.aim;
	}

	public void SwingHarp () {
		if (!swingStarted) {
			swingStarted = true;
			releaseTheta = Mathf.Atan2 (releaseHarpPosition.y - rider.transform.position.y, releaseHarpPosition.x - rider.transform.position.x);
			theta = releaseTheta + 2 * Mathf.PI;
			float aimTheta = releaseTheta + aimThetaModifier;
			hurlAim.x = Mathf.Cos (aimTheta);
			hurlAim.y = Mathf.Sin (aimTheta);
			releaseTheta += earlyReleaseAngleRad;
		} else if (theta < releaseTheta) {
			swingStarted = false;
			swingingHarp = false;
			harp.Fire (harp.transform, hurlAim);
			harp.GetComponent<BoxCollider2D> ().enabled = true;
		} else {
			theta = theta - swingSpeed * Time.deltaTime;
			float x = rider.transform.position.x + Mathf.Cos (theta) * harpHurlOffset;
			float y = rider.transform.position.y + Mathf.Sin (theta) * harpHurlOffset;
			harp.transform.position = new Vector3 (x, y, 0);
		}
	}

	void EnableHarpCollider () {
		harp.GetComponent<BoxCollider2D> ().enabled = true;
		throwingHarp = false;
	}

	public float GetEffectiveMass (Vector3 whalePosition, Vector3 pullingDirection) {
		float totalMass = rb.mass;
		foreach (Harpoon h in otherHarps) {
			Vector3 thisBirdPullDir = transform.position - h.GetHarpooner ().transform.position;
			thisBirdPullDir.Normalize ();
			float m = h.GetHarpooner ().GetComponent<Bird> ().GetEffectiveMass (transform.position, thisBirdPullDir);
			Vector3 v = whalePosition - transform.position;
			float theta = Vector3.Angle (pullingDirection, v);
			if (theta < 90f) {
				theta = theta * Mathf.Deg2Rad;
				totalMass += m * Mathf.Cos (theta);
			}
		}
		return totalMass;
	}


	public void BeenHarpooned (Harpoon h) {
	}
		

	public void HarpoonReleased (Harpoon h) {
		RemoveHarp (h);
	}
		
	public bool GetDead () {
		return dead;
	}

	public void Heal () {
		if (rider) {
			sr.color = rider.color;
		} else {
			sr.color = Color.black;
		}
		damaged = false;
		health = maxHealth;
		CancelInvoke ("Flash");
	}
}
