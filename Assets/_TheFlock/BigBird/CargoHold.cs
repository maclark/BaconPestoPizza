using UnityEngine;
using System.Collections;

public class CargoHold : MonoBehaviour {
	public GameObject cargoPrefab;
	public Cargo[,] cargo = new Cargo[3, 3];
	public float columnWidth = 1f;
	public float rowWidth = 1.2f;
	public float holdWidthOffset = -1f;
	public float holdHeightOffset = -1.2f;
	public Transform loadingPlatform;
	public int xSelector;
	public int ySelector;
	public Cargo platformCargo;



	// Use this for initialization
	void Start () {
		GameObject cargoObj = Instantiate (cargoPrefab, transform.position, Quaternion.identity) as GameObject;
		Cargo cargo = cargoObj.GetComponent<Cargo> ();
		cargo.transform.parent = transform;
		platformCargo = cargo;
		xSelector = 0;
		ySelector = 0;
		GrabSwapOrStoreCargo ();


		GameObject cargoObj1 = Instantiate (cargoPrefab, transform.position, Quaternion.identity) as GameObject;
		Cargo cargo1 = cargoObj1.GetComponent<Cargo> ();
		cargo1.transform.parent = transform;
		platformCargo = cargo1;
		xSelector = 2;
		ySelector = 2;
		GrabSwapOrStoreCargo ();

		GameObject cargoObj2 = Instantiate (cargoPrefab, transform.position, Quaternion.identity) as GameObject;
		Cargo cargo2 = cargoObj2.GetComponent<Cargo> ();
		cargo2.transform.parent = transform;
		platformCargo = cargo2;
		xSelector = 1;
		ySelector = 0;
		GrabSwapOrStoreCargo ();
		platformCargo = null;
	}

	public void GrabSwapOrStoreCargo () {
		if (platformCargo == null) {
			if (cargo [xSelector, ySelector] != null) {
				//Grab
				MoveToLoadingPlatform (cargo [xSelector, ySelector].transform);
				cargo [xSelector, ySelector] = null;
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
			print ("moving player to plat");
			MoveSelector (t, 1, -1);
			tPlayer.MoveToPlatform ();
		}
	}
}
