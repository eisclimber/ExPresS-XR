using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


namespace ExPresSXR.Interaction
{
    [Obsolete("OffsetInteractables are deprecated as the behavior is now natively supported by XRGrabInteractables when 'UseDynamicAttach' is enabled.")]
    // Thanks to 'VR with Andrew' on Youtube
    public class OffsetInteractable : XRGrabInteractable
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
            GameObject attachObject = new("Attach");

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
            bool isDirect = interactor is XRDirectInteractor;
            attachTransform.SetPositionAndRotation(
                isDirect ? interactor.attachTransform.position : transform.position,
                isDirect ? interactor.attachTransform.rotation : transform.rotation
            );
        }
    }
}