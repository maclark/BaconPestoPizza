using UnityEngine;
using System.Collections;

public abstract class Station : MonoBehaviour {

	public enum StationType {DOCK, TURRET, COCKPIT, CARGO_HOLD, LOADING_PLATFORM}

	protected StationType type;
	protected Player user;
	protected PlayerInput pi;
	protected GameManager gm;

	protected void OnAwake () {
		gm = GameObject.FindObjectOfType<GameManager> ();
	}

	public void SetStationType (StationType t) {
		type = t;
	}

	public abstract void HandleInput ();

	public abstract void Man (Player p);

	public abstract void Abandon ();

	public abstract void MakeAvailable ();

	public abstract void MakeUnavailable ();

	public StationType GetStationType () {
		return type;
	}
}
