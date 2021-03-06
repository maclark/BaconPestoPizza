﻿using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

	public int playerNum = 0;
	public bool checkingInput = false;
	public bool canHighlight = false;
	public bool LStickInUse = false;
	public string joystick = "unset";
	public float moveCooldown = .2f;
	public float stationChangeDistance = 10f;
	public float timeOfLastStickUse;
	public LayerMask mask;
	public Transform station;

	public string LSHorizontal = "empty";
	public string LSVertical = "empty";
	public string RSHorizontal = "empty";
	public string RSVertical = "empty";
	public string leftTrigger = "empty";
	public string rightTrigger = "empty";
	public string DPadHorizontal = "empty";
	public string DPadVertical = "empty";
	public string backButton = "empty";
	public string startButton = "empty";
	public string aCrossButton = "empty";
	public string bCircleButton = "empty";
	public string xSquareButton = "empty";
	public string yTriangleButton = "empty";
	public string LB = "empty";
	public string RB = "empty";
	public string leftClick = "empty";
	public string rightClick = "empty";

	public enum State {NEUTRAL, FLYING, CHANGING_STATIONS, DOCKED, ON_TURRET, PILOTING, NAVIGATING, IN_BUBBLE, IN_WEB, POWERBIRD, IN_HOLD, ON_PLATFORM, IN_COOP, ON_FOOT}
	public State state = State.DOCKED;
	public State selectedState = State.NEUTRAL;


	public Station realStation;
	public Station realSelectedStation;


	public GameManager gm;
	public SpriteRenderer sr;
	public SystemsHandler sysH;
	public StickHandler sh;


	private BigBird bigBird;
	private Player p;
	//private Turret turret;
	private bool releasedRightTrigger = true;
	private bool isXboxController = false;
	private bool stationcasting = false;
	private float timeOfStationcast;

	void Awake () {
		gm = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
		sr = GetComponent<SpriteRenderer> ();
		bigBird = GameObject.FindObjectOfType<BigBird>() as BigBird;
		p = GetComponent<Player> ();
		sh = new StickHandler ();
	}

	void Start () {
		sysH = new SystemsHandler (bigBird.rightCans, bigBird.leftCans, bigBird.cannonballPrefab);
	}

	void Update () {
		if (joystick == "unset") {
			for (int i = 1; i < 12; i++) { 

				if (Input.GetKeyDown ("joystick " + i + " button 7")) {
					if (playerNum == i) {
						isXboxController = true;
						joystick = "_p" + i;
					}
				}
			}

			//joystick is set, so player has hit start
			if (joystick != "unset") {
				SetButtonNames ();
				p.StartPlayer ();
				StartCoroutine (p.HighlightSecs ());
			}

			return;
		}  

		if (canHighlight) {
			if (Input.GetButtonDown (startButton)) {
				p.Highlight ();
			} else if (Input.GetButtonUp (startButton)) {
				p.Unhighlight ();
			}
		}

		if (realStation) {
			realStation.HandleInput ();
		} else if (state == State.NEUTRAL) {
			HandleNeutralInput ();
		}  else if (state == State.FLYING) {
			HandleFlyingInput ();
		}  else if (state == State.CHANGING_STATIONS) {
			HandleChangingStations ();
		}  else if (state == State.ON_TURRET) {
			Debug.Log ("ON TURRET WTF?");
		}  else if (state == State.DOCKED) {
			Debug.Log ("ON DOCK WTF?");
			//HandleDockedInput ();
		}  else if (state == State.PILOTING) {
			Debug.Log ("PILOTING WTF?");
			//HandlePilotingInput ();
		}  else if (state == State.NAVIGATING) {
			HandleNavigationInput ();
		}  else if (state == State.IN_WEB) {
			HandleInWebInput ();
		}  else if (state == State.POWERBIRD) {
			HandlePowerbirdInput ();
		}  else if (state == State.IN_HOLD) {
			Debug.Log ("IN HOLD WTF?");
			//HandleInHoldInput ();
		}  else if (state == State.ON_PLATFORM) {
			//HandleOnPlatformInput ();
		}  else if (state == State.IN_COOP) {
			HandleInCoopInput ();
		}  else if (state == State.ON_FOOT) {
			HandleOnFootInput ();
		}
	}

	//TODO this only good for leaving bigbird. need more instructions if
	//going to use this for general station movement
	public void AbandonStation () {
		if (state == State.NEUTRAL) {
		}  else if (state == State.ON_TURRET) {
			//turret.transform.GetComponentInChildren<SpriteRenderer> ().color = Color.grey;
		}  else if (state == State.DOCKED) {
			//if (p.b) {
			//	p.Debird (bigBird.transform);
			//}		
		}  else if (state == State.PILOTING) {
			//bigBird.turn = 0;
			//station.GetComponentInChildren<SpriteRenderer> ().color = Color.grey;
			//sr.sortingOrder = 2;
		}  else if (state == State.NAVIGATING) {
			HandleNavigationInput ();
		}  else if (state == State.IN_HOLD) {
			//gm.bigBird.hold.Occupy (false);
		}  else if (state == State.ON_PLATFORM) {
			//gm.bigBird.hold.Occupy (false);
		}

		if (station) {
			station.GetComponent<BoxCollider2D> ().enabled = true;
		}
	}

	void ManStation () {
		state = selectedState;
		if (state == State.ON_TURRET) {
			station.GetComponentInChildren<SpriteRenderer> ().color = p.color;
			return;
		}  else if (state == State.PILOTING) {
			station.GetComponentInChildren<SpriteRenderer> ().color = new Color(.37f, .26f, .15f);
			sr.sortingOrder = 0;
			return;
		}  else if (state == State.IN_COOP) {
			sr.enabled = true;
			p.ManifestFlesh (transform.position, "BigBird", 2);
			p.body.trigger.SetActive (true);
			p.body.transform.parent = station.transform;
			return;
		}  else if (state == State.ON_FOOT) {
			p.Disembark (station.transform.position);
			return;
		}

		sr.enabled = false;

	}

	void HandleChangingStations () {
		//TODO can get stuck in changing stations at start because of being in Neutral
		if (Input.GetButtonUp(bCircleButton)) {
			if (realSelectedStation) {
				realSelectedStation.Man (p);
				realStation = realSelectedStation;
			} else ManStation ();
			return;
		}  
		sr.enabled = true;

		float horizontalness = 0f;
		float verticalness = 0f;

		if (Input.GetAxisRaw(LSHorizontal) != 0 || Input.GetAxisRaw(LSVertical) != 0) {
			if (stationcasting == false) {
				stationcasting = true;
				timeOfStationcast = Time.time;
				verticalness = Input.GetAxis(LSVertical);
				horizontalness = Input.GetAxis(LSHorizontal);
			}  else if (Time.time > timeOfStationcast + moveCooldown) {
				stationcasting =  false; 
			}

		}  else if (Input.GetAxisRaw(LSHorizontal) == 0 && Input.GetAxisRaw(LSVertical) == 0) {
			stationcasting =  false;
		}  

		RaycastHit2D hit = Physics2D.Raycast (transform.position, new Vector2 (horizontalness, verticalness), stationChangeDistance, mask);
		if (hit) {
			if (hit.collider.name == "BoardingZone" && !bigBird.Landed) {
				return;
			}


			if (realSelectedStation) {
				realSelectedStation.MakeAvailable ();
			} else if (station) {
				station.GetComponent<BoxCollider2D> ().enabled = true;
			}



			station = hit.collider.transform;
			realSelectedStation = hit.collider.transform.GetComponent<Station> ();



			if (realSelectedStation) {
				print ("real station hit: " + hit.collider.transform.name);
				realSelectedStation.MakeUnavailable ();
			} else {
				print ("fake station hit: " + hit.collider.transform.name);
				station.GetComponent<BoxCollider2D> ().enabled = false;
			}


			if (realSelectedStation) {
				transform.position = realSelectedStation.transform.position;
			} else {
				transform.position = station.position;
			}



			if (hit.collider.name == "Cockpit") {
				//selectedState = State.PILOTING;
			}  else if (hit.collider.name == "Dock") {
				//selectedState = State.DOCKED;
			}  else if (hit.collider.name == "Turret") {
				//turret = station.GetComponent<Turret> ();
				//selectedState = State.ON_TURRET;
			}  else if (hit.collider.name == "CargoPlatform") {
				//selectedState = State.ON_PLATFORM;
			}  else if (hit.collider.name == "CargoHold") {
				//selectedState = State.IN_HOLD;
			}  else if (hit.collider.name == "Coop") {
				selectedState = State.IN_COOP;
			}  else if (hit.collider.name == "BoardingZone") {
				selectedState = State.ON_FOOT;
			}
		}
	}

	void HandleNeutralInput () {
		if (Input.GetButtonDown (bCircleButton)) {
			state = State.CHANGING_STATIONS;
			return;
		}
	}

	void HandleFlyingInput () {
		p.b.direction = new Vector2 (Input.GetAxis (LSHorizontal), Input.GetAxis (LSVertical));

		Vector2 rightStick = new Vector2 (Input.GetAxis (RSHorizontal), Input.GetAxis (RSVertical));
		if (rightStick != Vector2.zero) {
			p.aim = rightStick;
		}  else if (p.aim == Vector3.zero) {
			if (p.b.direction != Vector2.zero) {
				p.aim = new Vector3 (p.b.direction.x, p.b.direction.y, 0);
			}
		}
		p.aim.Normalize ();

		if (!p.b.hurledHarp && !p.b.harpLoaded && !p.b.catchingHarp) {
			if (Input.GetAxisRaw (leftTrigger) > 0) {
				p.b.LoadHarp ();
			}  
		}  else if (p.b.hurledHarp) {
			if (Input.GetAxisRaw (leftTrigger) > 0) {
				p.b.harp.SetRecalling (true);
			}  else if (Input.GetAxis (leftTrigger) <= 0) {
				p.b.harp.SetRecalling (false);
			}
		}  else if (p.b.harpLoaded) {
			if (Input.GetAxisRaw (leftTrigger) <= 0) {
				p.b.SwingHurlHarpoon ();
			}
		}  else if (p.b.catchingHarp) {
			if (Input.GetAxisRaw (leftTrigger) <= 0) {
				p.b.catchingHarp = false;
			}
		}

		if (!p.GetHoldingString () && !p.b.harpLoaded) {
			HandleWeaponFiring ();
		}

		if (Input.GetButtonDown (LB)) {
			if (p.b.canRoll) {
				p.b.rolling = true;
				p.b.canRoll = false;
				StartCoroutine (p.b.EndRoll ());
			}
		}

		if (Input.GetButtonDown (yTriangleButton)) {
			if (p.b.harpLoaded) {
				p.b.UnloadHarp ();
				p.b.catchingHarp = true;
			}  else {
				p.CycleWeapons ();
			}
		}

		if (Input.GetButtonDown (xSquareButton)) {
			if (p.w.roundsLeftInClip < p.w.clipSize && !p.w.reloading) {
				p.StartCoroutine (p.w.Reload ());
			}
		}

		if (Input.GetButtonDown (rightClick)) {
			if (p.b.hurledHarp) {
				p.b.harp.ToggleGripping ();
			}
		}

		if (Input.GetButtonDown (RB)) {
			if (p.b.canBoost) {
				StartCoroutine( p.b.Boost ());
			}
		}
	}

	void HandleDockedInput () {
		/*if (Input.GetButtonDown (bCircleButton)) {
			if (p.b) {
				p.Debird (bigBird.transform);
			}		
			state = State.CHANGING_STATIONS;
			return;
		}

		if (Input.GetButtonDown (xSquareButton)) {
			if (station.GetComponent<Dock> ().item) {
				p.itemTouching = station.GetComponent<Dock> ().item;
				p.PickUpItem ();
			}  else if (!station.GetComponent<Dock> ().item && p.itemHeld) {
				p.DropItem ();
			}
		}

		if (p.b == null && station.GetComponent<Dock> ().bird) {
			p.BoardBird (station.GetComponent<Dock> ().bird);
		}

		if (Input.GetButtonDown (aCrossButton)) {
			p.b.UndockFromBigBird ();
			if (!p.b.docked) {
				state = State.FLYING;
			}
		}*/
	}

	void HandleInCoopInput () {
		if (Input.GetButtonDown (bCircleButton)) {
			p.transform.parent = station;
			p.body.trigger.SetActive (false);
			p.body.gameObject.SetActive (false);
			state = State.CHANGING_STATIONS;
			return;
		}

		p.body.direction = new Vector2 (Input.GetAxis (LSHorizontal), Input.GetAxis (LSVertical));

		Vector2 rightStick = new Vector2 (Input.GetAxis (RSHorizontal), Input.GetAxis (RSVertical));
		if (rightStick != Vector2.zero) {
			p.aim = rightStick;
		}  else if (p.aim == Vector3.zero) {
			if (p.body.direction != Vector2.zero) {
				p.aim = new Vector3 (p.body.direction.x, p.body.direction.y, 0);
			}
		}

		if (Input.GetButtonDown (xSquareButton)) {
			if (p.itemHeld) {
				p.DropItem ();
			}  else if (p.itemTouching) {
				p.PickUpItem ();
			}
		}
	}

	void HandleTurretInput () {
		print ("HandleTurretInput");
		/*if (Input.GetButtonDown (bCircleButton)) {
			state = State.CHANGING_STATIONS;
			turret.transform.GetComponentInChildren<SpriteRenderer> ().color = Color.black;
			return;
		}

		turret.Rotate (new Vector3( Input.GetAxis(LSHorizontal), Input.GetAxis(LSVertical), 0f));
		if (Input.GetAxis (rightTrigger) < 0) {
			turret.RightTrigger ();
		}

		if (Input.GetButtonDown (aCrossButton)) {
			turret.PressedA ();
		}*/
	}

	void HandlePilotingInput () {
		/*if (Input.GetButtonDown (bCircleButton)) {
			bigBird.turn = 0;
			station.GetComponentInChildren<SpriteRenderer> ().color = Color.black;
			state = State.CHANGING_STATIONS;
			sr.sortingOrder = 2;
			return;
		}

		/*if (Input.GetButtonDown (xSquareButton)) {
			bigBird.turn = 0;
			state = State.NAVIGATING;
			return;
		} */

		/*if (bigBird.Landed || bigBird.landing) {
			if (Input.GetButtonDown (xSquareButton)) {
				bigBird.LiftOff ();
			}
			return;
		}

		if (Input.GetButtonDown (xSquareButton)) {
			if (bigBird.nearestPad) {
				bigBird.BigDock ();
				return;
			}
		}

		bigBird.turn = -Mathf.RoundToInt (Input.GetAxis (LSHorizontal));

		if (Input.GetButtonDown (aCrossButton)) {
			if (!bigBird.GetEngineOn ()) {
				bigBird.TurnEngineOn ();
			}
		} else if (Input.GetButtonUp (aCrossButton)) {
			if (bigBird.GetEngineOn ()) {
				bigBird.TurnEngineOff ();
			}
		}

		if (Input.GetAxisRaw (rightTrigger) < 0) {
			sysH.FireBroadside (true);
		}  else if (Input.GetAxisRaw (leftTrigger) > 0) {
			sysH.FireBroadside (false);
		}*/
	}

	void HandleNavigationInput () {
		if (Input.GetButtonUp (xSquareButton)) {
			gm.StopNavigating ();
			state = State.PILOTING;
			return;
		}

		gm.Navigate (Input.GetAxis (LSHorizontal), Input.GetAxis (LSVertical));
	}

	void HandleInHoldInput () {
		/*if (Input.GetButtonDown (bCircleButton)) {
			gm.bigBird.hold.MoveToLoadingPlatform (p.transform);
			gm.bigBird.hold.Occupy (false);
			state = State.CHANGING_STATIONS;
			return;
		}

		if (!sr.enabled) {
			sr.enabled = true;
			gm.bigBird.hold.xSelector = 1;
			gm.bigBird.hold.ySelector = 1;
			sh.verticalStickInUse = false;
			sh.horizontalStickInUse = false;
			gm.bigBird.hold.Occupy (true);
		}

		sh.HandleInHoldSticks (p, LSVertical, LSHorizontal);

		if (Input.GetButtonDown (aCrossButton)) {
			gm.bigBird.hold.GrabSwapOrStoreCargo ();
		}

		if (Input.GetButtonDown (xSquareButton)) {
			gm.bigBird.hold.PressedA (p);
		}
		*/
	}

	void HandleOnPlatformInput () {
		/*if (Input.GetButtonDown (bCircleButton)) {
			gm.bigBird.hold.Occupy (false);
			state = State.CHANGING_STATIONS;
			return;
		}

		if (!sr.enabled) {
			sr.enabled = true;
			gm.bigBird.hold.xSelector = 1;
			gm.bigBird.hold.ySelector = -1;
			sh.verticalStickInUse = false;
			sh.horizontalStickInUse = false;
			gm.bigBird.hold.Occupy (true);
		}

		sh.HandleOnPlatformSticks (p, LSVertical, LSHorizontal);

		if (Input.GetButtonDown (xSquareButton)) {
			gm.bigBird.hold.PressedAOnPlatform (p);
		}

		if (Input.GetButtonDown (aCrossButton)) {
			if (gm.bigBird.hold.platformCargo) {
				gm.bigBird.hold.Dump (gm.bigBird.hold.platformCargo.transform);
			}
		}*/
	}

	void HandleOnFootInput () {
		if (Input.GetButtonDown (bCircleButton)) {
			p.body.PressedB ();
			return;
		}

		p.body.direction = new Vector2 (Input.GetAxis (LSHorizontal), Input.GetAxis (LSVertical));

		Vector2 rightStick = new Vector2 (Input.GetAxis (RSHorizontal), Input.GetAxis (RSVertical));
		if (rightStick != Vector2.zero) {
			p.aim = rightStick;
		}  else if (p.aim == Vector3.zero) {
			if (p.body.direction != Vector2.zero) {
				p.aim = new Vector3 (p.body.direction.x, p.body.direction.y, 0);
			}
		}

		if (Input.GetButtonDown (xSquareButton)) {
			if (p.itemHeld) {
				p.DropItem ();
			}  else if (p.itemTouching) {
				p.PickUpItem ();
			}
		}
	}

	void HandleInWebInput () {
		Vector2 rightStick = new Vector2( Input.GetAxis(RSHorizontal), Input.GetAxis(RSVertical));
		if (rightStick != Vector2.zero) {
			p.aim = rightStick;
		}  
		else if (p.aim == Vector3.zero) {
			if (p.b.direction != Vector2.zero) {
				p.aim = new Vector3 (p.b.direction.x, p.b.direction.y, 0);
			}
		}
		HandleWeaponFiring ();
	}

	void HandlePowerbirdInput () {
		p.b.direction = new Vector2 (Input.GetAxis (LSHorizontal), Input.GetAxis (LSVertical));

		Vector2 rightStick = new Vector2 (Input.GetAxis (RSHorizontal), Input.GetAxis (RSVertical));
		if (rightStick != Vector2.zero) {
			p.aim = rightStick;
		}  else if (p.aim == Vector3.zero) {
			if (p.b.direction != Vector2.zero) {
				p.aim = new Vector3 (p.b.direction.x, p.b.direction.y, 0);
			}
		}
		p.aim.Normalize ();

		if (Input.GetButtonDown (RB)) {
			if (p.b.canBoost) {
				StartCoroutine( p.b.Boost ());
			}
		}

		HandleWeaponFiring ();
	}

	void HandleWeaponFiring () {
		if (p.w.automatic) {
			if (!p.w.firing && Input.GetAxis (rightTrigger) < 0) {
				p.w.firing = true;
				InvokeRepeating ("FireBullet", 0, p.w.fireRate);
			}  else if (p.w.firing && Input.GetAxis (rightTrigger) >= 0) {
				p.w.firing = false;
				p.CancelInvoke ();
				CancelInvoke ();
			}
		}  else {
			if (Input.GetAxis (rightTrigger) < 0 && p.w.cocked && releasedRightTrigger && !p.w.reloading) {
				p.w.Fire (p.aim);
				releasedRightTrigger = false;
			}  else if (Input.GetAxis (rightTrigger) >= 0) {
				releasedRightTrigger = true;
			}
		}
	}

	void FireBullet () {
		p.w.Fire (p.aim);
	}

	void SetButtonNames () {

		if (isXboxController) {
			LSHorizontal 		= "x_axis" + joystick;
			LSVertical 			= "y_axis" + joystick;
			RSHorizontal 		= "4th_axis" + joystick;
			RSVertical 			= "5th_axis" + joystick;
			DPadHorizontal 		= "6th_axis" + joystick;
			DPadVertical 		= "7th_axis" + joystick;
			leftTrigger 		= "3rd_axis" + joystick;
			rightTrigger 		= "3rd_axis" + joystick;

			aCrossButton 		= "button0" + joystick;
			bCircleButton 		= "button1" + joystick;
			xSquareButton 		= "button2" + joystick;
			yTriangleButton 	= "button3" + joystick;
			LB 					= "button4" + joystick;
			RB 					= "button5" + joystick;
			backButton 			= "button6" + joystick;
			startButton 		= "button7" + joystick;
			leftClick 			= "button8" + joystick;
			rightClick 			= "button9" + joystick;
		}  
		//setup ps3 buttons
		else {
			LSHorizontal 		= "6th_axis" + joystick;
			LSVertical 			= "7th_axis" + joystick;
			RSHorizontal 		= "3rd_axis" + joystick;
			RSVertical 			= "5th_axis" + joystick;
			DPadHorizontal 		= "x_axis" + joystick;
			DPadVertical 		= "y_axis" + joystick;

			leftTrigger 		= "button6" + joystick;
			rightTrigger 		= "button7" + joystick;

			aCrossButton 		= "button2" + joystick;
			bCircleButton 		= "button1" + joystick;
			xSquareButton 		= "button3" + joystick;
			yTriangleButton		= "button0" + joystick;
			LB 					= "button4" + joystick;
			RB 					= "button5" + joystick;
			backButton 			= "button8" + joystick;
			startButton 		= "button9" + joystick;
			leftClick 			= "button10" + joystick;
			rightClick 			= "button11" + joystick;

		}
	}
}

/*
 * 
 * 

		#region
		////For mapping game pad
		if (checkingInput) {
			for (int i = 0; i < 20; i++) {
				if (Input.GetKeyDown("joystick 1 button "+i)) {
					// do something
					print("pressed joystick 1 button " + i);
				}

				if (Input.GetKeyDown("joystick 2 button "+i)) {
					// do something
					print("pressed joystick 2 button " + i);
				}

				if (Input.GetKeyDown("joystick 3 button "+i)) {
					// do something
					print("pressed joystick 3 button " + i);
				}

				if (Input.GetKeyDown("joystick 4 button "+i)) {
					// do something
					print("pressed joystick 4 button " + i);
				}

				if (Input.GetKeyDown("joystick 5 button "+i)) {
					// do something
					print("pressed joystick 5 button " + i);
				}

				if (Input.GetKeyDown("joystick 6 button "+i)) {
					// do something
					print("pressed joystick 6 button " + i);
				}

				if (Input.GetKeyDown("joystick 7 button "+i)) {
					// do something
					print("pressed joystick 7 button " + i);
				}

				if (Input.GetKeyDown("joystick 8 button "+i)) {
					// do something
					print("pressed joystick 8 button " + i);
				}

				if (Input.GetKeyDown("joystick 9 button "+i)) {
					// do something
					print("pressed joystick 9 button " + i);
				}

				if (Input.GetKeyDown("joystick 10 button "+i)) {
					// do something
					print("pressed joystick 10 button " + i);
				}

				if (Input.GetKeyDown("joystick 11 button "+i)) {
					// do something
					print("pressed joystick 11 button " + i);
				}
			}		
		

			if (Input.GetAxis("x_axis_p1") != 0) {
				print("x_axis_p1");
			}

			if (Input.GetAxis("y_axis_p1") != 0) {
				print("y_axis_p1");
			}

			if (Input.GetAxis("3rd_axis_p1") != 0) {
				print("3rd_axis_p1");
			}

			if (Input.GetAxis("5th_axis_p1") != 0) {
				print("5th_axis_p1");
			}

			if (Input.GetAxis("6th_axis_p1") != 0) {
				print("6th_axis_p1");
			}

			if (Input.GetAxis("7th_axis_p1") != 0) {
				print("7th_axis_p1");
			}

			if (Input.GetAxis("8th_axis_p1") != 0) {
				print("8th_axis_p1");
			}

			if (Input.GetAxis("9th_axis_p1") != 0) {
				print("9th_axis_p1");
			}

			if (Input.GetAxis("10th_axis_p1") != 0) {
				print("10th_axis_p1");
			}

			if (Input.GetAxis("11th_axis_p1") != 0) {
				print("11th_axis_p1");
			}

			if (Input.GetAxis("12th_axis_p1") != 0) {
				print("11th_axis_p1");
			}

			if (Input.GetAxis("13th_axis_p1") != 0) {
				print("13th_axis_p1");
			}

			if (Input.GetAxis("14th_axis_p1") != 0) {
				print("14th_axis_p1");
			}

			if (Input.GetAxis("15th_axis_p1") != 0) {
				print("15th_axis_p1");
			}

			if (Input.GetAxis("16th_axis_p1") != 0) {
				print("16th_axis_p1");
			}

			if (Input.GetAxis("16th_axis_p1") != 0) {
				print("16th_axis_p1");
			}


		
		}
			



		#endregion

*/
