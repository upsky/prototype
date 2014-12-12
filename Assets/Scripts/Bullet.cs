﻿using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public float damage = 1f;

	public Unit.Faction faction;

	private Unit myOvner;

	public void Launch(Vector3 target, Unit.Faction faction, float damage, Unit myOvner) {
		this.damage = damage;
		this.faction = faction;
		this.myOvner = myOvner;
		rigidbody.velocity = target;
		Invoke("KillHimself", 1.5f);
	}

	private void KillHimself() {
		Destroy(gameObject);
	}

	public void OnTriggerEnter(Collider other) {
		if(other.transform.GetComponent<Unit>() &&
		   faction != other.transform.GetComponent<Unit>().faction) {
			other.transform.GetComponent<Unit>().GetDamage(damage);
			Destroy(gameObject);
		}
	}
}
