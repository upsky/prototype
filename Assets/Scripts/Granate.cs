using UnityEngine;
using System.Collections;

public class Granate : MonoBehaviour {

	public float damage = 1f;
	public ParticleSystem btoom;
	public bool isEnemy;
	public AnimationCurve curva;
	public TrailRenderer line;
	public GameObject damageRadiusObj;

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
				Vector3.MoveTowards(new Vector3(myTransform.position.x,0,myTransform.position.z),new Vector3(targetPos.x,0,targetPos.z),Time.deltaTime*5f);
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
		Collider[] units = Physics.OverlapSphere(transform.position,damageRadius);
		foreach (var key in units) {
			if(key.GetComponent<Unit>() != null) {
				Unit target = key.GetComponent<Unit>();
				if(target.isEnemy != myOvner.isEnemy) {
					GameObject damageRad = (GameObject)Instantiate(damageRadiusObj,transform.position,transform.rotation);
					damageRad.transform.localScale = new Vector3(damageRadius,damageRadius,damageRadius);
					damageRad.transform.parent = MapUtilities.ProjectilesContainer;
					target.ApplyDamage(damage);
				}
			}
		}
	}

	private void KillHimself() {
		Destroy(gameObject);
	}

	private Unit myOvner;

	float damageRadius = 4;

	public void Launch(Vector3 target, bool isEnemy, float damage, Unit myOvner, float damageRadius) {
		this.damage = damage;
		this.isEnemy = isEnemy;
		this.myOvner = myOvner;
		this.damageRadius = damageRadius;
		AdjustmentMovement(target);
	}
}
