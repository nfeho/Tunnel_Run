using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    // instance of player
    GameObject player;
    FPSRunnerMotor playerScript;
    float currentTimeSpeed = 1;

    public static int completeScore { get; set; }
    public static int scoreFromObjectives { get; set; }

    [SerializeField]
    Text scoreText;

    void Awake () {

        player = GameObject.FindGameObjectWithTag("Player");
    }
	
	// Update is called once per frame
	void Update () {
		if (Time.timeScale > 0) {

			float playerPosition = player.transform.position.z;
			if (playerPosition < 500) {
				currentTimeSpeed = 1.0f;
			} else if (playerPosition < 1000) {
				currentTimeSpeed = 1.1f;
			} else if (playerPosition < 1500) {
				currentTimeSpeed = 1.2f;
			} else if (playerPosition < 2000) {
				currentTimeSpeed = 1.3f;
			} else if (playerPosition < 2600) {
                currentTimeSpeed = 1.4f;
            } else if (playerPosition < 3200) {
                currentTimeSpeed = 1.5f;
            } else if (playerPosition < 3900) {
                currentTimeSpeed = 1.6f;
            } else if (playerPosition < 4700) {
                currentTimeSpeed = 1.7f;
            } else if (playerPosition < 5600) {
                currentTimeSpeed = 1.8f;
            }

        Time.timeScale = currentTimeSpeed;
            completeScore = (int)(player.transform.position.z / 10) + scoreFromObjectives;

            scoreText.text = "Score : " + completeScore;
		}
    }
}
