using UnityEditor.EditorTools;
using UnityEngine;

namespace ExPresSXR.Minigames.Archery.TargetSpawner
{
    /// <summary>
    /// A class that can be used to reference a target to avoid searching for it in a GameObject's children.
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