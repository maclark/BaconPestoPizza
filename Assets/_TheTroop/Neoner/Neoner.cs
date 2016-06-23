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
	private NeonerInput ni;


	void Awake () {
		gm = GameObject.FindObjectOfType<GameManager> ();
		ni = GetComponent<NeonerInput> ();
		sr = GetComponent<SpriteRenderer> ();
	}

	void Start () {
		body = gm.GetNeonerBody (this);
		kanga = GetComponentInParent<Kanga> ();
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
		transform.position = ni.station.position;

		if (kanga.Shield != null) {
			kanga.Shield.SetColor (new Color (0f, 0f, 0f, .5f));
		}
		kanga.color = Color.black;
		kanga.body.GetComponent<SpriteRenderer>().color = Color.black;
		kanga.transform.rotation = newParent.transform.rotation;
		kanga.rider = null;
		kanga = null;
	}

	public void Dock (Dock d) {
		reticle.gameObject.SetActive (false);
		sr.sortingLayerName = "BigBird";
		sr.sortingOrder = 2;
		ni.station = d.transform;
		ni.state = NeonerInput.State.DOCKED;
		ni.CancelInvoke ();
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

	public void SpiritAway (Transform master, NeonerInput.State s) {
		transform.parent = master;
		body.gameObject.SetActive (false);
		sr.sortingLayerName = "BigBird";
		sr.sortingOrder = 1;
		ni.state = s;
	}

	public void Disembark (Vector3 disembarkPoint) {
		ni.AbandonStation ();
		sr.enabled = true;
		ManifestFlesh (disembarkPoint, "Buildings", 2);
		ni.state = NeonerInput.State.ON_FOOT;
	}
}
