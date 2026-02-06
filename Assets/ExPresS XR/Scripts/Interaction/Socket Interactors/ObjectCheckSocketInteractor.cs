using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace ExPresSXR.Interaction.Interactors
{
    public class ObjectCheckSocketInteractor : HighlightableSocketInteractor
    {
        private XRGrabInteractable _targetObject;
        public XRGrabInteractable TargetObject
        {
            get => _targetObject;
            set => _targetObject = value;
        }

        [SerializeField]
        private bool _allowInvalidHover;

        public override bool CanHover(IXRHoverInteractable interactable)
            => base.CanHover(interactable) && (IsObjectMatch(interactable) || _allowInvalidHover);

        protected override Material GetHoveredInteractableMaterial(IXRHoverInteractable interactable)
        {
            if (!IsObjectMatch(interactable))
            {
                return interactableCantHoverMeshMaterial;
            }
            return base.GetHoveredInteractableMaterial(interactable);
        }

        public override bool CanSelect(IXRSelectInteractable interactable)
            => base.CanSelect(interactable) && IsObjectMatch(interactable);

        private bool IsObjectMatch(IXRInteractable interactable)
        {
            XRGrabInteractable grabInteractable = interactable.transform.GetComponent<XRGrabInteractable>();
            return _targetObject != null && grabInteractable != null && grabInteractable.gameObject == _targetObject.gameObject;
        }
    }
}