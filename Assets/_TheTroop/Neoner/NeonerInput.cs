using UnityEngine;
using System.Collections;

public class NeonerInput : MonoBehaviour {

	public int playerNum = 0;
	public bool checkingInput = false;
	public bool canHighlight = false;
	public string joystick = "unset";
	public float moveCooldown = .2f;
	public float stationChangeDistance = 10f;
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

	public float FakeLSVerticalRaw;
	public float FakeLSHorizontalRaw;
	public float FakeLSHorizontal;
	public float FakeLSVertical;
	public float FakeRSHorizontal;
	public float FakeRSVertical;
	public float FakeLeftTrigger;
	public float FakeRightTrigger;
	public float FakeDPadHorizontal;
	public float FakeDPadVertical;

	public bool FakebackButton;
	public bool FakestartButton;
	public bool FakeACrossButtonDown;
	public bool FakeBCircleButton;
	public bool FakeBCircleButtonUp;
	public bool FakeXSquareButton;
	public bool FakeXSquareButtonDown;
	public bool FakeYTriangleButton;
	public bool FakeYTriangleButtonDown;
	public bool FakeLBButtonDown;
	public bool FakeRBButtonDown;
	public bool FakeLeftClickButtonDown;
	public bool FakeRightClickButtonDown;


	public enum State {NEUTRAL, FLYING, CHANGING_STATIONS, DOCKED, ON_TURRET, PILOTING, NAVIGATING, IN_BUBBLE, IN_WEB, POWERBIRD, IN_HOLD, ON_PLATFORM, IN_COOP, ON_FOOT}
	public State state = State.DOCKED;
	public State selectedState = State.NEUTRAL;


	//private GameManager gm;
	private SpriteRenderer sr;
	private Neoner n;
	//private Turret turret;
	//private bool releasedRightTrigger = true;
	//private bool isXboxController = false;
	private bool stationcasting = false;
	private float timeOfStationcast;

	void Awake () {
		//gm = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
		sr = GetComponent<SpriteRenderer> ();
		n = GetComponent<Neoner> ();
	}

	void Update () {
		if (state == State.NEUTRAL) {
			HandleNeutralInput ();
		}  else if (state == State.FLYING) {
			HandleFlyingInput ();
		}  else if (state == State.CHANGING_STATIONS) {
			HandleChangingStations ();
		}  else if (state == State.ON_TURRET) {
			HandleTurretInput ();
		}  else if (state == State.DOCKED) {
			HandleDockedInput ();
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
			if (n.kanga) {
				n.Dekanga (station);
			}		
		} 

		if (station) {
			station.GetComponent<BoxCollider2D> ().enabled = true;
		}
	}

	void ManStation () {
		state = selectedState;
		if (state == State.ON_TURRET) {
			station.GetComponentInChildren<SpriteRenderer> ().color = n.color;
			return;
		}  else if (state == State.PILOTING) {
			station.GetComponentInChildren<SpriteRenderer> ().color = new Color(.37f, .26f, .15f);
			sr.sortingOrder = 0;
			return;
		}  else if (state == State.ON_FOOT) {
			n.Disembark (station.transform.position);
			return;
		}

		sr.enabled = false;

	}

	void HandleChangingStations () {
		//TODO can get stuck in changing stations at start because of being in Neutral
		if (FakeBCircleButton) {
			ManStation ();
			return;
		}  
		sr.enabled = true;

		float horizontalness = 0f;
		float verticalness = 0f;

		if (FakeLSHorizontalRaw != 0 || FakeLSVerticalRaw != 0) {
			if (stationcasting == false) {
				stationcasting = true;
				timeOfStationcast = Time.time;
				verticalness = FakeLSVertical;
				horizontalness = FakeLSHorizontal;
			}  else if (Time.time > timeOfStationcast + moveCooldown) {
				stationcasting =  false; 
			}

		}  else if (FakeLSHorizontalRaw == 0 && FakeLSVerticalRaw == 0) {
			stationcasting =  false;
		}  

		RaycastHit2D hit = Physics2D.Raycast (transform.position, new Vector2 (horizontalness, verticalness), stationChangeDistance, mask);
		if (hit) {
			if (hit.collider.name == "BoardingZone") {
				return;
			}

			if (station) {
				station.GetComponent<BoxCollider2D> ().enabled = true;
			}
			station = hit.collider.transform;
			station.GetComponent<BoxCollider2D> ().enabled = false;
			transform.position = station.position;

			if (hit.collider.name == "Cockpit") {
				selectedState = State.PILOTING;
			}  else if (hit.collider.name == "Dock") {
				selectedState = State.DOCKED;
			}  else if (hit.collider.name == "Turret") {
				//turret = station.GetComponent<Turret> ();
				selectedState = State.ON_TURRET;
			}  else if (hit.collider.name == "BoardingZone") {
				selectedState = State.ON_FOOT;
			}
		}
	}

	void HandleNeutralInput () {
		if (FakeBCircleButton) {
			state = State.CHANGING_STATIONS;
			return;
		}
	}

	void HandleFlyingInput () {
		n.kanga.badInputDirection = new Vector2 (FakeLSHorizontal, FakeLSVertical);

		Vector2 rightStick = new Vector2 (FakeRSHorizontal, FakeRSVertical);
		if (rightStick != Vector2.zero) {
			n.aim = rightStick;
		}  else if (n.aim == Vector3.zero) {
			if (n.kanga.badInputDirection != Vector2.zero) {
				n.aim = new Vector3 (n.kanga.badInputDirection.x, n.kanga.badInputDirection.y, 0);
			}
		}
		n.aim.Normalize ();

		if (!n.kanga.hurledHarp && !n.kanga.harpLoaded && !n.kanga.catchingHarp) {
			if (FakeLeftTrigger > 0) {
				n.kanga.LoadHarp ();
			}  
		}  else if (n.kanga.hurledHarp) {
			if (FakeLeftTrigger > 0) {
				n.kanga.harp.SetRecalling (true);
			}  else if (FakeLeftTrigger <= 0) {
				n.kanga.harp.SetRecalling (false);
			}
		}  else if (n.kanga.harpLoaded) {
			if (FakeLeftTrigger <= 0) {
				n.kanga.SwingHurlHarpoon ();
			}
		}  else if (n.kanga.catchingHarp) {
			if (FakeLeftTrigger <= 0) {
				n.kanga.catchingHarp = false;
			}
		}

		if (!n.kanga.harpLoaded) {
			HandleWeaponFiring ();
		}

		if (FakeBCircleButtonUp) {
			if (n.kanga.canRoll) {
				n.kanga.rolling = true;
				n.kanga.canRoll = false;
				StartCoroutine (n.kanga.EndRoll ());
			}
		}

		if (FakeYTriangleButtonDown) {
			if (n.kanga.harpLoaded) {
				n.kanga.UnloadHarp ();
				n.kanga.catchingHarp = true;
			}
		}

		if (FakeXSquareButtonDown) {
			if (n.w.roundsLeftInClip <n.w.clipSize && !n.w.reloading) {
				n.StartCoroutine (n.w.Reload ());
			}
		}

		if (FakeRightClickButtonDown) {
			if (n.kanga.hurledHarp) {
				n.kanga.harp.ToggleGripping ();
			}
		}

		if (FakeRBButtonDown) {
			if (n.kanga.canBoost) {
				StartCoroutine(n.kanga.Boost ());
			}
		}
	}

	void HandleDockedInput () {
		if (FakeBCircleButton) {
			if (n.kanga) {
				n.Dekanga (station);
			}		
			state = State.CHANGING_STATIONS;
			return;
		}

		if (n.kanga == null && station.GetComponent<Dock> ().kanga) {
			n.BoardKanga (station.GetComponent<Dock> ().kanga);
		}

		if (FakeACrossButtonDown) {
			n.kanga.Undock ();
			if (!n.kanga.docked) {
				state = State.FLYING;
			}
		}
	}

	void HandleTurretInput () {
		/*
		if (FakeBCircleButton) {
			state = State.CHANGING_STATIONS;
			turret.transform.GetComponentInChildren<SpriteRenderer> ().color = Color.black;
			return;
		}

		turret.Rotate (new Vector3( FakeLSHorizontal, FakeLSVertical, 0f));
		if (FakeRightTrigger < 0) {
			turret.RightTrigger ();
		}

		if (FakeACrossButtonDown) {
			turret.PressedA ();
		}*/
	}
		

	void HandleOnFootInput () {
		if (FakeBCircleButton) {
			n.body.PressedB ();
			return;
		}

		n.body.direction = new Vector2 (FakeLSHorizontal, FakeLSVertical);

		Vector2 rightStick = new Vector2 (FakeRSHorizontal, FakeRSVertical);
		if (rightStick != Vector2.zero) {
			n.aim = rightStick;
		}  else if (n.aim == Vector3.zero) {
			if (n.body.direction != Vector2.zero) {
				n.aim = new Vector3 (n.body.direction.x, n.body.direction.y, 0);
			}
		}
	}
	void HandleWeaponFiring () {
	}
	/*

	void FireBullet () {
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
	}*/
}
