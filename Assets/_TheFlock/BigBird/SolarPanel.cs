using UnityEngine;
using System.Collections;

public class SolarPanel : MonoBehaviour {
	//public float absorptionRate = 1000f;
	public float detachTorque = 10f;
	public Vector3 carryOffset;
	public Bird b;

	private GameManager gm;
	private SpriteRenderer sr;
	private Rigidbody2D rb;
	private BoxCollider2D bc;
	private Harpoonable hool;
	private Joint2D jo;
	private Sun localSun;
	private Quaternion initialLocalRot;
	private float xOffset;
	private float yOffset;
	private float breakForce;
	private bool loose;
	private bool absorbing;

	void Awake () {
		gm = GameObject.FindObjectOfType<GameManager> ();
		sr = GetComponent<SpriteRenderer> ();
		rb = GetComponent<Rigidbody2D> ();
		bc = GetComponent<BoxCollider2D> ();
		hool = GetComponent<Harpoonable> ();
		jo = GetComponent<Joint2D> ();
		xOffset = transform.localPosition.x;
		yOffset = transform.localPosition.y;
		initialLocalRot = transform.localRotation;
		breakForce = jo.breakForce;
	}

	void Update () {
		if (!loose && !b) {
			MoveToBigBirdShoulder ();
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (loose) {
			if (other.tag == "Bird") {
				Bird birdie = other.GetComponent<Bird> ();
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
		
	public void BeenHarpooned (Harpoon h) {
		
	}

	void OnJointBreak2D (Joint2D brokenJoint) {
		DetachFromBigBird ();
	}

	public void DetachFromBigBird () {
		transform.parent = null;
		rb.isKinematic = false;
		bc.enabled = true;
		loose = true;
		if (localSun) {
			LeftSolarSource (localSun);
		}
		rb.AddTorque (Random.Range (-detachTorque, detachTorque));
		sr.sortingLayerName = "Birds";
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

		gameObject.AddComponent<HingeJoint2D> ();
		jo = GetComponent<HingeJoint2D> ();
		jo.breakForce = breakForce;
		jo.connectedBody = gm.bigBird.GetComponent<Rigidbody2D> ();
	}

	void MoveToBigBirdShoulder () {
		Vector3 rightOffset = gm.bigBird.transform.right * xOffset * gm.bigBird.transform.localScale.x;
		Vector3 upOffset = gm.bigBird.transform.up * yOffset * gm.bigBird.transform.localScale.y;
		transform.position = gm.bigBird.transform.position + rightOffset + upOffset;
		transform.rotation = gm.bigBird.transform.rotation;
		transform.localRotation = initialLocalRot;
	}
}
