using UnityEngine.XR.Interaction.Toolkit;


namespace ExPresSXR.Interaction
{
    public class TagCheckSocketInteractor : HighlightableSocketInteractor
    {
        public string targetTag = "";

        public override bool CanHover(IXRHoverInteractable interactable)
            => base.CanHover(interactable) && IsTagMatch(interactable);

        public override bool CanSelect(IXRSelectInteractable interactable)
            => base.CanSelect(interactable) && IsTagMatch(interactable);

        private bool IsTagMatch(IXRInteractable interactable)
            => interactable.transform.CompareTag(targetTag) || (targetTag == "" && interactable.transform.CompareTag("Untagged"));
    }
}