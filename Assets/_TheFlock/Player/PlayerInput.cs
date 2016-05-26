using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

	public Turret turret;
	public float doubleTapThreshold = .5f;
	public string joystick = "unset";
	public LayerMask mask;
	public Transform station;

	public bool checkingInput = false;
	public bool releasedRightTrigger = true;
	public string LSHorizontal = "LS_Horizontal";
	public string LSVertical = "LS_Vertical";
	public string RSHorizontal = "RS_Horizontal";
	public string RSVertical = "RS_Vertical";
	public string rightTrigger = "RT";
	public string leftTrigger = "LT";
	public string rightClick = "R_Click";
	public string menuButton = "Menu";
	public string aButton = "A";
	public string bButton = "B";
	public string xButton = "X";
	public string yButton = "Y";
	public string LB = "LB";
	public string RB = "RB";

	public enum State {NEUTRAL, FLYING, CHANGING_STATIONS, DOCKED, ON_TURRET, IN_COCKPIT, IN_BUBBLE}
	public State state = State.DOCKED;
	public State selectedState = State.NEUTRAL;


	private GameManager gm;
	private SpriteRenderer sr;
	private BigBird bigBird;
	private Player p;

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

		if (joystick == "unset") {
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
			}
			return;
		}
		
		if (Input.GetButtonDown (menuButton)) {
			gm.TogglePause();
		}

		if (state == State.FLYING) {
			HandleFlyingInput ();
		}

		if (state != State.FLYING && Input.GetButtonDown (bButton)) {
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
			if (!p.w.firing && Input.GetAxis (rightTrigger) > 0) {
				//print ("!firing, and RT down: " + rightTrigger);
				p.w.firing = true;
				InvokeRepeating ("FireBullet", p.w.fireRate, p.w.fireRate);
			} else if (p.w.firing && Input.GetAxis (rightTrigger) <= 0) {
				//print ("firing, and RT up: " + rightTrigger);
				p.w.firing = false;
				p.CancelInvoke ();
				CancelInvoke ();
			}
		} else {
			if (Input.GetAxis (rightTrigger) > 0 && p.w.readyToFire && releasedRightTrigger && !p.w.reloading) {
				p.w.Fire (p.aim);
				releasedRightTrigger = false;
			} else if (Input.GetAxis (rightTrigger) <= 0) {
				releasedRightTrigger = true;
			}
		}

		if (Input.GetAxis (leftTrigger) > 0) {
			if (p.b.canBoost) {
				StartCoroutine( p.b.Boost ());
			}
		}

		if (Input.GetButtonDown (yButton)) {
			p.CycleWeapons ();
		}

		if (Input.GetButtonDown (xButton)) {
			p.StartCoroutine (p.Reload ());
		}

		if (Input.GetButtonDown (rightClick)) {
			if (!p.b.hasHarpoon) {
				p.b.DetachHarpoon ();
			} else {
				p.b.HurlHarpoon ();
			}
		}

		if (Input.GetButtonDown (LB)) {
			if (p.b.canRoll) {
				p.b.rolling = true;
				p.b.canRoll = false;
				StartCoroutine (p.b.EndRoll ());
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
		if (Input.GetButtonUp(bButton)) {
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

		if (Input.GetButtonDown (aButton)) {
			p.b.Undock ();
			if (!p.b.docked) {
				state = State.FLYING;
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
		if (bigBird.Landed) {
			if (Input.GetButtonDown (aButton)) {
				bigBird.LiftOff ();
			}
			return;
		}

		if (Input.GetButtonDown (aButton)) {
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
		LSHorizontal = "LS_Horizontal" + joystick;
		LSVertical = "LS_Vertical" + joystick;
		RSHorizontal = "RS_Horizontal" + joystick;
		RSVertical = "RS_Vertical" + joystick;
		rightTrigger = "RT" + joystick;
		leftTrigger = "LT" + joystick;
		rightClick = "R_Click" + joystick;
		menuButton = "Menu" + joystick;
		bButton = "B" + joystick;
		aButton = "A" + joystick;
		xButton = "X" + joystick;
		yButton = "Y" + joystick;
		LB = "LB" + joystick;
		RB = "RB" + joystick;
	}
}
