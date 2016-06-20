﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class Bird : MonoBehaviour {

	public LayerMask releaseMask;
	public Player p;
	public int health = 100;
	public int maxHealth = 100;
	public float forceMag = 60f;
	public float hydratedForceMag = 100f;
	public float dehydratedForceMag = 60f;

	public float boostForceMag = 60f;
	public float flashGap = 1f;
	public float flashDuration = .2f;
	public float damageDuration = 10f;
	public float flashDampener = .1f;
	public float hitInvincibilityDuration = .25f;
	public float boostCooldown = 2f;
	public float dockRayLength = 8f;
	public float undockColliderDelay = .2f;
	

	public float fullTank = 60f;
	public float lowGasWarning = 10f;
	public float gas = 60f;
	public float gasPerSecond = 1f;
	public float gasPerBoost = 2f;
	public float gasLightDelay = .2f;
	public float iconAboveOffset = 5f;
	public float harpHurlOffset = 1f;
	public float swingSpeed = 3f;
	public float windUpTime = 1f;
	public float aimThetaModifier = 1f;
	public float earlyReleaseAngleRad = Mathf.PI / 4;
	public float rollDuration = 1f;
	public float rerollDelay = 1f;
	public float rollSpeed = 1f;
	public float loveLasts = 2f;

	public bool docked = false;
	public bool webbed = false;
	public bool powered = false;
	public bool harpLoaded = false;
	public bool aimingHarp = false;
	public bool swingingHarp = false;
	public bool windingUp = false;
	public bool throwingHarp = false;
	public bool catchingHarp = false;
	public bool pregnant = false;
	public bool inHeat = false;
	public bool hasBattery = false;
	public bool ranOutOfGas = false;
	public bool colorSet = false;

	public Transform body;
	public GameObject harpoonPrefab;
	public GameObject gasLightPrefab;
	public GameObject eggPrefab;
	public GameObject lovePrefab;
	public Sprite powerbird;
	public Sprite greyhound;
	public OrbWeb webTrap;
	public Powerbird power;
	public Hatchling pup;
	public Circuit circuit;
	public List<Bird> linkedBirds = new List<Bird> ();
	public List<Harpoon> otherHarps = new List<Harpoon> ();

	[HideInInspector]
	public Color color;
	[HideInInspector]
	public bool damaged = false, invincible = false, canBoost = true, hurledHarp = false, canRoll = true, rolling = false;
	[HideInInspector]
	public Vector2 direction = Vector2.zero, pullDir = Vector2.zero, orthoPullDir = Vector2.zero;
	[HideInInspector]
	public Harpoon harp;

	private Rigidbody2D rb;
	private GameManager gm;
	private SpriteRenderer sr;
	private BigBird bigBird;
	private Dock dock = null;
	private Shield shield;
	private ReloadIndicator reloadIndicator;
	private GameObject gasLight;
	private Harpoon loadedHarp;
	private Vector3 releaseHarpPosition;
	private Vector3 windUpOffset;
	private Vector3 hurlAim;
	private float swingStartTime;
	private float windUpStartTime;
	private float theta;
	private float releaseTheta;
	private bool gasLightFlashing = false;
	private bool swingStarted = false;
	private bool windUpStarted = false;


	void Awake() {
		rb = GetComponent<Rigidbody2D> ();
		gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager> ();
		sr = body.GetComponent<SpriteRenderer> ();
		bigBird = GameObject.FindObjectOfType<BigBird>() as BigBird;
		shield = GetComponentInChildren<Shield> (true);
		reloadIndicator = GetComponentInChildren<ReloadIndicator> (true);
	}

	void Start () {
		if (!colorSet) {
			color = sr.color;
			colorSet = true;
		}



		DockOnBigBird ();
		LayEgg ();

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
						forceMag = dehydratedForceMag;
					}
				}
			}

			if (aimingHarp) {
				AimHarp ();
			} else if (swingingHarp) {
				SwingHarp ();
			} else if (windingUp) {
				WindUp ();
			}

			if (rolling) {
				AileronRoll ();
			}
		}
		if (!docked) {
			transform.position = gm.ClampToScreen (transform.position);
		}
	}

	void FixedUpdate () {
		if (!docked && !webbed && !powered) {
			HandleFlying ();
		}

		//check if just boosted
		if (!canBoost) {
			forceMag = hydratedForceMag;
		}	
	}


	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "EnemyBullet") {
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
		} else if (other.tag == "Harpoonable") {
			/*Item otherC = other.GetComponent<Item> ();
			if (otherC) {
				if (otherC.itemType == Item.ItemType.BIRD_SHIELD) {
					if (!shield.gameObject.activeSelf) {
						shield.ActivateShield ();
						Destroy (other.gameObject);
					}
				}
			}*/
		}
	}

	void OnCollisionEnter2D (Collision2D coll) {
		if (coll.gameObject.name == "BigBird") {
			DockOnBigBird ();
		} else if (coll.transform.tag == "Enemy") {
			//TODO jousting
			//print (rb.velocity.sqrMagnitude);
			if (!invincible) {
				if (shield.gameObject.activeSelf) {
					//shield.DeactivateShield ();
					//StartCoroutine (HitInvincibility (hitInvincibilityDuration));
					//coll.gameObject.GetComponent<Flyer> ().Die ();
				} else { 
					//TakeDamage (coll.transform.GetComponent<Flyer> ().kamikazeDamage);
					//coll.gameObject.GetComponent<Flyer> ().Die ();
				}
			}
		} else if (coll.transform.tag == "Bird") {
			if (inHeat) {
				pregnant = true;
				MakeLove ();
				inHeat = false;
			}
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
		if (pup) {
			pup.Die ();
			pup = null;
		}
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
		gm.RemoveAlliedTransform (transform);
		DetachOtherHarps ();
		if (harp) {
			harp.Die ();
		}
		if (p) {
			p.Bubble (rb.velocity);
		}
		Destroy (gameObject);
	}
			
	public bool DockOnBigBird () {
		dock = bigBird.GetNearestOpenDock (transform.position);

		if (!dock) {
			docked = false;
			return docked;
		}

		if (dock.item) {
			Hatchling h = dock.item.GetComponent<Hatchling> ();
			if (h && pup) {
				docked = false;
				return docked;
			}
		}

		if (pup) {
			pup.transform.position = dock.transform.position;
			pup.transform.rotation = dock.transform.rotation;
			pup.transform.parent = dock.transform;
			pup.flying = false;
			dock.item = pup.transform;
			pup = null;
		}

		if (dock.item) {
			if (p) {
				p.itemTouching = dock.item;
			}
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
		gasLightFlashing = false;
		ranOutOfGas = false;
		Destroy (gasLight);
		CancelInvoke ();
		rb.Sleep ();

		if (p) {
			p.DockOnBigBird (dock);
		}
		bigBird.AddToDockedBirds (this);
		dock.bird = this;
		dock.gameObject.layer = LayerMask.NameToLayer ("Stations");
		docked = true;
		return docked;
	}

	public void UndockFromBigBird () {
		if (health < maxHealth) {
			Debug.Log ("Can't undock unhealthy bird.");
			return;
		}

		Vector3 releaseDirection = transform.position - bigBird.transform.position;
		releaseDirection.Normalize ();
		RaycastHit2D hit = Physics2D.Raycast (dock.transform.position, releaseDirection, dockRayLength, releaseMask);
		if (hit.transform != null) {
			print ("undock blocked by " + hit.transform.name);
			return;
		}

		if (dock.transform.parent == bigBird.transform) {
			bigBird.RemoveFromDockedBirds (this);
			if (pregnant) {
				LayEgg ();
			} else {
				dock.gameObject.layer = LayerMask.NameToLayer ("Default");
			}

			if (dock.item) {
				Hatchling h = dock.item.GetComponent<Hatchling> ();
				if (h && !pup) {
					pup = h;
					pup.UndockFromBigBird (this);
					dock.item = null;
				}
			}
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
		p.Undock ();
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
		if (docked) {
			yield break;
		}
		sr.color = startColor;
		yield return new WaitForSeconds (flashGap);
		if (docked) {
			yield break;
		}
		StartCoroutine (Flash (timeAtDamage));
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
				print ("death time: " + Time.time);
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

	IEnumerator HitInvincibility (float duration) {	
		invincible = true;
		Color startColor = sr.color;
		sr.color = new Color (startColor.r, startColor.g, startColor.b, .1f);
		p.GetComponent<SpriteRenderer> ().color = sr.color;
		yield return new WaitForSeconds (duration);
		if (p) { 
			p.GetComponent<SpriteRenderer> ().color = p.color;
		} 

		sr.color = startColor;
		invincible = false;
	}

	public IEnumerator Boost () {
		if (ranOutOfGas || direction == Vector2.zero) {
			yield break;
		}
		canBoost = false;
		gas -= gasPerBoost;
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

	void LowOnGas () {
		Vector3 position = new Vector3 (transform.position.x, transform.position.y + iconAboveOffset, 0f);
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
			p.aim.Normalize ();
			harp.transform.parent = null;

			////Offset harp so it doesn't immediately collide with ship
			releaseHarpPosition = p.transform.position + p.aim * harpHurlOffset;

			//simple launch (not swinging or winding up)
			harp.transform.position = releaseHarpPosition;
			harp.Fire (harp.transform, p.aim);
			harp.GetComponent<BoxCollider2D> ().enabled = true;

			hurledHarp = true;
			aimingHarp = false;
			harpLoaded = false;
		}
	
	}

	public void SwingHurlHarpoon () {
		if (!hurledHarp && !swingingHarp) {
			p.aim.Normalize ();
			harp.transform.parent = null;

			////Offset harp so it doesn't immediately collide with ship
			releaseHarpPosition = p.transform.position + p.aim * harpHurlOffset;

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
		p.w.firing = false;
		p.CancelInvoke ();
		p.GetComponent<PlayerInput> ().CancelInvoke ();


		GameObject harpoon = Instantiate (harpoonPrefab, p.transform.position + p.aim, transform.rotation) as GameObject;
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
		harp.transform.position = p.transform.position + p.aim;
	}

	public void SwingHarp () {
		if (!swingStarted) {
			swingStarted = true;
			releaseTheta = Mathf.Atan2 (releaseHarpPosition.y - p.transform.position.y, releaseHarpPosition.x - p.transform.position.x);
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
			float x = p.transform.position.x + Mathf.Cos (theta) * harpHurlOffset;
			float y = p.transform.position.y + Mathf.Sin (theta) * harpHurlOffset;
			harp.transform.position = new Vector3 (x, y, 0);
		}
	}

	public void WindUp () {
		if (!windUpStarted) {
			windUpStarted = true;
			windUpStartTime = Time.time;
			hurlAim = p.aim;
		} else if ((Time.time - windUpStartTime) > windUpTime) {
			harp.Fire (harp.transform, hurlAim);
			windingUp = false;
			windUpStarted = false;
			Invoke ("EnableHarpCollider", .5f);
		} else {
			harp.transform.position = p.transform.position + Vector3.Lerp (hurlAim, -hurlAim * 2, (Time.time - windUpStartTime) / windUpTime);
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

	public void LayEgg () {
		if (!dock.item) {
			GameObject eggObj = Instantiate (eggPrefab, transform.position, Quaternion.identity) as GameObject;
			eggObj.transform.parent = dock.transform;
			dock.item = eggObj.GetComponent<Egg> ().transform;
			pregnant = false;
		}
	}

	public void EatGreens () {
		inHeat = true;
	}

	void MakeLove () {
		Vector3 spawnPosition = new Vector3 (transform.position.x, transform.position.y + iconAboveOffset, 0f);
		GameObject love = Instantiate (lovePrefab, spawnPosition, Quaternion.identity) as GameObject;
		Destroy (love, loveLasts);
	}

	public void RandomColor () {
		sr.color = Random.ColorHSV ();
	}

	public void Webbed (OrbWeb ow) {
		ow.captive = transform;
		webTrap = ow;
		webbed = true;
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
		transform.position = ow.transform.position;
		transform.parent = ow.transform;
		rb.Sleep ();
		//DisableColliders ();

		if (p) {
			p.Webbed (ow);
		}
	}

	public void Unwebbed () {
		webbed = false;
		webTrap = null;
		transform.parent = null;
		//EnableColliders ();
		if (p) {
			p.Unwebbed ();
		}
	}

	public void Powered (Powerbird pb) {
		power = pb;
		powered = true;
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
		GetComponent<Rigidbody2D> ().mass = .5f;
		GetComponent<Rigidbody2D> ().drag = 0f;
		DetachOtherHarps ();
		transform.parent = pb.transform;
		rb.Sleep ();

		if (p) {
			p.Powered (pb);
		}
		
	}

	public void Unpowered () { 
		powered = false;
		power = null;
		transform.parent = null;
		GetComponent<Rigidbody2D> ().mass = 1f;
		GetComponent<Rigidbody2D> ().drag = 5f;
		//EnableColliders ();
		if (p) {
			p.Unpowered ();
		}
	}

	void SetRigidbodiesKinematic (bool set) {
		Rigidbody2D[] rigs = GetComponentsInChildren<Rigidbody2D> ();
		for (int i = 0; i < rigs.Length; i++) {
			rigs [i].isKinematic = set;
		}
	}

	public void BeenHarpooned (Harpoon h) {
		Bird birdie = h.GetHarpooner ().GetComponent<Bird> ();

		linkedBirds.Clear ();
		//AttemptWeb (birdie);

		Circuit circ = new Circuit ();
		circ.AttemptCircuit (birdie);
	}




	public void HarpoonReleased (Harpoon h) {
		RemoveHarp (h);
	}
}