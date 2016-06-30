using UnityEngine;
using System.Collections;

public class CargoHold : Station {

	public LoadingPlatform platform;
	public GameObject rubyPrefab;
	public GameObject greensPrefab;
	public GameObject powerbirdPrefab;
	public GameObject birdShieldPrefab;
	public GameObject batteryPrefab;
	public Item[,] cargo = new Item[3, 3];
	public Item platformCargo;
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
	private bool full = false;

	void Awake () {
		coverRenderer = GetComponentInChildren <SpriteRenderer> ();
		base.OnAwake ();
	}

	// Use this for initialization
	void Start () {
		base.SetStationType (Station.StationType.TURRET);

		CreateCargo (batteryPrefab);
		CreateCargo (greensPrefab);
		CreateCargo (greensPrefab);
		CreateCargo (powerbirdPrefab);
		CreateCargo (birdShieldPrefab);
		CreateCargo (rubyPrefab);
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
				Item c = cargo [xSelector, ySelector];
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
		Item tItem = t.GetComponent<Item> ();
		if (tItem) {
			MoveToCompartment (t, 1, -1);
			platformCargo = tItem;
		}
		Player tPlayer = t.GetComponent<Player> ();
		if (tPlayer) {
			//TODO what if someone else is on platform??
			MoveSelector (t, 1, -1);
			Abandon ();
			platform.Man (tPlayer);
			////tPlayer.MoveToPlatform ();
		}
	}

	public void Occupy (bool isOccupied) {
		if (isOccupied) {
			coverRenderer.enabled = false;
		} else coverRenderer.enabled = true;
	}

	public void Dump (Transform t) {
		t.parent = null;

		Item tItem = t.GetComponent<Item> ();
		if (tItem) {
			platformCargo = null;
			if (tItem.GetComponent<Harpoonable> ()) {
				tItem.GetComponent<Harpoonable> ().enabled = true;
			}
			if (gm.bigBird.Landed) {
				t.gameObject.layer = LayerMask.NameToLayer ("Crossover");
			}
		}

		StartCoroutine (tItem.EnableColliders ());

		Vector3 dir = transform.position - gm.bigBird.transform.position;
		dir.Normalize ();
		Rigidbody2D trb = t.GetComponent<Rigidbody2D> ();
		trb.isKinematic = false;
		trb.AddTorque (Random.Range (-torqueSkew, torqueSkew));
		trb.AddForce (dir * dumpForce);
	}

	//TODO could require someone to help with the unloading on big bird?
	public void Load (Transform t) {
		//check if item being loaded
		Item it = t.GetComponent<Item> ();
		if (it) {
			//check if it's a puppy (can't load puppies)
			//TODO puppies die if left in hold for x amount of time
			Hatchling pup = t.GetComponent<Hatchling> ();
			if (pup) {
				return;
			} else if (platformCargo == null) {
				if (it.GetComponent<Harpoonable> ()) {
					it.GetComponent<Harpoonable> ().enabled = false;
				}
				t.rotation = transform.rotation;
				t.parent = transform;
				t.tag = "Untagged";
				t.gameObject.layer = LayerMask.NameToLayer ("Default");
				t.GetComponent<Collider2D> ().enabled = false;
				t.GetComponent<Rigidbody2D> ().Sleep ();
				t.GetComponent<SpriteRenderer> ().sortingLayerName = "BigBird";
				t.GetComponent<SpriteRenderer> ().sortingOrder = 1;
				MoveToLoadingPlatform (t);
			}
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

	public void CreateCargo (GameObject obj) {
		//TODO some definite optimization that can be done with GetAvailableCompartment ()
		//and CheckForFull and the member variable full.
		int[] compartment = GetAvailableCompartment ();
		if (compartment == null) {
			Debug.Log ("no available compartment");
			return;
		}
		int xSelectorTemp = xSelector;
		int ySelectorTemp = ySelector;
		Item tempCargo = platformCargo;

		GameObject itemObj = Instantiate (obj, transform.position, Quaternion.identity) as GameObject;
		Item item = itemObj.GetComponent<Item> ();
		//item.RandomType ();
		item.transform.parent = transform;
		platformCargo = item;
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

	public void PressedA (Player p) {
		if (p.itemHeld) {
			//attempt swap
			if (cargo [xSelector, ySelector] == null) {
				cargo [xSelector, ySelector] = p.itemHeld.GetComponent<Item> ();
				p.DropItem ();
				if (p.itemHeld) {
					//drop failed
					cargo [xSelector, ySelector] = null;
				}
			} else {
				//cargo compartment is not empty
				Item tempItem = cargo [xSelector, ySelector].GetComponent<Item> ();
				cargo [xSelector, ySelector] = p.itemHeld.GetComponent<Item> ();
				p.DropItem ();
				if (p.itemHeld) {
					//drop failed
					cargo [xSelector, ySelector] = null;
				}
				p.itemTouching = tempItem.transform;
				p.PickUpItem ();			
			}
		} 
		//just try to pick up
		else if (!p.itemHeld) {
			if (cargo [xSelector, ySelector] == null) {
				//compartment is empty and not holding anything, do nothing				
			} else {
				//cargo compartment is not empty, pick it up
				p.itemTouching = cargo [xSelector, ySelector].transform;
				p.PickUpItem ();
				if (p.itemHeld) {
					cargo [xSelector, ySelector] = null;
					full = false;
				}
			}
		}
	}

	public void PressedAOnPlatform (Player p) {
		if (p.itemHeld) {
			//attempt swap
			if (platformCargo == null) {
				platformCargo = p.itemHeld.GetComponent<Item> ();
				p.DropItem ();
				if (p.itemHeld) {
					//drop failed
					platformCargo = null;
				}
			} else {
				//cargo compartment is not empty
				Item tempItem = platformCargo.GetComponent<Item> ();
				platformCargo = p.itemHeld.GetComponent<Item> ();
				p.DropItem ();
				if (p.itemHeld) {
					//drop failed
					platformCargo = null;
				}
				p.itemTouching = tempItem.transform;
				p.PickUpItem ();			
			}
		} 
		//just try to pick up
		else if (!p.itemHeld) {
			if (platformCargo == null) {
				//compartment is empty and not holding anything, do nothing				
			} else {
				//cargo compartment is not empty, pick it up
				p.itemTouching = platformCargo.transform;
				p.PickUpItem ();
				if (p.itemHeld) {
					platformCargo = null;
				}
			}
		}
	}

	public override void HandleInput () {
		if (Input.GetButtonDown (pi.bCircleButton)) {
			////TODO What if someone else is on platform?
			//gm.bigBird.hold.MoveToLoadingPlatform (user.transform);
			//pi.realSelectedStation = 
			Abandon ();
			return;
		}

		if (!pi.sr.enabled) {
			pi.sr.enabled = true;
			gm.bigBird.hold.xSelector = 1;
			gm.bigBird.hold.ySelector = 1;
			pi.sh.verticalStickInUse = false;
			pi.sh.horizontalStickInUse = false;
			gm.bigBird.hold.Occupy (true);
		}


		if (Input.GetButtonDown (pi.aCrossButton)) {
			gm.bigBird.hold.GrabSwapOrStoreCargo ();
		}

		if (Input.GetButtonDown (pi.xSquareButton)) {
			gm.bigBird.hold.PressedA (user);
		}

		HandleSticks ();

	}

	void HandleSticks () {
		if (Input.GetAxisRaw (pi.LSVertical) != 0 || Input.GetAxisRaw (pi.LSHorizontal) != 0) {
			if (pi.LStickInUse == false) {
				pi.LStickInUse = true;
				pi.timeOfLastStickUse = Time.time;
				Vector3 dir = new Vector3 (Input.GetAxis (pi.LSHorizontal), Input.GetAxis (pi.LSVertical), 0);
				float upness = Vector3.Dot (dir, gm.bigBird.transform.up);
				float overness = Vector3.Dot (dir, gm.bigBird.transform.right);
				if (Mathf.Abs (upness) > Mathf.Abs (overness)) {
					//move up/down relative to player
					if (upness > 0) {
						gm.bigBird.hold.SelectorStep (user.transform, 0, 1);
					} else {
						gm.bigBird.hold.SelectorStep (user.transform, 0, -1);
					}
				} else {
					//move right/left relative to player
					if (overness > 0) {
						gm.bigBird.hold.SelectorStep (user.transform, 1, 0);
					} else {
						gm.bigBird.hold.SelectorStep (user.transform, -1, 0);
					}
				}
			} else if (Time.time > pi.timeOfLastStickUse + pi.moveCooldown) {
				pi.LStickInUse =  false;
			}
		} else {
			pi.LStickInUse = false;
		}
	}

	public override void Man (Player p) {
		print ("manning hold");
		gm.bigBird.hold.Occupy (true);
		user = p;
		pi = user.GetComponent<PlayerInput> ();
		//pi.realStation = this;
		//pi.realSelectedStation = pi.realStation;
		pi.state = PlayerInput.State.IN_HOLD;
	}


	public override void Abandon () {
		print ("abandoning hold");
		gm.bigBird.hold.Occupy (false);
		pi.state = PlayerInput.State.CHANGING_STATIONS;
		pi.realStation = null;
		user = null;
		pi = null;
	}

	public override void MakeAvailable () {
		print ("making available hold");
		GetComponent<Collider2D> ().enabled = true;
	}

	public override void MakeUnavailable () {
		print ("making unavailable hold");
		GetComponent<Collider2D> ().enabled = false;
	}
}
