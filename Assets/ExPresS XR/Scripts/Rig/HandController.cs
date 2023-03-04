using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace ExPresSXR.Rig
{
    public class HandController : MonoBehaviour
    {

        [SerializeField]
        private HandModelMode _handModelMode;
        public HandModelMode handModelMode
        {
            get => _handModelMode;
            set
            {
                _handModelMode = value;

                TrySetHandModelModeInController(_interactionController, _handModelMode);
                TrySetHandModelModeInController(_teleportationController, _handModelMode);
            }
        }

        [SerializeField]
        private bool _interactionEnabled = true;
        public bool interactionEnabled
        {
            get => _interactionEnabled;
            set
            {
                _interactionEnabled = value;

                if (_interactionController != null)
                {
                    // Enable/Disable Interactors to prevent interaction
                    if (_interactionController.GetComponent<XRDirectInteractor>())
                    {
                        _interactionController.GetComponent<XRDirectInteractor>().enabled = _interactionEnabled;
                    }

                    if (_interactionController.GetComponent<XRRayInteractor>())
                    {
                        _interactionController.GetComponent<XRRayInteractor>().enabled = _interactionEnabled;
                    }
                }
            }
        }

        [Space]

        [SerializeField]
        private bool _teleportationEnabled = true;
        public bool teleportationEnabled
        {
            get => _teleportationEnabled;
            set => _teleportationEnabled = value;
        }

        [SerializeField]
        private bool _uiInteractionEnabled = true;
        public bool uiInteractionEnabled
        {
            get => _uiInteractionEnabled;
            set
            {
                _uiInteractionEnabled = value;

                if (_uiInteractionController != null)
                {
                    _uiInteractionController.enabled = _uiInteractionEnabled;
                }
            }
        }

        [SerializeField]
        private bool _gripDeactivatesTeleportation = false;
        public bool gripDeactivatesTeleportation
        {
            get => _gripDeactivatesTeleportation;
            set => _gripDeactivatesTeleportation = value;
        }

        [Space]

        [SerializeField]
        private ActionBasedController _interactionController;

        [SerializeField]
        private ActionBasedController _teleportationController;

        [SerializeField]
        private ActionBasedController _uiInteractionController;

        [Space]

        [Tooltip("Interaction Controller's XRGrabInteractor where interactables will be attached to.")]
        [SerializeField]
        private XRBaseInteractor _grabInteractor;

        [Space]

        [SerializeField]
        private InputActionReference _teleportModeActivationReference;

        [SerializeField]
        private InputActionReference _teleportModeDeactivationReference;


        [Space]
        public UnityEvent OnTeleportActivate;
        public UnityEvent OnTeleportCancel;


        private void Awake()
        {
            // Set hand model mode, as the prefabs are not instantiated at runtime
            handModelMode = _handModelMode;

            // _interactionController.gameObject.SetActive(_interactionEnabled);
            // _teleportationController.gameObject.SetActive(_teleportationEnabled);

            // Activate Teleport Mode (Teleports on release). Default: Move Joystick Up
            _teleportModeActivationReference.action.performed += TeleportModeActivate;

            // Deactivate Teleport Mode upon successful teleportation
            _teleportModeActivationReference.action.canceled += TeleportModeCancel;

            // Cancel Teleport Mode (if enabled) via Teleport Cancel Input: Default: Grip
            if (_gripDeactivatesTeleportation)
            {
                _teleportModeDeactivationReference.action.performed += TeleportModeCancel;
            }
        }

        private void TeleportModeActivate(InputAction.CallbackContext obj)
        {
            if (teleportationEnabled)
            {
                OnTeleportActivate.Invoke();
            }
        }

        private void TeleportModeCancel(InputAction.CallbackContext obj) 
            => StartCoroutine(DeactivateTeleporterCoroutine());

        private void DeactivateTeleporter() => OnTeleportCancel.Invoke();

        private IEnumerator DeactivateTeleporterCoroutine()
        {
            yield return new WaitForSeconds(0.1f);
            DeactivateTeleporter();
        }

        private void TrySetHandModelModeInController(ActionBasedController controller, HandModelMode mode)
        {
            if (controller != null && controller.model != null)
            {
                if (controller.model.gameObject.TryGetComponent(out AutoHandModel handModel))
                {
                    handModel.handModelMode = mode;
                }
            }
        }


        // Disable Interaction Hand Collision when selecting, enable when exiting HOVER (prevents objects from being pushed away when exiting select)
        public void OnInteractionControllerSelectEntered(SelectEnterEventArgs args) => SetIgnoreHandInteractableCollisions(args, false);

        public void OnInteractionControllerHoverExited(HoverExitEventArgs args) => SetIgnoreHandInteractableCollisions(args, true);

        // These can be also used if interactable should be handled differently
        /*
        public void OnInteractionControllerSelectExited(SelectExitEventArgs args) => SetIgnoreHandInteractableCollisions(args, false);
        public void OnInteractionControllerHoverEntered(HoverEnterEventArgs args) => SetIgnoreHandInteractableCollisions(args, true);
        */

        public void SetIgnoreHandInteractableCollisions(BaseInteractionEventArgs args, bool collisionsEnabled)
        {
            if (_interactionController != null && _interactionController.model != null)
            {
                AutoHandModel handModel = _interactionController.model.gameObject.GetComponent<AutoHandModel>() as AutoHandModel;

                if (handModel != null)
                {
                    handModel.collisionsEnabled = collisionsEnabled;

                    Transform attach = handModel.currentAttach;

                    _grabInteractor.attachTransform = attach;
                }
            }
        }


        // Updates values that are changed from other scripts in the inspector
        private void OnValidate()
        {
            handModelMode = _handModelMode;
            teleportationEnabled = _teleportationEnabled;
            interactionEnabled = _interactionEnabled;
            gripDeactivatesTeleportation = _gripDeactivatesTeleportation;
        }
    }
}