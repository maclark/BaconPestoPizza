using UnityEngine;
using System.Collections.Generic;

public class Harpoon : MonoBehaviour {

	public Vector2 direction = Vector2.zero;
	public float moveForce = 100f;
	public float minWidth = .05f;
	public float maxWidth = .2f;
	public float minWidthTetherLength = 3;
	public float tetherMaxLength = 10f;
	public GameObject prefabChainLink;
	public GameObject harpooner = null;
	public GameObject harpooned = null;

	private Rigidbody2D rb;
	private bool tetherTaught = false;
	private Vector3[] tetherPositions = new Vector3[2];

	void Awake () {
		rb = GetComponent<Rigidbody2D> ();
	}

	void Update () {
		DrawTether ();
	}

	public void SetHarpooner (GameObject harpoonThrower) {
		harpooner = harpoonThrower;
	}

	public void SetHarpooned (GameObject harpoonRecipient) {
		harpooned = harpoonRecipient;
		transform.parent = harpooned.transform;
		GetComponent<Rigidbody2D> ().Sleep ();
		GetComponent<BoxCollider2D> ().enabled = false;
		SpriteRenderer[] harpoonSprites = GetComponentsInChildren<SpriteRenderer> ();
		for (int i = 0; i < harpoonSprites.Length; i++) {
			harpoonSprites [i].enabled = false;
		}
	}

	public void SetDirection (Vector2 dir) {
		direction = dir;
		direction.Normalize ();
	}

	public void Fire (Transform start, Vector2 aim) {
		transform.position = start.position;
		transform.rotation = start.rotation;
		SetDirection (aim);
		rb.AddForce (direction * moveForce);
	}

	public void Die () {
		Destroy (gameObject);
	}

	void DrawTether () {

		LineRenderer lr = GetComponent<LineRenderer> ();

		tetherPositions [0] = harpooner.transform.position;
		if (harpooned != null) {
			tetherPositions [1] = harpooned.transform.position;
		} else {
			tetherPositions [1] = transform.position;
		}
		lr.SetPositions (tetherPositions);

		float distance = Vector3.Distance (tetherPositions[0], tetherPositions[1]);
		float tetherWidth = Mathf.Lerp (maxWidth, minWidth, (distance - minWidthTetherLength )/ tetherMaxLength);
		lr.SetWidth (tetherWidth, tetherWidth);

		if (distance < minWidthTetherLength) {
			print ("make it blue");
			tetherTaught = false;
			lr.material.color = Color.blue;

		} else if (distance < tetherMaxLength + minWidthTetherLength) {
			tetherTaught = false;
			print ("make it gray");

			lr.material.color = Color.gray;
		}
		else {
			print ("make it red");

			lr.material.color = Color.red;
			//instantiate chain at midoint #TODO
			/*invisibleChain.A = harpooner;
			invisibleChain.B = harpooned;
			List<GameObject> prefabList = new List<GameObject> ();
			prefabList.Add (prefabChainLink);
			invisibleChain.getPrefabList = prefabList;
			*/tetherTaught = true;
		} 
	}
}
