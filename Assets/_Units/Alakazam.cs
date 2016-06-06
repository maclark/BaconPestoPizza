using UnityEngine;
using System.Collections;

public class Alakazam : Flyer {
	public int shieldMaxHealth = 300;
	public int shieldHealth = 300;
	public float skew = 2f;
	public float teleportDelay = 2f;
	public float teleportRange = 16f;
	public float portalDuration = 2f;
	public float alakazamRadius = 1.48f;
	public float shieldRadius = 1.8f;

	public Transform shield;
	public GameObject miniPortalPrefab;

	private CircleCollider2D cc;
	private bool shieldBroken = false;
	private bool teleporting = false;

	void Awake() {
		cc = GetComponent<CircleCollider2D> ();
		cc.radius = shieldRadius;
		base.OnAwake ();
	}

	void Start () {
		base.OnStart ();
	}

	void Update () {
		base.OnUpdate ();
	}

	void FixedUpdate () {
		base.OnFixedUpdate ();
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "PlayerBullet") {
			TakeShieldDamage (other.GetComponent<Projectile> ().damage);
			if (!teleporting) {
				StartCoroutine (Teleport ());
			}
			other.GetComponent<Bullet> ().Die ();
		}
		else if (other.tag == "Explosion") {
			base.TriggerEnter2D (other);
		}
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
	public void TakeShieldDamage (int dam) {
		print ("taking shield dam");
		if (!shieldBroken) {
			shieldHealth -= dam;
			if (shieldHealth <= 0) {
				shieldBroken = true;
				cc.radius = alakazamRadius;
				shield.GetComponent<SpriteRenderer> ().enabled = false;
			}
		} else {
			TakeDamage (dam, Color.red);
		}
	}
	IEnumerator Teleport () {
		teleporting = true;

		yield return new WaitForSeconds (teleportDelay);
		GameObject obj = Instantiate (miniPortalPrefab, transform.position, Quaternion.identity) as GameObject;
		Destroy (obj, portalDuration);
		float x = Random.Range (-teleportRange, teleportRange);
		float y = Random.Range (-teleportRange, teleportRange);
		transform.position = new Vector3 (x, y, 0);
		shieldHealth = shieldMaxHealth;
		shieldBroken = false;
		shield.GetComponent<SpriteRenderer> ().enabled = true;
		cc.radius = shieldRadius;

		teleporting = false;
	}
}
