using UnityEngine;
using System.Collections;

public class OrbWeb : MonoBehaviour {
	public Color color = Color.white;
	public Material stringMaterial;
	public float stringWidth = .2f;
	public float orbRadius = 1f;

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
		print ("webbed " + other.name);
	}

	public void SetWebbers (Transform[] webThrowers) {
		print ("set webbers");
		webbers = new Transform [webThrowers.Length];
		lrs = new LineRenderer [webThrowers.Length];

		GetComponentInChildren<CircleCollider2D> ().radius = orbRadius * webbers.Length;
		transform.localScale *= webbers.Length;

		for (int i = 0; i < webThrowers.Length; i++) {
			webbers [i] = webThrowers [i];
			GameObject webString = new GameObject ("WebString");
			webString.AddComponent<LineRenderer> ();
			webString.transform.parent = transform;
			LineRenderer lr = webString.GetComponent<LineRenderer> ();
			lr.material = stringMaterial;
			lr.SetColors (color, color);
			lr.SetWidth (stringWidth, stringWidth);
			lrs [i] = webString.GetComponent<LineRenderer> ();
		}

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
}
