using System.Collections;
using System.Configuration;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace ExPresSXR.Rig
{
    /// <summary>
    /// A manager for the different interaction components a hand.
    /// This includes managing teleport, hand visuals and poke interaction.
    /// </summary>
    [AddComponentMenu("ExPresS XR/Hand Controller")]
    public class HandControllerManager : ControllerInputActionManagerBase
    {
        /// <summary>
        /// The interaction attach controller of the hand used for far interaction.
        /// </summary>
        [SerializeField]
        [Tooltip("The interaction attach controller of the hand used for far interaction.")]
        private InteractionAttachController _attachController;

        /// <summary>
        /// The interaction attach controller of the hand used for far interaction.
        /// </summary>
        [SerializeField]
        [Tooltip("The interaction attach controller of the hand used for far interaction.")]
        private ScalingInteractionAttachController _scalingAttachController;

        [Space]

        /// <summary>
        /// The poke interactor of the hand.
        /// </summary>
        [SerializeField]
        [Tooltip("The poke interactor of the hand.")]
        private XRPokeInteractor _pokeInteractor;

        [Space]

        /// <summary>
        /// The Prefab for instantiating an auto hand model for this hand.
        /// </summary>
        [SerializeField]
        [Tooltip("The Prefab for instantiating an auto hand model for this hand.")]
        private AutoHandModel _handModel;


        #region Movement Configuration
        [SerializeField]
        [Tooltip("Whether or not teleportation can be canceled with the configured InputAction (usually the Grab-Input).")]
        private bool _teleportCancelEnabled;
        /// <summary>
        /// Whether or not teleportation can be canceled with the configured InputAction (usually the Grab-Input).
        /// </summary>
        public bool TeleportCancelEnabled
        {
            get => _teleportCancelEnabled;
            set
            {
                _teleportCancelEnabled = value;
                SetEnabled(_teleportModeCancel, _teleportCancelEnabled);
            }
        }

        [SerializeField]
        [Tooltip("Whether or not the forwards direction after teleporting can be chosen when rotating the joystick. "
                + "The TeleportationAreas must have `matchDirectionalInput` enabled for it to work.")]
        private bool _chooseTeleportForwardEnabled;
        /// <summary>
        /// Whether or not the forwards direction after teleporting can be chosen when rotating the joystick.
        /// The TeleportationAreas must have `matchDirectionalInput` enabled for it to work.
        /// </summary>
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
        [SerializeField]
        [Tooltip("Whether or not near interaction is enabled.")]
        private bool _nearInteractionEnabled;
        /// <summary>
        /// Whether or not near interaction is enabled.
        /// </summary>
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

        [Tooltip("Duration for which the hand collisions are disabled after grabbing an object to allow it to be thrown.")]
        [SerializeField]
        private float _afterGrabCollisionsDisabled = 0.3f;
        /// <summary>
        /// Duration in seconds for which the hand collisions are disabled after grabbing an object to allow it to be thrown.
        /// If set to `0.0f` hand model collisions will be turned on immediately.
        /// </summary>
        public float AfterGrabCollisionsDisabled
        {
            get => _afterGrabCollisionsDisabled;
            set => _afterGrabCollisionsDisabled = value;
        }

        [SerializeField]
        private bool _handModelCollisions;
        /// <summary>
        /// Whether or not the hand models have collisions to push objects. They are disabled when hovering an object.
        /// Does not affect collisions when teleporting, these are always disabled.
        /// Change the TeleportInteractors AutoHandModel to the one with collision to enable them.
        /// </summary>
        public bool HandModelCollisions
        {
            get => _handModelCollisions;
            set
            {
                _handModelCollisions = value;

                if (_handModel)
                {
                    _handModel.ModelCollisionsEnabled = _handModelCollisions;
                }
            }
        }
        #endregion

        #region Far Config
        [SerializeField]
        [Tooltip("Whether or not far (and/or ray) interaction is enabled.")]
        private bool _farInteractionEnabled;
        /// <summary>
        /// Whether or not far (and/or ray) interaction is enabled.
        /// </summary>
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

        [SerializeField]
        [Tooltip("Whether or not ray anchor control (i.e. using the joystick to rotate/move the grabbed objects) is enabled.")]
        private bool _farAnchorControlEnabled;
        /// <summary>
        /// Whether or not ray anchor control (i.e. using the joystick to rotate/move the grabbed objects) is enabled.
        /// </summary>
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

                if (_scalingAttachController != null)
                {
                    _scalingAttachController.useManipulationInput = value;
                }
            }
        }


        [SerializeField]
        [Tooltip("Whether pulling (and pushing) an object closer during far interaction is enabled.")]
        private bool _farPullCloserEnabled;
        /// <summary>
        /// Whether pulling (and pushing) an object closer during far interaction is enabled.
        /// </summary>
        public bool FarPullCloserEnabled
        {
            get => _farPullCloserEnabled;
            set
            {
                _farPullCloserEnabled = value;

                if (_attachController != null)
                {
                    _attachController.useDistanceBasedVelocityScaling = value;
                }

                if (_scalingAttachController != null)
                {
                    _scalingAttachController.useDistanceBasedVelocityScaling = value;
                }
            }
        }

        [SerializeField]
        [Tooltip("Whether or not the ray can also interact with UI.")]
        private bool _farUiInteractionEnabled;
        /// <summary>
        /// Whether or not the ray can also interact with UI.
        /// </summary>
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
        [SerializeField]
        [Tooltip("Whether or not poke interaction is enabled.")]
        private bool _pokeInteractionEnabled;
        /// <summary>
        /// Whether or not poke interaction is enabled.
        /// </summary>
        public bool PokeInteractionEnabled
        {
            get => _pokeInteractionEnabled;
            set
            {
                if (_pokeInteractor != null)
                {
                    _pokeInteractor.hoverEntered.RemoveListener(OnPokeHoverEntered);
                    _pokeInteractor.uiHoverEntered.RemoveListener(OnPokeUiHoverEntered);
                    _pokeInteractor.hoverExited.RemoveListener(OnPokeHoverExited);
                    _pokeInteractor.uiHoverExited.RemoveListener(OnPokeUiHoverExited);
                }

                _pokeInteractionEnabled = value;

                if (_pokeInteractor != null)
                {
                    _pokeInteractor.enabled = value;

                    _pokeInteractor.hoverEntered.AddListener(OnPokeHoverEntered);
                    _pokeInteractor.uiHoverEntered.AddListener(OnPokeUiHoverEntered);
                    _pokeInteractor.hoverExited.AddListener(OnPokeHoverExited);
                    _pokeInteractor.uiHoverExited.AddListener(OnPokeUiHoverExited);
                }
            }
        }

        [SerializeField]
        [Tooltip("If the auto hand should automatically switch to a pointing pose when hovering interactables.")]
        private bool _pokePointOnHover;
        /// <summary>
        /// If the auto hand should automatically switch to a pointing pose when hovering interactables.
        /// </summary>
        public bool PokePointOnHover
        {
            get => _pokePointOnHover;
            set
            {
                _pokePointOnHover = value;
            }
        }

        [SerializeField]
        [Tooltip("Whether or not the poke reticle (i.e. all Renderer-Components in the children of the PokeInteractor) is shown.")]
        private bool _pokeShowReticle;
        /// <summary>
        /// Whether or not the poke reticle (i.e. all Renderer-Components in the children of the PokeInteractor) is shown.
        /// </summary>
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

        [SerializeField]
        [Tooltip("Whether or not poking can be used with UI.")]
        private bool _pokeUiInteractionEnabled;
        /// <summary>
        /// Whether or not poking can be used with UI.
        /// </summary>
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
        [SerializeField]
        [Tooltip("Reticle for valid teleports.")]
        private GameObject _teleportValidReticle;
        /// <summary>
        /// Reticle for valid teleports.
        /// </summary>
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

        [SerializeField]
        [Tooltip("Reticle for invalid teleports.")]
        private GameObject _teleportInvalidReticle;
        /// <summary>
        /// Reticle for invalid teleports.
        /// </summary>
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

        [SerializeField]
        [Tooltip("Used to disable certain fields in the editor when controlled by a rig.\n Hidden in the editor!")]
        private bool _externallyControlled;
        /// <summary>
        /// Used to disable certain fields in the editor when controlled by a rig.
        /// Hidden in the editor!
        /// </summary>
        public bool ExternallyControlled
        {
            get => _externallyControlled;
            set => _externallyControlled = value;
        }


        private Coroutine _afterGrabCoroutine;


        protected override void SetupInteractorEvents()
        {
            base.SetupInteractorEvents();

            if (_nearFarInteractor != null)
            {
                _nearFarInteractor.selectEntered.AddListener(OnNearFarSelectEntered);
                _nearFarInteractor.selectExited.AddListener(OnNearFarSelectExited);
            }
        }

        protected override void TeardownInteractorEvents()
        {
            base.TeardownInteractorEvents();

            if (_nearFarInteractor != null)
            {
                _nearFarInteractor.selectEntered.RemoveListener(OnNearFarSelectEntered);
                _nearFarInteractor.selectExited.RemoveListener(OnNearFarSelectExited);
            }
        }


        protected virtual void OnNearFarSelectEntered(SelectEnterEventArgs args)
        {
            if (gameObject.activeInHierarchy && isActiveAndEnabled)
            {
                _afterGrabCoroutine = StartCoroutine(AfterGrabWaitTimer());
            }
        }

        protected virtual void OnNearFarSelectExited(SelectExitEventArgs args)
        {
            // Reset after grab no collisions
            if (_afterGrabCoroutine != null)
            {
                StopCoroutine(_afterGrabCoroutine);
                _afterGrabCoroutine = null;
            }
        }

        private void SetHandPointing(bool pointing)
        {
            if (PokePointOnHover && _handModel != null)
            {
                _handModel.SetHandPointing(pointing);
            }
        }

        private void SetAutoHandCollisionsCurrentlyEnabled(bool enabled)
        {
            if (_handModel != null)
            {
                _handModel.CollisionsCurrentlyEnabled = enabled;
            }
        }

        private IEnumerator AfterGrabWaitTimer()
        {
            // Disable Collisions
            SetAutoHandCollisionsCurrentlyEnabled(false);

            yield return new WaitForSeconds(_afterGrabCollisionsDisabled);

            // Enable auto hand Collisions
            SetAutoHandCollisionsCurrentlyEnabled(HandModelCollisions);

            _afterGrabCoroutine = null;
        }

        private void OnPokeHoverEntered(HoverEnterEventArgs args) => SetHandPointing(true);

        private void OnPokeHoverExited(HoverExitEventArgs args) => SetHandPointing(false);

        private void OnPokeUiHoverEntered(UIHoverEventArgs args) => SetHandPointing(true);

        private void OnPokeUiHoverExited(UIHoverEventArgs args) => SetHandPointing(false);
    }
}