using UnityEngine;
using System.Collections;

public class DoorSwitch : MonoBehaviour {

    [SerializeField]
    private GameObject inactiveSwitch;
    [SerializeField]
    private Transform door;

    private Material activeSwitchMaterial;

    private int lives;
    bool inactive;
    private void Awake()
    {
        lives = Random.Range(1, 3);
        gameObject.transform.parent.gameObject.SetActive(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        lives--;
        if(lives == 0)
        {
            gameObject.transform.parent.gameObject.SetActive(false);
            inactiveSwitch.transform.localScale = gameObject.transform.parent.transform.localScale;
            Instantiate(inactiveSwitch, transform.position, transform.rotation);
        }
    }
}
