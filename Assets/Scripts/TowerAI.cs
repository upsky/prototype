using UnityEngine;
using System.Linq;
using System.Collections;
using UnityEditor;

public class TowerAI : MonoBehaviour {

	public float findTargetDelay = 1.5f;

	private Unit unit;
	private Unit targetUnit;

	public Unit TargetUnit {
		get { return targetUnit; }
		set {
			if (targetUnit != null)
				targetUnit.onUnitDestroyed = null;

			targetUnit = value;

			if (targetUnit != null)
				targetUnit.onUnitDestroyed = () => { targetUnit = null; };
		}
	}

	private void StartInvoks() {
		InvokeRepeating("FindTraget",0,findTargetDelay);
	}

	private void StopInvoks() {
		CancelInvoke("FindTraget");
	}

	void Start() {
		unit = GetComponent<Unit>();

		InvokeRepeating("FindTraget",0,findTargetDelay);
		InvokeRepeating("Attack",0,unit.cd);
	}

	void Attack() {
		if (targetUnit == null)
			return;
		
		if (IsCanAttackTargetFromPoint(unit.AttackPoint))
		{
			unit.Weapon.Attack (targetUnit);
		}
	}

	bool IsCanAttackTargetFromPoint(Vector3 position) {
		if (targetUnit == null)
			return false;

		Vector3 dir = targetUnit.AttackPoint - position;

		if (dir.magnitude > unit.attackRadius)
			return false;

		return true;
	}

	void FindTraget() {
		Unit nearestUnit = UnitManager.instance.GetNearestUnit(!unit.isEnemy, transform.position);

		if (nearestUnit != null) {
			if (targetUnit == null) {
				targetUnit = nearestUnit;
			} else {
				float targetUnitDist = (transform.position - targetUnit.AttackPoint).magnitude;
				float nearestUnitDist = (transform.position - nearestUnit.AttackPoint).magnitude;
				
				if (nearestUnitDist < targetUnitDist - unit.attackRadius*0.5f) 
					targetUnit = nearestUnit;
			}
		}
	}

	void Update() {
		if (targetUnit != null)
			Debug.DrawLine(transform.position, targetUnit.AttackPoint, Color.red);
	}

//	private void RotateTowards (Transform target) {
//		Vector3 direction = (target.position - transform.position).normalized;
//		Quaternion lookRotation = Quaternion.LookRotation(direction);
//		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 2f);
//	}
}
