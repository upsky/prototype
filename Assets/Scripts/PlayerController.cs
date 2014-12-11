using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public Army army;

	private Unit currentUnit;

	public void OnPlayerSelected(int selected) {
		currentUnit = army.units[selected].GetComponent<Unit>();
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
	}
}