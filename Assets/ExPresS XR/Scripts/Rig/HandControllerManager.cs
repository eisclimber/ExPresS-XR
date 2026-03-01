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
        /// <summary>
        /// Whether or not teleportation can be canceled with the configured InputAction (usually the Grab-Input).
        /// </summary>
        [SerializeField]
        [Tooltip("Whether or not teleportation can be canceled with the configured InputAction (usually the Grab-Input).")]
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
        [Tooltip("Whether or not the forwards direction after teleporting can be chosen when rotating the joystick. "
                + "The TeleportationAreas must have `matchDirectionalInput` enabled for it to work.")]
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
        [Tooltip("Whether or not near interaction is enabled.")]
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
        [Tooltip("Whether or not far (and/or ray) interaction is enabled.")]
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
        [Tooltip("Whether or not ray anchor control (i.e. using the joystick to rotate/move the grabbed objects) is enabled.")]
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
        /// Whether pulling (and pushing) an object closer during far interaction is enabled.
        /// </summary>
        [SerializeField]
        [Tooltip("Whether pulling (and pushing) an object closer during far interaction is enabled.")]
        private bool _farPullCloserEnabled;
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
            }
        }

        /// <summary>
        /// Whether or not the ray can also interact with UI.
        /// </summary>
        [SerializeField]
        [Tooltip("Whether or not the ray can also interact with UI.")]
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
        [Tooltip("Whether or not poke interaction is enabled.")]
        private bool _pokeInteractionEnabled;
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

        /// <summary>
        /// If the auto hand should automatically switch to a pointing pose when hovering interactables.
        /// </summary>
        [SerializeField]
        [Tooltip("If the auto hand should automatically switch to a pointing pose when hovering interactables.")]
        private bool _pokePointOnHover;
        public bool PokePointOnHover
        {
            get => _pokePointOnHover;
            set
            {
                _pokePointOnHover = value;
            }
        }

        /// <summary>
        /// Whether or not the poke reticle (i.e. all Renderer-Components in the children of the PokeInteractor) is shown.
        /// </summary>
        [SerializeField]
        [Tooltip("Whether or not the poke reticle (i.e. all Renderer-Components in the children of the PokeInteractor) is shown.")]
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
        [Tooltip("Whether or not poking can be used with UI.")]
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
        [Tooltip("Reticle for valid teleports.")]
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
        /// Reticle for invalid teleports.
        /// </summary>
        [SerializeField]
        [Tooltip("Reticle for invalid teleports.")]
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
        /// Used to disable certain fields in the editor when controlled by a rig.
        /// Hidden in the editor!
        /// </summary>
        [SerializeField]
        [Tooltip("Used to disable certain fields in the editor when controlled by a rig.\n Hidden in the editor!")]
        private bool _externallyControlled;
        public bool ExternallyControlled
        {
            get => _externallyControlled;
            set => _externallyControlled = value;
        }


        private void SetHandPointing(bool pointing)
        {
            if (PokePointOnHover && _handModel != null)
            {
                _handModel.SetHandPointing(pointing);
            }
        }


        private void OnPokeHoverEntered(HoverEnterEventArgs args) => SetHandPointing(true);

        private void OnPokeHoverExited(HoverExitEventArgs args) => SetHandPointing(false);

        private void OnPokeUiHoverEntered(UIHoverEventArgs args) => SetHandPointing(true);

        private void OnPokeUiHoverExited(UIHoverEventArgs args) => SetHandPointing(false);
    }
}