using UnityEngine;
using System.Linq;
using System.Collections;
using UnityEditor;

public class PirateAI : MonoBehaviour {

	public NavMeshAgent move;
	public float findTargetDelay = 1.5f;

	private Unit unit;
	private Unit targetUnit;
	
	public bool playerOrder;

	private Color factionColor;


	public bool isTower = false;

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
	}

	public void SetMoveOrder(Vector3 newPos) {
		StopInvoks();
		MoveToTarget(newPos);
		playerOrder = true;
	}

	private void StartInvoks() {
		InvokeRepeating("FindTraget",0,findTargetDelay);
	}

	private void StopInvoks() {
		CancelInvoke("FindTraget");
	}

	PlayerController playerController;

	void Start() {
		move = GetComponent<NavMeshAgent>();
		unit = GetComponent<Unit>();
		playerController = GameObject.Find("Logic").GetComponent<PlayerController>();
		
		Color redf = new Color(1f, 0f, 0f, 0.6f);
		Color bluef = new Color(0f, 0f, 1f, 0.6f);
		factionColor = unit.isEnemy ? bluef:redf;

		move.avoidancePriority = Random.Range(30, 70);

		InvokeRepeating("FindTraget",0,findTargetDelay);
		InvokeRepeating("Attack",0,unit.cd);
	}

	void Attack() {
		if (targetUnit == null)
			return;
		
		if (IsCanAttackTargetFromPoint(unit.AttackPoint))
		{
			if (Vector3.Angle(targetUnit.AttackPoint - transform.position, transform.forward) < 45 && move.velocity.magnitude < 0.1f) {
				unit.Skin.Attack();
				unit.Weapon.Attack (targetUnit);
			}
		}
	}

	bool IsCanAttackTargetFromPoint(Vector3 position) {
		if (targetUnit == null)
			return false;

		Vector3 dir = targetUnit.AttackPoint - position;

		if (dir.magnitude > unit.attackRadius)
			return false;

		RaycastHit hit;
		//Debug.DrawRay(position, dir, factionColor);
		if (Physics.Raycast(position, dir, out hit)) {
			if (hit.collider.gameObject != targetUnit.gameObject)
			{
				Debug.DrawRay(hit.point, hit.normal*0.2f, Color.yellow);
				//Debug.Log("Ray hit: " + hit.collider.transform.gameObject, hit.collider.transform.gameObject);
				return false;
			}
		}

		return true;
	}

	Vector3 FindAttackingPosition() {
		if (IsCanAttackTargetFromPoint(unit.AttackPoint))
			return transform.position;

		const float traceAngleThreshold = 15.0f;
		Vector3 targetDir = targetUnit.AttackPoint - transform.position;
		float dist = targetDir.magnitude;
		Vector3 initialDir = targetDir/dist*Mathf.Min(unit.attackRadius*-0.7f, dist);

		float accumulateAngle = 0;
		while (accumulateAngle < 180f) {
			Vector3 leftWorld = initialDir; leftWorld = leftWorld.RotateY(accumulateAngle*Mathf.Deg2Rad);
			Vector3 rightWorld = initialDir; rightWorld = rightWorld.RotateY(-accumulateAngle*Mathf.Deg2Rad);
			
// 			Debug.DrawRay(targetUnit.transform.position, leftWorld, factionColor, findTargetDelay);
// 			Debug.DrawRay(targetUnit.transform.position, rightWorld, factionColor, findTargetDelay);
// 			Debug.DrawLine(transform.position, targetUnit.transform.position + leftWorld, factionColor, findTargetDelay);
// 			Debug.DrawLine(transform.position, targetUnit.transform.position + rightWorld, factionColor, findTargetDelay);

			RaycastHit hit;
			if (Physics.Raycast(targetUnit.transform.position + rightWorld + Vector3.up*50f, Vector3.down, out hit)) {
				Vector3 traced = hit.point + Vector3.up*0.5f;
				Debug.DrawRay(hit.point, hit.normal*0.5f, factionColor);
				Debug.DrawLine(traced, targetUnit.transform.position, factionColor, findTargetDelay);
				if (IsCanAttackTargetFromPoint(traced))
					return traced;
			}
			
			if (Physics.Raycast(targetUnit.transform.position + leftWorld + Vector3.up*50f, Vector3.down, out hit)) {
				Vector3 traced = hit.point + Vector3.up*0.5f;
				Debug.DrawRay(hit.point, hit.normal*0.5f, factionColor);
				Debug.DrawLine(traced, targetUnit.transform.position, factionColor, findTargetDelay);
				if (IsCanAttackTargetFromPoint(traced))
					return traced;
			}
			
			accumulateAngle += traceAngleThreshold;
		}

		return targetUnit.transform.position;
	}

	void FindTraget() {
		Unit nearestUnit = UnitManager.instance.GetNearestUnit(!unit.isEnemy, transform.position);

		if (nearestUnit != null) {
			if (targetUnit == null) 
				targetUnit = nearestUnit;
			else {
				float targetUnitDist = (transform.position - targetUnit.AttackPoint).magnitude;
				float nearestUnitDist = (transform.position - nearestUnit.AttackPoint).magnitude;
				
				if (nearestUnitDist < targetUnitDist - unit.attackRadius*0.5f) 
					targetUnit = nearestUnit;
			}
		}
				
		if (targetUnit != null && !IsCanAttackTargetFromPoint(unit.AttackPoint))
			MoveToTarget(FindAttackingPosition());
		else move.Stop();
	}

	void MoveToTarget(Vector3 targetPosition) {
		if (isTower)
			return;

		move.SetDestination(targetPosition);
		//Debug.DrawRay(targetPosition, Vector3.up*2f, factionColor, 1);
	}

	void Update() {
		if(move.remainingDistance < 0.1f && playerOrder) {
			StartInvoks();
			playerOrder = false;
		}
		
		if(move.velocity.magnitude <= 0.1f && targetUnit != null) {
			RotateTowards(targetUnit.transform);
		}

		unit.Skin.Speed = move.velocity.magnitude;

		if (Selection.activeGameObject == gameObject || Selection.activeGameObject == transform.GetChild(0).gameObject)
			factionColor.a = 0.6f;
		else
			factionColor.a = 0f;
		
		Debug.DrawLine(transform.position, move.destination, factionColor);
		Debug.DrawRay(move.destination, Vector2.up*2f, factionColor);
		if (targetUnit != null)
			Debug.DrawLine(transform.position, targetUnit.AttackPoint, factionColor);
	}

	private void RotateTowards (Transform target) {
		Vector3 direction = (target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(direction);
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 2f);
	}
}
