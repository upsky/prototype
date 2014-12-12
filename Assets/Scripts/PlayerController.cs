using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public Army army;

	private Unit currentUnit;

	public GameObject selectedUnit;

	public void OnPlayerSelected(int selected) {
		if(army.units[selected] != null) {
			currentUnit = army.units[selected].GetComponent<Unit>();
			selectedUnit.SetActive(true);
		}
	}
	
	RaycastHit hit;

	void RayHandler() {
		if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit) && currentUnit != null){
			if(hit.transform.tag == "enemy") {
				currentUnit.gameObject.GetComponent<PirateAI>().SetTargetOrder(hit.transform.GetComponent<Unit>());
			} else {
				currentUnit.gameObject.GetComponent<PirateAI>().SetMoveOrder(hit.point);
			}
		}
	}

	void Update() {
		if(Input.GetMouseButton(0)) {
			RayHandler();
		}
		if(currentUnit != null) {
			selectedUnit.transform.position = currentUnit.transform.position;
		} else {
			selectedUnit.SetActive(false);
		}
	}
}