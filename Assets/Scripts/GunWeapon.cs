using UnityEngine;
using System.Collections;

public class GunWeapon : Weapon {

	public Bullet bulletPrefab;
	public Granate granatePrefab;

	private bool isGrante = true;
	
	private bool first = false;

	public override void Attack(Unit target) {
		if(!first) {
			int i = Random.Range(0,2);
			if(i == 0 ){
				isGrante = true;
			} else {
				isGrante = false;
			}
			first = true;
		}

		if(isGrante) {
			Granate granate = (Instantiate(granatePrefab.gameObject, transform.position, Quaternion.identity) as GameObject).GetComponent<Granate>();
			granate.Launch(target.transform.position, unit.faction, unit.damage, unit);
		} else {
			Bullet bullet = (Instantiate(bulletPrefab.gameObject, transform.position, Quaternion.identity) as GameObject).GetComponent<Bullet>();
			bullet.Launch((target.transform.position - transform.position)*2f, unit.faction, unit.damage, unit);
		}
	}
}
