using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.Archery
{
    public class ArrowHitInvoker : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Reference to the object pool manager")]
        private ObjectPoolManager _objectPoolManager;

        public UnityEvent<Collision> OnTriggered;

        private void OnCollisionEnter(Collision collision)
        {
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            if (collision.gameObject.CompareTag("Target") || collision.gameObject.CompareTag("BadTarget"))
            {
                // Return arrow to the pool (the ones that doesn't hit a target are returned after a while)
                _objectPoolManager.ReturnToPool(gameObject.transform.parent.parent.gameObject);
                OnTriggered?.Invoke(collision);
            }
        }
    }
}
