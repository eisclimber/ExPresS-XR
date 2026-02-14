using ExPresSXR.Interaction;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace ExPresSXR.Interaction.Interactables
{
    public class RotationSnapGrabInteractable : ExPresSXRGrabInteractable
    {
        public UnityEvent<int> OnRotationSnapped;

        [SerializeField]
        private bool test = false;

        protected override void OnSelectEntering(SelectEnterEventArgs args)
        {
            // if (args.interactorObject is UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor)
            // {
            //     MatchAttachRotation(args.interactorObject);
            // }

            base.OnSelectEntering(args);
        }

        // private void MatchAttachRotation(UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor interactor)
        // {
        //     // Create attach transform if missing
        //     if (attachTransform == null)
        //     {
        //         GameObject attachGo = new("Attach Transform (Generated)");
        //         attachGo.transform.SetParent(transform);
        //         attachGo.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        //         attachTransform = attachGo.transform;
        //     }

        //     // Transform own rotation to the sockets attach local space (ignoring rotation of our attach!)
        //     Transform socketAttach = interactor.GetAttachTransform(this);
        //     Transform ownAttach = GetAttachTransform(interactor);
        //     if (test) Debug.Log("Socket Attach Rotation: " + socketAttach.rotation.eulerAngles + " vs ownAttach.rotation.eulerAngles: " + ownAttach.rotation.eulerAngles);
        //     Quaternion worldToSocketLocal = Quaternion.Inverse(socketAttach.rotation);
        //     Quaternion ownInSocketLocal = ownAttach.rotation * worldToSocketLocal;
        //     if (test) Debug.Log("Own in Socket Local Rotation: " + ownInSocketLocal.eulerAngles);

        //     // Snap own rotation to the xz-plane and 90 degree increments of y-rotation in the sockets space
        //     float eulerY = ownInSocketLocal.eulerAngles.y;
        //     if (test) Debug.Log("Euler Y: " + eulerY);
        //     int rotationSnaps = Mathf.RoundToInt(eulerY / 90.0f);
        //     if (test) Debug.Log("Rotation Snaps: " + rotationSnaps);
        //     Vector3 eulerSnapped = new(0.0f, rotationSnaps * 90.0f, 0.0f);
        //     if (test) Debug.Log("Euler Snapped: " + eulerSnapped);
        //     Quaternion ownInSocketLocalSnapped = Quaternion.Euler(eulerSnapped);
        //     // Convert snapped value to world space
        //     Quaternion snappedInWorldSpace = ownInSocketLocalSnapped * socketAttach.rotation;
        //     // Finally, we want to compensate the rotation -> use inverse 
        //     attachTransform.localRotation = Quaternion.Inverse(snappedInWorldSpace);
        //     // Emit signal to trigger rotation data
        //     OnRotationSnapped.Invoke(rotationSnaps);
        // }
    }
}