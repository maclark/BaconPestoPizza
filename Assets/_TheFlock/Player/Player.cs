using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public Vector3 ridingOffset;
	public Bird b = null;
	public Weapon w = null;
	public bool navigating = false;
	public Vector3 aim = Vector3.zero;
	public Sprite[] sprites = new Sprite[2];

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
		b.color = GetComponent<SpriteRenderer> ().color;
		b.GetComponent<SpriteRenderer>().color = b.color;
		GetComponent<ObjectPooler> ().enabled = true;
		GetComponent<ObjectPooler> ().SetPooledObjectsColor (b.color);
		b.transform.rotation = Quaternion.identity;
		transform.rotation = b.transform.rotation;
		transform.position = b.transform.position + ridingOffset;
		transform.parent = b.transform;
	}

	public void UnboardBird (Transform newParent) {
		sr.color = new Color (sr.color.r, sr.color.g, sr.color.b, 1);
		sr.sprite = sprites [0];
		sr.sortingLayerName = newParent.GetComponent<SpriteRenderer> ().sortingLayerName;
		sr.sortingOrder = 2;

		transform.parent = newParent;
		transform.position = pi.station.position;
		b.p = null;
		b.color = Color.black;
		b.GetComponent<SpriteRenderer>().color = Color.black;
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
		sr.sprite = sprites [0];
		GameObject bub = Instantiate (gm.bubblePrefab, transform.position, transform.rotation) as GameObject;
		gm.AddAlliedTransform (bub.transform);
		bub.GetComponent<Bubble> ().p = this;
		bub.GetComponent<Rigidbody2D> ().velocity = initialVelocity;
		transform.parent = bub.transform;
		pi.state = PlayerInput.State.IN_BUBBLE;
	}

	public void CycleWeapons () {
		hol.CycleWeapons ();
	}

	public IEnumerator Reload () {
		Weapon weaponToReload = w;
		w.reloading = true;
		yield return new WaitForSeconds (w.reloadSpeed);
		if (w == weaponToReload) {
			w.roundsLeftInClip = w.clipSize;
		}
		weaponToReload.reloading = false;
	}

	public void CockShotgun () {
		w.readyToFire = true;
	}
}
