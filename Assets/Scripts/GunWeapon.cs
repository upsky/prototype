using UnityEngine;
using System.Collections;

public class GunWeapon : Weapon {

	public Bullet bulletPrefab;
	public Granate granatePrefab;

	public enum WeaopnType{
		pistol,
		granate,
		bazuka,
		coreBazuka
	};
	public WeaopnType weaopnType;

	public override void Attack(Unit target) {
		switch(weaopnType) {
			case WeaopnType.granate:
				Granate(target);
				break;
			case WeaopnType.pistol:
				Pistol(target);
				break;
			case WeaopnType.bazuka:
				Bazuka(target);
				break;
			case WeaopnType.coreBazuka:
				CoreBazuka(target);
				break;
		}
	}
	Vector3 selfPos = Vector3.zero;

	void UpdateSelfPos() {
		selfPos = transform.position + new Vector3(0,2,0);
		if(GetComponent<TowerAI>() == null) {
			selfPos = unit.Skin.ShootPoint.position;
		}
	}

	private void Granate(Unit target) {
		UpdateSelfPos();
		Granate granate = (Instantiate(granatePrefab.gameObject, selfPos, Quaternion.identity) as GameObject).GetComponent<Granate>();
			
		granate.transform.parent = MapUtilities.ProjectilesContainer;
		granate.Launch(target.transform.position, unit.isEnemy, unit.damage, unit, unit.damageRadius);
	}

	private void Pistol(Unit target) {
		UpdateSelfPos();
		Vector3 dir = target.transform.position -  selfPos ;
		Bullet bullet = (Instantiate(bulletPrefab.gameObject, selfPos, Quaternion.LookRotation(dir)) as GameObject).GetComponent<Bullet>();
		bullet.transform.parent = MapUtilities.ProjectilesContainer;
		bullet.Launch((target.transform.position - transform.position).normalized*20f, unit.isEnemy, unit.damage, unit,0,3f);
	}

	private void CoreBazuka(Unit target) {
		UpdateSelfPos();
		Vector3 dir = target.transform.position - selfPos;
		Bullet bullet = (Instantiate(bulletPrefab.gameObject, selfPos, Quaternion.LookRotation(dir)) as GameObject).GetComponent<Bullet>();
		bullet.transform.parent = MapUtilities.ProjectilesContainer;
		bullet.Launch((target.transform.position - transform.position).normalized*20f, unit.isEnemy, unit.damage, unit,unit.damageRadius,1f);
	}

	private void Bazuka(Unit target) {
		UpdateSelfPos();
		Vector3 dir = target.transform.position - selfPos;
		Bullet bullet = (Instantiate(bulletPrefab.gameObject, selfPos, Quaternion.LookRotation(dir)) as GameObject).GetComponent<Bullet>();
		bullet.transform.parent = MapUtilities.ProjectilesContainer;
		bullet.Launch((target.transform.position - transform.position).normalized*20f, unit.isEnemy, unit.damage, unit,unit.damageRadius,6f);
	}
}
