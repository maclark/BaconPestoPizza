using UnityEngine;
using System.Collections;

public class Shopkeeper : MonoBehaviour {

	public int cannonballsPrice = 6;
	public int torpedoPrice = 3;
	public int unitOfEnergyPrice = 10;
	public int tonOfWaterPrice = 10;
	public int greensPrice = 60;
	public int rubyPrice = 20;
	public int eggPrice = 100;

	//public enum GoodsType {GEM, CANNONBALLS, TORPEDO, UNIT_ENERGY, TON_WATER, GREENS}

	public GameObject colliderChild;
	private GameManager gm;

	void Awake () {
		gm = GameObject.FindObjectOfType<GameManager> ();
	}

	public BoxCollider2D[] GetColliderChild () {
		BoxCollider2D[] colls = new BoxCollider2D[1];
		colls [0] = colliderChild.GetComponent<BoxCollider2D> ();
		return colls;
	}

	public void SellToPlayer (Cargo.CargoType ct) {
		switch (ct) {
		case Cargo.CargoType.CANNONBALLS:
			if (!gm.bbm.cannonballsMaxed) {
				gm.bbm.cannonballs += 3;
				gm.bbm.PayShop (cannonballsPrice);
			}
			break;
		case Cargo.CargoType.TORPEDO:
			if (!gm.bbm.torpedoesMaxed) {
				gm.bbm.torpedoes++;
				gm.bbm.PayShop (torpedoPrice);
			}
			break;
		case Cargo.CargoType.UNIT_ENERGY:
			if (!gm.GetBigBirdEnergyTank ().full) {
				gm.bbm.energyTank.IncreaseResource (2000);
				gm.bbm.PayShop (unitOfEnergyPrice);
			}
			break;
		case Cargo.CargoType.TON_WATER:
			if (!gm.GetBigBirdWaterTank ().full) {
				gm.bbm.waterTank.IncreaseResource (2000);
				gm.bbm.PayShop (tonOfWaterPrice);
			}
			break;
		case Cargo.CargoType.GREENS:
			if (!gm.bigBird.hold.GetFull ()) {
			//	SpawnCargo (Cargo.CargoType.GREENS);
				gm.bbm.PayShop (greensPrice);
			}
			break;
		default:
			break;

		}
	}


	public void BuyFromPlayer (Cargo.CargoType gt) {
		switch (gt) {
		case Cargo.CargoType.RUBY:
			gm.bbm.Collect (rubyPrice / 2);
			break;
		case Cargo.CargoType.EGG:
			gm.bbm.Collect (eggPrice / 2);
			break;
		case Cargo.CargoType.CANNONBALLS:
			gm.bbm.Collect (cannonballsPrice / 2);
			break;
		case Cargo.CargoType.TORPEDO:
			gm.bbm.Collect (torpedoPrice / 2);
			break;
		case Cargo.CargoType.UNIT_ENERGY:
			gm.bbm.Collect (unitOfEnergyPrice / 2);
			break;
		case Cargo.CargoType.TON_WATER:
			gm.bbm.Collect (tonOfWaterPrice / 2);
			break;
		case Cargo.CargoType.GREENS:
			gm.bbm.Collect (greensPrice / 2);
			break;
		default:
			break;
		}
	}
}
