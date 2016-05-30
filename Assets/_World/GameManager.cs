using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public GameObject bubblePrefab;
	public BigBird bigBird;
	public Vector3 birdScale = new Vector3 (1.25f, 1.25f, 1);
	public Vector3 appointment1 = new Vector3 (100, 0, 0);
	public bool inNavigation = false;
	public GameObject navPointerPrefab;
	public Text goldText;
	public List<Transform> appointments = new List<Transform> ();

	private List<Transform> alliedTransforms = new List<Transform> (); 
	private bool paused = false;
	private GameObject navPointer;

	void Awake () {
		bigBird = GameObject.FindGameObjectWithTag ("BigBird").GetComponent<BigBird> ();
		AddAlliedTransform (bigBird.transform);
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

	/// <summary>
	/// Inadvisable to have an appointment at the origin.
	/// </summary>
	/// <returns>The appointment.</returns>
	public Transform GetNextAppointment () {
		if (appointments.Count > 0) {
			Transform a = appointments [0];
			appointments.Remove (a);
			return a;
		} else
			return null;
	}
}
