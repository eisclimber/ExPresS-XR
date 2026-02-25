using System;
using UnityEngine;

namespace ExPresSXR.Rig.InverseKinematics
{
    /// <summary>
    /// A helper class for mapping between a transform for an ik, maintaining a positional and rotational offset.
    /// </summary>
    [Serializable]
    public class IKTransformMapping
    {
        /// <summary>
        /// The Transform that provides the target location. Use the Main Camera for the head and the hand's InteractionController for the hands.
        /// </summary>
        [SerializeField]
        [Tooltip("The Transform that provides the target location. "
            + "Use the Main Camera for the head and the hand's InteractionController for the hands.")]
        private Transform _vrTarget;

        /// <summary>
        /// The target of a constraint that will be moved with the '_vrTarget'.
        /// </summary>
        [SerializeField]
        [Tooltip("The target of a constraint that will be moved with the '_vrTarget'.")]
        private Transform _ikTarget;

        /// <summary>
        /// Additional positional offset that is applied to the _ikTarget's position. Use to align the hands 'hotspot' to the model.
        /// </summary>
        [SerializeField]
        [Tooltip("Additional positional offset that is applied to the _ikTarget's position. Use to align the hands 'hotspot' to the model.")]
        private Vector3 _positionOffset;

        /// <summary>
        /// Additional positional offset that is applied to the _ikTarget's position. Use to align the tracked object's rotation.
        /// </summary>
        [SerializeField]
        [Tooltip("Additional positional offset that is applied to the _ikTarget's position. Use to align the tracked object's rotation.")]
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
                Debug.LogError("Tracker should move the root but none provided! Provide a '_presenceRoot' or disable '_moveRoot'.");
            }

            _initialFollowPosition = _ikTarget.position;
            // _rotationOffset = _ikTarget.rotation;
        }
    }
}