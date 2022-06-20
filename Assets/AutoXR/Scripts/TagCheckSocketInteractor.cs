using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TagCheckSocketInteractor : XRSocketInteractor
{
    [SerializeField]
    public string targetTag = "";

    public override bool CanHover(IXRHoverInteractable interactable)
    {
        return base.CanHover(interactable) && IsTagMatch(interactable);
    }

    public override bool CanSelect(IXRSelectInteractable interactable)
    {
        return base.CanSelect(interactable) && IsTagMatch(interactable);
    }

    private bool IsTagMatch(IXRInteractable interactable) => (interactable.transform.tag == targetTag);
}
