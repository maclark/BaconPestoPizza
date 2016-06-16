using UnityEngine;
using System.Collections;

public class WaterSource : MonoBehaviour {

	public float gallons;
	public bool dry = false;
	public bool circular = true;

	private GameManager gm;

	void Awake () {
		gm = GameObject.FindObjectOfType<GameManager> ();
	}

	void Start () {
		if (circular) {
			gallons = (transform.localScale.x / 2) * (transform.localScale.x / 2) * Mathf.PI * gm.gallonsPerSquareUnit;
		}
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerStay2D (Collider2D other) {
		if (other.name == "BigBirdColliders") {
			gm.bbm.AtWaterSource (this);
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (other.name == "BigBirdColliders") {
			gm.bbm.LeftWaterSource (this);
		}
	}

	public float Gulp (float gulpAmount) {
		float gulp = gulpAmount;
		if (gallons - gulpAmount <= 0) {
			gulp = gallons;
			gallons = 0;
			dry = true;
		} else {
			gallons -= gulpAmount;
		}
		AdjustWaterLevel ();
		return gulp;
	}

	void AdjustWaterLevel () {
		float diameter = 2 * Mathf.Sqrt (gallons / (Mathf.PI * gm.gallonsPerSquareUnit));
		transform.localScale = new Vector2 (diameter, diameter);
	}
}
