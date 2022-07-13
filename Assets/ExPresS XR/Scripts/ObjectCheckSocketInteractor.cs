using UnityEngine.XR.Interaction.Toolkit;


namespace ExPresSXR.Interaction
{
    public class ObjectCheckSocketInteractor : HighlightableSocketInteractor
    {
        public XRGrabInteractable targetObject;

        public override bool CanHover(IXRHoverInteractable interactable)
            => base.CanHover(interactable) && IsObjectMatch(interactable);

        public override bool CanSelect(IXRSelectInteractable interactable)
            => base.CanSelect(interactable) && IsObjectMatch(interactable);

        private bool IsObjectMatch(IXRInteractable interactable)
        {
            XRGrabInteractable grabInteractable = interactable.transform.GetComponent<XRGrabInteractable>();
            return grabInteractable != null && grabInteractable.Equals(targetObject);
        }
    }
}