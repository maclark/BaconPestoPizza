using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class Bird : MonoBehaviour {

	public Player p;
	public int health = 100;
	public int maxHealth = 100;
	public float accelerationMagnitude = 60f;
	public float fireRate = .1f;
	public float releaseBoost = 60f;
	public bool docked = false;
	public float flashGap = 1f;
	public float flashDuration = .15f;
	public float flashDampener = .1f;
	public float hitInvincibilityDuration = .25f;
	public float boostCooldown = 2f;
	public int clipSize = 100;
	public float reloadSpeed = .5f;
	public float fullTank = 60f;
	public float lowGasWarning = 10f;
	public float gas = 60f;
	public float gasPerSecond = 1f;
	public float gasPerBoost = 2f;
	public float gasLightDelay = .2f;
	public GameObject harpoonPrefab;
	public GameObject gasLightPrefab;
	public List<Bird> linkedBirds;


	[HideInInspector]
	public Color color;
	[HideInInspector]
	public int roundsLeftInClip;
	[HideInInspector]
	public bool firing = false, damaged = false, invincible = false, canBoost = true, hasHarpoon = true;
	[HideInInspector]
	public Vector2 direction = Vector2.zero, pullDir = Vector2.zero, orthoPullDir = Vector2.zero, aim = Vector2.zero;
	[HideInInspector]
	public Harpoon harp;
	[HideInInspector]
	public Web web;

	private Rigidbody2D rb;
	private GameManager gm;
	private BigBird bigBird;
	private List<Harpoon> otherHarps;
	private Dock dock = null;
	private GameObject gasLight;
	private float startAccelerationgMagnitude;
	private bool ranOutOfGas = false;
	private bool gasLightFlashing = false;

	void Awake() {
		rb = GetComponent<Rigidbody2D> ();
		gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager> ();
		bigBird = GameObject.FindObjectOfType<BigBird>() as BigBird;
	}

	void Start () {
		color = GetComponent<SpriteRenderer> ().color;

		otherHarps = new List<Harpoon> ();
		linkedBirds = new List<Bird> ();

		roundsLeftInClip = clipSize;
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
		if (other.name == "BigBird") {
			if (!docked) {
				Dock ();
			}
		}
		else if (other.tag == "EnemyBullet") {
			if (!invincible) {
				print ("hit enemy bullet");
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
		else if (other.name == "Harpoon") {
			if (other.GetComponent<Harpoon> ().GetHarpooner ().name == gameObject.name) {
				hasHarpoon = true;
				Destroy (other.gameObject);
			} else {
				//implement linking
				Harpoon otherHarp = other.GetComponent<Harpoon> ();
				if (!otherHarp.recalling) {
					otherHarp.HarpoonObject (gameObject);
					otherHarps.Add (otherHarp);
				}
			}
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
			Die();
		} else {
			health -= dam;
			if (health < 0) {
				health = 0;
			}
			damaged = true;
			StartCoroutine (Invincibility (hitInvincibilityDuration));
			StartCoroutine (FlashDamage (Time.time));
		}
	}

	void Die() {
		gm.RemoveAlliedTransform (transform);
		Destroy (gameObject);
	}
			
	public bool Dock () {
		dock = bigBird.GetNearestOpenDock (transform.position);
		if (dock) {
			dock.bird = this;
			if (dock.transform.parent == bigBird.transform) {
				transform.position = dock.transform.position;
				bigBird.DockBird (this);
				docked = true;
			}
		} else {
			docked = false;
			return docked;
		}

		if (otherHarps.Count > 0) {
			print ("otherHarps.Count : " + otherHarps.Count);
			foreach (Harpoon h in otherHarps) {
				//#TODO maybe make harpooner/harpooned not public, bc every time you change those, you need to do other shit 
				h.DetachAndRecall (true);
			}
			otherHarps.Clear ();
		}

		transform.parent = bigBird.transform;
		GetComponent<BoxCollider2D> ().enabled = false;
		GetComponent<SpriteRenderer> ().color = color;
		GetComponent<SpriteRenderer> ().sortingLayerName = "BigBird";
		GetComponent<SpriteRenderer> ().sortingOrder = 1;
		rb.Sleep ();

		damaged = false;
		invincible = false;
		gasLightFlashing = false;
		ranOutOfGas = false;
		Destroy (gasLight);

		firing = false;
		CancelInvoke ();
		if (p) {
			dock.GetComponent<BoxCollider2D> ().enabled = false;
			p.GetComponent<PlayerInput> ().state = PlayerInput.State.docked;
			p.GetComponent<PlayerInput> ().CancelInvoke ();
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
			bigBird.UndockBird (this);
			dock.GetComponent<BoxCollider2D> ().enabled = true;
		}
		docked = false;
		dock.bird = null;
		dock = null;
		transform.parent = null;
		transform.rotation = Quaternion.identity;
		rb.WakeUp ();
		Vector3 releaseDirection = transform.position - bigBird.transform.position;
		releaseDirection.Normalize ();
		rb.AddForce (releaseDirection * releaseBoost * rb.mass);
		Invoke ("EnableCollider", .5f);
		GetComponent<SpriteRenderer> ().sortingLayerName = "Birds";
		GetComponent<SpriteRenderer> ().sortingOrder = 0;
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
				yield break;
			}
			yield return new WaitForSeconds (Mathf.Max (0f, flashGap - flashGapModifier));
			StartCoroutine (FlashDamage (timeAtDamage));
		}
	}

	public void FireBullet () {
		if (roundsLeftInClip > 0) {
			Bullet bullet = GetComponent<ObjectPooler>().GetPooledObject ().GetComponent<Bullet> ();
			bullet.gameObject.SetActive (true);
			bullet.Fire (transform, aim);
			roundsLeftInClip--;
		}

		if (roundsLeftInClip <= 0) {
			StartCoroutine (Reload ());
		}
	}

	/// <summary>
	/// Hurls the harpoon.
	/// </summary>
	public void HurlHarpoon () {
		if (hasHarpoon) {
			//Offset harp so it doesn't immediately collide with ship
			Vector3 aim3 = new Vector3 (aim.x, aim.y, 0);
			aim3.Normalize ();
			float x = GetComponent<BoxCollider2D> ().size.x;
			float y = GetComponent<BoxCollider2D> ().size.y;
			float d = Mathf.Sqrt (x * x + y * y);
			Vector3 startPosition = transform.position + aim3 * d;

			GameObject harpoon = Instantiate (harpoonPrefab, transform.position, transform.rotation) as GameObject;
			harpoon.transform.position = startPosition;
			harpoon.name = "Harpoon";
			harp = harpoon.GetComponent<Harpoon> ();
			harp.SetHarpooner (gameObject);
			harp.Fire (harp.transform, aim);
			hasHarpoon = false;
		}
	}

	public void DetachHarpoon () {
		if (harp) {
			harp.DetachAndRecall ();
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

	public IEnumerator Boost () {
		canBoost = false;
		gas -= gasPerBoost;
		accelerationMagnitude = releaseBoost;
		yield return new WaitForSeconds (boostCooldown);
		canBoost = true;
	}

	IEnumerator Reload () {
		yield return new WaitForSeconds (reloadSpeed);
		roundsLeftInClip = clipSize;
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
}
