using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoorPanelSwitchBehaviour : MonoBehaviour {

    // index 0 should be doors, 1 inactive switch (only display)
    [SerializeField]
    private List<GameObject> handledPrefabs = new List<GameObject>();

    private List<GameObject> handledObjects = new List<GameObject>();

    bool isActivated = false;

    // Use this for initialization
    void OnEnable () {
        switchInitialBehaviour();
	}
	
	// Update is called once per frame
	void Update () {
	    if (isActivated)
        {
            Transform moveable = handledObjects[0].transform.GetChild(2);
            if (moveable.position.y < 9.6f)
            {
                moveable.position += Vector3.up * Time.deltaTime * 10; // 10 je rychlost nechce se mi to ted resit kdyz se to nebude menit
            }
            else
            {
                handledObjects[0].GetComponent<AudioSource>().Stop();
            }
        }
	}

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
            return;

        isActivated = true;
        if (!handledObjects[0].GetComponent<AudioSource>().isPlaying)
        {
            handledObjects[0].GetComponent<AudioSource>().Play();
        }
        gameObject.transform.localScale = new Vector3(0, 0, 0);
        handledObjects[1].SetActive(true);
    }

    private void switchInitialBehaviour()
    {

        //adjust position
        gameObject.transform.parent.Translate(5, 4, -1);

        gameObject.transform.localScale = new Vector3(1, 1, 1); // we used scaling to make object invisible

        // index 0 should be doors, 1 inactive switch (only display)
        if (handledObjects.Count == 0)
        {
            handledObjects.Add(Instantiate(handledPrefabs[0]));
            handledObjects.Add(Instantiate(handledPrefabs[1]));
        }
                
        handledObjects[0].transform.transform.position = new Vector3(0, 2.7f, gameObject.transform.position.z + 5); // move behind the switch
        Vector3 position = handledObjects[0].transform.GetChild(2).transform.position;
        handledObjects[0].transform.GetChild(2).transform.position = new Vector3(position.x, 3.2f, position.z);
        isActivated = false;

        handledObjects[1].SetActive(false);
        handledObjects[1].transform.position = gameObject.transform.position;
    }
}
