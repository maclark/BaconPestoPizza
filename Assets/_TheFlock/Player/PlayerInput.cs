using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

	public int playerNum = 0;
	public bool checkingInput = false;
	public string joystick = "unset";
	public float moveCooldown = .2f;
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

	public enum State {NEUTRAL, FLYING, CHANGING_STATIONS, DOCKED, ON_TURRET, PILOTING, NAVIGATING, IN_BUBBLE, IN_WEB, IN_HOLD, ON_PLATFORM}
	public State state = State.DOCKED;
	public State selectedState = State.NEUTRAL;


	private GameManager gm;
	private SpriteRenderer sr;
	private BigBird bigBird;
	private StickHandler sh;
	private Player p;
	private Turret turret;
	private bool releasedRightTrigger = true;

	private bool rightTriggerInUse = false;
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

	void Update () {


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

		if (joystick == "unset") {
			for (int i = 1; i < 12; i++) { 

				if (Input.GetKeyDown ("joystick " + i + " button 7")) {
					if (playerNum == i) {
						isXboxController = true;
						joystick = "_p" + i;
					}
				} else if (Input.GetKeyDown ("joystick " + i + " button 9")) {
					if (playerNum == i) {
						isXboxController = false;
						joystick = "_p" + i;
					}
				}
			}

			//joystick is set, so player has hit start
			if (joystick != "unset") {
				SetButtonNames ();
				p.StartPlayer ();
			}

			return;
		} 


		if (Input.GetButtonDown (startButton)) {
			gm.TogglePause();
		}

		if (state == State.NEUTRAL) {
			HandleNeutralInput ();
		} 
		else if (state == State.FLYING) {
			HandleFlyingInput ();
		} else if (state == State.CHANGING_STATIONS) {
			HandleChangingStations ();
		} else if (state == State.ON_TURRET) {
			HandleTurretInput ();
		} else if (state == State.DOCKED) {
			HandleDockedInput ();
		} else if (state == State.PILOTING) {
			HandlePilotingInput ();
		} else if (state == State.NAVIGATING) {
			HandleNavigationInput ();
		} else if (state == State.IN_WEB) {
			HandleInWebInput ();
		} else if (state == State.IN_HOLD) {
			HandleInHoldInput ();
		} else if (state == State.ON_PLATFORM) {
			HandleOnPlatformInput ();
		}
	}

	void HandleNeutralInput () {
		if (Input.GetButtonDown (bCircleButton)) {
			state = State.CHANGING_STATIONS;
			return;
		}
	}

	void HandleFlyingInput () {
		p.b.direction = new Vector2( Input.GetAxis(LSHorizontal), Input.GetAxis(LSVertical));
		if (p.b.gas <= 0) {
			p.b.direction = Vector2.zero;
		}

		Vector2 rightStick = new Vector2( Input.GetAxis(RSHorizontal), Input.GetAxis(RSVertical));
		if (rightStick != Vector2.zero) {
			p.aim = rightStick;
		} 
		else if (p.aim == Vector3.zero) {
			if (p.b.direction != Vector2.zero) {
				p.aim = new Vector3 (p.b.direction.x, p.b.direction.y, 0);
			}
		}

		if (!p.GetHoldingString ()) {
			HandleFiring ();
		}

		if (Input.GetAxis (leftTrigger) > 0) {
			if (p.b.canRoll) {
				p.b.rolling = true;
				p.b.canRoll = false;
				StartCoroutine (p.b.EndRoll ());
			}
		}

		if (Input.GetButtonDown (yTriangleButton)) {
			p.CycleWeapons ();
		}

		if (Input.GetButtonDown (xSquareButton)) {
			if (p.w.roundsLeftInClip < p.w.clipSize && !p.w.reloading) {
				p.StartCoroutine (p.w.Reload ());
			}
		}

		if (Input.GetButtonDown (rightClick)) {
			if (p.b.harp) {
				p.b.harp.DetachAndRecall ();
			} else {
				p.b.HurlHarpoon ();
			}
		}

		if (Input.GetButtonDown (LB)) {
			if (p.b.canBoost) {
				StartCoroutine( p.b.Boost ());
			}
		}
	}
		


	void HandleChangingStations () {
		//TODO can get stuck in changing stations at start because of being in Neutral
		if (Input.GetButtonUp(bCircleButton)) {
			state = selectedState;
			if (state == State.ON_TURRET) {
				station.GetComponentInChildren<SpriteRenderer> ().color = p.color;
			} else if (state == State.PILOTING) {
				station.GetComponentInChildren<SpriteRenderer> ().color = p.color;
			}
			sr.enabled = false;
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
			} else if (Time.time > timeOfStationcast + moveCooldown) {
				stationcasting =  false;
			}

		} else if (Input.GetAxisRaw(LSHorizontal) == 0 && Input.GetAxisRaw(LSVertical) == 0) {
			stationcasting =  false;
		} 

		RaycastHit2D hit = Physics2D.Raycast (transform.position, new Vector2 (horizontalness, verticalness), Mathf.Infinity, mask);
		if (hit) {
			if (station) {
				station.GetComponent<BoxCollider2D> ().enabled = true;
			}
			station = hit.collider.transform;
			station.GetComponent<BoxCollider2D> ().enabled = false;
			transform.position = station.position;

			if (hit.collider.name == "Cockpit") {
				selectedState = State.PILOTING;
			} else if (hit.collider.name == "Dock") {
				selectedState = State.DOCKED;
			} else if (hit.collider.name == "Turret") {
				turret = station.GetComponent<Turret> ();
				selectedState = State.ON_TURRET;
			} else if (hit.collider.name == "CargoPlatform") {
				selectedState = State.ON_PLATFORM;
			}
		}
	}

	void HandleDockedInput () {
		if (Input.GetButtonDown (bCircleButton)) {
			if (p.b) {
				p.Debird (bigBird.transform);
			}		
			state = State.CHANGING_STATIONS;
			return;
		}

		if (p.b == null && station.GetComponent<Dock> ().bird) {
			p.BoardBird (station.GetComponent<Dock> ().bird);
		}

		if (Input.GetButtonDown (aCrossButton)) {
			p.b.Undock ();
			if (!p.b.docked) {
				state = State.FLYING;
			}
		}
	}

	void HandleTurretInput () {
		if (Input.GetButtonDown (bCircleButton)) {
			state = State.CHANGING_STATIONS;
			turret.transform.GetComponentInChildren<SpriteRenderer> ().color = Color.grey;
			return;
		}

		turret.Rotate (new Vector3( Input.GetAxis(LSHorizontal), Input.GetAxis(LSVertical), 0f));
		if (Input.GetAxis (rightTrigger) < 0) {
			turret.RightTrigger ();
		}

		if (Input.GetButtonDown (aCrossButton)) {
			turret.PressedA ();
		}
	}

	void HandlePilotingInput () {
		if (Input.GetButtonDown (bCircleButton)) {
			bigBird.turning = false;
			station.GetComponentInChildren<SpriteRenderer> ().color = Color.grey;
			state = State.CHANGING_STATIONS;
			return;
		}

		if (Input.GetButtonDown (xSquareButton)) {
			bigBird.turning = false;
			state = State.NAVIGATING;
			return;
		}

		if (bigBird.Landed) {
			if (Input.GetButtonDown (aCrossButton)) {
				bigBird.LiftOff ();
			}
			return;
		}

		if (Input.GetButtonDown (aCrossButton)) {
			if (bigBird.nearestPad) {
				bigBird.SetDown (bigBird.nearestPad);
				return;
			}
		}
			
		float x = Input.GetAxis (LSHorizontal);
		float y = Input.GetAxis(LSVertical); 

		if (Mathf.Sqrt (x * x + y * y) > bigBird.thresholdToTurnBigBird) {
			float angle = Mathf.Atan2 (Input.GetAxis (LSHorizontal), Input.GetAxis (LSVertical)) * Mathf.Rad2Deg;
			bigBird.turning = true;
			bigBird.SetTargetRotationZAngle (-angle);
		} else
			bigBird.turning = false;

		if (Input.GetButtonDown (LB)) {
			if (bigBird.GetEngineOn ()) {
				bigBird.TurnEngineOff ();
			} else {
				bigBird.TurnEngineOn ();
			}
		}
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
		if (Input.GetButtonDown (bCircleButton)) {
			gm.bigBird.hold.MoveToLoadingPlatform (p.transform);
			gm.bigBird.hold.Occupy (false);
			state = State.CHANGING_STATIONS;
			return;
		}

		sh.HandleInHoldSticks (p, LSVertical, LSHorizontal);
		/*
		if (Input.GetAxisRaw(LSVertical) != 0) {
			if (verticalStickInUse == false) {
				verticalStickInUse = true;
				timeOfLastVertStep = Time.time;
				if (Input.GetAxisRaw(LSVertical) > 0) {
					gm.bigBird.hold.SelectorStep (p.transform, 0, 1);
				} else {
					gm.bigBird.hold.SelectorStep (p.transform, 0, -1);
				}
			}  else if (Time.time > timeOfLastVertStep + moveCooldown) {
				verticalStickInUse =  false;
			}

		} else if (Input.GetAxisRaw (LSVertical) == 0) {
			verticalStickInUse = false;
		}


		if (Input.GetAxisRaw(LSHorizontal) != 0) {
			if (horizontalStickInUse == false) {
				horizontalStickInUse = true;
				timeOfLastHorzStep = Time.time;
				if (Input.GetAxisRaw(LSHorizontal) > 0) {
					gm.bigBird.hold.SelectorStep (p.transform, 1, 0);
				} else {
					gm.bigBird.hold.SelectorStep (p.transform, -1, 0);
				}
			} else if (Time.time > timeOfLastHorzStep + moveCooldown) {
				horizontalStickInUse =  false;
			}

		} else if (Input.GetAxisRaw (LSHorizontal) == 0) {
			horizontalStickInUse = false;
		}
		*/
		if (Input.GetAxisRaw(rightTrigger) != 0) {
			if (rightTriggerInUse == false) {
				rightTriggerInUse = true;
				gm.bigBird.hold.GrabSwapOrStoreCargo ();
			}

		} else if (Input.GetAxisRaw (DPadVertical) == 0) {
			rightTriggerInUse = false;
		}
	}

	void HandleOnPlatformInput () {
		if (Input.GetButtonDown (bCircleButton)) {
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

		if (Input.GetAxis (LSVertical) > 0) {
			state = State.IN_HOLD;
		}

		if (Input.GetAxis (rightTrigger) < 0) {
			if (gm.bigBird.hold.platformCargo) {
				gm.bigBird.hold.Dump (gm.bigBird.hold.platformCargo.transform);
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
		HandleFiring ();
	}



	void HandleFiring () {
		if (p.w.automatic) {
			if (!p.w.firing && Input.GetAxis (rightTrigger) < 0) {
				//print ("!firing, and RT down: " + rightTrigger);
				p.w.firing = true;
				InvokeRepeating ("FireBullet", 0, p.w.fireRate);
			} else if (p.w.firing && Input.GetAxis (rightTrigger) >= 0) {
				//print ("firing, and RT up: " + rightTrigger);
				p.w.firing = false;
				p.CancelInvoke ();
				CancelInvoke ();
			}
		} else {
			if (Input.GetAxis (rightTrigger) < 0 && p.w.cocked && releasedRightTrigger && !p.w.reloading) {
				p.w.Fire (p.aim);
				releasedRightTrigger = false;
			} else if (Input.GetAxis (rightTrigger) >= 0) {
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
