using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Collider))]
public class PlayerDetector : MonoBehaviour
{

    public UnityEvent OnPlayerEntered;
    public UnityEvent OnPlayerExited;


    private void Start()
    {
       if (!TryGetComponent(out Collider col) || !col.isTrigger)
       {
            Debug.LogError("Collider was either missing or not configured as trigger.");
       } 
    }


    private void OnTriggerEnter(Collider other)
    {
        if (isActiveAndEnabled && IsCollisionPlayerCharacterController(other))
        {
            OnPlayerEntered.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isActiveAndEnabled && IsCollisionPlayerCharacterController(other))
        {
            OnPlayerExited.Invoke();
        }
    }

    private bool IsCollisionPlayerCharacterController(Collider col) 
                    => col.gameObject.CompareTag("Player") && col.TryGetComponent(out CharacterController _);
}
