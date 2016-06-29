using UnityEngine;
using System.Collections;

public class SolarPanel : MonoBehaviour {
	public Vector3 carryOffset;
	public Bird b;
	public Transform solarTank;

	private GameManager gm;
	private SpriteRenderer sr;
	private Rigidbody2D rb;
	private Harpoonable hool;
	private LineRenderer lr;
	private Sun localSun;
	private Quaternion initialLocalRot;
	private Vector3[] tetherPositions = new Vector3[2];
	private float xOffset;
	private float yOffset;
	public bool lost;
	public bool loose;
	public bool affixedToBigBird = true;

	void Awake () {
		gm = GameObject.FindObjectOfType<GameManager> ();
		sr = GetComponent<SpriteRenderer> ();
		rb = GetComponent<Rigidbody2D> ();
		hool = GetComponent<Harpoonable> ();
		lr = GetComponent<LineRenderer> ();

	}

	void Start () {
		lr.sortingLayerName = "Projectiles";
		xOffset = transform.localPosition.x;
		yOffset = transform.localPosition.y;
		initialLocalRot = transform.localRotation;
	}

	void Update () {
		if (!affixedToBigBird && !lost) {
			DrawHose ();
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
		Sun sunTouched = other.GetComponent<Sun> ();
		if (sunTouched) {
			AtSolarSource (sunTouched);
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		Sun sunTouched = other.GetComponent<Sun> ();
		if (other.GetComponent<Sun> ()) {
			LeftSolarSource (sunTouched);
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
		if (localSun && !lost) {
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
		lr.enabled = true;
		DrawHose ();
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
		hool.enabled = false;
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
		hool.enabled = true;
	}


	public void Reset () {
		DetachFromBird ();
		CancelInvoke ();
		MoveToBigBirdShoulder ();

		affixedToBigBird = true;
		loose = false;
		hool.enabled = false;
		lost = false;
		lr.enabled = false;

		transform.parent = gm.bigBird.transform;
		rb.isKinematic = true;
		sr.sortingLayerName = "BigBird";
		sr.sortingOrder = 1;
	}

	void MoveToBigBirdShoulder () {
		Vector3 rightOffset = gm.bigBird.transform.right * xOffset * gm.bigBird.transform.localScale.x;
		Vector3 upOffset = gm.bigBird.transform.up * yOffset * gm.bigBird.transform.localScale.y;
		transform.position = gm.bigBird.transform.position + rightOffset + upOffset;
		transform.rotation = gm.bigBird.transform.rotation * initialLocalRot;
	}

	public void DrawHose () {
		tetherPositions [0] = transform.position;
		tetherPositions [1] = solarTank.position;
		lr.SetPositions (tetherPositions);

		if (gm.bbm.absorbing) {
			lr.SetColors (Color.yellow, Color.yellow);
		} else {
			lr.SetColors (Color.gray, Color.gray);
		}
	}
}



/*
	public void AdjustAbsorbing () { 
		if (b) {
			if (b.harp) {
				if (localSun && b.harp.isSolarHose) {
					b.harp.SetTetherColor (Color.yellow);
					GetComponent<Animator> ().SetBool ("isAbsorbing", true);
					gm.bbm.absorbing = true;
				}
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
	}*/
