using UnityEngine;
using System.Collections;

public class Holster {

	public enum WeaponType {MACHINE_GUN, RIFLE, SHOTGUN};
	public WeaponType currentWeapon;
	public Player p;

	private int currentSlot;
	private WeaponType[] slots = new WeaponType[3];
	private MachineGun machineGun;
	private Rifle rifle;
	private Shotgun shotgun;

	// Use this for initialization
	public Holster (Player player) {
		p = player;
		CreateWeapons ();
		slots [0] = WeaponType.MACHINE_GUN;
		slots [1] = WeaponType.RIFLE;
		slots [2] = WeaponType.SHOTGUN;
		currentSlot = 0;
		EquipCurrentSlot ();
	}

	public void CycleWeapons () {
		int nextWeaponSlot = currentSlot + 1;
		if (currentSlot + 1 >= slots.Length) {
			nextWeaponSlot = 0;
		}
		currentSlot = nextWeaponSlot;
		EquipCurrentSlot ();
	}

	private void EquipCurrentSlot () {
		if (p.w != null) {
			p.w.firing = false;
			p.CancelInvoke ();
			p.GetComponent<PlayerInput> ().CancelInvoke ();
		}

		WeaponType weap = slots [currentSlot];
		switch (weap) 
		{
		case WeaponType.MACHINE_GUN:
			p.w = machineGun;
			break;
		case WeaponType.RIFLE:
			p.w = rifle;
			break;
		case WeaponType.SHOTGUN:
			p.w = shotgun;
			break;
		default:

			p.w = machineGun;
			break;
		}
	}

	private void CreateWeapons () {
		machineGun = 	new MachineGun (this);
		rifle = 		new Rifle (this);
		shotgun = 		new Shotgun (this);
	}
}
