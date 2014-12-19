using UnityEngine;
using System.Collections;

public class ArmyUnitPosition : MonoBehaviour {

	[Range(0.0f, 10.0f)]
	public float probabilityWeight = 5f;
	private float rndProbabilityWeight;

	public float RndProbabilityWeight {
		get { return rndProbabilityWeight; }
	}

	public void RandomizeWeight() {
		rndProbabilityWeight = Random.Range(0f, 10f);
	}
}
