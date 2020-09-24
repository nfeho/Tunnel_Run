using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstacleBehavour : MonoBehaviour {

    private const string SideObstacle = "SideObstacle";
    private const string Laser = "Laser";
    private const string DoorSwitch = "DoorSwitch";
    private const string Drone = "Drone";

    private GameObject player;

    [SerializeField]
    private List<GameObject> handledPrefabs = new List<GameObject>();

    private List<GameObject> handledObjects = new List<GameObject>();
        
    [SerializeField]
    private string identName;
	
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }


    void Update()
    {
        
    }

    void OnEnable()
    {
        initialBehavoiur();
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
            return;

        switch (identName)
        {
            case Laser:
            case DoorSwitch:
                switchOnTriggerBehaviour();
                break;
        }
    }
    
    private void initialBehavoiur()
    {
        switch (identName)
        {
            case SideObstacle :
                sideObstacleInitialBehaviour();
                break;
            case Laser:
            case DoorSwitch:
                switchInitialBehaviour();
                break;
            case Drone:
                
                break;
        }
    }

    private void sideObstacleInitialBehaviour()
    {
        int[] optionX = {0, 4};
        int posX = optionX[Random.Range(0, optionX.Length)];
        gameObject.transform.position = new Vector3(posX, gameObject.transform.position.y, gameObject.transform.position.z);
        Debug.Log("Position set to " + gameObject.transform.position);
    }


    // -------------------------------------- switch section --------------------------------------------------------------

    private void switchInitialBehaviour()
    {
        //adjust position
        initialPosition();

        gameObject.transform.localScale = new Vector3(1, 1, 1); // we used scaling to make object invisible
        // index 0 should be active beam, 1 inactive beam, 2 inactive switch
        if (handledObjects.Count == 0)
        {
            handledObjects.Add(Instantiate(handledPrefabs[0]));
            handledObjects.Add(Instantiate(handledPrefabs[1]));
            handledObjects.Add(Instantiate(handledPrefabs[2]));
        }

        handledObjects[0].SetActive(true);
        handledObjects[0].transform.transform.position = new Vector3(0, 0, gameObject.transform.position.z + 5); // move behind the switch

        handledObjects[1].SetActive(false);
        handledObjects[1].transform.position = handledObjects[0].transform.position;
        
        handledObjects[2].transform.position = gameObject.transform.position;
        handledObjects[2].SetActive(false);
    }

    private void switchOnTriggerBehaviour()
    {
        gameObject.transform.localScale = new Vector3(0, 0, 0); // workaround for invisibility
        handledObjects[0].SetActive(false);
        handledObjects[1].SetActive(true);
        handledObjects[2].SetActive(true);
    }

    private void initialPosition()
    {
        switch (identName)
        {
            case Laser:
                gameObject.transform.Translate(4.9f, 2, 0);
                break;
        }
    }
    // ---------------------------------- switch section -------------------------------------------------------


}
