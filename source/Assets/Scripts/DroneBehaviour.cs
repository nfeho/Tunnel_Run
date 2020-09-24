using UnityEngine;
using System.Collections;

public class DroneBehaviour : MonoBehaviour {

    bool isActive = true;

	void OnEnable () {
        droneInitialBehaviour();
    }
	
	// Update is called once per frame
	void Update () {
        droneUpdateBehaviour();
    }

    void OnCollisionEnter(Collision other)
    {
        droneCollisionBehaviour();
    }

    const float minX = -4;
    const float maxX = 4;
    const float minY = 1.1f;
    const float maxY = 6.2f;
    const float speed = 4;

    // randomly found waypoint for drone
    Vector3 waypoint;

    private void droneInitialBehaviour()
    {
        isActive = true;
        gameObject.GetComponent<Collider>().enabled = true;
        gameObject.transform.position = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), gameObject.transform.position.z);
        gameObject.GetComponent<AudioSource>().Play();
        gameObject.GetComponent<Animator>().Play("Entry");
        setRandomWaypoint();
    }

    private void droneUpdateBehaviour()
    {

        if (isActive)
        {
            // if we reached waypoint we find new one
            if (gameObject.transform.position == waypoint)
            {
                setRandomWaypoint();
            }
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, waypoint, speed * Time.deltaTime);
        }
        else
        {
            if (gameObject.transform.position.y > minY)
            {
                gameObject.transform.position += Vector3.down * Time.deltaTime * 4;
            }
        }
    }

    private void setRandomWaypoint()
    {
        float x = gameObject.transform.position.x < 0 ? Random.Range(0, maxX) : Random.Range(minX, 0);
        float y = gameObject.transform.position.y < 3.5 ? Random.Range(3.5f, maxY) : Random.Range(minY, 3.5f);
        waypoint = new Vector3(x, y, gameObject.transform.position.z);
    }

    private void droneCollisionBehaviour()
    {
        if (isActive)
        {
            GameManager.scoreFromObjectives += 8;
        }
        isActive = false;
        gameObject.GetComponent<AudioSource>().Stop();
        gameObject.GetComponent<Animator>().Stop();
        
    }
}
