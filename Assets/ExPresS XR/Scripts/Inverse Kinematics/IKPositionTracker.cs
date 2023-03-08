using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExPresSXR.Rig.InverseKinematics
{
    [Tooltip("Applies additional VR-guided IK Mappings. For only tracking head and hands use IKBodyPresence!")]
    public class IKPositionTracker : MonoBehaviour
    {

        [Tooltip("The mapping for tracking an IK target.")]
        [SerializeField]
        private IKTransformMapping _mapping;

        [Tooltip("If enabled moves and rotates the '_presenceRoot' according to the target. "
            + "Enable for moving the body with the head.")]
        [SerializeField]
        private bool _moveRoot;

        [Tooltip("Root of the presence that is rotated if 'moveRoot' is enabled.")]
        [SerializeField]
        private Transform _presenceRoot;

        private void Start() => _mapping.InitializeMapping(_presenceRoot, _moveRoot);

        private void LateUpdate() =>  _mapping.UpdateMapping();
    }
}