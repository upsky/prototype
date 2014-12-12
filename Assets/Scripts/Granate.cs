using UnityEngine;
using System.Collections;

public class Granate : MonoBehaviour {

	public float damage = 1f;

	public Unit.Faction faction;

	public AnimationCurve curva;

	Transform myTransform;
	Vector3 targetPos = Vector3.zero;
	float pathLength = 0;

	private void AdjustmentMovement(Vector3 targetPos) {
		myTransform = transform;
		this.targetPos = targetPos;
		pathLength = (targetPos - myTransform.position).magnitude;
	}

	void Update() {
		float localPathLength = (targetPos - myTransform.position).magnitude;
		myTransform.position = myTransform.position + new Vector3(0,curva.Evaluate(localPathLength/pathLength)*10f,0);
	}

	private Unit myOvner;

	public void Launch(Vector3 target, Unit.Faction faction, float damage, Unit myOvner) {
		this.damage = damage;
		this.faction = faction;
		this.myOvner = myOvner;
		AdjustmentMovement(target);
		rigidbody.velocity = target;
	}

	public void OnTriggerEnter(Collider other) {
		if(other.transform.GetComponent<Unit>() &&
		   faction != other.transform.GetComponent<Unit>().faction) {
			other.transform.GetComponent<Unit>().GetDamage(damage);
			Destroy(gameObject);
		}
	}
}
