using UnityEngine;
using System.Collections;

public class Eaglehead : Item {

	public int minimumReigners = 1;
	public bool ready;
	public GameObject powerbirdPrefab;
	public static float colorSwapSpeed = .5f;
	public static float readySwapSpeed = .1f;


	private Harpoonable hool;

	void Awake () {
		hool = GetComponent<Harpoonable> ();
		OnAwake ();
	}

	void Start () {
		InvokeRepeating ("RandomColor", colorSwapSpeed, colorSwapSpeed);
	}

	void OnCollisionEnter2D (Collision2D coll) {
		if (ready) {
			Bird birdie = coll.transform.GetComponent<Bird> ();
			if (birdie) {
				if (birdie.harp) {
					if (birdie.harp.GetHarpooned ()) {
						if (birdie.harp.GetHarpooned ().transform != transform) {
							SummonThePowerbird (coll.transform.GetComponent<Bird> ());
						}
					} else {
						SummonThePowerbird (coll.transform.GetComponent<Bird> ());
					}
				} else {
					SummonThePowerbird (coll.transform.GetComponent<Bird> ());
				}
			}
		}
		CollisionEnter2D (coll);
	}

	void OnCollisionExit2D (Collision2D coll) {
		CollisionExit2D (coll);
	}

	void RandomColor () {
		sr.color = Random.ColorHSV ();
	}

	public override void Drop (Player p, string sortLayerName, int sortOrder, bool droppedItem=false, bool canDrop=true) {
		base.Drop (p, sortLayerName, sortOrder, droppedItem, canDrop);
	}

	public void BeenHarpooned (Harpoon h) {
		if (hool.GetOtherHarps ().Count >= minimumReigners) {
			ready = true;
			CancelInvoke ();
			InvokeRepeating ("RandomColor", readySwapSpeed, readySwapSpeed);

		}
	}

	public void HarpoonReleased (Harpoon h) {
		if (hool.GetOtherHarps ().Count < minimumReigners) {
			ready = false;
			CancelInvoke ();
			InvokeRepeating ("RandomColor", colorSwapSpeed, colorSwapSpeed);
		}
	}

	void SummonThePowerbird (Bird poweredBird) {
		GameObject pbObj = Instantiate (powerbirdPrefab, transform.position, Quaternion.identity) as GameObject;
		Powerbird pb = pbObj.GetComponent<Powerbird> ();
		Transform[] powerers = new Transform[hool.GetOtherHarps ().Count];
		int i = 0;
		foreach (Harpoon h in hool.GetOtherHarps ()) {
			powerers [i] = h.GetHarpooner ().transform;
			i++;
		}
		pb.thePowerbird = poweredBird.transform;
		pb.SetPowerers (powerers);

		poweredBird.Powered (pb);
		poweredBird.GetComponent<Rigidbody2D> ().velocity = GetComponent<Rigidbody2D> ().velocity;
		Destroy (gameObject);
	}
}
