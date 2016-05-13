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
	private Player p;
	private int harpButtonCount = 0;
	private float harpButtonCooler = .5f;

	void Awake () {
		gm = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
		bigBird = GameObject.FindObjectOfType<BigBird>() as BigBird;
		p = GetComponent<Player> ();
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
				//p.StartBird ();
				started = true;
			}
			return;
		}
		
		if (Input.GetButtonDown (menuButton)) {
			gm.TogglePause();
		}

		if (p.b.docked) {
			HandleDockedInput ();
		} else {
			HandleFlyingInput ();
		}
	}

	void HandleFlyingInput () {
		p.b.direction = new Vector2( Input.GetAxis(LSHorizontal), Input.GetAxis(LSVertical));
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
			if (harpButtonCooler > 0 && harpButtonCount == 1) {
				p.b.DetachHarpoon ();
			} else {
				harpButtonCooler = doubleTapThreshold;
				harpButtonCount += 1;
				p.b.HarpoonAction ();
			}
		}

		if (harpButtonCooler > 0) {
			harpButtonCooler -= Time.deltaTime;
		} else {
			harpButtonCount = 0;
		}

		if (Input.GetButton (LB)) {
			if (p.b.canBoost) {
				StartCoroutine( p.b.Boost ());
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
			if (p.b) {
				p.b.Undock ();
			} else {
				//navigating = false;
				//gm.CloseNavPanel ();
			}
		} else {
			float x = Input.GetAxis (LSHorizontal);
			float y = Input.GetAxis(LSVertical); 

			if (Mathf.Sqrt (x * x + y * y) > bigBird.thresholdToTurnBigBird) {
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
		p.b.FireBullet ();
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
