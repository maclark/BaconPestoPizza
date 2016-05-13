using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class Bird : MonoBehaviour {

	public Player p;
	public int hp = 100;
	public float moveForceMagnitude = 60f;
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
	public int gas = 100;
	public GameObject harpoonPrefab;
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
	private float startForceMag;

	void Awake() {
		rb = GetComponent<Rigidbody2D> ();
		gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager> ();
		bigBird = GameObject.FindObjectOfType<BigBird>() as BigBird;
	}

	void Start () {
		color = GetComponent<SpriteRenderer> ().color;
		GetComponent<ObjectPooler> ().SetPooledObjectsColor (color);

		otherHarps = new List<Harpoon> ();
		linkedBirds = new List<Bird> ();

		roundsLeftInClip = clipSize;
		startForceMag = moveForceMagnitude;

		Dock ();
		gm.AddAlliedTransform (transform);
	}

	void FixedUpdate () {
		if (!docked) {
			if (harp != null && harp.atMaxTether) {
				HandlePulling ();
			} else if (otherHarps.Count > 0) {
				bool pulledHarpooner = false;
				//TODO will need to actually calulate various tensions distributed...maybe
				foreach (Harpoon h in otherHarps) {
					if (h.atMaxTether) {
						HandlePullingHarpooner (h);
						pulledHarpooner = true;
					}
				}

				if (!pulledHarpooner) {
					rb.AddForce (direction * moveForceMagnitude);
				}

			}
			else {
				rb.AddForce (direction * moveForceMagnitude);
			}

			//check if just boosted
			if (!canBoost) {
				moveForceMagnitude = startForceMag;
			}
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
		else if (other.name == "Harpoon") {
			if (other.GetComponent<Harpoon> ().harpooner.name == gameObject.name) {
				hasHarpoon = true;
				Destroy (other.gameObject);
			} else {
				//implement linking
				Harpoon otherHarp = other.GetComponent<Harpoon> ();
				otherHarps.Add (otherHarp);
				if (!otherHarp.recalling) {
					otherHarp.HarpoonObject (gameObject);
				}
			}
		}
	}

	void HandlePulling () {
		pullDir = transform.position - harp.transform.position;
		pullDir.Normalize ();
		float pullMag = CalculatePullMag (direction, moveForceMagnitude);
		//ship is moving away from harpoon and harpoon is at max tether
		if (pullMag > 0) {
			//calculate pulling axis force
			//if pulling harpoonobject
			if (harp.harpooned != null) {
				float totalMass = rb.mass + harp.harpooned.GetComponent<Bird> ().GetComponent<Rigidbody2D> ().mass;
				rb.AddForce (pullDir * pullMag / totalMass);
				harp.harpooned.GetComponent<Bird> ().GetComponent<Rigidbody2D> ().AddForce (pullDir * pullMag / totalMass);
			} 
			//if no object attached to harp
			else {
				float shipAndHarpMass = rb.mass + harp.GetComponent<Rigidbody2D> ().mass;
				rb.AddForce (pullDir * pullMag / shipAndHarpMass);
				harp.GetComponent<Rigidbody2D> ().AddForce (pullDir * pullMag / shipAndHarpMass);
			}
			float orthoPullMag = CalculateOrthoPullMag (direction, moveForceMagnitude);
			rb.AddForce (orthoPullDir * orthoPullMag);
		}
		//ship is not moving away from harpoon, check for harpoon velocity
		else if (harp.GetComponent<Rigidbody2D> ().velocity != Vector2.zero) {

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

			//don't forget to move player
			rb.AddForce (direction * moveForceMagnitude);
		} 

		else {
			rb.AddForce (direction * moveForceMagnitude);
		}

	} 

	//#TODO what happens when both ships have harpooned each other? should ignore one harpoon
	//Very similar to HandlePulling(), but for when this player's ship is harpooned and that harpoon's tether is at max length
	void HandlePullingHarpooner (Harpoon otherHarp) {
		pullDir = transform.position - otherHarp.harpooner.transform.position;
		pullDir.Normalize ();
		float pullMag = CalculatePullMag (direction, moveForceMagnitude);
		if (pullMag > 0) {
			float totalMass = rb.mass + otherHarp.harpooner.GetComponent<Bird> ().GetComponent<Rigidbody2D> ().mass;
			rb.AddForce (pullDir * pullMag / totalMass);
			otherHarp.harpooner.GetComponent<Bird> ().GetComponent<Rigidbody2D> ().AddForce (pullDir * pullMag / totalMass);
		} 
		//at max tether, but heading towards harpoon, do don't get other mass or apply a force to other's rb
		else {
			rb.AddForce (pullDir * pullMag);
		}
		float orthoPullMag = CalculateOrthoPullMag (direction, moveForceMagnitude);
		rb.AddForce (orthoPullDir * orthoPullMag);
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
			damaged = true;
			StartCoroutine (Invincibility (hitInvincibilityDuration));
			StartCoroutine (FlashDamage (Time.time));
		}
	}

	void Die() {
		Destroy (gameObject);
		gm.RemoveAlliedTransform (transform);
	}
			
	public bool Dock () {
		dock = bigBird.GetNearestOpenDock (transform.position);
		if (dock) {
			dock.bird = this;
		} else {
			return false;
		}
		Vector3 dockPosition = dock.transform.position;
		if (dockPosition != Vector3.zero) {
			transform.Translate (dockPosition - transform.position);
			transform.parent = bigBird.transform;
			GetComponent<BoxCollider2D> ().enabled = false;
			GetComponent<SpriteRenderer> ().color = color;
			rb.Sleep ();
			firing = false;
			damaged = false;
			invincible = false;
			docked = true;
			CancelInvoke ();
			if (p) {
				p.GetComponent<PlayerInput> ().CancelInvoke ();
			}
			if (harp) {
				harp.DetachAndRecall ();
				Destroy (harp.gameObject);
				hasHarpoon = true;
			}
			return true;
		} else return false;
	}

	public void Undock () {
		docked = false;
		dock.bird = null;
		dock = null;
		transform.parent = null;
		transform.rotation = Quaternion.identity;
		rb.WakeUp ();
		Vector3 releaseDirection = transform.position - bigBird.transform.position;
		releaseDirection.Normalize ();
		rb.AddForce (releaseDirection * releaseBoost);
		Invoke ("EnableCollider", .5f);
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

	public void HarpoonAction () {
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
		moveForceMagnitude = releaseBoost;
		yield return new WaitForSeconds (boostCooldown);
		canBoost = true;
	}

	IEnumerator Reload () {
		yield return new WaitForSeconds (reloadSpeed);
		roundsLeftInClip = clipSize;
	}

	float CalculatePullMag (Vector3 dir, float forceMag) {
		float pullMagnitude = Vector3.Dot (dir * forceMag, pullDir);
		return pullMagnitude;
	}

	float CalculateOrthoPullMag (Vector3 dir, float forceMag) {
		orthoPullDir = new Vector3 (-pullDir.y, pullDir.x, 0);
		orthoPullDir.Normalize ();
		float orthoPullMagnitude = Vector3.Dot (dir * forceMag, orthoPullDir);
		return orthoPullMagnitude;
	}

}
