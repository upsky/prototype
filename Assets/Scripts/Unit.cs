using UnityEngine;
using System.Collections;

public class Unit: MonoBehaviour {

	public enum UnitType {
		sword,
		ball,
		harpoon,
		pistol,
		cannon
	}

	public enum Faction {
		red,
		blue
	}

	public UnitType unitType;

	public Faction faction;

	private float[] cdArr = {1,2,2,1,3};
	private float[] bulletSpeedArr = {1,2,3,1,3};
	private float[] damageArr = {1,2,2,1,3};
	private float[] attackRadiusArr = {100,5,3,5,1};
	private float[] damageRadiusArr = {1,2,1,1,3};

	public float cd = 0;
	public float bulletSpeed = 0;
	public float damage = 0;
	public float attackRadius = 0;
	public float damageRadius = 0;

	private float hp = 10;

	public void Init(UnitType type, Faction newFaction) {
		unitType = type;
		faction = newFaction;

		int selectedUnitType = (int)unitType;

		cd = cdArr[selectedUnitType];
		bulletSpeed = bulletSpeedArr[selectedUnitType];
		damage = damageArr[selectedUnitType];
		attackRadius = attackRadiusArr[selectedUnitType];
		damageRadius = damageRadiusArr[selectedUnitType];
	}

	public void ResurrectionUnit() {
		hp = 10;
	}

	public void GetDamage(float damage) {
		hp -= damage;
	}
}
