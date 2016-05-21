using UnityEngine;
using System.Collections;

public class NavPointer : MonoBehaviour {

	public float navPointSpeed;

	private string LSHorizontal;
	private string LSVertical;
	private Vector2 direction = Vector2.zero;
	private float realTimeOfLastUpdate = 0f;
	private GameManager gm;
	private Vector3[] positions = new Vector3[2];

	void Awake () {
		gm = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
	}

	// Use this for initialization
	void Start () {
		realTimeOfLastUpdate = Time.realtimeSinceStartup;
	}

	
	// Update is called once per frame
	void Update () {
		float deltaTime = Time.realtimeSinceStartup - realTimeOfLastUpdate;
		realTimeOfLastUpdate = Time.realtimeSinceStartup;
		direction = new Vector2( Input.GetAxis(LSHorizontal), Input.GetAxis(LSVertical));
		direction.Normalize ();
		transform.Translate (direction * navPointSpeed * deltaTime);
		DrawNavLine ();
	}

	public void SetAxes (string leftHorizontal, string leftVertical) {
		LSHorizontal = leftHorizontal;
		LSVertical = leftVertical;
	}

	void DrawNavLine () {
		positions [0] = gm.bigBird.transform.position;
		positions [1] = transform.position;
		GetComponent<LineRenderer> ().SetPositions (positions);
	}
}
