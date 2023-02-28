using System.Collections;
using System.Collections.Generic;
using ExPresSXR.Rig;
using ExPresSXR.Rig.InverseKinematics;
using UnityEngine;

namespace ExPresSXR.Rig.InverseKinematics
{
    public class IKBodyPresence : MonoBehaviour
    {
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
    }
}