using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public Vector3 ridingOffset;
	public Bird b = null;
	public bool navigating = false;
	public Sprite[] sprites = new Sprite[2];

	private GameManager gm;
	private SpriteRenderer sr;
	private PlayerInput pi;
	private BigBird bigBird;

	void Awake () {
		gm = GameObject.FindObjectOfType<GameManager> ();
		pi = GetComponent<PlayerInput> ();
		sr = GetComponent<SpriteRenderer> ();
		bigBird = GameObject.FindObjectOfType<BigBird> ();
	}

	public void StartPlayer () {
		sr.enabled = true;
		pi.state = PlayerInput.State.neutral;
	}

	public void BoardBird (Bird bird) {
		sr.sprite = sprites [1];
		sr.enabled = true;
		sr.sortingLayerName = "Birds";
		sr.sortingOrder = 1;

		b = bird;
		b.p = this;
		b.color = GetComponent<SpriteRenderer> ().color;
		b.GetComponent<SpriteRenderer>().color = b.color;
		b.GetComponent<ObjectPooler> ().enabled = true;
		b.GetComponent<ObjectPooler> ().SetPooledObjectsColor (b.color);
		transform.position = b.transform.position + ridingOffset;
		transform.parent = b.transform;
	}

	public void UnboardBird (Transform newParent) {
		sr.sprite = sprites [0];
		sr.sortingLayerName = newParent.GetComponent<SpriteRenderer> ().sortingLayerName;
		sr.sortingOrder = 1;

		transform.parent = newParent;
		b.p = null;
		b.color = Color.black;
		b.GetComponent<SpriteRenderer>().color = Color.black;
		b = null;
	}

	public void BoardBigBird () {
		sr.sortingLayerName = "BigBird";
		sr.sortingOrder = 1;

		transform.position = bigBird.transform.position;
		transform.parent = bigBird.transform;

		pi.state = PlayerInput.State.neutral;
	}

	public void Bubble (Vector3 initialVelocity) {
		sr.sprite = sprites [0];
		GameObject bub = Instantiate (gm.bubblePrefab, transform.position, transform.rotation) as GameObject;
		gm.AddAlliedTransform (bub.transform);
		bub.GetComponent<Bubble> ().p = this;
		bub.GetComponent<Rigidbody2D> ().velocity = initialVelocity;
		transform.parent = bub.transform;
		pi.state = PlayerInput.State.inBubble;
	}
}
