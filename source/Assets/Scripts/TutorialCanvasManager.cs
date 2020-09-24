using UnityEngine;
using System.Collections;

public class TutorialCanvasManager : MonoBehaviour {

	[SerializeField]
	private GameObject obstacleCanvas;
	[SerializeField]
	private GameObject mobileObstacleCanvas;

	private GameObject player;

	private bool done = false;

	void Awake()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		obstacleCanvas.gameObject.SetActive (false);
		mobileObstacleCanvas.gameObject.SetActive (false);
	}

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		if (gameObject.transform.position.z - player.transform.position.z > 23) {
			obstacleBehaviour ();
		} else {
			done = false;
		}
	}

	private void obstacleBehaviour () {
		if (gameObject.transform.position.z - player.transform.position.z < 25 && !done ) {
			Time.timeScale = 0;
            FPSRunnerMotor.Instance.canShoot = false;
#if UNITY_STANDALONE
            obstacleCanvas.gameObject.SetActive (true);
			#endif
			#if MOBILE_INPUT
			mobileObstacleCanvas.gameObject.SetActive (true);
			#endif

		}
	}

	public void Hide() {
		done = true;
		Time.timeScale = 1;
        FPSRunnerMotor.Instance.canShoot = true;
		#if UNITY_STANDALONE
		obstacleCanvas.gameObject.SetActive (false);
		#endif
		#if MOBILE_INPUT
		mobileObstacleCanvas.gameObject.SetActive (false);
		#endif

	}
}
