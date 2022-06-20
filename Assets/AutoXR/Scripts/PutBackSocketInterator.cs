using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PutBackSocketInteractor : XRSocketInteractor
{
    [SerializeField]
    private Transform _putBackObject;
    public Transform putBackObject
    {
        get => _putBackObject;
        set => putBackObject = value;
    }

    [SerializeField]
    private float _putBackTime;
    public float putBackTime
    {
        get => _putBackTime;
        set => _putBackTime = value;
    }


    public override bool CanHover(IXRHoverInteractable interactable)
    {
        return base.CanHover(interactable) && IsObjectMatch(interactable);
    }

    public override bool CanSelect(IXRSelectInteractable interactable)
    {
        return base.CanSelect(interactable) && IsObjectMatch(interactable);
    }

    private bool IsObjectMatch(IXRInteractable interactable) => (interactable.transform == _putBackObject);
}
