using UnityEngine;
using System.Collections;

public class SolarPanel : MonoBehaviour {
	public Vector3 carryOffset;
	public Bird b;

	private GameManager gm;
	private SpriteRenderer sr;
	private Rigidbody2D rb;
	private Harpoonable hool;
	private Sun localSun;
	private Quaternion initialLocalRot;
	private float xOffset;
	private float yOffset;
	private bool loose;
	private bool affixedToBigBird = true;
	private bool absorbing;

	void Awake () {
		gm = GameObject.FindObjectOfType<GameManager> ();
		sr = GetComponent<SpriteRenderer> ();
		rb = GetComponent<Rigidbody2D> ();
		hool = GetComponent<Harpoonable> ();
		xOffset = transform.localPosition.x;
		yOffset = transform.localPosition.y;
		initialLocalRot = transform.localRotation;
	}

	void Update () {
		if (!loose && !b) {
			MoveToBigBirdShoulder ();
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "Bird") {
			Bird birdie = other.GetComponent<Bird> ();
			if (affixedToBigBird && !birdie.GetPanel ()) {
				DetachFromBigBird ();
				AttachToBird (birdie);
			} else if (loose) {
				AttachToBird (birdie);
			}
		}
	}

	void OnTriggerStay2D (Collider2D other) {
		if (!loose) {
			Sun sunTouched = other.GetComponent<Sun> ();
			if (sunTouched) {
				AtSolarSource (sunTouched);
			}
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (!loose) {
			Sun sunTouched = other.GetComponent<Sun> ();
			if (other.GetComponent<Sun> ()) {
				LeftSolarSource (sunTouched);
			}
		}
	}

	public void AtSolarSource (Sun s) {
		localSun = s;
		AdjustAbsorbing ();
	}

	public void LeftSolarSource (Sun s) {
		if (localSun == s) {
			localSun = null;
		}
		AdjustAbsorbing ();
	}

	public void AdjustAbsorbing () { 
		if (b) {
			if (localSun && b.harp.isSolarHose) {
				b.harp.SetTetherColor (Color.yellow);
				GetComponent<Animator> ().SetBool ("isAbsorbing", true);
				gm.bbm.absorbing = true;
			} else {
				if (b.harp && b.harp.isSolarHose) {
					b.harp.SetTetherColor (Color.grey);
				}
				GetComponent<Animator> ().SetBool ("isAbsorbing", false);
				gm.bbm.absorbing = false;
			}
		} else if (!loose && localSun) {
			GetComponent<Animator> ().SetBool ("isAbsorbing", true);
			gm.bbm.absorbing = true;
		} else {
			GetComponent<Animator> ().SetBool ("isAbsorbing", false);
			gm.bbm.absorbing = false;
		}
	}

	public void DetachFromBigBird () {
		transform.parent = null;
		if (localSun) {
			LeftSolarSource (localSun);
		}
		affixedToBigBird = false;
	}

	public void AttachToBird (Bird carrierBird) {
		if (carrierBird.GetPanel ()) {
			return;
		}

		carrierBird.CarryPanel (this);
		b = carrierBird;
		transform.parent = b.transform;
		transform.position = b.transform.position + carryOffset;
		transform.rotation = Quaternion.identity;
		rb.angularVelocity = 0f;
		sr.sortingLayerName = "Projectiles";
		loose = false;
		hool.enabled = false;

		rb.isKinematic = true;
		hool.BreakLoose ();
	}

	public void DetachFromBird () {
		if (b == null) {
			return;
		}

		b.DropPanel ();
		b = null;
		transform.parent = null;

		rb.isKinematic = false;
		loose = true;
		//Invoke ("SetLoose", 1f);
	}

	void SetLoose () {
		loose = true;
	}

	public void Reset () {
		DetachFromBird ();
		CancelInvoke ();
		loose = false;

		MoveToBigBirdShoulder ();
		transform.parent = gm.bigBird.transform;
		sr.sortingLayerName = "BigBird";
		sr.sortingOrder = 1;
	}

	void MoveToBigBirdShoulder () {
		Vector3 rightOffset = gm.bigBird.transform.right * xOffset * gm.bigBird.transform.localScale.x;
		Vector3 upOffset = gm.bigBird.transform.up * yOffset * gm.bigBird.transform.localScale.y;
		transform.position = gm.bigBird.transform.position + rightOffset + upOffset;
		transform.rotation = gm.bigBird.transform.rotation;
		transform.localRotation = initialLocalRot;
		affixedToBigBird = true;
	}
}
