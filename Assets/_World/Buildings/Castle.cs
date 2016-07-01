using UnityEngine;
using System.Collections;

public class Castle : MonoBehaviour {
	public GameObject colliderChild;


	public BoxCollider2D[] GetColliderChild () {
		BoxCollider2D[] colls = new BoxCollider2D[1];
		colls [0] = colliderChild.GetComponent<BoxCollider2D> ();
		return colls;
	}
}
