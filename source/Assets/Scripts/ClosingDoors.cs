using UnityEngine;
using System.Collections;

public class ClosingDoors : MonoBehaviour {

    private GameObject player;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Use this for initialization
    void OnEnable() {
        closingDoorInitialBehaviour();
	}
	
	// Update is called once per frame
	void Update () {
        closingDoorUpdateBehaviour();
	}

    private void closingDoorInitialBehaviour()
    {
        Vector3 position = gameObject.transform.GetChild(2).transform.position;
        gameObject.transform.GetChild(2).transform.position = new Vector3(position.x, 11.5f, position.z);
        Debug.Log("Closed door position :" + position);
    }

    private void closingDoorUpdateBehaviour()
    {
        float playerDistance = gameObject.transform.position.z - player.transform.position.z;

        Transform moveable = gameObject.transform.GetChild(2);
        if (playerDistance < 20 && moveable.position.z > -1)
        {
            moveable.position += Vector3.down * Time.deltaTime * 0.23f * 10 ; // 10 je rychlost nechce se mi to ted resit kdyz se to nebude menit
            if (!gameObject.GetComponent<AudioSource>().isPlaying)
            {
                gameObject.GetComponent<AudioSource>().Play();
            }
        }
        else
        {
            gameObject.GetComponent<AudioSource>().Stop();
        }
    }
}
