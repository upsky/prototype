using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreCounter : MonoBehaviour {

	public static Text Score;
	public static float countScore = 0;

	void Start () {
		Score = GameObject.Find("Score").GetComponent<Text>();
	}

	public static void UpdateScore(float score) {
		countScore += score;
		Score.text ="Score: " + countScore; 
	}
}
