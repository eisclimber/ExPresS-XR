using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Credits to 'VR with Andrew' on Youtube

public class AutoXROffsetInteractable : XRGrabInteractable
{
    protected override void Awake() 
    {
        base.Awake();
        if (transform.Find("Attach") == null)
        {
            CreateAttach();
        }
    }

    private void CreateAttach()
    {
        GameObject attachObject = new GameObject("Attach");
            
        attachObject.transform.SetParent(transform);
        attachObject.transform.localPosition = Vector3.zero;
        attachObject.transform.localRotation = Quaternion.identity;

        attachTransform = attachObject.transform;
    }


    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        base.OnSelectEntering(args);
        MatchAttachPoint(args.interactorObject as XRBaseInteractor);
    }

    private void MatchAttachPoint(XRBaseInteractor interactor)
    {
        bool isDirect = (interactor is XRDirectInteractor);
        attachTransform.position = isDirect ? interactor.attachTransform.position : transform.position;
        attachTransform.rotation = isDirect ? interactor.attachTransform.rotation : transform.rotation;
    }
}
