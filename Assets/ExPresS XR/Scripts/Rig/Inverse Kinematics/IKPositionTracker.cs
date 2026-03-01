using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExPresSXR.Rig.InverseKinematics
{
    /// <summary>
    /// Applies additional VR-guided IK Mappings. For only tracking head and hands use IKBodyPresence!
    /// </summary>
    [Tooltip("Applies additional VR-guided IK Mappings. For only tracking head and hands use IKBodyPresence!")]
    public class IKPositionTracker : MonoBehaviour
    {
        /// <summary>
        /// The mapping for tracking an IK target.
        /// </summary>
        [SerializeField]
        [Tooltip("The mapping for tracking an IK target.")]
        private IKTransformMapping _mapping;

        /// <summary>
        /// If enabled moves and rotates the '_presenceRoot' according to the target. Enable for moving the body with the head.
        /// </summary>
        [SerializeField]
        [Tooltip("If enabled moves and rotates the '_presenceRoot' according to the target. "
            + "Enable for moving the body with the head.")]
        private bool _moveRoot;

        /// <summary>
        /// Root of the presence that is rotated if 'moveRoot' is enabled.
        /// </summary>
        [SerializeField]
        [Tooltip("Root of the presence that is rotated if 'moveRoot' is enabled.")]
        private Transform _presenceRoot;

        private void Start() => _mapping.InitializeMapping(_presenceRoot, _moveRoot);

        private void LateUpdate() =>  _mapping.UpdateMapping();
    }
}