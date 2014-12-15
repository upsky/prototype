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
}
