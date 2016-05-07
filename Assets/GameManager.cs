using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public BigBird bigBird;
	public Player player1;
	//private Player player2;
	//private Player player3;
	//private Player player4;
	public bool inNavigation = false;
	public GameObject navPointerPrefab;

	private List<Transform> alliedTransforms; 
	private bool paused = false;
	private GameObject navPointer;


	// Use this for initialization
	void Awake () {
		bigBird = GameObject.FindGameObjectWithTag ("BigBird").GetComponent<BigBird> ();
		player1 = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();
		alliedTransforms = new List<Transform> ();
		AddAlliedTransform (bigBird.transform);
		AddAlliedTransform (player1.transform);
	}

	void Start () {

	}

	// Update is called once per frame
	void Update () {
	}

	public List<Transform> GetAlliedTransforms () {
		return alliedTransforms;
	}

	public void RemoveAlliedTransform (Transform toRemove) {
		alliedTransforms.Remove (toRemove);
	}

	public void AddAlliedTransform (Transform toAdd) {
		alliedTransforms.Add (toAdd);
	}
	
	public void TogglePause () {
		paused = !paused;
		if (paused) {
			Time.timeScale = 0.0f;
			RenderSettings.skybox.color = Color.blue;
		} else {
			Time.timeScale = 1f;
			RenderSettings.skybox.color = Color.white;
		}
	}

	public void ToggleNavPanel (string leftHorizontal, string leftVertical) {
		if (!inNavigation) {
			inNavigation = true;
			print ("bigBird.transform.position " + bigBird.transform.position);
			navPointer = Instantiate (navPointerPrefab, bigBird.transform.position, Quaternion.identity) as GameObject; 
			navPointer.GetComponent<NavPointer> ().SetAxes (leftHorizontal, leftVertical);
		} else {
			inNavigation = false;
			Destroy (navPointer);
		}
	}
}
