using UnityEngine;
using System.Collections;

public class UnitHp : MonoBehaviour {

	RectTransform baseRectTransform;
	RectTransform currentRectTransform;

	void Start() {
		currentRectTransform = GetComponent<RectTransform>();
		baseRectTransform = new RectTransform();
		baseRectTransform = currentRectTransform;
	}

	public void HpHandler(float newHpFactor) {
		currentRectTransform.sizeDelta = new Vector2(100 * newHpFactor, baseRectTransform.sizeDelta.y);
	}
}
