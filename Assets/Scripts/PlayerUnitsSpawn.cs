using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PlayerUnitsSpawn : MonoBehaviour {

	public PivotsPath path;
	public List<Unit> unitsPrefabs;
	public float spawnDistance;
	public PlayerPartyFollowingCamera playerCamera;

	private int unitIdx;

	public void CallUnits(int count) {
		Vector3 spawnPoint = path.GetPathPoint(path.GetDistanceByPoint(playerCamera.transform.position) - spawnDistance);
		Vector3 camDbgPoint = path.GetPathPoint(path.GetDistanceByPoint(playerCamera.transform.position));
		
		Debug.DrawRay(camDbgPoint, Vector3.up*2f, Color.magenta, 5f);

		for (int i = 0; i < count; i++)
			CreateUnit(spawnPoint);
	}

	private Unit CreateUnit(Vector3 position) {
		Debug.DrawRay(position, Vector3.up*2f, Color.yellow, 5f);

		GameObject prefab = unitsPrefabs[unitIdx++].gameObject;
		unitIdx = unitIdx % unitsPrefabs.Count;
		GameObject unitObj = Instantiate(prefab, position, Quaternion.identity) as GameObject;
		Unit unit = unitObj.GetComponent<Unit>();
		unit.InitializeFaction(false);
		return unit;
	}
}
