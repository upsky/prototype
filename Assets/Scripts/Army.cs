using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Army : MonoBehaviour {
	
	public Unit.Faction faction;
	public GameObject[] prefab = new GameObject[5];

	public List<GameObject> units = new List<GameObject>();

	public Grid grid;
	
	void Start () {
		for(int i=0;i<prefab.Length;++i) {
			if(faction == Unit.Faction.blue) {
				units.Add(GameObject.Instantiate(prefab[i],grid.GetGridPoint(Grid.ObjectType.BotUnit),gameObject.transform.rotation) as GameObject);
				units[i].tag = "enemy";
			} else {
				units.Add(GameObject.Instantiate(prefab[i],grid.GetGridPoint(Grid.ObjectType.PlayerUnit),gameObject.transform.rotation) as GameObject);
			}

			units[i].GetComponent<Unit>().InitializeFaction(faction);
		}
	}

}
