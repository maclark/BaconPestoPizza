using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public bool isPlaying = false;
	public Color color;
	public Vector3 ridingOffset;
	public Bird b = null;
	public Weapon w = null;
	public Weapon storedWeapon = null;
	public Vector3 aim = Vector3.zero;
	public Sprite[] sprites = new Sprite[2];
	public PlayerBody body;
	public Transform itemHeld;
	public Transform itemTouching;

	private bool holdingString = false;
	private bool reigningPowerbird = false;
	private GameManager gm;
	private SpriteRenderer sr;
	private PlayerInput pi;
	private BigBird bigBird;
	private Holster hol;
	private ObjectPooler pooler;


	void Awake () {
		gm = GameObject.FindObjectOfType<GameManager> ();
		pi = GetComponent<PlayerInput> ();
		sr = GetComponent<SpriteRenderer> ();
		bigBird = GameObject.FindObjectOfType<BigBird> ();
		hol = new Holster (this);
		color = sr.color;
		body = gm.GetBody (this);
		pooler = GetComponent<ObjectPooler> ();
		pooler.SetPooledObjectsColor (color);
		pooler.SetPooledObjectsOwner (transform);
	}

	public void StartPlayer () {
		isPlaying = true;
		sr.enabled = true;
		pi.state = PlayerInput.State.NEUTRAL;
	}

	public void BoardBird (Bird bird) {
		sr.sprite = sprites [1];
		sr.enabled = true;

		b = bird;
		b.p = this;
		b.color = GetComponent<SpriteRenderer> ().color;
		b.body.GetComponent<SpriteRenderer>().color = b.color;
		b.Shield.SetColor (color);
		b.ReloadIndicator.SetColor (color);
		pooler.enabled = true;
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
		sr.sortingOrder = 1;

		transform.parent = newParent;
		transform.position = pi.station.position;

		if (b.Shield != null) {
			b.GetComponentInChildren<Shield> (true).DeactivateShield ();
		}
		b.color = Color.grey;
		b.body.GetComponent<SpriteRenderer>().color = Color.black;
		b.transform.rotation = bigBird.transform.rotation;
		b.p = null;
		b = null;
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
		if (itemHeld) {
			Destroy (itemHeld.gameObject);
			itemHeld = null;
		}
		sr.sprite = sprites [0];
		GameObject bub = Instantiate (gm.bubblePrefab, transform.position, transform.rotation) as GameObject;
		gm.AddAlliedTransform (bub.transform);
		bub.GetComponent<Bubble> ().p = this;
		bub.GetComponent<Rigidbody2D> ().velocity = initialVelocity;
		bub.GetComponent<SpriteRenderer> ().color = new Color (color.r, color.g, color.b, .5f);
		sr.color = color;
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
		if (itemHeld) {
			itemHeld.GetComponent<SpriteRenderer> ().sortingLayerName = sr.sortingLayerName;
			itemHeld.GetComponent<SpriteRenderer> ().sortingOrder = sr.sortingOrder + 1;
		}
		pi.station = d.transform;
		pi.state = PlayerInput.State.DOCKED;
		pi.CancelInvoke ();
		w.firing = false;
		d.GetComponent<BoxCollider2D> ().enabled = false;
	}

	public void Undock () {
		itemTouching = null;
		sr.sortingLayerName = "Birds";
		sr.sortingOrder = 1;
		if (itemHeld) {
			itemHeld.GetComponent<SpriteRenderer> ().sortingLayerName = sr.sortingLayerName;
			itemHeld.GetComponent<SpriteRenderer> ().sortingOrder = sr.sortingOrder + 1;
		}
	}



	public void MoveToPlatform () {
		sr.enabled = false;
		pi.state = PlayerInput.State.ON_PLATFORM;
	}

	public void ManifestFlesh (Vector3 descensionPosition, string sortingLayer, int sortingOrd) {
		body.gameObject.SetActive (true);
		body.transform.position = descensionPosition;
		body.transform.rotation = Quaternion.identity;
		transform.position = body.transform.position;// + body.playerOffset;
		transform.rotation = body.transform.rotation;
		transform.parent = body.transform;
		sr.sortingLayerName = sortingLayer;
		sr.sortingOrder = sortingOrd;
		if (itemHeld) {
			itemHeld.GetComponentInChildren<SpriteRenderer> ().sortingLayerName = sr.sortingLayerName;
			itemHeld.GetComponentInChildren<SpriteRenderer> ().sortingOrder = sr.sortingOrder + 1;
		}
	}

	public void SpiritAway (Transform master, PlayerInput.State s) {
		transform.parent = master;
		body.gameObject.SetActive (false);
		if (master == bigBird.transform) {
			sr.sortingLayerName = "BigBird";
			sr.sortingOrder = 1;
			if (itemHeld) {
				itemHeld.GetComponentInChildren<SpriteRenderer> ().sortingLayerName = sr.sortingLayerName;
				itemHeld.GetComponentInChildren<SpriteRenderer> ().sortingOrder = sr.sortingOrder + 1;
			}
		}
		pi.state = s;
	}

	public void Disembark (Vector3 disembarkPoint) {
		pi.AbandonStation ();
		sr.enabled = true;
		ManifestFlesh (disembarkPoint, "Buildings", 2);
		pi.state = PlayerInput.State.ON_FOOT;
	}

	public void DropItem () {
		itemHeld.GetComponent<Item> ().Drop (this, GetComponent<SpriteRenderer> ().sortingLayerName, GetComponent<SpriteRenderer> ().sortingOrder - 1);
	}

	public void PickUpItem () {
		if (!itemTouching) {
			return;
		}

		if (itemHeld) {
			DropItem ();
		}

		if (!itemHeld) {
			Item it = itemTouching.GetComponent<Item> ();
			if (it.forSale) {
				if (!it.keeper.SellToPlayer (it)) {
					return;
				}
			}

			if (pi.state == PlayerInput.State.IN_COOP) {
				pi.station.GetComponent<Coop> ().RemoveOccupant (itemTouching);
			} else if (pi.state == PlayerInput.State.DOCKED) {
				pi.station.GetComponent<Dock> ().item = null;
			}

			itemHeld = itemTouching;
			itemTouching = null;
			itemHeld.transform.position = transform.position;
			itemHeld.transform.parent = transform;

			Rigidbody2D itrb = itemHeld.GetComponent<Rigidbody2D> ();
			itrb.velocity = Vector3.zero;
			itrb.isKinematic = true;
			itrb.drag = 1f;

			SpriteRenderer itsr = itemHeld.GetComponentInChildren<SpriteRenderer> ();
			itsr.sortingLayerName = sr.sortingLayerName;
			itsr.sortingOrder = sr.sortingOrder + 1;

			itemHeld.GetComponent<Collider2D> ().enabled = false;
		}
	}

	public void Killed (Transform t) {
		print (name + " killed " + t.name);
		if (b) {
			if (b.pup) {
				b.pup.IncrementAssists ();
			}
		}
	}

	public void Webbed (OrbWeb ow) {
		if (ow.webType == OrbWeb.WebType.POWERBIRD) {
			storedWeapon = w;
			b.GetComponent<SpriteRenderer> ().sprite = b.powerbird;
			w = new PowerWeapon (hol);
		}
		b.InvokeRepeating ("RandomColor", Eaglehead.colorSwapSpeed, Eaglehead.colorSwapSpeed);
		pi.state = PlayerInput.State.IN_WEB;
		pi.CancelInvoke ();
	}

	public void Unwebbed () {
		if (storedWeapon != null) {
			pi.CancelInvoke ();
			w = storedWeapon;

			storedWeapon = null;
		}
		b.GetComponent<SpriteRenderer> ().sprite = b.greyhound;
		b.CancelInvoke ();
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

	public void Powered (Powerbird pb) {
		storedWeapon = w;
		b.GetComponentInChildren<SpriteRenderer> ().sprite = b.powerbird;
		w = new PowerWeapon (hol);
		pb.InvokeRepeating ("FlashColors", Eaglehead.colorSwapSpeed, Eaglehead.colorSwapSpeed);
		pi.state = PlayerInput.State.POWERBIRD;
		pi.CancelInvoke ();
	}

	public void Unpowered () {
		pi.CancelInvoke ();
		w = storedWeapon;
		storedWeapon = null;
		b.GetComponentInChildren<SpriteRenderer> ().sprite = b.greyhound;
		b.GetComponentInChildren<SpriteRenderer> ().color = b.color;
		//pb.CancelInvoke ();
		pi.CancelInvoke ();
		pi.state = PlayerInput.State.FLYING;
	}

	public void ReignPowerbird (Powerbird pb) {
		reigningPowerbird = true;
		print ("player" + pi.playerNum + " cries, 'POWER OVERWHELMING!'");
		pi.CancelInvoke ();
	}

	public void UnreignPowerbird () {
		reigningPowerbird = false;
	}

	public bool GetReigningBird () {
		return reigningPowerbird;
	}

	public void Equip (Weapon weap) {
		storedWeapon = w;
		w = weap;
		pi.CancelInvoke ();
	}

	public void Unequip () {
		w = storedWeapon;
		storedWeapon = null;
		pi.CancelInvoke ();
	}
}
