using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BigBirdManager : MonoBehaviour {
	public int hp = 10000;
	public int money = 0;
	public int cannonballs = 99;
	public int maxCannonballs = 99;
	public int torpedoes = 40;
	public int maxTorpedoes = 40;
	public float waterTankCapacity = 10000f;
	public float sweatRate = .3f;
	public float drinkRate = 20f;
	public bool cannonballsMaxed = true;
	public bool torpedoesMaxed = true;
	public bool drinking = false;
	public ResourceBar waterTank;
	public ResourceBar energyTank;
	public Text distance;
	public Text coins;

	private GameManager gm;
	private WaterSource localWater;
	private float water;

	void Awake () {
		gm = GameObject.FindObjectOfType<GameManager> ();
		waterTank = gm.GetBigBirdWaterTank ();
		energyTank = gm.GetBigBirdEnergyTank ();
	}


	void Start () {
		waterTank.capacity = waterTankCapacity;
		waterTank.SetResource (waterTankCapacity);
		energyTank.capacity = hp;
		waterTank.current = hp;
		energyTank.current = hp;
		distance.text = transform.position.y.ToString();
		coins.text = 0.ToString ();

	}


	void Update () {
		distance.text = transform.position.y.ToString();
	}


	public void GainCannonballs (int balls) {
		cannonballs += balls;
		if (cannonballs >= maxCannonballs) {
			cannonballsMaxed = true;
			cannonballs = maxCannonballs;
		}

	}

	public void GainTorpedoes (int torps) {
		torpedoes += torps;
		if (torpedoes >= maxTorpedoes) {
			torpedoesMaxed = true;
			torpedoes = maxTorpedoes;
		}
	}

	public void PayShop (int price) {
		money -= price;
		coins.text = money.ToString ();
	}

	public void Collect (float collection) {
		money += Mathf.RoundToInt (collection);
		coins.text = money.ToString ();
	}


	public void AtWaterSource (WaterSource s) {
		drinking = true;
		localWater = s;
	}

	public void LeftWaterSource (WaterSource s) {
		if (localWater == s) {
			drinking = false;
			localWater = null;
		}
	}

	public void Drink () {
		if (waterTank.full || localWater.dry) {
			return;
		} else {
			float thisGulp;
			if (localWater.gallons < drinkRate) {
				thisGulp = localWater.gallons;
			} else {
				thisGulp = drinkRate;
			}
			waterTank.IncreaseResource (localWater.Gulp (thisGulp));
		}
	}

	public void Sweat () {
		waterTank.DecreaseResource (sweatRate);

		if (waterTank.empty) {
			Dehydrated ();
		}
	}

	void Dehydrated () {
		Debug.Log ("do dehydration");
	}
}
