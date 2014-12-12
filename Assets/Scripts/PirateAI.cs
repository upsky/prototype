using UnityEngine;
using System.Linq;
using System.Collections;

public class PirateAI : MonoBehaviour {

	public NavMeshAgent move;

	private Unit unit;
	private Unit targetUnit;
	
	public bool playerOrder;

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

	public void SetTargetOrder(Unit newTarget) {
		StopInvoks();
		TargetUnit = newTarget;
		playerOrder = true;
		MoveToUnit(targetUnit);
	}

	public void SetMoveOrder(Vector3 newPos) {
		StopInvoks();
		MoveToTarget(newPos);
		playerOrder = true;
	}

	private void StartInvoks() {
		InvokeRepeating("FindTraget",0,0.1f);
	}

	private void StopInvoks() {
		CancelInvoke("FindTraget");
	}

	void Start() {
		move = GetComponent<NavMeshAgent>();
		unit = GetComponent<Unit>();
		InvokeRepeating("FindTraget",0,0.1f);
		InvokeRepeating("Attack",0,unit.cd);
	}

	void Attack() {
		if (targetUnit == null)
			return;

		if(move.velocity.magnitude > 0.1f) {
			return;
		}

		if(targetUnit != null && 
		   Vector3.Angle((targetUnit.transform.position - transform.position),transform.forward) < 45) {
			if((transform.position - targetUnit.transform.position).magnitude < unit.attackRadius) 
				unit.Weapon.Attack (targetUnit);
		}
	}

	void FindTraget() {
		Unit nearestUnit = UnitManager.instance.GetNearestUnit(unit.OppositeFaction, transform.position);

		if (nearestUnit != null) {
			if (targetUnit == null) 
				targetUnit = nearestUnit.GetComponent<Unit>();
			else {
				float targetUnitDist = (transform.position - targetUnit.transform.position).magnitude;
				float nearestUnitDist = (transform.position - nearestUnit.transform.position).magnitude;
				
				if (nearestUnitDist < targetUnitDist - unit.attackRadius*0.5f) 
					targetUnit = nearestUnit.GetComponent<Unit>();
			}
		}

		MoveToUnit(targetUnit);
	}

	void MoveToTarget(Vector3 targetPosition) {
		move.SetDestination(targetPosition);
		Debug.DrawRay(targetPosition, Vector3.up*2f, unit.faction == Unit.Faction.blue ? Color.blue:Color.red, 1);
	}

	void MoveToUnit(Unit targetUnit) {
		if (targetUnit == null)
			return;

		float newTargetUnitDist = (transform.position - targetUnit.transform.position).magnitude;
		if (newTargetUnitDist > unit.attackRadius) {
			MoveToTarget((targetUnit.transform.position - transform.position)/newTargetUnitDist*(newTargetUnitDist - unit.attackRadius*0.7f) + transform.position);
		}
		else move.Stop();
	}

	void Update() {
		if(move.remainingDistance < 0.1f && playerOrder) {
			StartInvoks();
			playerOrder = false;
		}

		if(move.velocity.magnitude <= 0.1f && targetUnit != null) {
			RotateTowards(targetUnit.transform);
		}
	}

	private void RotateTowards (Transform target) {
		Vector3 direction = (target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(direction);
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 2f);
	}
}
