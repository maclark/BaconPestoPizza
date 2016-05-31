using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

	public int playerNum = 0;
	public bool checkingInput = false;
	public string joystick = "unset";
	public LayerMask mask;
	public Transform station;


	public string LSHorizontal = "LS_Horizontal";
	public string LSVertical = "LS_Vertical";
	public string RSHorizontal = "RS_Horizontal";
	public string RSVertical = "RS_Vertical";
	public string leftTrigger = "LT";
	public string rightTrigger = "RT";
	public string DPadHorizontal = "dph";
	public string DPadVertical = "dpv";
	public string backButton = "Back";
	public string startButton = "Start";
	public string aCrossButton = "A";
	public string bCircleButton = "B";
	public string xSquareButton = "X";
	public string yTriangleButton = "Y";
	public string LB = "LB";
	public string RB = "RB";
	public string leftClick = "L_Click";
	public string rightClick = "R_Click";

	public enum State {NEUTRAL, FLYING, CHANGING_STATIONS, DOCKED, ON_TURRET, IN_COCKPIT, IN_BUBBLE}
	public State state = State.DOCKED;
	public State selectedState = State.NEUTRAL;


	private GameManager gm;
	private SpriteRenderer sr;
	private BigBird bigBird;
	private Player p;
	private Turret turret;
	private bool releasedRightTrigger = true;
	private bool isXboxController = false;


	void Awake () {
		gm = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
		sr = GetComponent<SpriteRenderer> ();
		bigBird = GameObject.FindObjectOfType<BigBird>() as BigBird;
		p = GetComponent<Player> ();
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

		if (state == State.FLYING) {
			HandleFlyingInput ();
		}

		if (state != State.FLYING && Input.GetButtonDown (bCircleButton)) {
			PrepareToChangeStations ();
		}

		if (state == State.CHANGING_STATIONS) {
			HandleChangingStations ();
		} else if (state == State.ON_TURRET) {
			HandleTurretInput ();
		} else if (state == State.DOCKED) {
			HandleDockedInput ();
		} else if (state == State.IN_COCKPIT) {
			HandlePilotingInput ();
		}
	}

	void HandleFlyingInput () {

		if (playerNum == 6) {
			print (Input.GetAxisRaw (rightTrigger));
		}

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
			if (!p.b.hasHarpoon) {
				p.b.DetachHarpoon ();
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
		
	void FireBullet () {
		p.w.Fire (p.aim);
	}

	void PrepareToChangeStations () {
		if (state == State.DOCKED) {
			if (p.b) {
				p.UnboardBird (bigBird.transform);
			}
		}
		p.navigating = false;
		state = State.CHANGING_STATIONS;
	}

	void HandleChangingStations () {
		if (Input.GetButtonUp(bCircleButton)) {
			state = selectedState;
			sr.enabled = false;
			return;
		} 

		sr.enabled = true;
		RaycastHit2D hit = Physics2D.Raycast (transform.position, new Vector2 (Input.GetAxis(LSHorizontal), Input.GetAxis(LSVertical)), Mathf.Infinity, mask);
		if (hit) {
			if (station) {
				station.GetComponent<BoxCollider2D> ().enabled = true;
			}
			station = hit.collider.transform;
			station.GetComponent<BoxCollider2D> ().enabled = false;
			transform.position = station.position;

			if (hit.collider.name == "Cockpit") {
				selectedState = State.IN_COCKPIT;
			} else if (hit.collider.name == "Dock") {
				selectedState = State.DOCKED;
			} else if (hit.collider.name == "Turret") {
				turret = station.GetComponent<Turret> ();
				selectedState = State.ON_TURRET;
			}
		}
	}

	void HandleDockedInput () {
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
		turret.Rotate (new Vector3( Input.GetAxis(LSHorizontal), Input.GetAxis(LSVertical), 0f));
		if (Input.GetButtonDown (aCrossButton)) {
			turret.PressedA ();
		}

		if (Input.GetButtonDown (xSquareButton)) {
			turret.PressedX ();
		}
	}

	void HandlePilotingInput () {
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
