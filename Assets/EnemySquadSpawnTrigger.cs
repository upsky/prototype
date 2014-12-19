using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class EnemySquadSpawnTrigger : MonoBehaviour {

	public List<Unit> squadPrefabs;
	private bool spawned;
	private SphereCollider sphereCollider;

	public void Spawn() {
		spawned = true;

		foreach (var unitPrefab in squadPrefabs) {
			GameObject unitObj = Instantiate(unitPrefab.gameObject, transform.position, Quaternion.identity) as GameObject;
			Unit unit = unitObj.GetComponent<Unit>();
			unit.InitializeFaction(true);
		}
	}

	void Awake() {
		sphereCollider = GetComponent<SphereCollider>();
	}

	void Update() {
		if (spawned)
			return;

		foreach (var unit in UnitManager.instance.units) {
			if (unit.isEnemy)
				continue;

			if ((transform.position - unit.transform.position).magnitude < sphereCollider.radius) {
				Spawn();
				return;
			}
		}
	}
}
