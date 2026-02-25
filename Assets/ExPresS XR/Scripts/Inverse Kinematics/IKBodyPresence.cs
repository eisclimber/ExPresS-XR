using UnityEngine;

namespace ExPresSXR.Rig.InverseKinematics
{
    public class IKBodyPresence : MonoBehaviour
    {
        /// <summary>
        /// The IK target for the head. Determines orientation of the IK model.
        /// </summary>
        [SerializeField]
        [Tooltip("The IK target for the head. Determines orientation of the IK model.")]
        private IKTransformMapping _head;

        /// <summary>
        /// The IK target for the left hand.
        /// </summary>
        [SerializeField]
        [Tooltip("The IK target for the left hand.")]
        private IKTransformMapping _leftHand;

        /// <summary>
        /// The IK target for the right hand.
        /// </summary>
        [SerializeField]
        [Tooltip("The IK target for the right hand.")]
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