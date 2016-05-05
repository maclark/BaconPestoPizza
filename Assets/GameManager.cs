using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public BigBird bigBird;
	public Player player1;
	//private Player player2;
	//private Player player3;
	//private Player player4;

	private List<Transform> alliedTransforms; 


	// Use this for initialization
	void Awake () {
		bigBird = GameObject.FindGameObjectWithTag ("BigBird").GetComponent<BigBird> ();
		player1 = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();
		alliedTransforms = new List<Transform> ();
		AddAlliedTransform (bigBird.transform);
		AddAlliedTransform (player1.transform);
	}

	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public List<Transform> GetAlliedTransforms () {
		return alliedTransforms;
	}

	public void RemoveAlliedTransform (Transform toRemove) {
		alliedTransforms.Remove (toRemove);
	}

	public void AddAlliedTransform (Transform toAdd) {
		alliedTransforms.Add (toAdd);
	}
	

}
