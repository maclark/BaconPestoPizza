using UnityEngine;
using System.Collections;

public class StickHandler{
	//TODO make player input not use these
	public bool verticalStickInUse = false;
	public bool horizontalStickInUse = false;
	/// <summary>
	/// //////////////////////
	/// </summary>

	public float moveCooldown = .2f;
	public bool LStickInUse = false;

	private GameManager gm;
	private float timeOfLastStickUse;

	public StickHandler () {
		gm = GameObject.FindObjectOfType<GameManager> ();
	}
		
	public void HandleInHoldSticks (Player p, string LSVertical, string LSHorizontal) {
		if (Input.GetAxisRaw (LSVertical) != 0 || Input.GetAxisRaw (LSHorizontal) != 0) {
			if (LStickInUse == false) {
				LStickInUse = true;
				timeOfLastStickUse = Time.time;
				Vector3 dir = new Vector3 (Input.GetAxis (LSHorizontal), Input.GetAxis (LSVertical), 0);
				float upness = Vector3.Dot (dir, gm.bigBird.transform.up);
				float overness = Vector3.Dot (dir, gm.bigBird.transform.right);
				if (Mathf.Abs (upness) > Mathf.Abs (overness)) {
					//move up/down relative to player
					if (upness > 0) {
						gm.bigBird.hold.SelectorStep (p.transform, 0, 1);
					} else {
						gm.bigBird.hold.SelectorStep (p.transform, 0, -1);
					}
				} else {
					//move right/left relative to player
					if (overness > 0) {
						gm.bigBird.hold.SelectorStep (p.transform, 1, 0);
					} else {
						gm.bigBird.hold.SelectorStep (p.transform, -1, 0);
					}
				}
			} else if (Time.time > timeOfLastStickUse + moveCooldown) {
				LStickInUse =  false;
			}
		} else {
			LStickInUse = false;
		}
	}


	public void HandleOnPlatformSticks (Player p, string LSVertical, string LSHorizontal) {
		Vector3 dir = new Vector3 (Input.GetAxis (LSHorizontal), Input.GetAxis (LSVertical), 0);
		float upness = Vector3.Dot (dir, gm.bigBird.transform.up);
		float overness = Vector3.Dot (dir, gm.bigBird.transform.right);
		if (Mathf.Abs (upness) > Mathf.Abs (overness)) {
			//move up/down relative to player
			if (upness > 0) {
				LStickInUse = true;
				timeOfLastStickUse = Time.time;

				p.GetComponent<PlayerInput> ().state = PlayerInput.State.IN_HOLD;
				gm.bigBird.hold.SelectorStep (p.transform, 0, 1);
			}
		}
	}
}
