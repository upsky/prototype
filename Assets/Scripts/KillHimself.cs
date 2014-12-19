using UnityEngine;
using System.Collections;

public class KillHimself : MonoBehaviour {
	
	void Start () {
		Invoke("DeleteObj",1);
	}

	void DeleteObj() {
		Destroy(gameObject);
	}
}
