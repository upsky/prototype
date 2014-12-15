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
	private bool first = false;

	public override void Attack(Unit target) {
		if(!first) {
			//weaopnType = (WeaopnType)Random.Range(0,4);
			first = true;
		}

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

	private void Granate(Unit target) {
		Granate granate = (Instantiate(granatePrefab.gameObject, unit.Skin.ShootPoint.position, Quaternion.identity) as GameObject).GetComponent<Granate>();
		granate.Launch(target.transform.position, unit.faction, unit.damage, unit);
	}

	private void Pistol(Unit target) {
		Vector3 dir = target.transform.position - unit.Skin.ShootPoint.position;
		Bullet bullet = (Instantiate(bulletPrefab.gameObject, unit.Skin.ShootPoint.position, Quaternion.LookRotation(dir)) as GameObject).GetComponent<Bullet>();
		bullet.Launch((target.transform.position - transform.position).normalized*20f, unit.faction, unit.damage, unit,false,2.5f);
	}

	private void CoreBazuka(Unit target) {
		Vector3 dir = target.transform.position - unit.Skin.ShootPoint.position;
		Bullet bullet = (Instantiate(bulletPrefab.gameObject, unit.Skin.ShootPoint.position, Quaternion.LookRotation(dir)) as GameObject).GetComponent<Bullet>();
		bullet.Launch((target.transform.position - transform.position).normalized*10f, unit.faction, unit.damage, unit,true,2.5f);
	}

	private void Bazuka(Unit target) {
		Vector3 dir = target.transform.position - unit.Skin.ShootPoint.position;
		Bullet bullet = (Instantiate(bulletPrefab.gameObject, unit.Skin.ShootPoint.position, Quaternion.LookRotation(dir)) as GameObject).GetComponent<Bullet>();
		bullet.Launch((target.transform.position - transform.position).normalized*30f, unit.faction, unit.damage, unit,true,2.5f);
	}
}
