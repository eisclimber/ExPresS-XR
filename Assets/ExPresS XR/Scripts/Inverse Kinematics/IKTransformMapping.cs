using System.Collections;
using System.Collections.Generic;
using ExPresSXR.Rig;
using UnityEngine;

namespace ExPresSXR.Rig.InverseKinematics
{
    [System.Serializable]
    public class IKTransformMapping
    {
        [Tooltip("The Transform that provides the target location. "
            + "Use the Main Camera for the head and the hand's InteractionController for the hands.")]
        [SerializeField]
        public Transform vrTarget;

        [Tooltip("The Target of an Constraint that will be moved with the 'vrTarget'.")]
        [SerializeField]
        private Transform ikTarget;


        [Tooltip("Additional positional offset that is applied to the _ikTarget's position. Use to align the hands 'hotspot' to the model.")]
        [SerializeField]
        private Vector3 _positionOffset;

        [Tooltip("Additional positional offset that is applied to the _ikTarget's position. Use to align the tracked object's rotation.")]
        [SerializeField]
        private Quaternion _rotationOffset;


        // [Tooltip("If enabled moves and rotates the '_presenceRoot' according to the target. "
        //     + "Enable for moving the body with the head.")]
        // [SerializeField]
        private bool _moveRoot;

        // [Tooltip("Root of the presence that is rotated if 'moveRoot' is enabled.")]
        // [SerializeField]
        private Transform _presenceRoot;

        // [Tooltip("!Will be retrieved from the target at start! Adjust only during playtime to find the correct position. .")]
        // [SerializeField]
        private Vector3 _initialFollowPosition;


        public void UpdateMapping()
        {
            if (vrTarget != null)
            {
                if (_moveRoot && _presenceRoot != null)
                {
                    _presenceRoot.position = vrTarget.position - _initialFollowPosition;
                    _presenceRoot.forward = Vector3.ProjectOnPlane(vrTarget.forward, Vector3.up).normalized;
                }
                ikTarget.SetPositionAndRotation(vrTarget.TransformPoint(_positionOffset),
                        vrTarget.rotation * _rotationOffset);            
            }
        }

        public void InitializeMapping(Transform presenceRoot, bool moveRoot)
        {
            _presenceRoot = presenceRoot;
            _moveRoot = moveRoot;

            if (_moveRoot && _presenceRoot == null)
            {
                Debug.LogError("Tracker should move the root but none provided! "
                    + "Provide a '_presenceRoot' or disable '_moveRoot'.");
            }

            _initialFollowPosition = ikTarget.position;
            // _rotationOffset = ikTarget.rotation;
        }
    }
}