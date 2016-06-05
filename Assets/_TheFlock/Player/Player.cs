using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public Color color;
	public Vector3 ridingOffset;
	public Bird b = null;
	public Weapon w = null;
	public Weapon storedWeapon = null;
	public Vector3 aim = Vector3.zero;
	public Sprite[] sprites = new Sprite[2];

	private bool holdingString = false;
	private GameManager gm;
	private SpriteRenderer sr;
	private PlayerInput pi;
	private BigBird bigBird;
	private Holster hol;


	void Awake () {
		gm = GameObject.FindObjectOfType<GameManager> ();
		pi = GetComponent<PlayerInput> ();
		sr = GetComponent<SpriteRenderer> ();
		bigBird = GameObject.FindObjectOfType<BigBird> ();
		hol = new Holster (this);
		color = sr.color;
	}

	public void StartPlayer () {
		sr.enabled = true;
		pi.state = PlayerInput.State.NEUTRAL;
	}

	public void BoardBird (Bird bird) {
		sr.sprite = sprites [1];
		sr.enabled = true;

		b = bird;
		b.p = this;
		//b.color = GetComponent<SpriteRenderer> ().color;
		//b.body.GetComponent<SpriteRenderer>().color = b.color;
		b.Shield.SetColor (color);
		b.ReloadIndicator.SetColor (color);
		GetComponent<ObjectPooler> ().enabled = true;
		GetComponent<ObjectPooler> ().SetPooledObjectsColor (color);
		b.transform.rotation = Quaternion.identity;
		b.body.rotation = Quaternion.identity;
		transform.rotation = b.transform.rotation;
		transform.position = b.transform.position + ridingOffset;
		transform.parent = b.body;
	}

	public void Debird (Transform newParent) {
		sr.color = color;
		sr.sprite = sprites [0];
		sr.sortingLayerName = newParent.GetComponent<SpriteRenderer> ().sortingLayerName;
		sr.sortingOrder = 2;

		transform.parent = newParent;
		transform.position = pi.station.position;

		if (b.Shield != null) {
			b.GetComponentInChildren<Shield> (true).DeactivateShield ();
		}
		b.transform.rotation = bigBird.transform.rotation;
		b.p = null;
		b = null;

		//b.color = Color.black;
		//b.body.GetComponent<SpriteRenderer>().color = Color.black;
	}

	public void BoardBigBird () {
		sr.sortingLayerName = "BigBird";
		sr.sortingOrder = 2;

		transform.position = bigBird.transform.position;
		transform.rotation = bigBird.transform.rotation;
		transform.parent = bigBird.transform;

		pi.state = PlayerInput.State.NEUTRAL;
	}

	public void Bubble (Vector3 initialVelocity) {
		if (b.harp) {
			b.harp.SetGripping (false);
			b.harp.SetRecalling (true);
		}
		sr.sprite = sprites [0];
		GameObject bub = Instantiate (gm.bubblePrefab, transform.position, transform.rotation) as GameObject;
		gm.AddAlliedTransform (bub.transform);
		bub.GetComponent<Bubble> ().p = this;
		bub.GetComponent<Rigidbody2D> ().velocity = initialVelocity;
		bub.GetComponent<SpriteRenderer> ().color = new Color (color.r, color.g, color.b, .5f);
		transform.parent = bub.transform;
		pi.CancelInvoke ();
		pi.state = PlayerInput.State.IN_BUBBLE;
	}

	public void CycleWeapons () {
		b.TurnOffReloadIndicator ();
		hol.CycleWeapons ();
	}

	public void CockWeapon () {
		w.CockWeapon ();
	}

	public void DockOnBigBird (Dock d) {
		sr.sortingLayerName = "BigBird";
		sr.sortingOrder = 2;
		pi.station = d.transform;
		pi.state = PlayerInput.State.DOCKED;
		pi.CancelInvoke ();
		w.firing = false;

		d.GetComponent<BoxCollider2D> ().enabled = false;
	}

	public void Webbed (OrbWeb ow) {
		if (ow.webType == OrbWeb.WebType.POWERBIRD) {
			storedWeapon = w;
			w = new PowerWeapon (hol);
		}
		pi.state = PlayerInput.State.IN_WEB;
		pi.CancelInvoke ();
	}

	public void Unwebbed () {
		if (storedWeapon != null) {
			pi.CancelInvoke ();
			w = storedWeapon;

			storedWeapon = null;
		}
		pi.CancelInvoke ();
		pi.state = PlayerInput.State.FLYING;
	}

	public void HoldWebString (OrbWeb ow) {
		holdingString = true;
		if (ow.webType == OrbWeb.WebType.POWERBIRD) {
			print ("player" + pi.playerNum + " cries, 'hold the line!'");
		}
		pi.CancelInvoke ();
	}

	public void ReleaseWebString () {
		holdingString = false;
	}

	public bool GetHoldingString () {
		return holdingString;
	}

	public void MoveToPlatform () {
		sr.enabled = false;
		pi.state = PlayerInput.State.ON_PLATFORM;
	}
}
