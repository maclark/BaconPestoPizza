using UnityEngine;
using System.Collections;

public class Shop : MonoBehaviour {

	public GameObject colliderChild;



	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public BoxCollider2D[] GetColliderChild () {
		BoxCollider2D[] colls = new BoxCollider2D[1];
		colls [0] = colliderChild.GetComponent<BoxCollider2D> ();
		return colls;
	}
}
