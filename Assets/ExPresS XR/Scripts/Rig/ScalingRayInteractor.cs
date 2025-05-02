using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using ExPresSXR.Interaction;

namespace ExPresSXR.Rig
{
    public partial class ScalingRayInteractor : XRRayInteractor
    {
        [SerializeField]
        private AnchorControlMode _anchorControlMode = AnchorControlMode.ScaleWithTranslateFallback;
        public AnchorControlMode anchorControlMode
        {
            get => _anchorControlMode;
            set => _anchorControlMode = value;
        }

        [SerializeField]
        private float _scaleSpeed = 1.0f;
        public float scaleSpeed
        {
            get => _scaleSpeed;
            set => _scaleSpeed = value;
        }


        protected override void TranslateAnchor(Transform rayOrigin, Transform anchor, float directionAmount)
        {
            bool canScale = TryGetSelectedScaleInteractableWrapper(out ExPresSXRGrabInteractable scaleInteractable);
            if (canScale 
                && (anchorControlMode == AnchorControlMode.Scale || anchorControlMode == AnchorControlMode.ScaleWithTranslateFallback) 
                && scaleInteractable.ScaleRange > 0.0f)
            {
                float speed = scaleInteractable.HasScaleSpeedOverride ? scaleInteractable.ScaleSpeedOverride : _scaleSpeed;
                scaleInteractable.ScaleFactor += directionAmount * speed * Time.deltaTime;
            }
            else if (anchorControlMode == AnchorControlMode.Translate || anchorControlMode == AnchorControlMode.ScaleWithTranslateFallback)
            {
                base.TranslateAnchor(rayOrigin, anchor, directionAmount);
            }
        }


        public bool TryGetSelectedScaleInteractableWrapper(out ExPresSXRGrabInteractable _scaleInteractable)
        {
            _scaleInteractable = hasSelection ? firstInteractableSelected as ExPresSXRGrabInteractable : null;
            return _scaleInteractable != null;
        }
    }

    public enum AnchorControlMode
    {
        Translate,
        Scale,
        ScaleWithTranslateFallback
    }
}