using UnityEngine;
using System.Collections;

public class PirateSkin : MonoBehaviour {

	public Transform shootPoint;
	private Animator animator;

	public float Speed {
		set { animator.SetFloat("speed", value); }
	}

	public Transform ShootPoint {
		get { return shootPoint; }
	}

	void Awake() {
		animator = GetComponent<Animator>();
	}

	public void Attack() {
		animator.SetTrigger("attack");
	}
}
