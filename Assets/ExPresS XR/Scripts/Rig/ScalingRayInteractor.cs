using UnityEngine;
using ExPresSXR.Interaction;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ExPresSXR.Rig
{
    public partial class ScalingRayInteractor : XRRayInteractor
    {
        [SerializeField]
        private AnchorControlMode _anchorControlMode = AnchorControlMode.ScaleWithTranslateFallback;
        public AnchorControlMode AnchorControlMode
        {
            get => _anchorControlMode;
            set => _anchorControlMode = value;
        }

        [SerializeField]
        private float _scaleSpeed = 1.0f;
        public float ScaleSpeed
        {
            get => _scaleSpeed;
            set => _scaleSpeed = value;
        }


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