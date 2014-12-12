using UnityEngine;
using System.Collections;

public class SwordWeapon: Weapon {

	public override void Attack(Unit target) {
		if ((target.transform.position - transform.position).magnitude < unit.attackRadius)
			target.GetDamage(unit.damage);
	}
}
