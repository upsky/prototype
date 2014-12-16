using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class AdvancedPirateAI : MonoBehaviour {

	[Serializable]
	public class TracePoint {
		public Vector3 point;
		public float weight;
	}

	public float attackingAngle = 45f;
	public Vector2 attackPointRefindRateRange = new Vector2(5f, 10f);
	public Vector2 targetRefindRateRange = new Vector2(1f, 2f);
	public float nearestUnitPriorityCoef = 0.6f;
	public float targetMovementThreshold = 2f;
	public float targetTraceAngleStep = 15f;
	public Vector2 tracePointsWeightRange = new Vector2(0f, 10f);

	private NavMeshAgent navMeshAgent;
	private Unit unit;
	private Unit targetUnit;
	
	private float attackPointRefindRate;
	private float targetRefindRate;
	private float lastTargetRefindTime;
	private float lastAttackPointRefindTime;
	private float lastAttackTime;
	private Vector3 targetUnitSearchedPos;
	private Vector3 attackingPoint;

	private List<TracePoint> tracePoints = new List<TracePoint>();

	void Start () {
		unit = GetComponent<Unit>();
		navMeshAgent = GetComponent<NavMeshAgent>();
	}
	
	void Update () {
		CheckTargetRefind();
		CheckAttackPointRefind();
		CheckTargetMovement();
		CheckAttacking();
		UpdateMovement();
	}

	void CheckTargetMovement() {
		if (targetUnit == null)
			return;

		if ((targetUnitSearchedPos - targetUnit.transform.position).magnitude > targetMovementThreshold)
			CheckAttackPointRefind(true);
	}

	void UpdateMovement() {
		navMeshAgent.destination = attackingPoint;
		if (navMeshAgent.velocity.magnitude < 0.1f && targetUnit != null)
			RotateTowards(targetUnit.AttackPoint);
	}

	void CheckTargetRefind(bool forcible = false) {
		if (Time.time < lastTargetRefindTime + targetRefindRate && !forcible)
			return;

		targetRefindRate = UnityEngine.Random.Range(targetRefindRateRange.x, targetRefindRateRange.y);
		lastTargetRefindTime = Time.time;

		Unit lastTargetUnit = targetUnit;

		Unit nearestEnemy = UnitManager.instance.GetNearestUnit(unit.OppositeFaction, transform.position);
		if (targetUnit == null)
			targetUnit = nearestEnemy;
		else {
			float targetUnitDistance = (transform.position - targetUnit.transform.position).magnitude;
			float nearestUnitDistance = (transform.position - nearestEnemy.transform.position).magnitude;

			if (nearestUnitDistance < targetUnitDistance - unit.attackRadius*nearestUnitPriorityCoef)
				targetUnit = nearestEnemy;
		}

		if (targetUnit != lastTargetUnit) {
			targetUnitSearchedPos = targetUnit.transform.position;
			CheckAttackPointRefind(true);
		}
	}

	void CheckAttackPointRefind(bool forcible = false) {
		if (Time.time < lastAttackPointRefindTime + attackPointRefindRate && !forcible || targetUnit == null)
			return;		

		attackPointRefindRate = UnityEngine.Random.Range(attackPointRefindRateRange.x, attackPointRefindRateRange.y);
		lastAttackPointRefindTime = Time.time;

		Vector3 thisPos = transform.position;
		tracePoints.Clear();

		//trace target rays
		float accAngle = 0;
		while(accAngle < 360) {
			Vector3 rayDir = new Vector3(Mathf.Cos(accAngle*Mathf.Deg2Rad), 0, Mathf.Sin(accAngle*Mathf.Deg2Rad))*unit.attackRadius;

			TracePoint tracePoint = new TracePoint(){ point = targetUnit.AttackPoint + rayDir };
			tracePoints.Add(tracePoint);

			//ground adjusting
			RaycastHit hit;
			if (Physics.Raycast(tracePoint.point + Vector3.up*50f, Vector3.down, out hit)) {
				Vector3 groundTracedPoint = hit.point + (unit.AttackPoint - thisPos);
				tracePoint.point = groundTracedPoint;

				//trace from target to ground point
				if (Physics.Raycast(targetUnit.AttackPoint, rayDir, out hit, unit.attackRadius)) {
					tracePoint.point = hit.point - rayDir.normalized;
				}
			}

			accAngle += targetTraceAngleStep;
		}

		//calculate weights by distance
		float maxDistance = 0;
		float minDistance = float.MaxValue;
		
		foreach (var tracePoint in tracePoints) {
			float dist = (tracePoint.point - thisPos).magnitude;
			maxDistance = Mathf.Max(maxDistance, dist);
			minDistance = Mathf.Min(minDistance, dist);
		}

		Debug.Log("Calc weights");
		foreach (var tracePoint in tracePoints) {
			float dist = (tracePoint.point - thisPos).magnitude + UnityEngine.Random.Range(-10f, 10f);
			float cf = 1f - (dist - minDistance)/(maxDistance - minDistance);

			Debug.Log("DST: " + dist + ", cf: " + cf);

			tracePoint.weight = Mathf.Lerp(tracePointsWeightRange.x, tracePointsWeightRange.y, cf);
		}

		//lower weight on possible attacked traced points
		foreach (var un in UnitManager.instance.units) {
			if (un.faction == unit.faction && un != targetUnit)
				continue;

			foreach (var tracePoint in tracePoints) {
				if ((tracePoint.point - un.AttackPoint).magnitude < un.attackRadius)
					tracePoint.weight *= 0.5f;
			}
		}

		//compute mid point by weights
		Vector3 pointSumm = Vector3.zero;
		float weightsSumm = 0;
		foreach (var tracePoint in tracePoints) {
			pointSumm += tracePoint.point*tracePoint.weight;
			weightsSumm += tracePoint.weight;
		}
		
		//update attacking point
		attackingPoint = pointSumm/weightsSumm;

		targetUnitSearchedPos = targetUnit.transform.position;
	}

	void OnDrawGizmosSelected() {

		Gizmos.DrawLine(unit.AttackPoint, attackingPoint);
		Gizmos.DrawCube(attackingPoint, new Vector3(1, 1, 1)*0.2f);

		if (targetUnit != null) {
			foreach (var pt in tracePoints) {
				Gizmos.DrawLine(pt.point, targetUnit.AttackPoint);
				Gizmos.DrawSphere(pt.point, pt.weight*0.1f);
			}
		}
	}

	void CheckAttacking() {
		if (Time.time < lastAttackTime + unit.cd)
			return;

		if (!IsCanAttackTarget())
			return;

		lastAttackTime = Time.time;

		unit.Skin.Attack();
		unit.Weapon.Attack(targetUnit);
	}

	bool IsCanAttackTarget() {		
		if (targetUnit == null)
			return false;

		if (navMeshAgent.velocity.magnitude > 0.1f)
			return false;

		Vector3 dir = targetUnit.AttackPoint - unit.AttackPoint;

		if (dir.magnitude > unit.attackRadius)
			return false;

		if (Vector3.Angle(transform.forward, dir) > attackingAngle)
			return false;

		RaycastHit hit;
		if (Physics.Raycast(unit.AttackPoint, dir, out hit)) {
			if (hit.collider.gameObject != targetUnit.gameObject) {
				return false;
			}
		}

		return true;
	}

	private void RotateTowards (Vector3 point) {
		Vector3 direction = (point - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(direction);
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
	}
}
