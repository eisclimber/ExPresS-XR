using System.Configuration;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
using UnityEngine.XR.Interaction.Toolkit.UI;

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
                _pokePointOnHover = value;
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