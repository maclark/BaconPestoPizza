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

	void Start ()	{
		container = new GameObject (pooledObject.name + "Container");

		pooledObjects = new List<GameObject>();
		for(int i = 0; i < pooledAmount; i++)
		{
			GameObject obj = (GameObject)Instantiate(pooledObject);
			obj.GetComponent<SpriteRenderer> ().color = color;
			obj.SetActive(false);
			obj.transform.parent = container.transform;
			pooledObjects.Add(obj);
		}
	}

	public GameObject GetPooledObject()
	{
		for(int i = 0; i< pooledObjects.Count; i++)
		{
			if(pooledObjects[i] == null)
			{
				GameObject obj = (GameObject)Instantiate(pooledObject);
				obj.GetComponent<SpriteRenderer> ().color = color;
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
			obj.GetComponent<SpriteRenderer> ().color = color;
			pooledObjects.Add(obj);
			obj.transform.parent = container.transform;
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
}