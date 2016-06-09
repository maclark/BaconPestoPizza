using UnityEngine;
using System.Collections;

public class Pump : MonoBehaviour {
	public Sprite pulledSprite;
	public Sprite coiledSprite;

	private LineRenderer lr;
	private SpriteRenderer sr;
	private SpriteRenderer srCoil;
	private bool recoil = false;
	private bool moveToBird = false;
	private Vector3 birdPos = Vector3.zero;
	private Vector3[] positions = new Vector3[2];

	void Awake () {
		lr = GetComponent<LineRenderer> ();	
		sr = GetComponent<SpriteRenderer> ();
		srCoil = transform.parent.GetComponent<SpriteRenderer> ();	
	}

	void Start () {
		lr.sortingLayerName = srCoil.sortingLayerName;
		lr.sortingOrder = srCoil.sortingOrder;
		sr.enabled = false;
		srCoil.sprite = coiledSprite;
	}
	
	void Update () {
		//TODO this has a lot of needless functions that i made during debugging and could be streamlined.
		//also, this script could be enabled/disabled when bigbird.gasline.count > 0.
		//would have to adjust how the birdGettingGas variable works however.
		if (moveToBird) {
			transform.position = birdPos;
			srCoil.sprite = pulledSprite;
			sr.enabled = true;
			moveToBird = false;
		}
		else if (recoil) {
			transform.position = transform.parent.position;
			sr.enabled = false;
			recoil = false;
		}
		DrawPumpHose ();
	}

	void DrawPumpHose () {
		positions [0] = transform.position;
		positions [1] = transform.parent.position;
		lr.SetPositions (positions);
	}

	public void RecoilHose () {
		srCoil.sprite = coiledSprite;
		recoil = true;
	}

	public void MoveToBirdPos (Vector3 pos) {
		moveToBird = true;
		birdPos = pos;
	}
}
