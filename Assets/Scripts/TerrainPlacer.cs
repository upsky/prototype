using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TerrainPlacer : MonoBehaviour {

	public Vector3 offset;
	public bool orient;
	public Vector3 orientOffset;

	void Awake() {
		if (Application.isPlaying)
			Destroy(this);
	}

	void Update() {
		RaycastHit[] hits = Physics.RaycastAll(transform.position + Vector3.up*500f, Vector3.down);

		foreach (var hit in hits) {
			if (hit.collider.GetComponent<Terrain>() != null) {
				transform.position = hit.point - offset;
				if (orient)
					transform.rotation = Quaternion.LookRotation(hit.normal)*Quaternion.Euler(orientOffset);
				break;
			}
		}
	}
}
