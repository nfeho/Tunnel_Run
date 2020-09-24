using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

    // prefix for highscore number - from locals
    private const string HSNum = "hsNum";
    // prefix for highscore plaer name - from locals
    private const string HSPlayer = "hsPlayer";
    // number of entries in high scores
    private const int numOfEntriesInHS = 6;

    [SerializeField]
	private GameObject MainMenuCanvas;
	[SerializeField]
	private GameObject HighScoresCanvas;

    // texts for high scores
    [SerializeField]
    List<Text> textsHS = new List<Text>();

	// Use this for initialization
	void Start () {
		HighScoresCanvas.gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void StartLevel () {
		SceneManager.LoadScene (1);
        Time.timeScale = 1;
        PauseMenuManager.Paused = false;
        GameManager.scoreFromObjectives = 0;
        // Start new level
    }

    public void StartTutorial () {
		SceneManager.LoadScene (2); // Tu bude scena pre Tutorial Run (2)
	}

	public void ShowHighScores () {
		// Make Main Menu Canvas Inactive
		MainMenuCanvas.gameObject.SetActive(false);
        // Make High Scores Canvas Active
        HighScoresCanvas.gameObject.SetActive(true);
        for (int i = 0; i < numOfEntriesInHS; i++)
        {
            string name = PlayerPrefs.GetString(i + HSPlayer);
            if (name == "")
            {
                textsHS[i].text = "";
            }
            else
            {
                textsHS[i].text = PlayerPrefs.GetString(i + HSPlayer) + " " + PlayerPrefs.GetInt(i + HSNum);
            }
            
        }
	}
    // maybe we will moved this into more propriet manager
    public static void writeFinalScore(int score, string name)
    {
        int newScore;
        string newName;
        int oldScore;
        string oldName;
        newScore = score;
        newName = name;
        for (int i = 0; i < numOfEntriesInHS; i++)
        {
            if (PlayerPrefs.HasKey(i + HSNum))
            {
                if (PlayerPrefs.GetInt(i + HSNum) < newScore)
                {
                    // new score is higher than the stored score
                    oldScore = PlayerPrefs.GetInt(i + HSNum);
                    oldName = PlayerPrefs.GetString(i + HSPlayer);
                    PlayerPrefs.SetInt(i + HSNum, newScore);
                    PlayerPrefs.SetString(i + HSPlayer, newName);
                    newScore = oldScore;
                    newName = oldName;
                }
            }
            else
            {
                PlayerPrefs.SetInt(i + HSNum, newScore);
                PlayerPrefs.SetString(i + HSPlayer, newName);
                newScore = 0;
                newName = "";
            }
        }
    }

    public static bool isScoreTop(int score)
    {
        return score > PlayerPrefs.GetInt((numOfEntriesInHS - 1) + HSNum);
    }

    public void GoBack () {
		HighScoresCanvas.gameObject.SetActive(false);
		MainMenuCanvas.gameObject.SetActive(true);
	}

	public void ExitGame () {
		Application.Quit ();
	}
}
