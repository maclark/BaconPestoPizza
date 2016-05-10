using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

	public string LSHorizontal = "LS_Horizontal_P1";
	public string LSVertical = "LS_Vertical_P1";
	public string RSHorizontal = "RS_Horizontal_P1";
	public string RSVertical = "RS_Vertical_P1";
	public string rightTrigger = "RT_P1";
	public string leftTrigger = "LT_P1";
	public string rightClick = "R_Click_P1";
	public string menuButton = "Menu_P1";
	public string aButton = "A_P1";
	public string interactButton = "B_P1";
	public string LBumper = "LB_P1";

	private GameManager gm;
	private BigBird bigBird;
	private Player p;

	void Awake () {
		gm = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
		bigBird = GameObject.FindObjectOfType<BigBird>() as BigBird;
		p = GetComponent<Player> ();
	}

	void Update () {

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
			p.firing = true;
			InvokeRepeating ("FireBullet", p.fireRate, p.fireRate);
		} else if (p.firing && Input.GetAxis (rightTrigger) <= 0) {
			p.firing = false;
			p.CancelInvoke ();
			CancelInvoke ();
		}

		if (Input.GetButtonDown (rightClick)) {
			p.HarpoonAction ();
		}

		if (Input.GetButton (LBumper)) {
			if (p.canBoost) {
				StartCoroutine( p.Boost ());
			}
		}
		#region
		else if (Input.GetButton ("10_P1")) {
			print ("10_P1");
		}
		else if (Input.GetButton ("11_P1")) {
			print ("11_P1");
		}
		else if (Input.GetButton ("12_P1")) {
			print ("12_P1");
		}
		else if (Input.GetButton ("13_P1")) {
			print ("13_P1");
		}
		else if (Input.GetButton ("14_P1")) {
			print ("14_P1");
		}
		else if (Input.GetButton ("15_P1")) {
			print ("15_P1");
		}
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

		if (Input.GetButtonDown (LBumper)) {
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
		
}
