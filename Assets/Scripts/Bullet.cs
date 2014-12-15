using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public float damage = 1f;

	public Unit.Faction faction;

	private Unit myOvner;

	private bool areaDamage = false;

	private float area = 2;

	public void Launch(Vector3 target, Unit.Faction faction, float damage, Unit myOvner, bool areaDamage, float bulletLifeTime) {
		this.damage = damage;
		this.faction = faction;
		this.myOvner = myOvner;
		this.areaDamage = areaDamage;
		rigidbody.velocity = target;
		Invoke("KillHimself", bulletLifeTime);
	}

	private void KillHimself() {
		if(areaDamage){ 
			BtoomDamage(area);
		}
		Destroy(gameObject);
	}

	private void BtoomDamage(float area) {
		Collider[] units = Physics.OverlapSphere(transform.position,area);
		foreach (var key in units) {
			if(key.GetComponent<Unit>() != null) {
				Unit target = key.GetComponent<Unit>();
				if(target.faction != myOvner.faction) {
					target.GetDamage(damage);
				}
			}
		}
	}

	public void OnTriggerEnter(Collider other) {
		if(other.transform.GetComponent<Unit>() &&
		   faction != other.transform.GetComponent<Unit>().faction) {
			if(areaDamage) {
				BtoomDamage(area);
			} else {
				other.transform.GetComponent<Unit>().GetDamage(damage);
			}
			Destroy(gameObject);
		}
	}
}
