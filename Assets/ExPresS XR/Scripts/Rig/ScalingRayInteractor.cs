using UnityEngine;
using ExPresSXR.Interaction;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ExPresSXR.Rig
{
    /// <summary>
    /// Allows ray interactions with XRInteractables whilst also allowing them to be scaled, if they were configured as a `ExPresSXRInteractable`.
    /// 
    /// Uses the RayInteractors `anchorControl`-Mode to de-/activate scaling or translation.
    /// The differentiation for anchorControl and scaling is handled via the anchorControlMode.
    /// </summary>
    public partial class ScalingRayInteractor : XRRayInteractor
    {
        /// <summary>
        /// Determines how the anchorControl/scaling InputActions (Joystick up/down) are handled.
        /// </summary>
        [SerializeField]
        [Tooltip("Determines how the anchorControl/scaling InputActions (Joystick up/down) are handled.")]
        private AnchorControlMode _anchorControlMode = AnchorControlMode.ScaleWithTranslateFallback;
        public AnchorControlMode AnchorControlMode
        {
            get => _anchorControlMode;
            set => _anchorControlMode = value;
        }

        /// <summary>
        /// How fast the scale is in-/decreased. Will be synchronized with deltaTime.
        /// </summary>
        [SerializeField]
        [Tooltip("How fast the scale is in-/decreased. Will be synchronized with deltaTime.")]
        private float _scaleSpeed = 1.0f;
        public float ScaleSpeed
        {
            get => _scaleSpeed;
            set => _scaleSpeed = value;
        }


        /// <summary>
        /// < inheritdoc />
        /// </summary>
        protected override void TranslateAttachTransform(Transform rayOrigin, Transform anchor, float directionAmount)
        {
            bool canScale = TryGetSelectedScaleInteractableWrapper(out ExPresSXRGrabInteractable scaleInteractable);
            if (canScale 
                && (_anchorControlMode == AnchorControlMode.Scale || _anchorControlMode == AnchorControlMode.ScaleWithTranslateFallback) 
                && scaleInteractable.ScaleRange > 0.0f)
            {
                float speed = scaleInteractable.HasScaleSpeedOverride ? scaleInteractable.ScaleSpeedOverride : _scaleSpeed;
                scaleInteractable.ScaleFactor += directionAmount * speed * Time.deltaTime;
            }
            else if (_anchorControlMode == AnchorControlMode.Translate || _anchorControlMode == AnchorControlMode.ScaleWithTranslateFallback)
            {
                base.TranslateAttachTransform(rayOrigin, anchor, directionAmount);
            }
        }

        private bool TryGetSelectedScaleInteractableWrapper(out ExPresSXRGrabInteractable _scaleInteractable)
        {
            _scaleInteractable = hasSelection ? firstInteractableSelected as ExPresSXRGrabInteractable : null;
            return _scaleInteractable != null;
        }
    }

    /// <summary>
    /// Determines how the anchorControl/scaling InputActions (Joystick up/down) are handled.
    /// </summary>
    public enum AnchorControlMode
    {
        Translate, /// <summary> Translate (move) the interactable. </summary>
        Scale, /// <summary> Scale the interactable. </summary>
        ScaleWithTranslateFallback /// <summary> Scale the interactable, falling back to translation not supported. </summary>
    }
}