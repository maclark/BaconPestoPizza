using UnityEngine;
using System.Collections;

public class Eggscript : MonoBehaviour {
	public Time age;
	public float birthTime;
	public float EggHatchDelay=30.0f;
	public float RollInterval=15.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}
	void Awake (){
		birthTime = Time.realtimeSinceStartup;
		InvokeRepeating ("Egg_Roll", EggHatchDelay, RollInterval);

	}
	void Egg_Roll(){
		//Instantiate new ship (bird) object,then kill the egg
		if (GetComponent<SpriteRenderer> ().color == Color.red) {

			GetComponent<SpriteRenderer> ().color = Color.gray;
		} else if (GetComponent<SpriteRenderer> ().color != Color.green) {
			GetComponent<SpriteRenderer> ().color = Color.red;
		}
		int roll = Random.Range (0, 100);
		if (roll >= 75) {
			GetComponent<SpriteRenderer> ().color = Color.green;
		}
		RollInterval /= 2;
	}
}
