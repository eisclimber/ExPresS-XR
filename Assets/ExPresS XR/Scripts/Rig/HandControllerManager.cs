using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;

namespace ExPresSXR.Rig
{
    [AddComponentMenu("ExPresS XR/Hand Controller")]
    public class HandControllerManager : ControllerInputActionManagerBase
    {
        [SerializeField]
        private InteractionAttachController _attachController;

        [Space]

        [SerializeField]
        private XRPokeInteractor _pokeInteractor;

        [Space]

        [SerializeField]
        private AutoHandModel _handModel;


        #region Movement Configuration
        /// <summary>
        /// Whether or not teleportation can be canceled with the configured InputAction (usually the Grab-Input).
        /// </summary>
        [SerializeField]
        private bool _teleportCancelEnabled;
        public bool TeleportCancelEnabled
        {
            get => _teleportCancelEnabled;
            set
            {
                _teleportCancelEnabled = value;
                SetEnabled(_teleportModeCancel, _teleportCancelEnabled);
            }
        }

        /// <summary>
        /// Whether or not the forwards direction after teleporting can be chosen when rotating the joystick.
        /// The TeleportationAreas must have `matchDirectionalInput` enabled for it to work.
        /// </summary>
        [SerializeField]
        private bool _chooseTeleportForwardEnabled;
        public bool ChooseTeleportForwardEnabled
        {
            get => _chooseTeleportForwardEnabled;
            set
            {
                _chooseTeleportForwardEnabled = value;

                if (_teleportInteractor != null)
                {
                    _teleportInteractor.manipulateAttachTransform = _chooseTeleportForwardEnabled;
                }
            }
        }
        #endregion

        #region Near Config
        /// <summary>
        /// Whether or not near interaction is enabled.
        /// </summary>
        [SerializeField]
        private bool _nearInteractionEnabled;
        public bool NearInteractionEnabled
        {
            get => _nearInteractionEnabled;
            set
            {
                _nearInteractionEnabled = value;

                if (_nearFarInteractor != null)
                {
                    _nearFarInteractor.enableNearCasting = value;
                }
            }
        }
        #endregion

        #region Far Config
        /// <summary>
        /// Whether or not far (and/or ray) interaction is enabled.
        /// </summary>
        [SerializeField]
        private bool _farInteractionEnabled;
        public bool FarInteractionEnabled
        {
            get => _farInteractionEnabled;
            set
            {
                _farInteractionEnabled = value;

                if (_nearFarInteractor != null)
                {
                    _nearFarInteractor.enableFarCasting = value;
                }

                if (_rayInteractor != null)
                {
                    _rayInteractor.enabled = value;
                }
            }
        }

        /// <summary>
        /// Whether or not ray anchor control (i.e. using the joystick to rotate/move the grabbed objects) is enabled.
        /// </summary>
        [SerializeField]
        private bool _farAnchorControlEnabled;
        public bool FarAnchorControlEnabled
        {
            get => _farAnchorControlEnabled;
            set
            {
                _farAnchorControlEnabled = value;

                if (_rayInteractor != null)
                {
                    _rayInteractor.manipulateAttachTransform = value;
                }

                if (_attachController != null)
                {
                    _attachController.useManipulationInput = value;
                }
            }
        }

        /// <summary>
        /// Whether or not the ray can also interact with UI.
        /// </summary>
        [SerializeField]
        private bool _farUiInteractionEnabled;
        public bool FarUiInteractionEnabled
        {
            get => _farUiInteractionEnabled;
            set
            {
                _farUiInteractionEnabled = value;

                if (_nearFarInteractor != null)
                {
                    _nearFarInteractor.enableUIInteraction = _farUiInteractionEnabled;
                }

                if (_rayInteractor != null)
                {
                    _rayInteractor.enableUIInteraction = _farUiInteractionEnabled;
                }
            }
        }
        #endregion

        #region Poke Config
        /// <summary>
        /// Whether or not poke interaction is enabled.
        /// </summary>
        [SerializeField]
        private bool _pokeInteractionEnabled;
        public bool PokeInteractionEnabled
        {
            get => _pokeInteractionEnabled;
            set
            {
                _pokeInteractionEnabled = value;

                if (_pokeInteractor != null)
                {
                    _pokeInteractor.enabled = value;
                }
            }
        }

        /// <summary>
        /// Whether or not the poke reticle (i.e. all Renderer-Components in the children of the PokeInteractor) is shown.
        /// </summary>
        [Tooltip("Turns all mesh renderers in children of the PokeInteractor on or off.")]
        [SerializeField]
        private bool _pokePointOnHover;
        public bool PokePointOnHover
        {
            get => _pokePointOnHover;
            set
            {
                if (_pokeInteractor != null)
                {
                    _pokeInteractor.HoverEntered.RemoveListener(SetHandPointPose(true));
                    _pokeInteractor.HoverExited.RemoveListener(SetHandPointPose(false));
                }

                _pokePointOnHover = value;

                if (_pokeInteractor != null)
                {
                    _pokeInteractor.HoverEntered.AddListener(SetHandPointPose(true));
                    _pokeInteractor.HoverExited.AddListener(SetHandPointPose(false));
                }
            }
        }

        /// <summary>
        /// Whether or not the poke reticle (i.e. all Renderer-Components in the children of the PokeInteractor) is shown.
        /// </summary>
        [Tooltip("Turns all mesh renderers in children of the PokeInteractor on or off.")]
        [SerializeField]
        private bool _pokeShowReticle;
        public bool PokeShowReticle
        {
            get => _pokeShowReticle;
            set
            {
                _pokeShowReticle = value;

                if (_pokeInteractor != null)
                {
                    foreach (Renderer renderer in _pokeInteractor.GetComponentsInChildren<Renderer>(true))
                    {
                        renderer.enabled = _pokeShowReticle;
                    }
                }
            }
        }

        /// <summary>
        /// Whether or not poking can be used with UI.
        /// </summary>
        [SerializeField]
        private bool _pokeUiInteractionEnabled;
        public bool PokeUiInteractionEnabled
        {
            get => _pokeUiInteractionEnabled;
            set
            {
                _pokeUiInteractionEnabled = value;

                if (_pokeInteractor != null)
                {
                    _pokeInteractor.enableUIInteraction = value;
                }
            }
        }
        #endregion

        #region Teleport Reticles
        /// <summary>
        /// Reticle for valid teleports.
        /// </summary>
        [SerializeField]
        private GameObject _teleportValidReticle;
        public GameObject TeleportValidReticle
        {
            get => _teleportValidReticle;
            set
            {
                _teleportValidReticle = value;

                if (_teleportInteractor != null && _teleportInteractor.TryGetComponent(out XRInteractorLineVisual visuals))
                {
                    visuals.reticle = value;
                }
            }
        }

        /// <summary>
        /// Reticle for valid teleports.
        /// </summary>
        [SerializeField]
        private GameObject _teleportInvalidReticle;
        public GameObject TeleportInvalidReticle
        {
            get => _teleportInvalidReticle;
            set
            {
                _teleportInvalidReticle = value;

                if (_teleportInteractor != null && _teleportInteractor.TryGetComponent(out XRInteractorLineVisual visuals))
                {
                    visuals.blockedReticle = value;
                }
            }
        }
        #endregion

        /// <summary>
        /// Hidden in the editor!
        /// Used to disable certain fields in the editor when controlled by a rig.
        /// </summary>
        [SerializeField]
        private bool _externallyControlled;
        public bool ExternallyControlled
        {
            get => _externallyControlled;
            set => _externallyControlled = value;
        }




















        // /// <summary>
        // /// Connects additional events.
        // /// </summary>
        // protected override void OnEnable()
        // {
        //     base.OnEnable();

        //     if (_directInteractor != null)
        //     {
        //         _directInteractor.selectExited.AddListener(OnDirectInteractorSelectExited);
        //         _directInteractor.selectEntered.AddListener(OnDirectInteractorSelectEntered);
        //     }

        //     if (TryGetAutoHand(out AutoHandModel autoHand))
        //     {
        //         autoHand.OnModelsLoaded.RemoveListener(UpdateAutoHandModelValues);
        //     }

        //     // Reticles (AutoHandModels) need to be instantiated so they are available in the next frame
        //     StartCoroutine(AutoHandReticleCreationTimer());
        // }


        // /// <summary>
        // /// Removes connected additional events.
        // /// </summary>
        // protected override void OnDisable()
        // {
        //     base.OnDisable();

        //     if (_directInteractor != null)
        //     {
        //         _directInteractor.selectExited.RemoveListener(OnDirectInteractorSelectExited);
        //         _directInteractor.selectEntered.RemoveListener(OnDirectInteractorSelectEntered);
        //     }

        //     if (TryGetAutoHand(out AutoHandModel autoHand))
        //     {
        //         autoHand.OnModelsLoaded.RemoveListener(UpdateAutoHandModelValues);
        //     }
        // }

        // /// <summary>
        // /// Expands the base function by disabling AutoHand-Collisions during teleport.
        // /// </summary>
        // /// <param name="context">Callback Context of the InputAction.</param>
        // protected override void OnStartTeleport(InputAction.CallbackContext context)
        // {
        //     base.OnStartTeleport(context);

        //     SetAutoHandCollisionsCurrentlyEnabled(false);
        // }

        // /// <summary>
        // /// Expands the base function by enabling AutoHand-Collisions after teleport.
        // /// </summary>
        // /// <param name="context">Callback Context of the InputAction.</param>
        // protected override void OnCancelTeleport(InputAction.CallbackContext context)
        // {
        //     base.OnCancelTeleport(context);

        //     SetAutoHandCollisionsCurrentlyEnabled(true);
        // }


        // private bool TryGetAutoHand(out AutoHandModel autoHand)
        // {
        //     autoHand = null;
        //     if (TryGetComponent(out XRBaseController controller) && controller.model != null)
        //     {
        //         return controller.model.TryGetComponent(out autoHand);
        //     }
        //     return false;
        // }

        // private void SetAutoHandCollisionsCurrentlyEnabled(bool enabled)
        // {
        //     if (TryGetAutoHand(out AutoHandModel autoHand))
        //     {
        //         autoHand.collisionsCurrentlyEnabled = enabled;
        //     }
        // }

        // /// <summary>
        // /// Manages which InputActions are available.
        // /// This is slightly different to how the base function handles it.
        // /// </summary>
        // protected override void UpdateLocomotionActions()
        // {
        //     // Disable/enable Teleport and Turn when Move is enabled/disabled.
        //     SetEnabled(m_Move, smoothMoveEnabled);
        //     SetEnabled(m_TeleportModeActivate, !smoothMoveEnabled && teleportationEnabled);
        //     SetEnabled(m_TeleportModeCancel, !smoothMoveEnabled && teleportationEnabled && teleportCancelEnabled);

        //     // Disable ability to turn when using continuous movement
        //     SetEnabled(m_Turn, !smoothMoveEnabled && smoothTurnEnabled);
        //     SetEnabled(m_SnapTurn, !smoothMoveEnabled && !smoothTurnEnabled && snapTurnEnabled);
        // }

        // private void NotifyOverwrittenBehavior()
        // {
        //     if (smoothMoveEnabled && teleportationEnabled)
        //     {
        //         Debug.LogWarning("SmoothMove and Teleportation are both enabled on this hand. Teleportation is disabled, as it is overwritten by SmoothMove.");
        //     }

        //     if (smoothMoveEnabled && smoothTurnEnabled)
        //     {
        //         Debug.LogWarning("SmoothMove and SmoothTurn are both enabled on this hand. SmoothTurn is disabled, as it is overwritten by SmoothMove.");
        //     }

        //     if ((smoothMoveEnabled || smoothTurnEnabled) && snapTurnEnabled)
        //     {
        //         Debug.LogWarning("SmoothMove and/or SmoothTurn are both enabled with SnapTurn on this hand. SnapTurn is disabled, as it is overwritten by SmoothMove/SmoothTurn.");
        //     }
        // }


        // private void OnDirectInteractorSelectExited(SelectExitEventArgs args)
        // {
        //     // Start wait timer 
        //     if (gameObject.activeInHierarchy && isActiveAndEnabled)
        //     {
        //         _afterGrabCoroutine = StartCoroutine(AfterGrabWaitTimer());
        //     }

        //     // Enable the snap turn InputAction if it was previously enabled
        //     SetEnabled(m_SnapTurn, !smoothMoveEnabled && !smoothTurnEnabled && snapTurnEnabled);
        //     SetEnabled(m_TeleportModeActivate, !smoothMoveEnabled && teleportationEnabled);
        // }


        // private void OnDirectInteractorSelectEntered(SelectEnterEventArgs args)
        // {
        //     // Stop wait timer
        //     if (_afterGrabCoroutine != null)
        //     {
        //         StopCoroutine(_afterGrabCoroutine);
        //     }
        //     _afterGrabCoroutine = null;

        //     // Disable the snap turn InputAction turn while grabbing when the interactor has anchor control enabled
        //     if (hasDirectInteractorScalingSelection)
        //     {
        //         SetEnabled(m_SnapTurn, false);
        //         SetEnabled(m_TeleportModeActivate, false);
        //     }
        // }


        // private bool hasDirectInteractorScalingSelection
        // {
        //     get
        //     {
        //         ScalingDirectInteractor scalableInteractor = _directInteractor as ScalingDirectInteractor;
        //         if (scalableInteractor != null)
        //         {
        //             return scalableInteractor.hasScalingSelection;
        //         }
        //         return false;
        //     }
        // }


        // private IEnumerator AutoHandReticleCreationTimer()
        // {
        //     // Not pretty but we wait a bit after initialization to get the attach
        //     yield return new WaitForSeconds(1.0f);
        //     UpdateAutoHandModelValues();
        // }


        // private void UpdateAutoHandModelValues()
        // {
        //     handModelMode = _handModelMode;
        //     handModelCollisions = _handModelCollisions;
        // }

        // private IEnumerator AfterGrabWaitTimer()
        // {
        //     // Disable Collisions
        //     SetAutoHandCollisionsCurrentlyEnabled(false);

        //     yield return new WaitForSeconds(_afterGrabWaitDuration);

        //     // Enable auto hand Collisions
        //     SetAutoHandCollisionsCurrentlyEnabled(handModelCollisions);

        //     _afterGrabCoroutine = null;
        // }


        // private void OnValidate()
        // {
        //     teleportationEnabled = _teleportationEnabled;
        //     teleportCancelEnabled = _teleportCancelEnabled;
        //     chooseTeleportForwardEnabled = _chooseTeleportForwardEnabled;
        //     smoothMoveEnabled = _smoothMoveEnabled;
        //     smoothTurnEnabled = _smoothTurnEnabled;
        //     grabMoveEnabled = _grabMoveEnabled;
        //     directInteractionEnabled = _directInteractionEnabled;
        //     pokeInteractionEnabled = _pokeInteractionEnabled;
        //     rayInteractionEnabled = _rayInteractionEnabled;
        //     rayAnchorControlEnabled = _farAnchorControlEnabled;
        //     uiRayInteractionEnabled = _uiRayInteractionEnabled;
        //     uiPokeInteractionEnabled = _uiPokeInteractionEnabled;
        //     handModelMode = _handModelMode;
        //     handModelCollisions = _handModelCollisions;
        //     scaleGrabbedObjects = _scaleGrabbedObjects;

        //     NotifyOverwrittenBehavior();
        // }
    }
}