using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ArrowFunction : MonoBehaviour
{
    [SerializeField, Tooltip("Arrow shot")]
    public GameObject arrowShotPrefab;
    [SerializeField, Tooltip("Arrow sticking")]
    public GameObject arrowStickingPrefab;
    [SerializeField, Tooltip("Speed of Arrow (normally at 20)")]
    private float speed = 20;

    [SerializeField, Tooltip("Release String Sound")]
    private AudioClip releaseStringSound;
    [SerializeField, Tooltip("Hit sound")]
    private AudioClip hitSound;

    private AudioSource audiosource;

    private GameObject arrowobject;
    private GameObject arrowSticking;


    private void Start()
    {
        audiosource = GetComponent<AudioSource>();
    }


    public void OnReleaseArrow(float pull)
    {

        audiosource.PlayOneShot(releaseStringSound, 0.3f);
        
        arrowobject = ObjectPoolManager.Spawn(arrowShotPrefab, transform.position, transform.rotation);
       
        arrowobject.GetComponent<Rigidbody>().AddForce(-transform.up * speed * pull , ForceMode.Impulse);
    }
 
  
    public void OnHit(Collision collision)
    {

            audiosource.Stop();
            audiosource.PlayOneShot(hitSound, 1f);
            arrowSticking = ObjectPoolManager.Spawn(arrowStickingPrefab, collision.contacts[0].point,Quaternion.Euler(arrowobject.transform.eulerAngles));
           
            arrowSticking.transform.SetParent(collision.contacts[0].otherCollider.transform.GetComponent<Rigidbody>().transform);
            
    }
    
}
