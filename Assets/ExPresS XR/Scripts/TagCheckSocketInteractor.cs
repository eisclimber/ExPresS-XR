using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TagCheckSocketInteractor : HighlightableSocketInteractor
{
    public string targetTag = "";

    public override bool CanHover(IXRHoverInteractable interactable)
        => base.CanHover(interactable) && IsTagMatch(interactable);

    public override bool CanSelect(IXRSelectInteractable interactable)
        => base.CanSelect(interactable) && IsTagMatch(interactable);

    private bool IsTagMatch(IXRInteractable interactable)
        => interactable.transform.tag == targetTag || (targetTag == "" && interactable.transform.tag == "Untagged");
}
