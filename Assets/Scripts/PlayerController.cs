using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public class PlayerController : MonoBehaviour {

	public Army army;

	private Unit currentUnit;

	public GameObject selectedUnit;
	public GameObject unitPathTarget;
	public GameObject unitEnemyTarget;

	public void OnPlayerSelected(int selected) {
		if(army.units[selected] != null) {
			currentUnit = army.units[selected].GetComponent<Unit>();
			selectedUnit.SetActive(true);
		}
	}
	
	RaycastHit hit;

	void RayHandler() {
		if(!EventSystemManager.currentSystem.IsPointerOverEventSystemObject()) {
			if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit) && currentUnit != null){
				if(hit.transform.tag == "isEnemy") {
					unitEnemyTarget.SetActive(true);
					unitEnemyTarget.transform.position = hit.point + new Vector3(0,0.5f,0);
					Invoke("HideUnitEnemyTarget",1f);
					currentUnit.gameObject.GetComponent<PirateAI>().SetTargetOrder(hit.transform.GetComponent<Unit>());
					HideUnitPathTarget();
				} else {
					if(hit.transform.tag == "friend") {
						currentUnit = hit.transform.GetComponent<Unit>();
						selectedUnit.SetActive(true);
					} else {
						currentUnit.gameObject.GetComponent<PirateAI>().SetMoveOrder(hit.point);
						unitPathTarget.SetActive(true);
						unitPathTarget.transform.position = hit.point + new Vector3(0,0.5f,0);
					}
				}
			} else {
				if(hit.transform.tag == "friend") {
					currentUnit = hit.transform.GetComponent<Unit>();
					selectedUnit.SetActive(true);
				} 
			}
		}
	}

	private void HideUnitEnemyTarget() {
		unitEnemyTarget.SetActive(false);
	}

	public void HideUnitPathTarget() {
		unitPathTarget.SetActive(false);
	}

	void Update() {
		if(Input.GetMouseButtonDown(0)) {
			RayHandler();
		}
		if(currentUnit != null) {
			selectedUnit.transform.position = currentUnit.transform.position;
		} else {
			unitPathTarget.SetActive(false);
			unitEnemyTarget.SetActive(false);
			selectedUnit.SetActive(false);
		}
	}
}