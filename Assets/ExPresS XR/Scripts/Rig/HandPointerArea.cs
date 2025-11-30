using ExPresSXR.Rig;
using UnityEngine;

public class HandPointerArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (!other.gameObject.CompareTag("Player") || rb == null)
        {
            return; // Not the player's hands -> not what we're looking for
        }
        VirtualHandAnimator handAnimator = rb.GetComponentInChildren<VirtualHandAnimator>();
        if (handAnimator != null)
        {
            handAnimator.PointAreaCollisions++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (!other.gameObject.CompareTag("Player") || rb == null)
        {
            return; // Not the player's hands -> not what we're looking for
        }
        VirtualHandAnimator handAnimator = rb.GetComponentInChildren<VirtualHandAnimator>();
        if (handAnimator != null)
        {
            handAnimator.PointAreaCollisions--;
        }
    }
}
