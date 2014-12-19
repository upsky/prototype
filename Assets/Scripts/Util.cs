using UnityEngine;
using System.Collections;

public static class Util {

	public static Vector3 RotateX( this Vector3 v, float angle )
    {
        float sin = Mathf.Sin( angle );
        float cos = Mathf.Cos( angle );
       
        float ty = v.y;
        float tz = v.z;

		return new Vector3(v.x, (cos * ty) - (sin * tz), (cos * tz) + (sin * ty));
    }
   
    public static Vector3 RotateY( this Vector3 v, float angle )
    {
        float sin = Mathf.Sin( angle );
        float cos = Mathf.Cos( angle );
       
        float tx = v.x;
        float tz = v.z;

		return new Vector3((cos * tx) + (sin * tz), v.y, (cos * tz) - (sin * tx));
    }

	public static Vector3 SmartLerp(this Vector3 from, Vector3 to, float coef, float minDelta = 0.001f) {
		Vector3 res = Vector3.Lerp(from, to, coef);
		if ((res - to).sqrMagnitude < minDelta*minDelta)
			res = to;

		return res;
	}

	public static void Reparent(this Transform tr, Transform newParent) {
		Vector3 locPos = tr.localPosition;
		Quaternion locRot = tr.localRotation;
		Vector3 locScale = tr.localScale;

		tr.parent = newParent;;

		tr.localPosition = locPos;
		tr.localRotation = locRot;
		tr.localScale = locScale;
	}
}
