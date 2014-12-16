using UnityEngine;
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
		public PirateSkin skinPrefab;
		public Faction faction;
	}

	public SkinDef[] skins;
	public Faction faction;
	
	public float hp = 10;
	public float cd = 0;
	public float bulletSpeed = 0;
	public float damage = 0;
	public float attackRadius = 0;
	public float damageRadius = 0;

	public Action onUnitDestroyed;
	private Weapon weapon;
	private PirateSkin skin;

	
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

	public PirateSkin Skin {
		get { return skin; }
	}

	public Vector3 AttackPoint {
		get { return transform.position + Vector3.up*1.5f; }
	}

	void Awake() {
		UnitManager.instance.OnUnitCreated(this);
		weapon = GetComponent<Weapon>();

		cd += UnityEngine.Random.Range(-0.1f, 0.1f);
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
		GameObject skinPrefab = skins.First(x => x.faction == faction).skinPrefab.gameObject;
		GameObject skinObject = Instantiate(skinPrefab) as GameObject;
		skin = skinObject.GetComponent<PirateSkin>();
		skin.transform.parent = transform;
		skin.transform.localPosition = Vector3.zero;
		skin.transform.localRotation = Quaternion.identity;
		skin.transform.localScale = new Vector3(1,1,1);

		if (faction == Faction.blue) tag = "enemy";
		else tag = "friend";
	}
}
