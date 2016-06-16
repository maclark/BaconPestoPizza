using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public GameObject bubblePrefab;
	public GameObject bigBirdWaterTank;
	public GameObject bigBirdEnergyTank;
	public GameObject navPrefab;
	public GameObject invisibleTarget;
	public GameObject bodyPrefab;
	public GameObject tunnelPrefab;
	public GameObject shopPrefab;
	public GameObject nextPortal;
	public Transform exitContainer;
	public Transform boundaryContainer;
	public Transform bodyContainer;
	public Transform enemyContainer;
	public Transform buildingContainer;
	public Vector3 appointment1 = new Vector3 (100, 0, 0);
	public Vector3 endOfPortalTailOffset = new Vector3 (0, 67.08f, 0);
	public Vector3 lastPortalPosition = new Vector3 (0, 0, 0);
	public List<Transform> appointments = new List<Transform> ();
	public Text goldText;
	public BigBird bigBird;
	public BigBirdManager bbm;
	public int zoneNumber = 0;
	public float screenClampBuffer = 1;
	public float gallonsPerSquareUnit = 14;
	public float warpDelay = 2f;

	private List<Transform> alliedTransforms = new List<Transform> (); 
	private bool paused = false;
	private NavPointer nav;
	private Tetris tetrisGod;

	void Awake () {
		bigBird = GameObject.FindObjectOfType<BigBird> ();
		bbm = bigBird.GetComponent<BigBirdManager> ();
		tetrisGod = GameObject.FindObjectOfType<Tetris> ();
		AddAlliedTransform (bigBird.transform);
		MakeInvisibleTarget ();

		MakeContainers ();
	}

	void Start () {
		goldText.text = bigBird.transform.position.y.ToString();
	}

	void Update () {
		goldText.text = bigBird.transform.position.y.ToString();
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

	public ResourceBar GetBigBirdWaterTank () {
		return bigBirdWaterTank.GetComponentInChildren<ResourceBar> ();
	}

	public ResourceBar GetBigBirdEnergyTank () {
		return bigBirdEnergyTank.GetComponentInChildren<ResourceBar> ();
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

	public Vector3 ClampToScreen (Vector3 position) {
		
		Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(Vector3.zero);
		Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(
			Camera.main.pixelWidth, Camera.main.pixelHeight));

		Rect cameraRect = new Rect (
			bottomLeft.x,
			bottomLeft.y,
			topRight.x - bottomLeft.x,
			topRight.y - bottomLeft.y);
		
		return new Vector3 (
			Mathf.Clamp(position.x, cameraRect.xMin + screenClampBuffer, cameraRect.xMax - screenClampBuffer),
			Mathf.Clamp(position.y, cameraRect.yMin + screenClampBuffer, cameraRect.yMax - screenClampBuffer),
			transform.position.z);
	}

	public void BrokeGate () {
		//spawn new level
		//gatesBroken++;
		//tetrisGod.SpawnRectangleField (new Vector2 (-tetrisGod.width / 2, tetrisGod.height * gatesBroken), tetrisGod.width, tetrisGod.height, tetrisGod.tetroAttempts);
	}

	public PlayerBody GetBody () {
		GameObject obj = Instantiate (bodyPrefab, transform.position, Quaternion.identity) as GameObject;
		obj.SetActive (false);
		obj.transform.parent = bodyContainer;
		PlayerBody pBody = obj.GetComponent<PlayerBody> ();
		return pBody;
	}

	public IEnumerator WarpBigBird (Vector3 warpExit) {
		yield return new WaitForSeconds (warpDelay);
		bigBird.transform.position = warpExit;
	}

	public void SpawnNewZone () {
		zoneNumber++;
		tetrisGod.origin.y += tetrisGod.rows * tetrisGod.roomHeight + 100f;
		tetrisGod.SpawnNewZone ();
	}

	void MakeContainers () {
		exitContainer = new GameObject ().transform;
		exitContainer.transform.name = "ZoneExits";
		exitContainer.transform.parent = transform;
		boundaryContainer = new GameObject ().transform;
		boundaryContainer.transform.name = "BoundaryWalls";
		boundaryContainer.transform.parent = transform;
		bodyContainer = new GameObject ().transform;
		bodyContainer.transform.name = "Bodies";
		bodyContainer.transform.parent = transform;
		enemyContainer = new GameObject ().transform;
		enemyContainer.transform.name = "Enemies";
		enemyContainer.transform.parent = transform;
		buildingContainer = new GameObject ().transform;
		buildingContainer.transform.name = "Buildings";
		buildingContainer.transform.parent = transform;
	}
}
