using UnityEngine;
using System.Collections;

public class CargoHold : MonoBehaviour {
	public GameObject cargoPrefab;
	public Cargo[,] cargo = new Cargo[3, 3];
	public Cargo platformCargo;
	public Transform loadingPlatform;
	public float columnWidth = 1f;
	public float rowWidth = 1.2f;
	public float holdWidthOffset = -1f;
	public float holdHeightOffset = -1.2f;
	public float dumpForce = 100f;
	public float torqueSkew = 10f;
	public int xSelector;
	public int ySelector;

	private SpriteRenderer coverRenderer;
	private GameManager gm;
	private bool full = false;


	void Awake () {
		gm = GameObject.FindObjectOfType<GameManager> ();
		coverRenderer = GetComponentInChildren <SpriteRenderer> ();
	}

	// Use this for initialization
	void Start () {
		CreateCargo ();
		CreateCargo ();
		CreateCargo ();
	}

	public void GrabSwapOrStoreCargo () {
		if (platformCargo == null) {
			if (cargo [xSelector, ySelector] != null) {
				//Grab
				MoveToLoadingPlatform (cargo [xSelector, ySelector].transform);
				cargo [xSelector, ySelector] = null;
				full = false;
			} else {
			}
		} else {
			if (cargo [xSelector, ySelector] != null) {
				//Swap
				Cargo c = cargo [xSelector, ySelector];
				MoveToCompartment (platformCargo.transform, xSelector, ySelector);
				cargo [xSelector, ySelector] = platformCargo;
				MoveToLoadingPlatform (c.transform);
			} else {
				//Store
				MoveToCompartment (platformCargo.transform, xSelector, ySelector);
				cargo [xSelector, ySelector] = platformCargo;
				platformCargo = null;
				CheckForFull ();
			}
		}
	}

	void MoveToCompartment (Transform t, int columnIndex, int rowIndex) {
		Vector3 rightOffset = transform.right * (holdWidthOffset + columnIndex * columnWidth);
		Vector3 upOffset = transform.up * (holdHeightOffset + rowIndex * rowWidth);
		t.position = transform.position + rightOffset + upOffset;
	}

	public void MoveSelector (Transform t, int column, int row) {
		MoveToCompartment (t, column, row);
		xSelector = column;
		ySelector = row;
	}

	public void SelectorStep (Transform t, int xStep, int yStep) {
		int columnIndex = xSelector + xStep;
		int rowIndex = ySelector + yStep;

		if (rowIndex < 0) {
			MoveToLoadingPlatform (t);
			return;
		}

		columnIndex = columnIndex > 2 ? 2 : columnIndex;
		columnIndex = columnIndex < 0 ? 0 : columnIndex;
		rowIndex = rowIndex > 2 ? 2 : rowIndex;

		MoveSelector (t, columnIndex, rowIndex);
	}

	public void MoveToLoadingPlatform (Transform t) {
		Cargo tCar = t.GetComponent<Cargo> ();
		if (tCar) {
			MoveToCompartment (t, 1, -1);
			platformCargo = tCar;
		}
		Player tPlayer = t.GetComponent<Player> ();
		if (tPlayer) {
			MoveSelector (t, 1, -1);
			tPlayer.MoveToPlatform ();
		}
	}

	public void Occupy (bool isOccupied) {
		if (isOccupied) {
			coverRenderer.enabled = false;
		} else coverRenderer.enabled = true;
	}

	public void Dump (Transform t) {
		t.parent = null;

		Cargo tCar = t.GetComponent<Cargo> ();
		if (tCar) {
			platformCargo = null;
			t.tag = "Harpoonable";
			if (gm.bigBird.Landed) {
				t.gameObject.layer = LayerMask.NameToLayer ("Crossover");
			}
		}

		StartCoroutine (tCar.Dumped ());

		Vector3 dir = transform.position - gm.bigBird.transform.position;
		dir.Normalize ();
		Rigidbody2D trb = t.GetComponent<Rigidbody2D> ();
		trb.AddTorque (Random.Range (-torqueSkew, torqueSkew));
		trb.AddForce (dir * dumpForce);
	}

	//TODO could require someone to help with the unloading on big bird?
	public void Load (Transform t) {
		Cargo tCar = t.GetComponent<Cargo> ();
		if (tCar && platformCargo == null) {
			t.rotation = transform.rotation;
			t.parent = transform;
			t.tag = "Untagged";
			if (gm.bigBird.Landed) {
				t.gameObject.layer = LayerMask.NameToLayer ("Default");
			}
			t.GetComponent<BoxCollider2D> ().enabled = false;
			t.GetComponent<Rigidbody2D> ().Sleep ();
			MoveToLoadingPlatform (t);
		}
	}

	void EnableCollider () {
		GetComponent<BoxCollider2D> ().enabled = true;
	}

	int[] GetAvailableCompartment () {
		int[] availableCompartment = new int[2];
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 3; j++) {
				if (cargo [i, j] == null) {
					availableCompartment [0] = i;
					availableCompartment [1] = j;
					return availableCompartment;
				}
			}
		}
		return null;
	}

	public void CreateCargo () {
		//TODO some definite optimization that can be done with GetAvailableCompartment ()
		//and CheckForFull and the member variable full.
		int[] compartment = GetAvailableCompartment ();
		if (compartment == null) {
			Debug.Log ("no available compartment");
			return;
		}
		int xSelectorTemp = xSelector;
		int ySelectorTemp = ySelector;
		Cargo tempCargo = platformCargo;

		GameObject cargoObj = Instantiate (cargoPrefab, transform.position, Quaternion.identity) as GameObject;
		Cargo cargo = cargoObj.GetComponent<Cargo> ();
		cargo.RandomType ();
		cargo.transform.parent = transform;
		platformCargo = cargo;
		xSelector = compartment[0];
		ySelector = compartment[1];
		GrabSwapOrStoreCargo ();

		xSelector = xSelectorTemp;
		ySelector = ySelectorTemp;
		platformCargo = tempCargo;
	}

	public void CheckForFull () {
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 3; j++) {
				if (cargo [i, j] == null) {
					full = false;
					return;
				}
			}
		}
		full = true;
	}

	public bool GetFull () {
		return full;
	}
}
