using UnityEngine;
using System.Collections;

public class GunWeapon : Weapon {

	public Bullet bulletPrefab;

	public override void Attack(Unit target) {
		Bullet bullet = (Instantiate(bulletPrefab.gameObject, transform.position, Quaternion.identity) as GameObject).GetComponent<Bullet>();
		bullet.Launch((target.transform.position - transform.position).normalized*20f, unit.faction, unit.damage, unit);
	}
}
