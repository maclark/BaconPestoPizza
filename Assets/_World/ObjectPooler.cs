using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour
{
	public GameObject pooledObject;
	public int pooledAmount = 20;
	public bool willGrow = true;

	public List<GameObject> pooledObjects;

	private GameObject container;
	private Color color = Color.magenta;
	private Transform owner;

	void Start ()	{
		if (container == null) {
			container = new GameObject (pooledObject.name + "Container");
		}

		pooledObjects = new List<GameObject>();
		for(int i = 0; i < pooledAmount; i++)
		{
			GameObject obj = (GameObject)Instantiate(pooledObject);
			if (obj.GetComponent<SpriteRenderer> ()) {
				obj.GetComponent<SpriteRenderer> ().color = color;
			}
			if (obj.GetComponent<Projectile> ()) {
				obj.GetComponent<Projectile> ().owner = owner;
			}
			obj.SetActive(false);
			obj.transform.parent = container.transform;
			pooledObjects.Add(obj);
		}
	}

	public void SetPooledObject (GameObject objectWanted) {
		pooledObject = objectWanted;
	}

	public void SetContainer (GameObject containerWanted) {
		container = containerWanted;
	}

	public GameObject GetPooledObject()
	{
		for (int i = 0; i < pooledObjects.Count; i++)
		{
			if(pooledObjects[i] == null)
			{
				GameObject obj = (GameObject)Instantiate(pooledObject);
				if (obj.GetComponent<SpriteRenderer> ()) { 
					obj.GetComponent<SpriteRenderer> ().color = color;
				}
				obj.GetComponent<Projectile> ().owner = owner;
				obj.SetActive(false);
				pooledObjects[i] = obj;
				return pooledObjects[i];
			}
			if(!pooledObjects[i].activeInHierarchy)
			{
				return pooledObjects[i];
			}    
		}

		if (willGrow)
		{
			GameObject obj = (GameObject)Instantiate(pooledObject);
			if (obj.GetComponent<SpriteRenderer> ()) {
				obj.GetComponent<SpriteRenderer> ().color = color;
			}
			if (obj.GetComponent<Projectile> ()) {
				obj.GetComponent<Projectile> ().owner = owner;
			}
			obj.transform.parent = container.transform;
			pooledObjects.Add(obj);
			return obj;
		}

		return null;
	}

	public void SetPooledObjectsColor (Color poolerColor) {
		color = poolerColor;

		foreach (GameObject obj in pooledObjects) {
			obj.GetComponent<SpriteRenderer> ().color = color;
		}
	}

	//#TODO could set other properties, like tags
	public void SetPooledObjectsOwner (Transform t) {
		owner = t;
		foreach (GameObject obj in pooledObjects) {
			obj.GetComponent<Projectile> ().owner = owner;
		}
	}
}