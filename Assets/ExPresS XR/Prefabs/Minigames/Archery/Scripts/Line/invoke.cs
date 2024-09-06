using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class invoke : MonoBehaviour
{
    [SerializeField]
    private UnityEvent<Collider> triggered;
    [SerializeField]
    public bool isactive;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Respawn" && isactive)
        {
            
            triggered?.Invoke(other);

        }
    }
}
