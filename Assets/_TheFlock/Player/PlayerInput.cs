using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

	public Turret turret;
	public float doubleTapThreshold = .5f;
	public string joystick = "unset";
	public bool playerStarted = false;
	public LayerMask mask;

	public bool checkingInput = false;
	public string LSHorizontal = "LS_Horizontal";
	public string LSVertical = "LS_Vertical";
	public string RSHorizontal = "RS_Horizontal";
	public string RSVertical = "RS_Vertical";
	public string rightTrigger = "RT";
	public string leftTrigger = "LT";
	public string rightClick = "R_Click";
	public string menuButton = "Menu";
	public string aButton = "A";
	public string xButton = "X";
	public string bButton = "B";
	public string LB = "LB";
	public string RB = "RB";

	public enum State {flying, changingStations, docked, onTurret, inCockpit}
	public State state = State.docked;
	public State selectedState = State.docked;
	private GameManager gm;
	private SpriteRenderer sr;
	private BigBird bigBird;
	private Player p;
	private Transform station;

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
					print("pressed a joystick 1 button: " + i);
				}

				if (Input.GetKeyDown("joystick 2 button "+i)) {
					// do something
					print("pressed a joystick 2 button: " + i);
				}

				if (Input.GetKeyDown("joystick 3 button "+i)) {
					// do something
					print("pressed a joystick 3 button: " + i);
				}

				if (Input.GetKeyDown("joystick 4 button "+i)) {
					// do something
					print("pressed a joystick 4 button: " + i);
				}

				if (Input.GetKeyDown("joystick 5 button "+i)) {
					// do something
					print("pressed a joystick 5 button: " + i);
				}

				if (Input.GetKeyDown("joystick 6 button "+i)) {
					// do something
					print("pressed a joystick 6 button: " + i);
				}
			}
		}

		#endregion

		if (!playerStarted) {
			if (Input.GetButtonDown (menuButton)) {
				if (menuButton == "Menu_P1") {
					joystick = "_P1";
				} else if (menuButton == "Menu_P2") {
					joystick = "_P2";
				} else if (menuButton == "Menu_P3") {
					joystick = "_P3";
				} else if (menuButton == "Menu_P4") {
					joystick = "_P4";
				} else if (menuButton == "Menu_P5") {
					joystick = "_P5";
				} else if (menuButton == "Menu_P6") {
					joystick = "_P6";
				} else if (menuButton == "Menu_P7") {
					joystick = "_P7";
				} else if (menuButton == "Menu_P8") {
					joystick = "_P8";
				}
			} 
				
			if (joystick == "unset") {
			} else {
				SetButtonNames ();
				p.StartPlayer ();
				state = State.docked;
				station = p.b.GetDock ().transform;
				playerStarted = true;
				joystick = "set";
			}
			return;
		}
		
		if (Input.GetButtonDown (menuButton)) {
			gm.TogglePause();
		}

		if (state == State.flying) {
			HandleFlyingInput ();
		}

		if (Input.GetButtonDown (bButton)) {
			state = State.changingStations;
		}

		if (state == State.changingStations) {
			HandleChangingStations ();
		} else if (state == State.onTurret) {
			HandleTurretInput ();
		} else if (state == State.docked) {
			HandleDockedInput ();
		} else if (state == State.inCockpit) {
			HandlePilotingInput ();
		}
	}

	void HandleFlyingInput () {
		p.b.direction = new Vector2( Input.GetAxis(LSHorizontal), Input.GetAxis(LSVertical));
		if (p.b.gas <= 0) {
			p.b.direction = Vector2.zero;
		}

		Vector2 rightStick = new Vector2( Input.GetAxis(RSHorizontal), Input.GetAxis(RSVertical));
		if (rightStick != Vector2.zero) {
			p.b.aim = rightStick;
		} 
		else if (p.b.aim == Vector2.zero) {
			if (p.b.direction != Vector2.zero) {
				p.b.aim = p.b.direction;
			}
		}

		if (!p.b.firing && Input.GetAxis (rightTrigger) > 0) {
			//print ("!firing, and RT down: " + rightTrigger);
			p.b.firing = true;
			InvokeRepeating ("FireBullet", p.b.fireRate, p.b.fireRate);
		} else if (p.b.firing && Input.GetAxis (rightTrigger) <= 0) {
			//print ("firing, and RT up: " + rightTrigger);
			p.b.firing = false;
			p.CancelInvoke ();
			CancelInvoke ();
		}

		if (Input.GetButtonDown (rightClick)) {
			if (!p.b.hasHarpoon) {
				p.b.DetachHarpoon ();
			} else {
				p.b.HurlHarpoon ();
			}
		}

		if (Input.GetButton (LB)) {
			if (p.b.canBoost) {
				StartCoroutine( p.b.Boost ());
			}
		}
	}
		
	void FireBullet () {
		p.b.FireBullet ();
	}

	void HandleChangingStations () {
		if (Input.GetButtonUp(bButton)) {
			state = selectedState;
			sr.enabled = false;
			return;
		} 

		sr.enabled = true;
		int stationsMask = LayerMask.NameToLayer ("Stations");
		int nonStationsMask = ~stationsMask;
		RaycastHit2D hit = Physics2D.Raycast (transform.position, new Vector2 (Input.GetAxis(LSHorizontal), Input.GetAxis(LSVertical)), Mathf.Infinity, mask);
		if (hit) {
			if (hit.collider.name == "Cockpit") {
				station.GetComponent<BoxCollider2D> ().enabled = true;
				station = hit.collider.transform;
				station.GetComponent<BoxCollider2D> ().enabled = false;
				transform.position = station.position;
				selectedState = State.inCockpit;
			} else if (hit.collider.name == "Dock") {
				station.GetComponent<BoxCollider2D> ().enabled = true;
				station = hit.collider.transform;
				station.GetComponent<BoxCollider2D> ().enabled = false;
				//p.BoardBird (station.GetComponent<Bird> ());
				transform.position = station.position;
				selectedState = State.docked;
			} else if (hit.collider.name == "Turret") {
				station.GetComponent<BoxCollider2D> ().enabled = true;
				station = hit.collider.transform;
				station.GetComponent<BoxCollider2D> ().enabled = false;
				transform.position = station.position;
				selectedState = State.onTurret;
			}
		}
	}

	void HandleDockedInput () {
		if (Input.GetButtonDown (aButton)) {
			if (p.b) {
				p.b.Undock ();
				state = State.flying;
			}
		}
	}

	void HandleTurretInput () {
		turret.Rotate (new Vector3( Input.GetAxis(LSHorizontal), Input.GetAxis(LSVertical), 0f));
		if (Input.GetButtonDown (aButton)) {
			turret.PressedA ();
		}

		if (Input.GetButtonDown (xButton)) {
			turret.PressedX ();
		}
	}

	void HandlePilotingInput () {
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

		if (Input.GetButtonDown (aButton)) {
			p.navigating = !p.navigating;
			gm.TogglePause ();
			gm.ToggleNavPanel (LSHorizontal, LSVertical);
		}
	}
		

	void SetButtonNames () {
		LSHorizontal = "LS_Horizontal" + joystick;
		LSVertical = "LS_Vertical" + joystick;
		RSHorizontal = "RS_Horizontal" + joystick;
		RSVertical = "RS_Vertical" + joystick;
		rightTrigger = "RT" + joystick;
		leftTrigger = "LT" + joystick;
		rightClick = "R_Click" + joystick;
		menuButton = "Menu" + joystick;
		aButton = "A" + joystick;
		xButton = "X" + joystick;
		bButton = "B" + joystick;
		LB = "LB" + joystick;
		RB = "RB" + joystick;
	}
}
