using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public float damage = 1f;

	public Unit.Faction faction;

	public void Launch(Vector3 target, Unit.Faction faction, float damage) {
		this.damage = damage;
		this.faction = faction;
		rigidbody.velocity = target;
	}

	public void OnTriggerEnter(Collider other) {
		if(other.transform.GetComponent<Unit>() &&
		   faction != other.transform.GetComponent<Unit>().faction) {
			other.transform.GetComponent<Unit>().GetDamage(damage);
			Destroy(gameObject);
		}
	}
}
