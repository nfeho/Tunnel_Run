using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Time.timeScale = 1;
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void Retry () {
		Time.timeScale = 1;
        AudioListener.pause = false;
		SceneManager.LoadScene (2);
	}

	public void GoToMainMenu () {
		Time.timeScale = 1;
        AudioListener.pause = false;
        SceneManager.LoadScene (0);
	}

	public void HideCanvas() {
		Time.timeScale = 1;
	}
}
