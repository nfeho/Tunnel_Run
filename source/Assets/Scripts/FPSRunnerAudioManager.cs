using UnityEngine;
using System.Collections;

public class FPSRunnerAudioManager : MonoBehaviour {

    [SerializeField]
    private AudioClip playerSliding;
    [SerializeField]
    private AudioClip playerMoveLeft;
    [SerializeField]
    private AudioClip playerMoveRight;
    [SerializeField]
    private AudioClip playerShoot;


    public void PlaySliding()
    {
        AudioSource.PlayClipAtPoint(playerSliding, transform.position);
    }

    public void PlayMoveLeft()
    {
        AudioSource.PlayClipAtPoint(playerMoveLeft, transform.position, 0.5f);
    }

    public void PlayMoveRight()
    {
        AudioSource.PlayClipAtPoint(playerMoveRight, transform.position,0.5f);
    }

    public void PlayShoot()
    {
        AudioSource.PlayClipAtPoint(playerShoot, transform.position, 0.5f);
    }
}
