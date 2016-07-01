using UnityEngine;
using System.Collections;

public class OffScreenIndicator : MonoBehaviour {

	public Transform target;
	public Transform arrow;
	public float speed;
	public float arrowRadius;
	public float buffer = 5f;

	private GameManager gm;

	void Awake () {
		gm = GameObject.FindObjectOfType<GameManager> ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 direction = target.transform.position - transform.position;
		direction.Normalize ();
		transform.Translate (direction * speed * Time.deltaTime);
		transform.position = gm.ClampToScreen (transform.position, buffer);
		arrow.transform.position = transform.position + direction * arrowRadius;
		arrow.rotation = Quaternion.LookRotation (transform.forward, direction);
	}
}
