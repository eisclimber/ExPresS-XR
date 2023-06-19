using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using ExPresSXR.Rig;

namespace ExPresSXR.Interaction
{
    /// <summary>
    /// Rumbles the given controllers when the RidigBody of their model collides with this object. 
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class CollisionHandRumbler : MonoBehaviour
    {
        [SerializeField]
        private RumbleDescription _rumble;
        public RumbleDescription rumble
        {
            get => _rumble;
            set => _rumble = value;
        }

        [Space]

        [SerializeField]
        private XRBaseController _leftXrController;

        public XRBaseController leftXrController
        {
            get => _leftXrController;
            set => _leftXrController = value;
        }

        [SerializeField]
        private XRBaseController _rightXrController;

        public XRBaseController rightXrController
        {
            get => _rightXrController;
            set => _rightXrController = value;
        }
        

        private void Awake() {
            if (_leftXrController == null && _rightXrController == null)
            {
                Debug.LogWarning("No XR Controller was provided. This Component will have no effect.");
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (IsControllerTransformMatch(other, _leftXrController))
            {
                HapticImpulseTrigger.PerformHapticEvent(_rumble, _leftXrController);
            }

            if (IsControllerTransformMatch(other, _rightXrController))
            {
                HapticImpulseTrigger.PerformHapticEvent(_rumble, _rightXrController);
            }
        }

        private bool IsControllerTransformMatch(Collision col, XRBaseController ctrl)
        {
            Transform modelTransform = GetModelTransform(ctrl);
            return col.rigidbody != null && modelTransform != null 
                && col.rigidbody.transform == modelTransform;
        }
        
        private Transform GetModelTransform(XRBaseController ctrl)
            => ctrl != null && ctrl.model != null ? ctrl.model.transform : null;
    }
}