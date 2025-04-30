using UnityEngine;

namespace ExPresSXR.Minigames.Archery.TargetSpawner
{
    /// <summary>
    /// A component that can and should be added to the root of a prefab if the target component is not at the root.
    /// It allows referencing the target from the root, avoiding having to search the prefab for it.
    /// </summary>
    public class TargetProxy : MonoBehaviour
    {
        /// <summary>
        /// Target to be referenced.
        /// </summary>
        [SerializeField]
        [Tooltip("Target to be referenced.")]
        private Target _target;
        public Target Target
        {
            get => _target;
        }

        private void Awake()
        {
            if (_target == null)
            {
                Debug.Log("No reference to a target set. Please set it or remove the component to avoid errors.", this);
            }
        }
    }
}