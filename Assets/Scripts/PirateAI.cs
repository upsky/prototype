using UnityEngine;
using System.Linq;
using System.Collections;

public class PirateAI : MonoBehaviour {

	private Army blueArmy;
	private Army redArmy;
	private Army currentArmy;
	private Army oppositeArmy;

	public NavMeshAgent move;

	private Unit unit;
	private Unit targetUnit;

	public GameObject bullet;
	
	public bool playerOrder;

	public void SetTargetOrder(Unit newTarget) {
		StopInvoks();
		targetUnit = newTarget;
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

	public void SetNullTarget() {
		oppositeArmy.units.Remove(targetUnit.gameObject);
		targetUnit = null;
	}

	void Start() {
	
		blueArmy = GameObject.Find("BlueArmy").GetComponent<Army>();
		redArmy = GameObject.Find("RedArmy").GetComponent<Army>();

		unit = GetComponent<Unit>();

		if(unit.faction == Unit.Faction.red) {
			currentArmy = redArmy;
			oppositeArmy = blueArmy;
		} else {
			currentArmy = blueArmy;
			oppositeArmy = redArmy;
		}
		InvokeRepeating("FindTraget",0,0.1f);
		InvokeRepeating("Attack",0,unit.cd);
	}

	void Attack() {
		if(move.velocity.magnitude > 0.1f) {
			return;
		}

		if(targetUnit != null) {
			if((transform.position - targetUnit.transform.position).magnitude < unit.attackRadius) {
				GameObject currentBullet = (GameObject)GameObject.Instantiate(bullet,transform.position, Quaternion.identity);
				
				currentBullet.GetComponent<Bullet>().Launch((targetUnit.transform.position - transform.position)*2f,
				                                            unit.faction,
				                                            unit.damage,unit);
			}
		}
	}

	void FindTraget() {

		GameObject nearestUnit = null;
		float minDist = float.MaxValue;
		foreach(var unitObj in oppositeArmy.units)
		{
			float dst = (unitObj.transform.position - transform.position).magnitude;
			if (dst < minDist)
			{
				minDist = dst;
				nearestUnit = unitObj;
			}
		}

		if (targetUnit == null)
			targetUnit = nearestUnit.GetComponent<Unit>();
		else {
			float targetUnitDist = (transform.position - targetUnit.transform.position).magnitude;
			float nearestUnitDist = (transform.position - nearestUnit.transform.position).magnitude;

			if (nearestUnitDist < targetUnitDist - unit.attackRadius*0.5f)
				targetUnit = nearestUnit.GetComponent<Unit>();
			else
				move.Stop();
		}

		float newTargetUnitDist = (transform.position - targetUnit.transform.position).magnitude;
		if (newTargetUnitDist > unit.attackRadius) {
			MoveToTarget((targetUnit.transform.position - transform.position)/newTargetUnitDist*(newTargetUnitDist - unit.attackRadius*0.7f));
		}
	}

	void MoveToTarget(Vector3 targetPosition) {
		move.SetDestination(targetPosition);
		Debug.DrawRay(targetPosition, Vector3.up*2f, Color.red, 1);
	}

	void MoveToUnit(Unit targetUnit){
		float newTargetUnitDist = (transform.position - targetUnit.transform.position).magnitude;
		if (newTargetUnitDist > unit.attackRadius) {
			MoveToTarget((targetUnit.transform.position - transform.position)/newTargetUnitDist*(newTargetUnitDist - unit.attackRadius*0.7f));
		}
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
