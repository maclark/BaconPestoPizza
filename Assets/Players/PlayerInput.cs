using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

	public float doubleTapThreshold = .5f;
	public string joystick = "unset";
	public bool started = false;

	public string LSHorizontal = "LS_Horizontal";
	public string LSVertical = "LS_Vertical";
	public string RSHorizontal = "RS_Horizontal";
	public string RSVertical = "RS_Vertical";
	public string rightTrigger = "RT";
	public string leftTrigger = "LT";
	public string rightClick = "R_Click";
	public string menuButton = "Menu";
	public string aButton = "A";
	public string interactButton = "B";
	public string LB = "LB";
	public string RB = "RB";

	private GameManager gm;
	private BigBird bigBird;
	private Bird p;
	private int harpButtonCount = 0;
	private float harpButtonCooler = .5f;

	void Awake () {
		gm = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
		bigBird = GameObject.FindObjectOfType<BigBird>() as BigBird;
		p = GetComponent<Bird> ();
	}

	void Update () {

		if (!started) {
			if (Input.GetButtonDown (menuButton)) {
				if (menuButton == "Menu_P1") {
					joystick = "_P1";
				} else if (menuButton == "Menu_P2") {
					joystick = "_P2";
				} else if (menuButton == "Menu_P3") {
					joystick = "_P3";
				}
			}
				
			if (joystick == "unset") {
			} else {
				SetButtonNames ();
				p.StartPlayer ();
				started = true;
			}
			return;
		}
		
		if (Input.GetButtonDown (menuButton)) {
			gm.TogglePause();
		}

		if (p.docked) {
			HandleDockedInput ();
		} else {
			HandleFlyingInput ();
		}
	}

	void HandleFlyingInput () {
		p.direction = new Vector2( Input.GetAxis(LSHorizontal), Input.GetAxis(LSVertical));
		Vector2 rightStick = new Vector2( Input.GetAxis(RSHorizontal), Input.GetAxis(RSVertical));
		if (rightStick != Vector2.zero) {
			p.aim = rightStick;
		} 
		else if (p.aim == Vector2.zero) {
			if (p.direction != Vector2.zero) {
				p.aim = p.direction;
			}
		}

		if (!p.firing && Input.GetAxis (rightTrigger) > 0) {
			//print ("!firing, and RT down: " + rightTrigger);
			p.firing = true;
			InvokeRepeating ("FireBullet", p.fireRate, p.fireRate);
		} else if (p.firing && Input.GetAxis (rightTrigger) <= 0) {
			//print ("firing, and RT up: " + rightTrigger);
			p.firing = false;
			p.CancelInvoke ();
			CancelInvoke ();
		}

		if (Input.GetButtonDown (rightClick)) {
			if (harpButtonCooler > 0 && harpButtonCount == 1) {
				p.DetachHarpoon ();
			} else {
				harpButtonCooler = doubleTapThreshold;
				harpButtonCount += 1;
				p.HarpoonAction ();
			}
		}

		if (harpButtonCooler > 0) {
			harpButtonCooler -= Time.deltaTime;
		} else {
			harpButtonCount = 0;
		}

		if (Input.GetButton (LB)) {
			if (p.canBoost) {
				StartCoroutine( p.Boost ());
			}
		}
		#region
		////For mapping game pad
		/*
		for (int i = 0; i < 20; i++) {
			if (Input.GetKeyDown("joystick 2 button "+i)) {
				// do something
				print("pressed a joystick 2 button: " + i);
			}
		}

		for (int i = 0; i < 20; i++) {
			if (Input.GetKeyDown("joystick 1 button "+i)) {
				// do something
				print("pressed a joystick 1 button: " + i);
			}
		}

		for (int i = 0; i < 20; i++) {
			if (Input.GetKeyDown("joystick 3 button "+i)) {
				// do something
				print("pressed a joystick 3 button: " + i);
			}
		}
		*/

		#endregion
	}

	void HandleDockedInput () {
		if (Input.GetButtonDown (interactButton)) {
			if (!p.navigating) {
				p.Undock ();
			} else {
				//navigating = false;
				//gm.CloseNavPanel ();
			}
		} else {
			float x = Input.GetAxis (LSHorizontal);
			float y = Input.GetAxis(LSVertical); 

			if (Mathf.Sqrt (x * x + y * y) > p.thresholdToTurnBigBird) {
				float angle = Mathf.Atan2 (Input.GetAxis (LSHorizontal), Input.GetAxis (LSVertical)) * Mathf.Rad2Deg;
				bigBird.turning = true;
				bigBird.SetTargetRotationZAngle (-angle);
			} else
				bigBird.turning = false;
		}

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

	void FireBullet () {
		p.FireBullet ();
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
		interactButton = "B" + joystick;
		LB = "LB" + joystick;
		RB = "RB" + joystick;
	}
}
