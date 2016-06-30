using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public bool isPlaying = false;
	public bool isCaptain = false;
	public bool female = false;
	public float reticleOffset = 2f;
	public float highlightSecs = 4f;
	public Color color;
	public Vector3 ridingOffset;
	public Vector3 playerHighlightPos = new Vector3 (-.1f, 0f, 0f);
	public Vector3 birdHighlightPos = new Vector3 (-.1f, 0f, 0f);
	public Bird b = null;
	public Weapon w = null;
	public Weapon storedWeapon = null;
	public Vector3 aim = Vector3.zero;
	public Sprite[] finnSprites = new Sprite[2];
	public Sprite[] fionaSprites = new Sprite[2];
	public Sprite[] sprites = new Sprite[2];
	public PlayerBody body;
	public Transform itemHeld;
	public Transform itemTouching;
	public Transform reticle;
	public Transform highlighter;

	private bool highlighting;
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

	}

	void Start () {
		body = gm.GetBody (this);
	}

	public void StartPlayer () {
		isPlaying = true;
		gm.AddAlliedTransform (transform);
		if (gm.captain == null) {
			isCaptain = true;
			gm.captain = this;
		}
		female = Random.Range(0, 1f) > .5 ? true : false;
		if (female) {
			sprites = fionaSprites;
		} else {
			sprites = finnSprites;
		}
		sr.enabled = true;
		color = gm.playerColors [pi.playerNum - 1];
		pooler = GetComponent<ObjectPooler> ();
		pooler.SetPooledObjectsColor (color);
		pooler.SetPooledObjectsOwner (transform);
		reticle.GetComponent<SpriteRenderer> ().color = color;
		highlighter.GetComponent<SpriteRenderer> ().color = new Color (color.r, color.g, color.b, .75f);
		MountAndCharge ();
	}

	void Update () {
		if (pi.state == PlayerInput.State.NEUTRAL ||
		    pi.state == PlayerInput.State.CHANGING_STATIONS ||
		    pi.state == PlayerInput.State.IN_HOLD ||
			pi.state == PlayerInput.State.ON_PLATFORM ||
			pi.state == PlayerInput.State.PILOTING ||
		    pi.state == PlayerInput.State.IN_COOP) {
			transform.rotation = Quaternion.identity;
		} else if (pi.state == PlayerInput.State.FLYING ||
			pi.state == PlayerInput.State.POWERBIRD) {
			reticle.transform.position = transform.position + aim * reticleOffset;
		}
	}

	void MountAndCharge () {
		Bird mount = gm.bigBird.GetUnboardedBird ();
		if (mount) {
			mount.GetDock ().MakeUnavailable ();
			mount.GetDock ().Man (this);
			BoardBird (mount);
			mount.GetDock ().Launch ();
		}
	}

	public void BoardBird (Bird bird) {
		sr.sprite = sprites [1];
		sr.enabled = true;

		b = bird;
		b.rider = this;
		b.color = color;
		b.body.GetComponent<SpriteRenderer>().color = b.color;
		b.Shield.SetColor (color);
		b.ReloadIndicator.SetColor (color);
		pooler.enabled = true;
		b.transform.rotation = Quaternion.identity;
		b.body.rotation = Quaternion.identity;
		transform.rotation = b.transform.rotation;
		transform.position = b.transform.position + ridingOffset;
		transform.parent = b.body;
		if (b.follower) {
			Hatchling pup = b.follower.GetComponent<Hatchling> ();
			if (pup) {
				pup.GetComponent<SpriteRenderer> ().color = color;
				pup.transform.rotation = b.transform.rotation;
			}
		}
	}

	public void Debird (Transform newParent) {
		sr.color = Color.white;
		sr.sprite = sprites [0];
		sr.sortingLayerName = newParent.GetComponent<SpriteRenderer> ().sortingLayerName;

		transform.parent = newParent;
		transform.position = pi.realStation.transform.position;

		if (b.Shield != null) {
			b.Shield.SetColor (new Color (0f, 0f, 0f, .5f));
		}
		b.color = Color.black;
		b.body.GetComponent<SpriteRenderer>().color = Color.black;
		if (b.follower) {
			Hatchling pup = b.follower.GetComponent<Hatchling> ();
			if (pup) {
				pup.GetComponent<SpriteRenderer> ().color = Color.grey;
			}
		}
		b.transform.rotation = bigBird.transform.rotation;
		b.rider = null;
		b = null;
	}

	public void BoardBigBird () {
		sr.sortingLayerName = "BigBird";
		sr.sortingOrder = 2;

		if (itemHeld) {
			if (itemHeld.GetComponent<Friend> ()) {
				print ("item held board big bird");
				gm.OpenPortal (itemHeld.GetComponent<Friend> ());
			}
		}
		transform.position = bigBird.transform.position;
		transform.rotation = bigBird.transform.rotation;
		transform.parent = bigBird.transform;

		pi.state = PlayerInput.State.NEUTRAL;
	}

	public void Bubble (Vector3 initialVelocity) {
		reticle.gameObject.SetActive (false);
		if (itemHeld) {
			Destroy (itemHeld.gameObject);
			itemHeld = null;
		}

		sr.sprite = sprites [0];
		GameObject bub = Instantiate (gm.bubblePrefab, transform.position, transform.rotation) as GameObject;
		gm.AddAlliedTransform (bub.transform);
		bub.GetComponent<Bubble> ().p = this;
		bub.GetComponent<Rigidbody2D> ().velocity = initialVelocity;
		sr.color = Color.white;
		bub.GetComponentInChildren<SpriteRenderer> ().color = new Color (color.r, color.g, color.b, .5f);
		transform.parent = bub.transform;
		pi.CancelInvoke ();
		pi.state = PlayerInput.State.IN_BUBBLE;
		b.rider = null;
		b = null;
	}

	public void CycleWeapons () {
		b.TurnOffReloadIndicator ();
		hol.CycleWeapons ();
	}

	public void CockWeapon () {
		w.CockWeapon ();
	}

	public void DockOnBigBird (Dock d) {
		d.MakeUnavailable ();
		pi.realStation = d;
		pi.realSelectedStation = d;
		d.Man (this);

		pi.CancelInvoke ();
		w.firing = false;
		reticle.gameObject.SetActive (false);
		sr.sortingLayerName = "BigBird";
		sr.sortingOrder = 2;
	}

	public void Undock () {
		reticle.gameObject.SetActive (true);
		aim = Vector3.right;
		reticle.transform.position = transform.position + aim * reticleOffset;
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
		if (pi.realStation) {
			pi.realStation.Abandon ();
		} else {
			pi.AbandonStation ();
		}
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
			if (itemHeld.GetComponent<Item>().itemType == Item.ItemType.FRIEND) {
				print ("picking up friend try");
				itemHeld.GetComponent<Friend> ().PickedUpFriend ();
			}
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
		if (b) {
			if (b.follower) {
				Hatchling pup = b.follower.GetComponent<Hatchling> ();
				if (pup) {
					pup.IncrementAssists ();
				}
			}
		}
	}

	public void Webbed (OrbWeb ow) {
		if (ow.webType == OrbWeb.WebType.POWERBIRD) {
			storedWeapon = w;
			//b.GetComponent<SpriteRenderer> ().sprite = b.thunderbird;
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
		w = new PowerWeapon (hol);
		sr.sprite = sprites [0];
		transform.position = transform.parent.position;
		b.GetComponentInChildren<SpriteRenderer> ().sprite = pb.thunderbird;
		pb.InvokeRepeating ("FlashColors", Eaglehead.readySwapSpeed, Eaglehead.readySwapSpeed);
		pi.state = PlayerInput.State.POWERBIRD;
		pi.CancelInvoke ();
	}

	public void Unpowered () {
		pi.CancelInvoke ();
		w = storedWeapon;
		storedWeapon = null;
		sr.sprite = sprites [1];
		transform.position = transform.parent.position + ridingOffset;
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
		if (storedWeapon != null) {
			w = storedWeapon;
			storedWeapon = null;
			pi.CancelInvoke ();
		}
	}

	void OnBecameInvisible () {
		//start ticker, then ghost
		Ghost ();
	}

	void OnBecameVisible () {
		//if ghost then Unghost, if ticking, stop ticker
	}

	void Ghost () {
	}

	public IEnumerator HighlightSecs () {
		if (b) {
			highlighter.transform.position = transform.position + birdHighlightPos;
		} else {
			highlighter.transform.position = transform.position + playerHighlightPos;
		}
		highlighter.gameObject.SetActive (true);
		yield return new WaitForSeconds (highlightSecs);
		if (!highlighting) {
			highlighter.gameObject.SetActive (false);
		}
		pi.canHighlight = true;
	}

	public void Highlight () {
		highlighting = true;
		if (b) {
			highlighter.transform.position = transform.position + birdHighlightPos;
		} else {
			highlighter.transform.position = transform.position + playerHighlightPos;
		}
		highlighter.gameObject.SetActive (true);

	}

	public void Unhighlight () {
		highlighting = false;
		highlighter.gameObject.SetActive (false);
	}
}
