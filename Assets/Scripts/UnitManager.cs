using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UnitManager : MonoBehaviour {

	public List<Unit> units;
	public Transform playerUnitsContainer;
	public Transform enemyUnitsContainer;
	public static UnitManager instance;


	void Awake() {
		instance = this;
	}


	public void OnUnitCreated(Unit unit) {
		units.Add(unit);
		if (unit.isEnemy) unit.transform.Reparent(enemyUnitsContainer);
		else              unit.transform.Reparent(playerUnitsContainer);
	}

	public void OnUnitDestroyed(Unit unit) {
		units.Remove (unit);
	}

	public Unit GetNearestUnit(bool isEnemy, Vector3 point) {
		Unit res = null;
		float minDist = float.MaxValue;
		foreach(var unit in units) {
			if (unit.isEnemy != isEnemy)
				continue;

			float dist = (point - unit.transform.position).magnitude;
			if (dist < minDist) {
				res = unit;
				minDist = dist;
			}
		}
		return res;
	}

	public Unit GetRandomUnitInRadius(bool isEnemy, Vector3 point, float radius = float.MaxValue) {
		List<Unit> factionUnits = units.FindAll(x => x.isEnemy == isEnemy && (x.transform.position - point).magnitude < radius);

		if (factionUnits.Count == 0)
			return null;

		return factionUnits[UnityEngine.Random.Range(0, factionUnits.Count - 1)];
	}

	public static Transform GetUnitsContainer(bool isEnemy) {
		if (isEnemy)
			return instance.enemyUnitsContainer;

		return instance.playerUnitsContainer;
	}
}
