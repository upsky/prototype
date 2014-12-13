using UnityEngine;
using System.Collections;

public class Granate : MonoBehaviour {

	public float damage = 1f;

	public ParticleSystem btoom;

	public Unit.Faction faction;

	public AnimationCurve curva;

	public TrailRenderer line;

	Transform myTransform;
	Vector3 targetPos = Vector3.zero;
	float pathLength = 0;
	bool start = false;

	private void AdjustmentMovement(Vector3 targetPos) {
		myTransform = transform;
		this.targetPos = targetPos;
		pathLength = (new Vector2(targetPos.x,targetPos.z) - new Vector2(myTransform.position.x,myTransform.position.z)).magnitude;
		start = true;
	}

	void Update() {
		if(start) {
			float localPathLength = (new Vector2(targetPos.x,targetPos.z) - new Vector2(myTransform.position.x,myTransform.position.z)).magnitude;
			myTransform.position = new Vector3(0,curva.Evaluate(localPathLength/pathLength)*3f,0) + 
				Vector3.MoveTowards(new Vector3(myTransform.position.x,0,myTransform.position.z),new Vector3(targetPos.x,0,targetPos.z),Time.deltaTime*3f);
			if((targetPos-myTransform.position).magnitude < 1f) {
				btoom.Play();
				GetComponent<Renderer>().enabled = false;
				start = false;
				BtoomDamage();
				Invoke("KillHimself",2f);
			}
		}
	}

	private void BtoomDamage() {
		Collider[] units = Physics.OverlapSphere(transform.position,3);
		foreach (var key in units) {
			if(key.GetComponent<Unit>() != null) {
				Unit target = key.GetComponent<Unit>();
				if(target.faction != myOvner.faction) {
					target.GetDamage(damage);
				}
			}
		}
	}

	private void KillHimself() {
		Destroy(gameObject);
	}

	private Unit myOvner;

	public void Launch(Vector3 target, Unit.Faction faction, float damage, Unit myOvner) {
		this.damage = damage;
		this.faction = faction;
		this.myOvner = myOvner;
		AdjustmentMovement(target);
	}
}
