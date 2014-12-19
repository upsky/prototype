using UnityEngine;
using System.Collections;

public class MapObjectProbability : MonoBehaviour {

	[Range(0.0f, 1.0f)]
	public float probability = 0.5f;

	void Awake() {
		if (Random.Range(0f, 1f) > probability)
			Destroy(gameObject);
	}
}
