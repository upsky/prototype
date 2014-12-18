using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PlayerPartyFollowingCamera : MonoBehaviour {

	public float moveCoef = 2f;

	public void Update() {
		Vector3 posSumm = Vector3.zero;
		float playerUnitsCount = 0f;
		foreach (var un in UnitManager.instance.units) {
			if (un.isEnemy)
				continue;

			posSumm += un.transform.position;
			playerUnitsCount += 1f;
		}

		if (playerUnitsCount < 1.5f)
			return;

		posSumm /= playerUnitsCount;

		transform.position = Vector3.Lerp(transform.position, posSumm, Time.deltaTime*moveCoef);
	}
}
