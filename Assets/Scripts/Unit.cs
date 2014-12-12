﻿using UnityEngine;
using System;
using System.Linq;
using System.Collections;

public abstract class Weapon : MonoBehaviour {
	
	protected Unit unit;
	
	void Awake() {
		unit = GetComponent<Unit>();
	}
	
	public abstract void Attack(Unit target);
}

public class Unit: MonoBehaviour {

	public enum Faction {
		red,
		blue
	}

	[Serializable]
	public struct SkinDef {
		public GameObject skinPrefab;
		public Faction faction;
	}

	public SkinDef[] skins;
	public Faction faction;

	public float cd = 0;
	public float bulletSpeed = 0;
	public float damage = 0;
	public float attackRadius = 0;
	public float damageRadius = 0;

	public Action onUnitDestroyed;
	private Weapon weapon;

	private float hp = 10;
	
	public Faction OppositeFaction {
		get {
			if (faction == Faction.red)
				return Faction.blue;

			return Faction.red;
		}
	}

	public Weapon Weapon {
		get { return weapon; }
	}

	void Awake() {
		UnitManager.instance.OnUnitCreated(this);
		weapon = GetComponent<Weapon>();
	}

	void OnDestroy() {
		UnitManager.instance.OnUnitDestroyed(this);
		if (onUnitDestroyed != null)
			onUnitDestroyed();
	}

	public void ResurrectionUnit() {
		hp = 10;
	}

	public void GetDamage(float damage) {
		hp -= damage;
		if (hp < 0) {
			Destroy(gameObject);
		}
	}

	public void InitializeFaction(Faction faction) {
		this.faction = faction;
		GameObject skinPrefab = skins.First(x => x.faction == faction).skinPrefab;
		GameObject skin = Instantiate(skinPrefab) as GameObject;
		skin.transform.parent = transform;
		skin.transform.localPosition = Vector3.zero;
		skin.transform.localRotation = Quaternion.identity;
		skin.transform.localScale = new Vector3(1,1,1);
	}
}
