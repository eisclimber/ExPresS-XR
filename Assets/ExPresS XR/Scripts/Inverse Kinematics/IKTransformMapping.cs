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
        private Transform _vrTarget;

        [Tooltip("The Target of an Constraint that will be moved with the '_vrTarget'.")]
        [SerializeField]
        private Transform _ikTarget;


        [Tooltip("Additional positional offset that is applied to the _ikTarget's position. Use to align the hands 'hotspot' to the model.")]
        [SerializeField]
        private Vector3 _positionOffset;

        [Tooltip("Additional positional offset that is applied to the _ikTarget's position. Use to align the tracked object's rotation.")]
        [SerializeField]
        private Quaternion _rotationOffset;

        private bool _moveRoot;
        private Transform _presenceRoot;
        private Vector3 _initialFollowPosition;


        public void UpdateMapping()
        {
            if (_vrTarget != null)
            {
                if (_moveRoot && _presenceRoot != null)
                {
                    _presenceRoot.position = _vrTarget.position - _initialFollowPosition;
                    _presenceRoot.forward = Vector3.ProjectOnPlane(_vrTarget.forward, Vector3.up).normalized;
                }
                _ikTarget.SetPositionAndRotation(_vrTarget.TransformPoint(_positionOffset),
                        _vrTarget.rotation * _rotationOffset);            
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

            _initialFollowPosition = _ikTarget.position;
            // _rotationOffset = _ikTarget.rotation;
        }
    }
}