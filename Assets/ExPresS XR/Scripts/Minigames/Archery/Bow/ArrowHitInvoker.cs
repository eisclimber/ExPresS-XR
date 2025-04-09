using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.Archery
{
    public class ArrowHitInvoker : MonoBehaviour
    {
        public UnityEvent<Collision> OnTriggered;

        private void OnCollisionEnter(Collision collision)
        {
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            if (collision.gameObject.CompareTag("Target") || collision.gameObject.CompareTag("BadTarget"))
            {
                // Return arrow to the pool (the ones that doesn't hit a target are returned after a while)
                ObjectPoolManager.ReturnToPool(gameObject.transform.parent.parent.gameObject);
                OnTriggered?.Invoke(collision);
            }
        }
    }
}
