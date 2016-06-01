using UnityEngine;
using System.Collections;

public class NavPointer : MonoBehaviour {

	public float navPointSpeed;
	public Transform target;

	private float horizontalMotion;
	private float verticalMotion;
	private float realTimeOfLastUpdate = 0f;
	private Vector2 direction = Vector2.zero;
	private GameManager gm;
	private Vector3[] positions = new Vector3[2];
	private LineRenderer lr;
	private SpriteRenderer sr;

	void Awake () {
		gm = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
		lr = GetComponent<LineRenderer> ();
		sr = GetComponent<SpriteRenderer> ();
	}

	// Use this for initialization
	void Start () {
		realTimeOfLastUpdate = Time.realtimeSinceStartup;
		lr.sortingLayerName = "Projectiles";
	}

	
	// Update is called once per frame
	void Update () {
		float deltaTime = Time.realtimeSinceStartup - realTimeOfLastUpdate;
		realTimeOfLastUpdate = Time.realtimeSinceStartup;
		direction = new Vector2 (horizontalMotion, verticalMotion);
		direction.Normalize ();
		transform.Translate (direction * navPointSpeed * deltaTime);
		DrawNavLine ();
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag != "EnemyBullet") {
			if (other.tag != "BigBird") {
				if (other.transform.parent) {
					if (other.transform.parent.tag != "BigBird") {
						sr.color = Color.green;
						target = other.transform;
					}
				} else {
					sr.color = Color.green;
					target = other.transform;
				}
			} 
		}
		target = null;
	}

	void OnTriggerExit2D (Collider2D other) {
		if (other.tag != "EnemyBullet") {
			sr.color = Color.blue;
			target = null;
		}
	}

	public void SetAxes (float leftHorizontal, float leftVertical) {
		horizontalMotion = leftHorizontal;
		verticalMotion = leftVertical;
	}

	void DrawNavLine () {
		positions [0] = gm.bigBird.transform.position;
		positions [1] = transform.position;
		GetComponent<LineRenderer> ().SetPositions (positions);
	}
}
