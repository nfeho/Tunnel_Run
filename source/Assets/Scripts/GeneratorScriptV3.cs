using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneratorScriptV3 : MonoBehaviour
{

    [SerializeField]
    private GameObject roomPrefab;
    [SerializeField]
    private GameObject[] obstaclesPrefabs;
    [SerializeField]
    private float roomLength = 40f;
    [SerializeField]
    private int amountOfActiveRooms = 6;
    [SerializeField]
    private float obstacleBetweenDistance = 30f;
    [SerializeField]
    private float obstacleSpawningDistance = 100f;

    private List<GameObject> activeObstacles;
    private List<GameObject> activeRooms;
    private List<GameObject> inactiveObstacles;
    
    private int noObstacleProbability = 4; //one of n probability;

    private Transform playerTransform;
    private float roomSpawnZ;
    private float obstacleSpawnZ;
    private int lastObstacleIndex = -1;
    // Use this for initialization
    void Start()
    {
        activeObstacles = new List<GameObject>();
        activeRooms = new List<GameObject>();
        inactiveObstacles = new List<GameObject>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        roomSpawnZ = 0;
        obstacleSpawnZ = obstacleSpawningDistance; //we need to start putting obstacles after we spawned first few rooms


        for (int i = 0; i < amountOfActiveRooms; i++)
        {
            SpawnRoom();
            SpawnObstacle();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ((playerTransform.position.z - roomLength - 5) > (roomSpawnZ - amountOfActiveRooms * roomLength))
        {
            SpawnRoom();
            DeleteRoom();

        }

        if ((playerTransform.position.z) > obstacleSpawnZ - obstacleSpawningDistance)
        {
            SpawnObstacle();
        }
        //DeleteObstacle();
        MakeObstacleInactive();

    }

    private void SpawnRoom()
    {
        GameObject room;
        room = Instantiate(roomPrefab) as GameObject;
        room.transform.SetParent(transform);
        room.transform.position = Vector3.forward * roomSpawnZ;
        activeRooms.Add(room);
        roomSpawnZ += roomLength;


    }

    private void DeleteRoom()
    {
        Destroy(activeRooms[0]);
        activeRooms.RemoveAt(0);
    }

    private void SpawnObstacle()
    {


        int obstaclePrefabIndex = GetRandomObstaclePrefabIndex();
        if (obstaclePrefabIndex != -1)
        {
            GameObject obstacle = null;
            GameObject obstaclePrefab = obstaclesPrefabs[obstaclePrefabIndex];
            // firstly lets search already created objects
            for (int i = 0; i < inactiveObstacles.Count; i++)
            {
                GameObject obj = inactiveObstacles[i];
                if (obj.name.Equals(obstaclePrefab.name))
                {
                    obstacle = obj;
                    obstacle.transform.position = Vector3.forward * (obstacleSpawnZ + obstacleBetweenDistance);
                    obstacle.SetActive(true);
                    foreach(Transform child in obstacle.GetComponentInChildren<Transform>())
                    {
                        child.gameObject.SetActive(true);
                        
                    }
                    inactiveObstacles.RemoveAt(i);
                    break;
                }
            }
            // if obstacle was not found in inactive we will clone new one
            if (obstacle == null)
            {
                obstacle = Instantiate(obstaclePrefab, Vector3.forward * (obstacleSpawnZ + obstacleBetweenDistance), obstaclePrefab.transform.rotation) as GameObject;
                // we want to set default name to object (remove "clone") for easy comparing
                obstacle.name = obstaclePrefab.name;
                obstacle.transform.SetParent(transform);
            }
            activeObstacles.Add(obstacle);
            obstacleSpawnZ += obstacleBetweenDistance;
        }
    }

    /**
     * This should replace deleting objects
     */
    private void MakeObstacleInactive()
    {
        if (activeObstacles.Count > 0)
        {
            if (activeObstacles[0].transform.position.z < playerTransform.position.z - obstacleBetweenDistance)
            {
                activeObstacles[0].SetActive(false);
                inactiveObstacles.Add(activeObstacles[0]);
                activeObstacles.RemoveAt(0);
            }
        }
    }
    
    /**
     * This method should be raplced by making obstacles inactive
     */
    private void DeleteObstacle()
    {
        if (activeObstacles.Count > 0)
        {
            if (activeObstacles[0].transform.position.z < playerTransform.position.z - obstacleBetweenDistance)
            {
                Destroy(activeObstacles[0]);
                activeObstacles.RemoveAt(0);
            }
        }
    }

    private int GetRandomObstaclePrefabIndex()
    {
        if (obstaclesPrefabs.Length < 1) return -1;

        if (Random.Range(1, noObstacleProbability) == 1) return -1; //there will be no obstacle

        int randomIndex = lastObstacleIndex;
        while (randomIndex == lastObstacleIndex)
        {
            randomIndex = Random.Range(0, obstaclesPrefabs.Length);
        }

        lastObstacleIndex = randomIndex;
        return lastObstacleIndex;

    }

}
