using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public BigBird bigBird;
	//public Player player1;
	//private Player player2;
	//private Player player3;
	//private Player player4;
	public bool inNavigation = false;
	public GameObject navPointerPrefab;
	public Text goldText;

	private List<Transform> alliedTransforms; 
	private bool paused = false;
	private GameObject navPointer;

	// Use this for initialization
	void Awake () {
		bigBird = GameObject.FindGameObjectWithTag ("BigBird").GetComponent<BigBird> ();
		alliedTransforms = new List<Transform> ();
		AddAlliedTransform (bigBird.transform);
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
			navPointer = Instantiate (navPointerPrefab, bigBird.transform.position, Quaternion.identity) as GameObject; 
			navPointer.GetComponent<NavPointer> ().SetAxes (leftHorizontal, leftVertical);
		} else {
			inNavigation = false;
			Destroy (navPointer);
		}
	}
	/*
	public void Link (Player playerA, Player playerB) {
		foreach (Player p in linkedPlayers) {
			if (p.name == playerA.name) {
			} else if (p.name == playerB.name) {
			}
		}
	}*/
}
