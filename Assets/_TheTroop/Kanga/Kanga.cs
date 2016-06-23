using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Kanga : Flyer {
	public float skew = 2f;
	public Neoner rider;
	public int health;
	public int maxHealth;
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

	public bool damaged = false, invincible = false, canBoost = true, hurledHarp = false, canRoll = true, rolling = false;
	[HideInInspector]
	public Vector2 badInputDirection = Vector2.zero, pullDir = Vector2.zero, orthoPullDir = Vector2.zero;

	public Harpoon harp;


	public enum Pattern {NEUTRAL, ZIGZAGGING, RETREATING, HUNTING, CIRCLING}
	public Pattern pat;
	public bool circling = false;
	public bool playing = false;
	public float minPatternTime = 2f;
	public float maxPatternTime = 8f;
	public float circleTheta;
	public float radsPerSec = Mathf.PI;
	public float radius;
	public float minRadius = 10f;
	public float maxRadius = 20f;
	public float retreatSkew = 5f;
	public Vector2 kangaDir;

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
		sr = body.GetComponent<SpriteRenderer> ();
		shield = GetComponentInChildren<Shield> (true);
		reloadIndicator = GetComponentInChildren<ReloadIndicator> (true);
		base.OnAwake ();
	}

	void Start () {
		if (!colorSet) {
			color = Random.ColorHSV();
			Shield.SetColor (color);
			colorSet = true;
		}
		kanga = sr.sprite;
		startAttacking = true;
		InvokeRepeating ("SetNearestTarget", 0f, .5f);
		Invoke ("RandomAileron", 0f);
		Invoke ("RandomBoost", 0f);
		InitiateHunting ();
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
		//Chase ();
		HandlePattern ();
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
					TakeDamage (other.GetComponent<Bullet> ().damage, Color.red);
					other.GetComponent<Bullet> ().Die ();
				}
			} else if (other.tag == "Enemy") {
				if (!invincible) {
					if (shield.gameObject.activeSelf) {
						shield.DeactivateShield ();
						StartCoroutine (HitInvincibility (hitInvincibilityDuration));
						other.GetComponent<Flyer> ().Die ();
					} else { 
						TakeDamage (other.GetComponent<Flyer> ().kamikazeDamage, Color.red);
						other.GetComponent<Flyer> ().Die ();
					}
				}
				base.TriggerEnter2D (other);
			}
		}
	}

	void OnBecameVisible () {
		base.BecameVisible ();
	}

	void OnBecameInvisible () {
		base.BecameInvisible ();
	}

		
	void InitiateCircling () {
		pat = Pattern.CIRCLING;
		radius = maxRadius;
		startAttacking = Random.Range (0f, 1f) > .5 ? true : false;
		Invoke ("ExitCircling", 5f);
	}

	public void HandleCircling () {
		if (!target) {
			print ("lost target");
			return;
		}
		circleTheta += radsPerSec * Time.deltaTime;
		float x = radius * Mathf.Cos (circleTheta);
		float y = radius * Mathf.Sin (circleTheta);
		Vector3 perimeterPoint = new Vector3 (target.transform.position.x + x, target.transform.position.y + y, 0f);
		Vector3 toPerimeterPoint = perimeterPoint - transform.position;
		toPerimeterPoint.Normalize ();
		rider.reticle.transform.position = perimeterPoint;
		transform.position = perimeterPoint;
		//rb.AddForce (toPerimeterPoint * forceMag);	
	}

	void ExitCircling () {
		stopFiring = true;
		ChangePattern ();
	}


	void InitiateRetreating () {
		pat = Pattern.RETREATING;
		InvokeRepeating ("SkewRetreating", 0f, .5f);
		Invoke ("ExitRetreating", .5f);
	}

	public void HandleRetreating () {
		rb.AddForce (kangaDir * forceMag);	
	}

	void SkewRetreating () {
		if (!target) {
			return;
		}
		kangaDir = transform.position - target.transform.position;
		float xSkew = Random.Range (-retreatSkew, retreatSkew);
		float ySkew = Random.Range (-retreatSkew, retreatSkew);
		kangaDir.x += xSkew;
		kangaDir.y += ySkew;
		kangaDir.Normalize ();
	}

	void ExitRetreating () {
		CancelInvoke ("SkewRetreating");
		ChangePattern ();
	}

	void InitiateZigZagging () {
		pat = Pattern.ZIGZAGGING;
		startAttacking = Random.Range (0f, 1f) > .5 ? true : false;
		InvokeRepeating ("ZigZag", 0f, 1f);
		Invoke ("ExitZigZagging", 5f);
	}

	public void HandleZigZagging () {
		rb.AddForce (kangaDir * forceMag);	
	}

	void ZigZag () {
		float xSkew = Random.Range (-retreatSkew, retreatSkew);
		float ySkew = Random.Range (-retreatSkew, retreatSkew);
		kangaDir.x = xSkew;
		kangaDir.y = ySkew;
		kangaDir.Normalize ();
	}

	void ExitZigZagging () {
		stopFiring = true;
		CancelInvoke ("ZigZag");
		ChangePattern ();
	}

	void InitiateHunting () {
		pat = Pattern.HUNTING;
		InvokeRepeating ("Hunt", 0f, .25f);
		Invoke ("ExitHunting", 10f);
	}

	public void HandleHunting () {
		if (!target) {
			SetNearestTarget ();
		}
		if (Vector3.Distance (transform.position, target.position) > attackRange) {  
			kangaDir.Normalize ();
			rb.AddForce (kangaDir * forceMag);
		} else {
			ExitHunting ();
		}
	}

	void Hunt () {
		if (!target) {
			return;
		}
		kangaDir = target.position - transform.position;
		kangaDir.Normalize ();
	}

	void ExitHunting () {
		CancelInvoke ("Hunt");
		InitiateNeutral ();
	}

	void InitiateNeutral () {
		pat = Pattern.NEUTRAL;
		kangaDir = Vector3.zero;
		startAttacking = true;
		Invoke ("ExitNeutral", 2f);
	}

	public void HandleNeutral () {
		rb.AddForce (kangaDir * forceMag);	
	}

	void ExitNeutral () {
		stopFiring = Random.Range (0f, 1f) > .5 ? true : false;
		ChangePattern ();
	}

	void HandlePattern () {
		switch (pat) {
		case Pattern.NEUTRAL:
			HandleNeutral ();
			break;
		case Pattern.RETREATING:
			HandleRetreating ();
			break;
		case Pattern.ZIGZAGGING:
			HandleZigZagging ();
			break;
		case Pattern.HUNTING:
			HandleHunting ();
			break;
		case Pattern.CIRCLING:
			HandleCircling ();
			break;
		default:
			break;
		}
	
	}

	void ChangePattern () {
		int i = Random.Range (0, (int)Pattern.CIRCLING + 1);
		switch (i) {
		case (int)Pattern.NEUTRAL:
			InitiateNeutral ();
			break;
		case (int)Pattern.RETREATING:
			InitiateRetreating ();
			break;
		case (int)Pattern.ZIGZAGGING:
			InitiateZigZagging ();
			break;
		case (int)Pattern.HUNTING:
			InitiateHunting ();
			break;
		case (int)Pattern.CIRCLING:
			InitiateCircling ();
			break;
		default:
			break;
		}
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

	void OnCollisionEnter2D (Collision2D coll) {		
		if (coll.collider.gameObject.name == "HippoDock") {
			DockOnHeavyHippo (coll.transform.GetComponent<Dock> ());
		}
	}

	void OnCollisionExit2D (Collision2D coll) {

	}

	void HandleFlying () {
		if (kangaDir == Vector2.zero) {
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
				rb.AddForce (kangaDir * forceMag);
			} 
			//Has launched harpoon and it is at max tether
			else {
				HandleHarpTaut ();
			}
		} 
		//Hasn't launched harp and not harpooned by anything, move normally
		else {
			rb.AddForce (kangaDir * forceMag);
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
			WhaleDynamics wd = new WhaleDynamics (transform, draggers, forceMag, kangaDir);
			wd.CalculateWhaleDragging ();
		} 
		//If harp and harpooners aren't at max tether, then fly normally
		else {
			rb.AddForce (kangaDir * forceMag);
		}
	}

	void HandleHarpTaut () {
		//Calculate and add force parallel to pull vector
		pullDir = transform.position - harp.transform.position;
		pullDir.Normalize ();
		float pullMagnitude = Vector3.Dot (kangaDir * forceMag, pullDir);
		if (pullMagnitude > 0) {
			PullHarp (pullMagnitude);
		}
		else {
			//Check if harp is moving away from bird
			if (harp.GetComponent<Rigidbody2D> ().velocity != Vector2.zero) {
				//TetherHarp ();
			}
			//Ship is not pulling on harp, so it moves normally
			rb.AddForce (kangaDir * forceMag);
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
		float orthoPullMag = Vector3.Dot (kangaDir * forceMag * rb.mass, orthoPullDir);		

		rb.AddForce (pullDir * pullMag / totalMass);
		rbOther.AddForce (pullDir * pullMag / totalMass);
		rb.AddForce (orthoPullDir * orthoPullMag);
	}

	/// <summary>
	/// flag gap increases linearly
	/// flash duration is constant
	/// </summary>
	/// <param name="dam">Dam.</param> testing this
	public override void TakeDamage (int dam, Color c) {
		if (shield.gameObject.activeSelf) {
			shield.DeactivateShield ();
			StartCoroutine (HitInvincibility (hitInvincibilityDuration));
		} else {
			base.TakeDamage (dam, Color.red);
		}
	}

	public void TakeOneShotKill () {
		Die ();
	}

	public override void Die () {
		if (dead) {
			return;
		}
		dead = true;

		DetachOtherHarps ();
		if (harp) {
			harp.Die ();
		}

		base.Die ();
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
		dock.kanga = this;
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
			Invoke ("RandomAileron", Random.Range (0f, 8f));
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

	void RandomAileron () {
		rolling = true;
		canRoll = false;
		StartCoroutine (EndRoll ());
	}

	void RandomBoost () {
		StartCoroutine (Boost ());
		Invoke ("RandomBoost", Random.Range (1f, 12f));
	}
}
