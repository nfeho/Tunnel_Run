using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class Break : MonoBehaviour {

    [SerializeField]
    private AudioClip glassBreak;
    [SerializeField]
	private GameObject brokenObject;
    [SerializeField]
    private float magnitude = 1f;
    [SerializeField]
    private float radius = 0.5f;
    [SerializeField]
    private float power = 0.5f;
    [SerializeField]
    private float upwards =1f;


    private int lives;
    private GameObject initObject;

    private void OnEnable()
    {
        // EventManager.StartListening(GeneratorScriptV3.e_setActive, new UnityAction<object>(setActive));
        lives = Random.Range(1, 3);
    }

    void setActive(object value)
    {
        lives = Random.Range(1, 3);
        gameObject.SetActive(true);
    }
    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "Bullet")
        {
            lives--;
            if (lives == 0)
            {
                Debug.Log("Collision");
                gameObject.SetActive(false);
                brokenObject.transform.localScale = transform.localScale;
                var obj = Instantiate(brokenObject, transform.position, transform.rotation);
                AudioSource.PlayClipAtPoint(glassBreak, transform.position, 0.35f);
                Vector3 exposionPos = other.contacts[0].point;
                Collider[] colliders = Physics.OverlapSphere(exposionPos, radius);
                //Debug.Log("Colliders count: " + colliders.Length);
                foreach (Collider hit in colliders)
                {
                    if (hit.tag == "Breakable" && hit.GetComponent<Rigidbody>())
                    {
                        float distance = Vector3.Distance(exposionPos, hit.transform.position);
                        hit.GetComponent<Rigidbody>().AddExplosionForce(power * other.relativeVelocity.magnitude, exposionPos, radius, upwards);

                        Vector3 forceVec = other.relativeVelocity * power * 1 / (distance + 1);
                        //Debug.Log("Collider " + hit.name + " with distance: " + distance + " has forceVec: " + forceVec);
                        hit.GetComponent<Rigidbody>().AddForce(forceVec, ForceMode.Impulse);
                    }
                }
                Destroy(obj, 10);
                //Debug.Log (other.rigidbody.velocity);
            }
       }

        
    }
}
