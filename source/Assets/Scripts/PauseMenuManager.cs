using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour  {

	[SerializeField]
	private GameObject InGameMenuCanvas;
    [SerializeField]
    InputField nameForHS;
    [SerializeField]
	public static bool Paused = false;

    // Use this for initialization
    void Start () {
		InGameMenuCanvas.gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Cancel")) {
			Pause ();
		}
	}
		
	public void StartLevel () {
		SceneManager.LoadScene (1);
		Time.timeScale = 1;
		Paused = false;
        AudioListener.pause = false;
        GameManager.scoreFromObjectives = 0;
        FPSRunnerMotor.Instance.canShoot = true;
		// Start new level
	}

	public void Continue () {
		InGameMenuCanvas.gameObject.SetActive (false);
		Time.timeScale = 1;
        FPSRunnerMotor.Instance.canShoot = true;
        AudioListener.pause = false;
        FPSRunnerMotor.Instance.SetCustomCursor();
        Paused = false;
		// Unpause
	}

	public void ExitToMainMenu () {
		Time.timeScale = 1;
		Paused = false;
        FPSRunnerMotor.Instance.canShoot = true;
        AudioListener.pause = false;
        SceneManager.LoadScene (0);
	}

	public void Pause () {
		Time.timeScale = 0;
        AudioListener.pause = true;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Paused = true;
		InGameMenuCanvas.gameObject.SetActive (true);
	}

    public void continueAfterGameOver()
    {
        if (LevelManager.isScoreTop(GameManager.completeScore))
        {
            LevelManager.writeFinalScore(GameManager.completeScore, nameForHS.text);
        }
        nameForHS.gameObject.SetActive(false);
        ExitToMainMenu();
    }
}
