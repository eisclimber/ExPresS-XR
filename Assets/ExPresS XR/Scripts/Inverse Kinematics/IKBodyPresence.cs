using System.Collections;
using System.Collections.Generic;
using ExPresSXR.Rig;
using ExPresSXR.Rig.InverseKinematics;
using UnityEngine;

namespace ExPresSXR.Rig.InverseKinematics
{
    public class IKBodyPresence : MonoBehaviour
    {
        [Tooltip("The ExPresS XR Rig used to retrieve the IK target positions.")]
        [SerializeField]
        private ExPresSXRRig _rig;
        public ExPresSXRRig rig
        {
            get => _rig;
            set 
            {
                _rig = value;

                if (_rig != null)
                {
                    UpdateIKConnections();
                }
            }
        }

        [Tooltip("The IK target for the head.")]
        [SerializeField]
        private IKTransformMapping _head;

        [Tooltip("The IK target for the left hand.")]
        [SerializeField]
        private IKTransformMapping _leftHand;

        [Tooltip("The IK target for the left hand.")]
        [SerializeField]
        private IKTransformMapping _rightHand;

        private void Start() {
            _head.InitializeMapping(transform, true);
            _leftHand.InitializeMapping(transform, false);     
            _rightHand.InitializeMapping(transform, false);     
        }


        private void LateUpdate() {
            _head.UpdateMapping();
            _leftHand.UpdateMapping();
            _rightHand.UpdateMapping();
        }


        private void UpdateIKConnections()
        {
            if (!_rig.mainRigCamera.TryGetComponent(out _head.vrTarget))
            {
                Debug.LogError("The rig does not have a IKPositionTracker-Component attached to the MainCamera-Component. " +
                    "Inverse Kinematics for the head will not work.");
            }

            if (!_rig.leftHandController.TryGetComponent(out _leftHand.vrTarget))
            {
                Debug.LogError("The rig does not have a IKPositionTracker-Component attached to the left hand. " +
                    "Inverse Kinematics for the left hand will not work.");
            }

            if (!_rig.rightHandController.TryGetComponent(out _rightHand.vrTarget))
            {
                Debug.LogError("The rig does not have a IKPositionTracker-Component attached to the right hand. " +
                    "Inverse Kinematics for the right hand will not work.");
            }
        }
    }
}