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
	public GameObject neonBodyPrefab;
	public GameObject tunnelPrefab;
	public GameObject shopPrefab;
	public GameObject castlePrefab;
	public GameObject nextPortal;
	public GameObject poolerObject;
	public GameObject positronPrefab;
	public GameObject dropPrefab;
	public GameObject sunPrefab;
	public GameObject flamePrefab;
	public GameObject boundaryContainer;
	public GameObject bodyContainer;
	public GameObject enemyContainer;
	public GameObject buildingContainer;
	public GameObject positronContainer;
	public GameObject dropContainer;
	public GameObject sunContainer;
	public GameObject flameContainer;
	public GameObject exitContainer;
	public int zoneNumber = 0;
	public float screenClampBuffer = 1;
	public float gallonsPerSquareUnit = 14;
	public float warpDelay = 2f;public Vector3 appointment1 = new Vector3 (100, 0, 0);
	public Player captain;
	public Text goldText;
	public ObjectPooler positronPooler;
	public ObjectPooler dropPooler;
	public ObjectPooler sunPooler;
	public ObjectPooler flamePooler;
	public BigBird bigBird;
	public BigBirdManager bbm;
	public Vector3 endOfPortalTailOffset = new Vector3 (0, 67.08f, 0);
	public Vector3 lastPortalPosition = new Vector3 (0, 0, 0);
	public List<Transform> appointments = new List<Transform> ();
	public Color[] playerColors = new Color[8];
	public List<Bird> birds = new List<Bird> (); 

	private List<Transform> alliedTransforms = new List<Transform> (); 
	private bool paused = false;
	private NavPointer nav;
	private Tetris tetrisGod;
	private SunMaker sunGod;


	void Awake () {
		bigBird = GameObject.FindObjectOfType<BigBird> ();
		bbm = bigBird.GetComponent<BigBirdManager> ();
		bbm.money = 500;
		tetrisGod = GameObject.FindObjectOfType<Tetris> ();
		sunGod = new SunMaker ();
		MakeInvisibleTarget ();
		MakeContainers ();
		SetPlayerColors ();
	}

	void Start () {
		Pool (ref positronPooler, positronPrefab, positronContainer, Color.white);
		Pool (ref dropPooler, dropPrefab, dropContainer, Color.blue);
		Pool (ref sunPooler, sunPrefab, sunContainer, Color.yellow);
		Pool (ref flamePooler, flamePrefab, flameContainer, Color.white);

		InvokeRepeating ("MakeSuns", 0f, sunGod.sunFrequency);
	}

	void Pool (ref ObjectPooler pooler, GameObject prefab, GameObject container, Color c) {
		GameObject poolerObj = Instantiate (poolerObject) as GameObject;
		pooler = poolerObj.GetComponent<ObjectPooler> ();
		pooler.SetPooledObject (prefab);
		pooler.SetPooledObjectsColor (c);
		pooler.SetContainer (container);
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

	public Vector3 ClampToScreen (Vector3 position, float buffer) {
		
		Vector3 bottomLeft = Camera.main.ScreenToWorldPoint (Vector3.zero);
		Vector3 topRight = Camera.main.ScreenToWorldPoint (new Vector3(
			Camera.main.pixelWidth, Camera.main.pixelHeight));

		Rect cameraRect = new Rect (
			bottomLeft.x,
			bottomLeft.y,
			topRight.x - bottomLeft.x,
			topRight.y - bottomLeft.y);
		
		return new Vector3 (
			Mathf.Clamp(position.x, cameraRect.xMin + buffer, cameraRect.xMax - buffer),
			Mathf.Clamp(position.y, cameraRect.yMin + buffer, cameraRect.yMax - buffer),
			transform.position.z);
	}

	public void BrokeGate () {
		//spawn new level
		//gatesBroken++;
		//tetrisGod.SpawnRectangleField (new Vector2 (-tetrisGod.width / 2, tetrisGod.height * gatesBroken), tetrisGod.width, tetrisGod.height, tetrisGod.tetroAttempts);
	}

	public PlayerBody GetBody (Player p) {
		GameObject obj = Instantiate (bodyPrefab, transform.position, Quaternion.identity) as GameObject;
		obj.SetActive (false);
		obj.transform.parent = bodyContainer.transform;
		PlayerBody pBody = obj.GetComponent<PlayerBody> ();
		pBody.SetPlayer (p);
		return pBody;
	}

	public NeonerBody GetNeonerBody (Neoner n) {
		GameObject obj = Instantiate (neonBodyPrefab, transform.position, Quaternion.identity) as GameObject;
		obj.SetActive (false);
		obj.transform.parent = bodyContainer.transform;
		NeonerBody nBody = obj.GetComponent<NeonerBody> ();
		//nBody.SetNeoner (n);
		return nBody;
	}

	public IEnumerator WarpBigBird (Vector3 warpExit) {
		foreach (Transform t in alliedTransforms) {
			Bird b = t.GetComponentInChildren<Bird> ();
			if (b) {
				print ("t has b, docking it");
				b.DockOnBigBird ();
			} else {
				Bird b2 = t.GetComponentInParent<Bird>();
				if (b2) {
					print ("t's parent has bird");
					b2.DockOnBigBird ();
				} else {
					Bubble bub = t.GetComponent<Bubble> ();
					if (bub) {
						print ("p is bubbed");
						bub.p.BoardBigBird ();
						Destroy (bub.gameObject);
					} else {
						Player p = t.GetComponentInChildren<Player> ();
						if (p) {
							print ("p doesn't have b, board it");
							p.BoardBigBird ();
						} 
					}
				}
			}
		}
		yield return new WaitForSeconds (warpDelay);
		bigBird.transform.position = warpExit;
	}

	public void SpawnNewZone () {
		zoneNumber++;
		tetrisGod.origin.y += tetrisGod.rows * tetrisGod.roomHeight + 100f;
		tetrisGod.SpawnNewZone ();
	}

	void MakeSuns () {
		sunGod.MakeSunshine ();
	}

	void MakeContainers () {
		exitContainer = new GameObject ();
		exitContainer.transform.name = "exitContainer";
		exitContainer.transform.parent = transform;

		boundaryContainer = new GameObject ();
		boundaryContainer.transform.name = "boundaryContainer";
		boundaryContainer.transform.parent = transform;

		bodyContainer = new GameObject ();
		bodyContainer.transform.name = "bodyContainer";
		bodyContainer.transform.parent = transform;

		enemyContainer = new GameObject ();
		enemyContainer.transform.name = "enemyContainer";
		enemyContainer.transform.parent = transform;

		buildingContainer = new GameObject ();
		buildingContainer.transform.name = "buildingContainer";
		buildingContainer.transform.parent = transform;

		positronContainer = new GameObject ();
		positronContainer.transform.name = "positronContainer";
		positronContainer.transform.parent = transform;

		dropContainer = new GameObject ();
		dropContainer.transform.name = "dropContainer";
		dropContainer.transform.parent = transform;

		flameContainer = new GameObject ();
		flameContainer.transform.name = "flameContainer";
		flameContainer.transform.parent = transform;
	}

	void SetPlayerColors () {
		playerColors[0] = new Color (234f/255, 100f/255, 100f/255);
		playerColors[1] = new Color (96f/255, 103f/255, 255f/255);
		playerColors[2] = new Color (50f/255, 187f/255, 206f/255);
		playerColors[3] = new Color (255f/255, 180f/255, 253f/255);
		playerColors[4] = new Color (34f/255, 114f/255, 34f/255);
		playerColors[5] = new Color (216f/255, 218f/255, 130f/255);
		playerColors[6] = new Color (254f/255, 159f/255, 113f/255);
		playerColors[7] = new Color (255f/255, 255f/255, 255f/255);
		/*

		playerColors[0] = Color.FromRgb (234f, 100f, 100f);
		playerColors[1] = Color.FromRgb (96f, 103f, 255f);
		playerColors[2] = Color.FromRgb (50f, 187f, 206f);
		playerColors[3] = Color.FromRgb (255f, 180f, 253f);
		playerColors[4] = Color.FromRgb (34f, 114f, 34f);
		playerColors[5] = Color.FromRgb (216f, 218f, 130f);
		playerColors[6] = Color.FromRgb (254f, 159f, 113f);
		playerColors[7] = Color.FromRgb (255f, 255f, 255f);

		
		playerColors [0] = Color.red;
		playerColors[1] = Color.blue;
		playerColors [2] = Color.green;
		playerColors[3] = Color.white;
		playerColors[4] = Color.grey;
		playerColors[5] = Color.cyan;
		playerColors[6] = Color.magenta;
		playerColors[7] = Color.clear;
		*/
	}
}
