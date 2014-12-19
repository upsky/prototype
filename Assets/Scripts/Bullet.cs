using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public float damage = 1f;
	public bool isEnemy;

	public GameObject damageRadiusObj;

	private Unit myOvner;
	private float damageRadius = 0;

	public void Launch(Vector3 target, bool isEnemy, float damage, Unit myOvner, float damageRadius, float bulletLifeTime) {
		this.damage = damage;
		this.isEnemy = isEnemy;
		this.myOvner = myOvner;
		this.damageRadius = damageRadius;
		rigidbody.velocity = target;
		Invoke("KillHimself", bulletLifeTime);
	}

	private void KillHimself() {
		if(damageRadius != 0){ 
			BtoomDamage(damageRadius);
		}
		Destroy(gameObject);
	}

	private void BtoomDamage(float area) {
		Collider[] units = Physics.OverlapSphere(transform.position,area);
		foreach (var key in units) {
			if(key.GetComponent<Unit>() != null) {
				Unit target = key.GetComponent<Unit>();
				if(target.isEnemy != myOvner.isEnemy) {
					GameObject damageRad = (GameObject)Instantiate(damageRadiusObj,transform.position,transform.rotation);
					damageRad.transform.localScale = new Vector3(area,area,area);
					damageRad.transform.parent = MapUtilities.ProjectilesContainer;
					target.ApplyDamage(damage);
				}
			}
		}
	}

	public void OnTriggerEnter(Collider other) {
		if(other.transform.GetComponent<Unit>() && isEnemy != other.transform.GetComponent<Unit>().isEnemy) {
			if(damageRadius != 0) {
				BtoomDamage(damageRadius);
			} else {
				other.transform.GetComponent<Unit>().ApplyDamage(damage);
			}
			Destroy(gameObject);
		}
	}
}
