using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using ExPresSXR.Interaction;
using Unity.XR.CoreUtils;

namespace ExPresSXR.Rig
{
    public class ScalingDirectInteractor : XRDirectInteractor
    {
        [SerializeField]
        private bool _scalingEnabled = true;
        public bool scalingEnabled
        {
            get => _scalingEnabled;
            set => _scalingEnabled = value;
        }

        [SerializeField]
        private float _scaleSpeed = 1.0f;
        public float scaleSpeed
        {
            get => _scaleSpeed;
            set => _scaleSpeed = value;
        }

        public bool hasScalingSelection
        {
            get => _scalingEnabled && TryGetSelectedScaleInteractableWrapper(out ScalableGrabInteractable _);
        }


        /// <inheritdoc />
        public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractor(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                // Handle Device-based controllers because why not
                var ctrl = xrController as XRController;
                if (ctrl != null && ctrl.inputDevice.isValid)
                {
                    ctrl.inputDevice.IsPressed(ctrl.moveObjectIn, out var inPressed, ctrl.axisToPressThreshold);
                    ctrl.inputDevice.IsPressed(ctrl.moveObjectOut, out var outPressed, ctrl.axisToPressThreshold);

                    if (inPressed || outPressed)
                    {
                        var directionAmount = inPressed ? 1.0f : -1.0f;
                        ScaleSelection(directionAmount);
                    }
                }

                // Handle action-based controllers
                var actionBasedController = xrController as ActionBasedController;
                if (actionBasedController != null
                    && TryRead2DAxis(actionBasedController.translateAnchorAction.action, out Vector2 scaleAmt))
                {
                    ScaleSelection(scaleAmt.y);
                }
            }
        }

        /// <summary>
        /// Copied from the XRRayInteractor because it is not accessible in this scope. Reads the value from the given InputAction as Vector2.  
        /// </summary>
        /// <param name="action">InputAction to be read.</param>
        /// <param name="output">Vector2 read from the InputAction.</param>
        /// <returns>If the value could be read successfully.</returns>
        protected bool TryRead2DAxis(InputAction action, out Vector2 output)
        {
            if (action != null)
            {
                output = action.ReadValue<Vector2>();
                return true;
            }
            output = default;
            return false;
        }

        protected virtual void ScaleSelection(float directionAmount)
        {
            if (_scalingEnabled && TryGetSelectedScaleInteractableWrapper(out ScalableGrabInteractable _scaleInteractable))
            {
                _scaleInteractable.scaleFactor += directionAmount * _scaleSpeed * Time.deltaTime;
            }
        }

        public bool TryGetSelectedScaleInteractableWrapper(out ScalableGrabInteractable _scaleInteractable)
        {
            _scaleInteractable = hasSelection ? firstInteractableSelected as ScalableGrabInteractable : null;
            return _scaleInteractable != null;
        }
    }
}
