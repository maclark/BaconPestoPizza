using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {

	public Transform holder;
	public float max = 1;
	public float current = 1;

	public void AdjustHealth (float newHealth) {
		current = newHealth;
		holder.localScale = new Vector3 (current / max, 1, 1);
	}
}
