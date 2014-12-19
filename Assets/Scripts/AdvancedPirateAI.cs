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
	public Vector2 tracePointsWeightByDistanceRange = new Vector2(5f, 10f);
	public Vector2 tracePointsWeightByRandomRange = new Vector2(5f, 10f);
	public Vector2 centerTracePointWeightRange = new Vector2(2f, 7f);
	public float dangerousWeightsCoef = 0.5f;

	[Range(0f, 1f)]
	public float tracePointsRandomCoef = 0.3f;

	private NavMeshAgent navMeshAgent;
	private Unit unit;
	private Unit targetUnit;
	private float stoppingDistance;
	
	private float attackPointRefindRate;
	private float targetRefindRate;
	private float lastTargetRefindTime;
	private float lastAttackPointRefindTime;
	private float lastAttackTime;
	private Vector3 targetUnitSearchedPos;
	private Vector3 attackingPoint;

	private List<TracePoint> tracePoints = new List<TracePoint>();
	private List<float> tracePointsRandomWeights = new List<float>();
	private float centerWeight;
	private float currCenterWeight;

	private bool playerOrderMovement;
	private bool playerOrderTarget;
	private Action onPlayerOrderComplete;

	private MeshRenderer dbgMeshRenderer;
	private MeshFilter dbgMeshFilter;
	private GameObject dbgMeshObject;

	private bool selected;

	public void SetMovingOrder(Vector3 point, Action onComplete) {
		playerOrderTarget = false;
		playerOrderMovement = true;
		targetUnit = null;
		attackingPoint = point;
		navMeshAgent.destination = point;
		navMeshAgent.stoppingDistance = 1f;
		onPlayerOrderComplete = onComplete;
	}

	public void SetTargetOrder(Unit target, Action onComplete) {
		playerOrderMovement = false;
		playerOrderTarget = true;
		targetUnit = target;
		onPlayerOrderComplete = onComplete;
	}

	void Start () {
		unit = GetComponent<Unit>();
		navMeshAgent = GetComponent<NavMeshAgent>();

		navMeshAgent.avoidancePriority = UnityEngine.Random.Range(10, 100);
		stoppingDistance = navMeshAgent.stoppingDistance;

		dbgMeshObject = new GameObject("dbgMesh");
		dbgMeshObject.transform.parent = GameObject.FindWithTag("AIDebug").transform;
		dbgMeshRenderer = dbgMeshObject.AddComponent<MeshRenderer>();
		dbgMeshFilter = dbgMeshObject.AddComponent<MeshFilter>();
		dbgMeshFilter.mesh = new Mesh();
		dbgMeshRenderer.material = new Material(Shader.Find("Sprites/Default"));
		dbgMeshRenderer.material.mainTexture = new Texture();
	}

	void OnDestroy() {
		if (playerOrderMovement || playerOrderTarget)
			onPlayerOrderComplete();

		Destroy(dbgMeshObject);
	}
	
	void Update () {
		CheckTargetRefind();
		CheckAttackPointRefind(false, true);
		CheckTargetMovement();
		CheckAttacking();
		UpdateMovement();

		
		dbgMeshObject.SetActive(selected);
		selected = false;
	}

	void CheckTargetMovement() {
		if (targetUnit == null)
			return;

		if ((targetUnitSearchedPos - targetUnit.transform.position).magnitude > targetMovementThreshold)
			CheckAttackPointRefind(true);
	}

	void UpdateMovement() {

		if (!playerOrderMovement) {
			if (targetUnit != null)
				navMeshAgent.SetDestination( navMeshAgent.destination.SmartLerp(attackingPoint, Time.deltaTime*3f, 0.1f) );
			else
				navMeshAgent.Stop();
		}
		else {
			if ((navMeshAgent.destination - transform.position).magnitude - navMeshAgent.stoppingDistance < 0.2f) {
				navMeshAgent.stoppingDistance = stoppingDistance;
				playerOrderMovement = false;
				onPlayerOrderComplete();

				//Debug.Log("Moving order complete!", gameObject);
			}
		}


		if (navMeshAgent.velocity.magnitude < 0.1f && targetUnit != null) {
			Vector3 lookPoint = targetUnit.AttackPoint;
			lookPoint.y = transform.position.y;
			RotateTowards(lookPoint);
		}

		unit.Skin.Speed = navMeshAgent.velocity.magnitude;
	}

	void CheckTargetRefind(bool forcible = false) {
		if (playerOrderTarget || playerOrderMovement)
			return;

		if (Time.time < lastTargetRefindTime + targetRefindRate && !forcible)
			return;

		targetRefindRate = UnityEngine.Random.Range(targetRefindRateRange.x, targetRefindRateRange.y);
		lastTargetRefindTime = Time.time;

		Unit lastTargetUnit = targetUnit;

		Unit nearestEnemy = UnitManager.instance.GetNearestUnit(!unit.isEnemy, transform.position);
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
			CheckAttackPointRefind(true, true);
		}
	}

	void CheckAttackPointRefind(bool forcible = false, bool updateRandomWeights = false) {
		if (playerOrderMovement)
			return;

		if (Time.time < lastAttackPointRefindTime + attackPointRefindRate && !forcible || targetUnit == null)
			return;		

		attackPointRefindRate = UnityEngine.Random.Range(attackPointRefindRateRange.x, attackPointRefindRateRange.y);
		lastAttackPointRefindTime = Time.time;

		Vector3 thisPos = unit.AttackPoint;
		tracePoints.Clear();

		//trace target rays
		float accAngle = 0;
		while(accAngle <= 360) {
			Vector3 rayDir = new Vector3(Mathf.Cos(accAngle*Mathf.Deg2Rad), 0, Mathf.Sin(accAngle*Mathf.Deg2Rad))*unit.attackRadius*0.7f;

			TracePoint tracePoint = new TracePoint(){ point = targetUnit.AttackPoint + rayDir };
			tracePoints.Add(tracePoint);

			//ground adjusting
			RaycastHit[] landHits = Physics.RaycastAll(tracePoint.point + Vector3.up*50f, Vector3.down);
			foreach (var hit in landHits) {
				if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Land"))
					continue;

				Vector3 groundTracedPoint = hit.point + (unit.AttackPoint - transform.position);
				tracePoint.point = groundTracedPoint;

				rayDir = groundTracedPoint - targetUnit.AttackPoint;				

				break;
			}

			//trace from target to ground point
			RaycastHit[] obsHits = Physics.RaycastAll(targetUnit.AttackPoint, rayDir.normalized, unit.attackRadius*0.7f);
			//obsHits.ToList().ForEach(x => Debug.DrawRay(x.point, x.normal*0.5f, Color.yellow, 1f));

			foreach (var ht in obsHits) {
				if (ht.collider == collider)
					continue;

				tracePoint.point = ht.point - rayDir.normalized;

				break;
			}

			accAngle += targetTraceAngleStep;
		}

		//update random weights
		if (updateRandomWeights) {		
			centerWeight = UnityEngine.Random.Range(centerTracePointWeightRange.x, centerTracePointWeightRange.y);
			tracePointsRandomWeights.Clear();
			int supports = tracePoints.Count/4;
			float[] randomSupp = new float[supports];
			for (int i= 0; i < supports; i++)
				randomSupp[i] = UnityEngine.Random.Range(tracePointsWeightByRandomRange.x, tracePointsWeightByRandomRange.y);

			for (int i = 0; i < tracePoints.Count; i++) {
				float cf = (float)i/(float)tracePoints.Count;
				int id = Mathf.Clamp( Mathf.FloorToInt(cf*(float)supports), 0, supports - 2);
				float idCf = (cf - (float)id/(float)supports)/(1f/(float)supports);

				tracePointsRandomWeights.Add(Mathf.Lerp(randomSupp[id], randomSupp[id + 1], idCf));
			}
		}

		//calculate weights by distance
		float maxDistance = 0;
		float minDistance = float.MaxValue;
		
		foreach (var tracePoint in tracePoints) {
			float dist = (tracePoint.point - thisPos).magnitude;
			maxDistance = Mathf.Max(maxDistance, dist);
			minDistance = Mathf.Min(minDistance, dist);
		}

		int ix = 0;
		foreach (var tracePoint in tracePoints) {
			float dist = (tracePoint.point - thisPos).magnitude;
			float distCf = 1f - (dist - minDistance)/(maxDistance - minDistance);			
			float distWeight = Mathf.Lerp(tracePointsWeightByDistanceRange.x, tracePointsWeightByDistanceRange.y, distCf);
			float randomWeight = tracePointsRandomWeights[ix];

			tracePoint.weight = Mathf.Lerp(distWeight, randomWeight, tracePointsRandomCoef);
			ix++;
		}

		//lower weight on possible attacked traced points
		currCenterWeight = centerWeight;
		foreach (var un in UnitManager.instance.units) {
			if (un.isEnemy == unit.isEnemy || un == targetUnit)
				continue;

			foreach (var tracePoint in tracePoints) {
				if ((tracePoint.point - un.AttackPoint).magnitude < un.attackRadius)
					tracePoint.weight *= dangerousWeightsCoef;
			}

			if ((targetUnit.AttackPoint - un.AttackPoint).magnitude < un.attackRadius)
				currCenterWeight *= dangerousWeightsCoef;
		}

		//get point by weight
		Vector3 res = targetUnit.AttackPoint;
		float maxWeight = 0f;

		for (int i = 0; i < tracePoints.Count; i++) {
			TracePoint l = tracePoints[i];
			TracePoint r = tracePoints[(i + 1)%tracePoints.Count];

			float summaryWeigth = centerWeight + l.weight + r.weight;
			Vector3 pt = (targetUnit.AttackPoint*centerWeight + l.point*l.weight + r.point*r.weight)/summaryWeigth;

			if (summaryWeigth > maxWeight) {
				res = pt;
				maxWeight = summaryWeigth;
			}
		}
		
		//update attacking point
		attackingPoint = res;

		targetUnitSearchedPos = targetUnit.transform.position;

		BuildDbgMesh();
	}

	void OnDrawGizmosSelected() {
		
		if (!playerOrderMovement) {
			Gizmos.DrawLine(attackingPoint, navMeshAgent.destination);
		}

		Gizmos.DrawLine(unit.AttackPoint, navMeshAgent.destination);
		Gizmos.DrawCube(navMeshAgent.destination, new Vector3(1, 1, 1)*0.2f);

		if (targetUnit != null && !playerOrderMovement) {
// 			foreach (var pt in tracePoints) {
// 				//Gizmos.DrawLine(pt.point, targetUnit.AttackPoint);
// 				Gizmos.DrawSphere(pt.point, pt.weight*0.01f);
// 			}
// 			
			for (int i = 0; i < tracePoints.Count; i++) {
 				//Gizmos.DrawSphere(tracePoints[i].point, tracePoints[i].weight*0.01f);
				if (i > 0)
					Gizmos.DrawLine(tracePoints[i].point, tracePoints[i - 1].point);
			}
		}

		selected = true;
	}

	void BuildDbgMesh() {

		Mesh mesh = dbgMeshFilter.mesh;
		mesh.Clear();

		if (targetUnit == null)
			return;

		int polyCount = tracePoints.Count - 1;
		Vector3[] verticies = new Vector3[polyCount*3];
		Vector2[] uv = new Vector2[polyCount*3];
		Color[] colors = new Color[polyCount*3];
		int[] polyIndexes = new int[polyCount*3];
		
		Color minWeightColor = new Color(1f, 0f, 0f, 0.5f);
		Color maxWeightColor = new Color(0f, 1f, 0f, 0.5f);
		
		float minWeight = Mathf.Min( tracePoints.Min(x => x.weight), currCenterWeight);
		float maxWeight = Mathf.Max( tracePoints.Max(x => x.weight), currCenterWeight);

		for (int i = 0; i < tracePoints.Count - 1; i++) {
			verticies[i*3 + 0] = targetUnit.AttackPoint;
			verticies[i*3 + 1] = tracePoints[i].point;
			verticies[i*3 + 2] = tracePoints[i + 1].point;

			colors[i*3 + 0] = Color.Lerp(minWeightColor, maxWeightColor, (currCenterWeight - minWeight)/(maxWeight - minWeight));
			colors[i*3 + 1] = Color.Lerp(minWeightColor, maxWeightColor, (tracePoints[i].weight - minWeight)/(maxWeight - minWeight));
			colors[i*3 + 2] = Color.Lerp(minWeightColor, maxWeightColor, (tracePoints[i + 1].weight - minWeight)/(maxWeight - minWeight));
			
			polyIndexes[i*3 + 0] = i*3 + 0;
			polyIndexes[i*3 + 1] = i*3 + 2;
			polyIndexes[i*3 + 2] = i*3 + 1;
		}
		
		mesh.vertices = verticies;
		mesh.colors = colors;
		mesh.triangles = polyIndexes;
		mesh.uv = uv;
	}

	void CheckAttacking() {
		if (playerOrderTarget && targetUnit == null) {
			playerOrderTarget = false;
			onPlayerOrderComplete();
		}

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
