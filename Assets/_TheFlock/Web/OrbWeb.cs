using UnityEngine;
using System.Collections;

public class OrbWeb : MonoBehaviour {
	public Color color = Color.white;
	public Material stringMaterial;
	public Transform captive;
	public float stringWidth = .2f;
	public float orbRadius = 1f;

	public enum WebType {TURRET, NET, POWERBIRD}
	public WebType webType = WebType.POWERBIRD;

	private Transform[] webbers;
	private LineRenderer[] lrs;

	void Awake () {
		lrs = GetComponentsInChildren<LineRenderer> (true);
	}
	
	void Update () {
		transform.position = CalculateCenterOfWeb ();
		DrawStrings ();
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "Player" && !CheckIfWebber (other.transform)) {
			other.transform.GetComponent<Bird> ().Webbed (this);
		}
	}

	public void SetWebbers (Transform[] webThrowers) {
		webbers = new Transform [webThrowers.Length];
		lrs = new LineRenderer [webThrowers.Length];

		transform.localScale *= webbers.Length;
		//GetComponentInChildren<CircleCollider2D> ().radius = orbRadius * webbers.Length;
		transform.localScale *= webbers.Length;

		for (int i = 0; i < webThrowers.Length; i++) {
			webbers [i] = webThrowers [i];
			Bird birdie = webbers [i].GetComponent<Bird> ();
			birdie.p.HoldWebString (this);
			birdie.harp.web = this;

			GameObject webString = new GameObject ("WebString");
			webString.AddComponent<LineRenderer> ();
			webString.transform.parent = transform;

			SpriteRenderer sr = GetComponentInChildren<SpriteRenderer> ();
			sr.color = color;

			LineRenderer lr = webString.GetComponent<LineRenderer> ();
			lr.material = stringMaterial;
			lr.SetColors (color, color);
			lr.SetWidth (stringWidth, stringWidth);
			lrs [i] = webString.GetComponent<LineRenderer> ();
		}

	}

	public void Break () {
		if (captive) {
			captive.SendMessage ("Unwebbed", SendMessageOptions.DontRequireReceiver);
		}

		for (int i = 0; i < webbers.Length; i++) {
			Bird birdie = webbers [i].GetComponent<Bird> ();
			birdie.p.ReleaseWebString ();
			birdie.harp.web = null;
			birdie.harp.DetachAndRecall ();
		}
		GetComponentInChildren<Transform> ().parent = null;
		Destroy (gameObject);
	}

	Vector3 CalculateCenterOfWeb () {
		float x = 0;
		float y = 0;
		for (int i = 0; i < webbers.Length; i++) {
			x += webbers [i].transform.position.x;
			y += webbers [i].transform.position.y;
		}
		x = x / webbers.Length;
		y = y / webbers.Length;
		return new Vector3 (x, y, 0);
	}

	void DrawStrings () {
		Vector3[] stringEnds = new Vector3[2];
		stringEnds [0] = transform.position;
		for (int i = 0; i < lrs.Length; i++) {
			stringEnds [1] = webbers [i].transform.position;
			lrs [i].SetPositions (stringEnds);
		}
	}

	bool CheckIfWebber (Transform other) {
		for (int i = 0; i < webbers.Length; i++) {
			if (webbers [i] == other) {
				return true;
			}
		}
		return false;
	}

	//TODO make differentee from break?
	public void Consumed () {
		Break ();
	}
}
