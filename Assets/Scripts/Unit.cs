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
		
	public PirateSkin playersSkin;
	public PirateSkin enemySkin;
	public bool isEnemy;
	
	public float hp = 10;
	public float cd = 0;
	public float bulletSpeed = 0;
	public float damage = 0;
	public float attackRadius = 0;
	public float damageRadius = 0;

	public Action onUnitDestroyed;
	private Weapon weapon;
	private PirateSkin skin;
	private float baseHp;


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
		baseHp = hp;
	}

	void OnDestroy() {
		UnitManager.instance.OnUnitDestroyed(this);
		if(isEnemy) {
			ScoreCounter.UpdateScore(10);
		}
		if (onUnitDestroyed != null)
			onUnitDestroyed();
	}


	public void ApplyDamage(float damage) {
		hp -= damage;

		BroadcastMessage("HpHandler",hp/baseHp);

		if (hp < 0) {
			Destroy(gameObject);
		}
	}

	public void InitializeFaction(bool isEnemy) {
		this.isEnemy = isEnemy;

		GameObject skinPrefab = isEnemy ? enemySkin.gameObject:playersSkin.gameObject;
		GameObject skinObject = Instantiate(skinPrefab) as GameObject;
		skin = skinObject.GetComponent<PirateSkin>();
		skin.transform.Reparent(transform);

		if (isEnemy) tag = "enemy";
		else         tag = "friend";
	}
}
