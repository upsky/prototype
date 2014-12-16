using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UnitManager : MonoBehaviour {

	public List<Unit> units;
	public static UnitManager instance;


	void Awake() {
		instance = this;
	}


	public void OnUnitCreated(Unit unit) {
		units.Add(unit);
	}

	public void OnUnitDestroyed(Unit unit) {
		units.Remove (unit);
	}

	public Unit GetNearestUnit(Unit.Faction faction, Vector3 point) {
		Unit res = null;
		float minDist = float.MaxValue;
		foreach(var unit in units) {
			if (unit.faction != faction)
				continue;

			float dist = (point - unit.transform.position).magnitude;
			if (dist < minDist) {
				res = unit;
				minDist = dist;
			}
		}
		return res;
	}

	public Unit GetRandomUnitInRadius(Unit.Faction faction, Vector3 point, float radius = float.MaxValue) {
		List<Unit> factionUnits = units.FindAll(x => x.faction == faction && (x.transform.position - point).magnitude < radius);

		if (factionUnits.Count == 0)
			return null;

		return factionUnits[UnityEngine.Random.Range(0, factionUnits.Count - 1)];
	}
}
