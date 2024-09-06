using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Invoke : MonoBehaviour
{
    [SerializeField]
    public UnityEvent<Collision> Triggered;
    private void OnCollisionEnter(Collision collision)
    {
        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = Quaternion.identity;


        if (collision.gameObject.tag == "Target" || collision.gameObject.tag == "BadTarget")
        {
            //return arrow to Pool (all the ones which doesnt hit are returned after a specific time through a timer)
            ObjectPoolManager.ReturntoPool(gameObject.transform.parent.transform.parent.gameObject);
            Triggered?.Invoke(collision);
            
        }
    }
    
}
