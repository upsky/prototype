using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public class PlayerController : MonoBehaviour {

	public GameObject unitPathTarget;
	public GameObject unitEnemyTarget;
	
	private Unit targetOrderUnit;
	private int targetUnitOrders;
	private int moveUnitOrders;

	void RayHandler() {
		if(EventSystemManager.currentSystem.IsPointerOverEventSystemObject()) 
			return;

		RaycastHit hit;
		if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
			return;

		Unit hitUnit = hit.collider.GetComponent<Unit>();
		if (hitUnit != null && hitUnit.isEnemy) {			

			targetOrderUnit = hitUnit;
			targetUnitOrders = 0;
			unitEnemyTarget.SetActive(true);
			unitPathTarget.SetActive(false);

			foreach (var unit in UnitManager.instance.units) {
				if (unit.isEnemy)
					continue;

				unit.GetComponent<AdvancedPirateAI>().SetTargetOrder(hitUnit, UnitTargetOrderComplete);
				targetUnitOrders++;
			}

			Debug.Log("Target order: " + targetOrderUnit, targetOrderUnit.gameObject);
		}
		else {
			moveUnitOrders = 0;
			unitPathTarget.SetActive(true);
			unitEnemyTarget.SetActive(false);
			unitPathTarget.transform.position = hit.point;

			foreach (var unit in UnitManager.instance.units) {
				if (unit.isEnemy)
					continue;

				unit.GetComponent<AdvancedPirateAI>().SetMovingOrder(hit.point, UnitMovementOrderComplete);
				moveUnitOrders++;
			}

			//Debug.Log("Moving order: " + hit.point);
		}
	}

	void Update() {
		if(Input.GetMouseButtonDown(0)) {
			RayHandler();
		}

		if (targetOrderUnit != null) {
			unitEnemyTarget.transform.position = targetOrderUnit.transform.position;
		}
	}

	void UnitTargetOrderComplete() {
		if (unitEnemyTarget == null)
			return;

		targetUnitOrders--;
		if (targetUnitOrders == 0) {
			targetOrderUnit = null;
			unitEnemyTarget.SetActive(false);
		}
	}

	void UnitMovementOrderComplete() {
		if (unitPathTarget == null)
			return;

		moveUnitOrders--;
		if (moveUnitOrders == 0) {
			unitPathTarget.SetActive(false);
		}
	}
}