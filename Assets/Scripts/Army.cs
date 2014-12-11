using UnityEngine;
using System.Collections;

public class Army : MonoBehaviour {

	public GameObject[] prefab = new GameObject[5];

	public GameObject[] units = new GameObject[5];

	public Unit.Faction faction;

	public Grid grid;
	
	void Start () {
		for(int i=0;i<prefab.Length;++i) {
			if(faction == Unit.Faction.blue) {
				units[i] = GameObject.Instantiate(prefab[i],grid.GetGridPoint(Grid.ObjectType.BotUnit),gameObject.transform.rotation) as GameObject;
				units[i].tag = "enemy";
			} else {
				units[i] = GameObject.Instantiate(prefab[i],grid.GetGridPoint(Grid.ObjectType.PlayerUnit),gameObject.transform.rotation) as GameObject;
			}
			units[i].GetComponent<Unit>().Init(Unit.UnitType.pistol,faction);
		}
	}

}
