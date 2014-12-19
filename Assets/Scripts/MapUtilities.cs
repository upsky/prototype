using UnityEngine;
using System.Collections;

public class MapUtilities : MonoBehaviour {

	public static MapUtilities instance;
	public Transform projectilesContainer;

	public static Transform ProjectilesContainer {
		get { return instance.projectilesContainer; }
	}

	void Awake() {
		instance = this;
	}
}
