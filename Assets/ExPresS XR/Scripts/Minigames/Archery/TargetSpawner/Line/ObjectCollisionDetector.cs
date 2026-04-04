using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.Archery.TargetSpawner.Line
{
    /// <summary>
    /// A proxy component that allows passing a collision to another object but only with a certain object.
    /// </summary>
    public class ObjectCollisionDetector : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The object this component detects collisions with.")]
        private GameObject _objectToDetect;
        /// <summary>
        /// The object this component detects collisions with.
        /// </summary>
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