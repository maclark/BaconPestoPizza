using UnityEngine;
using System.Collections;

public class Neoner : MonoBehaviour {

	public float reticleOffset = 2f;
	public Color color;
	public Vector3 ridingOffset;
	public Kanga kanga = null;
	public Weapon w = null;
	public Weapon storedWeapon = null;
	public Vector3 aim = Vector3.zero;
	public Sprite[] sprites = new Sprite[2];
	public NeonerBody body;
	public Transform reticle;

	private GameManager gm;
	private SpriteRenderer sr;
	private NeonerInput pi;
	private Holster hol;


	void Awake () {
		gm = GameObject.FindObjectOfType<GameManager> ();
		pi = GetComponent<PlayerInput> ();
		sr = GetComponent<SpriteRenderer> ();
		hol = new Holster (this);

	}

	void Start () {
		body = gm.GetNeonerBody (this);
		color = sr.color;
		reticle.GetComponent<SpriteRenderer> ().color = color;
	}

	void Update () {
		reticle.transform.position = transform.position + aim * reticleOffset;
	}

	public void BoardKanga (Kanga k) {
		sr.sprite = sprites [1];
		sr.enabled = true;

		kanga = k;
		kanga.rider = this;
		kanga.color = color;
		kanga.body.GetComponent<SpriteRenderer>().color = kanga.color;
		kanga.Shield.SetColor (color);
		kanga.ReloadIndicator.SetColor (color);
		kanga.transform.rotation = Quaternion.identity;
		kanga.body.rotation = Quaternion.identity;
		transform.rotation = kanga.transform.rotation;
		transform.position = kanga.transform.position + ridingOffset;
		transform.parent = kanga.body;
	}

	public void Dekanga (Transform newParent) {
		sr.color = Color.white;
		sr.sprite = sprites [0];
		sr.sortingLayerName = newParent.GetComponent<SpriteRenderer> ().sortingLayerName;

		transform.parent = newParent;
		transform.position = pi.station.position;

		if (kanga.Shield != null) {
			kanga.Shield.SetColor (new Color (0f, 0f, 0f, .5f));
		}
		kanga.color = Color.black;
		kanga.body.GetComponent<SpriteRenderer>().color = Color.black;
		kanga.transform.rotation = newParent.transform.rotation;
		kanga.rider = null;
		kanga = null;
	}

	public void CycleWeapons () {
		kanga.TurnOffReloadIndicator ();
		hol.CycleWeapons ();
	}

	public void CockWeapon () {
		w.CockWeapon ();
	}

	public void Dock (Dock d) {
		reticle.gameObject.SetActive (false);
		sr.sortingLayerName = "BigBird";
		sr.sortingOrder = 2;
		pi.station = d.transform;
		pi.state = PlayerInput.State.DOCKED;
		pi.CancelInvoke ();
		w.firing = false;
		d.GetComponent<BoxCollider2D> ().enabled = false;
	}

	public void Undock () {
		reticle.gameObject.SetActive (true);
		aim = Vector3.right;
		reticle.transform.position = transform.position + aim * reticleOffset;
		sr.sortingLayerName = "Birds";
		sr.sortingOrder = 1;
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
	}

	public void SpiritAway (Transform master, PlayerInput.State s) {
		transform.parent = master;
		body.gameObject.SetActive (false);
		sr.sortingLayerName = "BigBird";
		sr.sortingOrder = 1;
		pi.state = s;
	}

	public void Disembark (Vector3 disembarkPoint) {
		pi.AbandonStation ();
		sr.enabled = true;
		ManifestFlesh (disembarkPoint, "Buildings", 2);
		pi.state = PlayerInput.State.ON_FOOT;
	}
}
