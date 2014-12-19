using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class MyTestMath : MonoBehaviour {

	[Serializable]
	public class Weight {
		public Transform transform;

		[Range(0, 100)]
		public float weight;
	}

	public List<Weight> weights = new List<Weight>();
	public bool check;

	void Update() {
		if (check) {
			check = false;
			weights.Clear();

			foreach (Transform child in transform) {
				weights.Add(new Weight(){ transform = child });
			}
		}
	}

	void OnDrawGizmosSelected() {
		float weightsSumm = 0;
		Vector3 posSumm = Vector3.zero;

		foreach (var wt in weights) {
			weightsSumm += wt.weight;
			posSumm += wt.transform.position*wt.weight;
		}

		Vector3 res = posSumm/weightsSumm;

		Gizmos.DrawSphere(res, 1f);
	}
}
