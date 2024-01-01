using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinReset : MonoBehaviour
{

    [SerializeField, Tooltip("Position to reset the coin after hitting / missing")]
    private Transform resetPosition;


    private void Start()
    {
        transform.position = resetPosition.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && !other.CompareTag("Player"))
        {
            ResetOwnPosition();
        }
    }
    

    public void ResetOwnPosition()
    {
        if (TryGetComponent(out Rigidbody rb))
        {
            rb.velocity = Vector3.zero;
        }
        transform.position = resetPosition.position;
    }
}
