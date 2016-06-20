using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BigBirdManager : MonoBehaviour {
	public int money = 0;
	public int cannonballs = 99;
	public int maxCannonballs = 99;
	public int torpedoes = 40;
	public int maxTorpedoes = 40;
	public float waterTankCapacity = 10000f;
	public float energyTankCapacity = 2000f;
	public float sweatRate = .3f;
	public float drinkRate = 20f;
	public float absorbRate = 40f;
	public bool cannonballsMaxed = true;
	public bool torpedoesMaxed = true;
	public bool drinking = false;
	public bool absorbing = false;
	public bool shieldUp = true;
	public ResourceBar waterTank;
	public ResourceBar energyTank;
	public Text distance;
	public Text coins;
	public Transform bigBirdColliders;
	public Transform flammableCompartments;

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
		waterTank.current = waterTankCapacity;
		energyTank.capacity = energyTankCapacity;
		energyTank.SetResource (energyTankCapacity);
		energyTank.current = energyTankCapacity;

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

	public bool PayShop (int price) {
		if (money - price >= 0) {
			money -= price;
			coins.text = money.ToString ();
			return true;
		} else {
			print ("can't afford item");
			return false;
		}
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

	public void Absorb () {
		if (energyTank.full) {
			return;
		} else {
			energyTank.IncreaseResource (absorbRate);
			shieldUp = true;
		}
	}


	public void Sweat () {
		waterTank.DecreaseResource (sweatRate);

		if (waterTank.empty) {
			Dehydrated ();
		}
	}

	public void GasOut (float amount) {
		waterTank.DecreaseResource (amount);
		if (waterTank.empty) {
			Dehydrated ();
		}
	}

	void Dehydrated () {
		Debug.Log ("do dehydration");
	}

	public void SolarShieldDown () {
		//activate flammable compartments
		//toggle all box collidres? no. just turn off main ones and turn on flammables.
		print ("SHIELD DOWN");
		//Collider2D[] colls = bigBirdColliders.GetComponents<Collider2D> ();
		bigBirdColliders.gameObject.SetActive (false);
		flammableCompartments.gameObject.SetActive (true);
		shieldUp = false;
	}
}
