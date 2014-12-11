using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Grid : MonoBehaviour {

	public enum ObjectType{
		PlayerUnit,
		BotUnit,
		Tower,
		Block,
		Free
	}

	[Serializable]
	public struct GridType {
		public Vector2 position;
		public ObjectType type;
	}

	public List<GridType> gridArray = new List<GridType>();

	void OnDrawGizmos() {
		foreach(var gridPoint in gridArray) 
			Gizmos.DrawRay(new Vector3(gridPoint.position.x, 0, gridPoint.position.y) + 
			               Vector3.down*100.0f, Vector3.up*200.0f);
	}

	public Vector3 GetGridPoint(ObjectType type) {
		GridType point = gridArray.First(x => x.type == type);
		gridArray.Remove(point);
		return new Vector3(point.position.x, 0, point.position.y);
	}
}
