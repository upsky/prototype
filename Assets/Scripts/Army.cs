using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Army : MonoBehaviour {
	
	public Unit.Faction faction;
	public GameObject[] prefab = new GameObject[5];

	public List<GameObject> units = new List<GameObject>();
	
	void Start () {
		List<ArmyUnitPosition> unitPositions = GetComponentsInChildren<ArmyUnitPosition>().ToList();
		unitPositions.ForEach(x => x.RandomizeWeight());
		unitPositions.Sort((x, y) => (int)(x.RndProbabilityWeight - y.RndProbabilityWeight));

		for(int i = 0; i < prefab.Length; ++i) {
			units.Add(GameObject.Instantiate(prefab[i], unitPositions[i].transform.position, unitPositions[i].transform.rotation) as GameObject);
			units[i].GetComponent<Unit>().InitializeFaction(faction);
		}

		unitPositions.ForEach(x => Destroy(x.gameObject));
	}

}
