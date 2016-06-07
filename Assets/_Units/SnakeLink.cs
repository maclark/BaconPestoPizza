using UnityEngine;
using System.Collections;

public class SnakeLink : MonoBehaviour {
	public SnakeLink bigBrother;
	public SnakeLink littleBrother;
	public SpriteRenderer sr;
	public Rigidbody2D rb;
	public BoxCollider2D bc;
	public Vector3 lastPosition;
	public bool isHead = false;
	public int initialLinks = 10;
	public int linksToSpawn;
	public Sprite square;
	public GameObject link;
	public GameObject linkPrefab;
	public Vector3 stepVector;
	public float lastStepTime;
	public float stepTime = .5f;
	public bool spawnOnStep;

	private static bool initializedLink = false;
	private static int num;

	void Awake () {

	}

	void Start () {
		if (!initializedLink) {
			InitializeLink ();
		}

		if (isHead) {
			Spawn (null, null, square, initialLinks - 1);
		}

		if (isHead) {
		}
	}

	void Update () {
		if (!isHead && bigBrother == null) { 
			isHead = true;
		}

		if (Time.time - lastStepTime > stepTime) {
			if (isHead) {
				Lead ();
			}				
		}
	}

	void InitializeLink () {
		link = new GameObject ();
		link.AddComponent<SpriteRenderer> ();
		link.AddComponent<Rigidbody2D> ();
		link.AddComponent<BoxCollider2D> ();
		link.AddComponent<SnakeLink> ();
		link.SetActive (false);
		initializedLink = true;
	}

	void SpawnLilBro () {
		GameObject newLink = Instantiate (linkPrefab, transform.position, Quaternion.identity) as GameObject;
		SnakeLink slink = newLink.GetComponent <SnakeLink> ();
		slink.isHead = false;
		littleBrother = slink;
		slink.Spawn (this, null, square, linksToSpawn);
	}

	public void Spawn (SnakeLink bigBro, SnakeLink lilBro, Sprite scale, int linksLeftToSpawn) {
		num++;
		gameObject.transform.name = "snake" + num.ToString ();

		gameObject.SetActive (true);
		bigBrother = bigBro;
		littleBrother = lilBro;

		sr = GetComponent<SpriteRenderer> ();
		rb = GetComponent<Rigidbody2D> ();
		bc = GetComponent<BoxCollider2D> ();
		stepVector = transform.up;

		sr.sprite = scale;
		//rb.isKinematic = true;
		//bc.isTrigger = true;
		linksToSpawn = linksLeftToSpawn - 1;
		spawnOnStep = linksToSpawn > 0 ? true : false; 
	}

	public void Lead () {
		lastStepTime = Time.time;
		int turn = Random.Range (0, 10);
		if (turn < 8) {
		} else if (turn == 8) {
			if (stepVector == transform.up) {
				stepVector = transform.right;
			} else if (stepVector == transform.right) {
				stepVector = -transform.up;
			} else if (stepVector == -transform.up) {
				stepVector = -transform.right;
			} else if (stepVector == -transform.right) {
				stepVector = transform.up;
			}
		} else {
			if (stepVector == transform.up) {
				stepVector = -transform.right;
			} else if (stepVector == transform.right) {
				stepVector = transform.up;
			} else if (stepVector == -transform.up) {
				stepVector = transform.right;
			} else if (stepVector == -transform.right) {
				stepVector = -transform.up;
			}
		}
		lastPosition = transform.position;
		transform.position += stepVector;
		if (littleBrother) {
			littleBrother.Follow ();
		} else if (spawnOnStep) {
			SpawnLilBro ();
		}
	}

	public void Follow () {
		lastPosition = transform.position;
		transform.position = bigBrother.lastPosition;
		if (littleBrother) {
			littleBrother.Follow ();
		} else if (spawnOnStep) {
			SpawnLilBro ();
		}
	}

	public void LostBigBrother () {
		bigBrother = bigBrother.bigBrother;
		Follow ();
	}

	public void Die () {
		if (littleBrother) {
			littleBrother.LostBigBrother ();
		}
		Destroy (gameObject);
	}
}
