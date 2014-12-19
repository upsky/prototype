using UnityEngine;
using System.Collections;

public class PivotsPath : MonoBehaviour {

	public Transform pivotsObject;
	private float length;

	public float Length {
		get { return length; }
	}

	void Awake() {
		length = 0;
		for (int i = 0; i < pivotsObject.childCount - 1; i++)
			length += (pivotsObject.GetChild(i).position - pivotsObject.GetChild(i + 1).position).magnitude;
	}

	public Vector3 GetPathPoint(float distance) {
		distance = Mathf.Clamp(distance, 0, length);
		float lenSumm = 0;
		for (int i = 0; i < pivotsObject.childCount - 1; i++) {
			Vector3 ta = pivotsObject.GetChild(i).position;
			Vector3 tb = pivotsObject.GetChild(i + 1).position;

			Vector3 dir = tb - ta;
			float segLen = dir.magnitude;
			lenSumm += segLen;

			if (lenSumm >= distance) {
				lenSumm -= segLen;
				float cf = (distance - lenSumm)/segLen;
				return Vector3.Lerp(ta, tb, cf);
			}
		}

		return pivotsObject.GetChild(0).position;
	}

	public float GetDistanceByPoint(Vector3 point) {
		float lenSumm = 0;
		float res = 0;
		float resMinDist = float.MaxValue;
		Vector3 projPoint = Vector3.zero;

		for (int i = 0; i < pivotsObject.childCount - 1; i++) {			
			Vector3 ta = pivotsObject.GetChild(i).position;
			Vector3 tb = pivotsObject.GetChild(i + 1).position;

			Vector3 dir = tb - ta;
			float segLen = dir.magnitude;
			Vector3 dir1 = dir/segLen;

			float lProj = Vector3.Dot(dir1, point - ta);
			if (lProj < 0 || lProj > segLen)
				continue;
			
			Vector3 n = Vector3.Cross(dir1, Vector3.up);
			float proj = Vector3.Dot(point - ta, n);

			Debug.DrawRay(ta, n, new Color(1f, 0f, 0f, 0.5f), 5f);

			if (Mathf.Abs(proj) < resMinDist) {
				resMinDist = Mathf.Abs(proj);
				projPoint = point - n*proj;
				res = lenSumm + (projPoint - ta).magnitude;
			}

			lenSumm += segLen;
		}

		Debug.DrawLine(point, projPoint, Color.green, 5f);

		return res;
	}
	
	void OnDrawGizmos() {
		if (pivotsObject == null)
			return;

		for (int i = 0; i < pivotsObject.childCount - 1; i++)
			Gizmos.DrawLine(pivotsObject.GetChild(i).position, pivotsObject.GetChild(i + 1).position);
	}
}
