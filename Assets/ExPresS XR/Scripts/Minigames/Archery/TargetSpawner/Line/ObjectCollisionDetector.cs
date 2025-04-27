using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.Archery.TargetSpawner.Line
{
    public class ObjectCollisionDetector : MonoBehaviour
    {
        /// <summary>
        /// The object this component detects collisions with.
        /// </summary>
        [SerializeField]
        [Tooltip("The object this component detects collisions with.")]
        private GameObject _objectToDetect;
        public GameObject ObjectToDetect
        {
            get => _objectToDetect;
            set => _objectToDetect = value;
        }


        /// <summary>
        /// Emitted if a collision with the configured GameObject occurs.
        /// </summary>
        public UnityEvent<Collider> OnTriggered;

        private void OnTriggerEnter(Collider other)
        {
            if (isActiveAndEnabled && other.gameObject == _objectToDetect)
            {
                OnTriggered?.Invoke(other);
            }
        }
    }
}