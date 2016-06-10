using UnityEngine;
using System.Collections;

public class ResourceBar : MonoBehaviour {

	public Transform holder;
	public float capacity = 1;
	public float current = 1;
	public bool full = false;
	public bool empty = false;

	public void SetResource (float newAmount) {
		current = newAmount;
		if (current >= capacity) {
			current = capacity;
			full = true;
		}
		holder.localScale = new Vector3 (current / capacity, 1, 1);
	}

	public void IncreaseResource (float increase) {
		
		current += increase;

		if (empty) {
			empty = false;
		}

		if (current >= capacity) {
			current = capacity;
			full = true;
		}
		holder.localScale = new Vector3 (current / capacity, 1, 1);
	}

	public void DecreaseResource (float decrease) {
		current -= decrease;

		if (full) {
			full = false;
		}

		if (current <= 0) {
			current = 0;
			empty = true;
		}

		holder.localScale = new Vector3 (current / capacity, 1, 1);
	}
}
