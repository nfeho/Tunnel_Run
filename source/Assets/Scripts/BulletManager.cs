using UnityEngine;
using System.Collections;

public class BulletManager : MonoBehaviour
{

    public GameObject bulletParticle;

    void OnCollisionEnter(Collision other)
    {
        var obj = GameObject.Instantiate(bulletParticle, transform.position, transform.rotation) as GameObject;
        Destroy(gameObject);
        var exp = obj.GetComponent<ParticleSystem>();
        exp.Play();
        Destroy(obj, exp.duration);
    }
}
