using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public GameObject bubblePrefab;
	public BigBird bigBird;
	public GameObject bigBirdHealthBar;
	public GameObject navPrefab;
	public GameObject invisibleTarget;
	public Vector3 birdScale = new Vector3 (1.25f, 1.25f, 1);
	public Vector3 appointment1 = new Vector3 (100, 0, 0);
	public Text goldText;
	public List<Transform> appointments = new List<Transform> ();

	private List<Transform> alliedTransforms = new List<Transform> (); 
	private bool paused = false;
	private NavPointer nav;

	void Awake () {
		bigBird = GameObject.FindGameObjectWithTag ("BigBird").GetComponent<BigBird> ();
		AddAlliedTransform (bigBird.transform);
		MakeInvisibleTarget ();
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

	public HealthBar GetBigBirdHealthBar () {
		return bigBirdHealthBar.GetComponent<HealthBar> ();
	}

	public void Navigate (float leftHorizontal, float leftVertical) {
		if (nav == null) {
			GameObject navObj = GameObject.Instantiate(navPrefab, bigBird.transform.position, Quaternion.identity) as GameObject;
			nav = navObj.GetComponent<NavPointer> ();
		}
		nav.SetAxes (leftHorizontal, leftVertical);
	}

	public void StopNavigating () {
		bigBird.SetTarget (nav);
		nav.SetAxes (0, 0);
		Destroy (nav.gameObject);
	}

	public void MakeInvisibleTarget () {
		invisibleTarget = new GameObject ();
		invisibleTarget.AddComponent<BoxCollider2D> ();
		invisibleTarget.GetComponent<BoxCollider2D> ().isTrigger = true;
		invisibleTarget.name = "InvisibleTarget";
	}
}
