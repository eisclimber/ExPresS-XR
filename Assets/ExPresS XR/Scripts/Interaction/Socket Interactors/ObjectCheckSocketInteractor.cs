using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace ExPresSXR.Interaction
{
    public class ObjectCheckSocketInteractor : HighlightableSocketInteractor
    {
        public XRGrabInteractable targetObject;

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
            return targetObject != null && grabInteractable != null && grabInteractable.gameObject == targetObject.gameObject;
        }
    }
}