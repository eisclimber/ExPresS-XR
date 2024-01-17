using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;
using ExPresSXR.UI;
using ExPresSXR.Misc;

namespace ExPresSXR.Rig
{
    [AddComponentMenu("ExPresS XR/ExPresS XR Rig")]
    [RequireComponent(typeof(XROrigin))]
    public class ExPresSXRRig : MonoBehaviour
    {
        #region Config
        [Tooltip("How the rig is controlled, either per controller, via Head Gaze or Eye Gaze.")]
        [SerializeField]
        private InputMethod _inputMethod = InputMethod.Controller;
        public InputMethod inputMethod
        {
            get => _inputMethod;
            set
            {
                _inputMethod = value;

                if (_leftHandController != null)
                {
                    _leftHandController.gameObject.SetActive(_inputMethod == InputMethod.Controller);
                }

                if (_rightHandController != null)
                {
                    _rightHandController.gameObject.SetActive(_inputMethod == InputMethod.Controller);
                }

                if (_eyeGazeController != null)
                {
                    _eyeGazeController.gameObject.SetActive(_inputMethod == InputMethod.EyeGaze);
                }

                if (_headGazeController != null)
                {
                    _headGazeController.gameObject.SetActive(_inputMethod == InputMethod.HeadGaze);
                }

                if (_headGazeReticle != null)
                {
                    _headGazeReticle.gameObject.SetActive(_inputMethod == InputMethod.HeadGaze);
                }
            }
        }

        [Tooltip("Presets of how the player can move through space.")]
        [SerializeField]
        private MovementPreset _movementPreset = MovementPreset.Teleport;
        public MovementPreset movementPreset
        {
            get => _movementPreset;
            set
            {
                _movementPreset = value;

                RigConfigurator.ApplyMovementPreset(currentConfigData);
            }
        }

        [Tooltip("Flags for enabling different interaction options with controllers.")]
        [SerializeField]
        private InteractionOptions _interactionOptions;
        public InteractionOptions interactionOptions
        {
            get => _interactionOptions;
            set
            {
                _interactionOptions = value;

                RigConfigurator.ApplyInteractionsOptions(currentConfigData);
            }
        }
        #endregion

        #region Head Gaze
        [Tooltip("Allow reselection of currently hovered Interactable with HeadGaze.")]
        [SerializeField]
        private bool _headGazeCanReselect;
        public bool headGazeCanReselect
        {
            get => _headGazeCanReselect;
            set
            {
                _headGazeCanReselect = value;

                if (_headGazeController != null)
                {
                    _headGazeController.canReselect = _headGazeCanReselect;
                }
            }
        }


        [Tooltip("Determines how long in seconds the head must be kept focussed on an interaction for it to be (re-)selected.")]
        [SerializeField]
        private float _headGazeTimeToSelect;
        public float headGazeTimeToSelect
        {
            get => _headGazeTimeToSelect;
            set
            {
                _headGazeTimeToSelect = value;

                if (_headGazeController != null)
                {
                    _headGazeController.timeToSelect = _headGazeTimeToSelect;
                }
            }
        }


        [Tooltip("Reference to the HeadGazeReticle that is displayed as interaction indicator and crosshair for Head Gaze.")]
        [SerializeField]
        private HeadGazeReticle _headGazeReticle;
        public HeadGazeReticle headGazeReticle
        {
            get => _headGazeReticle;
            set
            {
                _headGazeReticle = value;

                // Must be already in the inspector!!!
                if (_headGazeController != null)
                {
                    _headGazeController.headGazeReticle = _headGazeReticle;
                }
            }
        }
        #endregion

        #region XR Controllers
        [Tooltip("Reference to the *left* HandControllerManager of the ExPresS XR Rig.")]
        [SerializeField]
        private HandControllerManager _leftHandController;
        public HandControllerManager leftHandController
        {
            get => _leftHandController;
            set
            {
                _leftHandController = value;

                RigConfigurator.ApplyEverything(currentConfigData);
            }
        }

        [Tooltip("Reference to the *right* HandControllerManager of the ExPresS XR Rig.")]
        [SerializeField]
        private HandControllerManager _rightHandController;
        public HandControllerManager rightHandController
        {
            get => _rightHandController;
            set
            {
                _rightHandController = value;

                RigConfigurator.ApplyEverything(currentConfigData);
            }
        }

        [Tooltip("Reference to the XRGazeInteractor for EyeGaze-Interactions of the ExPresS XR Rig.")]
        [SerializeField]
        private XRGazeInteractor _eyeGazeController;
        public XRGazeInteractor eyeGazeController
        {
            get => _eyeGazeController;
            set
            {
                _eyeGazeController = value;

                RigConfigurator.ApplyEverything(currentConfigData);
            }
        }

        [Tooltip("Reference to the HeadGazeController of the ExPresS XR Rig.")]
        [SerializeField]
        private HeadGazeController _headGazeController;
        public HeadGazeController headGazeController
        {
            get => _headGazeController;
            set
            {
                _headGazeController = value;

                RigConfigurator.ApplyEverything(currentConfigData);
            }
        }
        #endregion

        #region Head Collisions
        [Tooltip("Prevents the players Camera from clipping through Objects and looking inside them by actively puhing the player back.")]
        [SerializeField]
        private bool _headCollisionEnabled;
        public bool headCollisionEnabled
        {
            get => _headCollisionEnabled;
            set
            {
                _headCollisionEnabled = value;

                if (_playerHeadCollider != null)
                {
                    _playerHeadCollider.collisionPushbackEnabled = _headCollisionEnabled;
                }
            }
        }

        [Tooltip("Shows a vignette effect (corners get blurry) if the players Camera is clipping through Objects and looking inside them."
                    + " Does not require headCollisionEnabled to be enabled to work")]
        [SerializeField]
        private bool _showCollisionVignetteEffect;
        public bool showCollisionVignetteEffect
        {
            get => _showCollisionVignetteEffect;
            set
            {
                _showCollisionVignetteEffect = value;

                if (_playerHeadCollider != null)
                {
                    _playerHeadCollider.showCollisionVignetteEffect = _showCollisionVignetteEffect;
                }
            }
        }
        #endregion

        #region Misc References
        [Tooltip("Reference to the LocomotionManager of the ExPresS XR Rig.")]
        [SerializeField]
        private LocomotionSystem _locomotionSystem;
        public LocomotionSystem locomotionSystem
        {
            get => _locomotionSystem;
            set
            {
                _locomotionSystem = value;
            }
        }


        [Tooltip("Reference to the fadeRect of the ExPresS XR Rig.")]
        [SerializeField]
        private FadeRect _fadeRect;
        public FadeRect fadeRect
        {
            get => _fadeRect;
            set
            {
                _fadeRect = value;
            }
        }


        [Tooltip("Must be a PlayerHeadCollider-Component attached to the Main Camera GameObject.")]
        [SerializeField]
        private PlayerHeadCollider _playerHeadCollider;
        public PlayerHeadCollider playerHeadCollider
        {
            get => _playerHeadCollider;
            set
            {
                _playerHeadCollider = value;

                if (_playerHeadCollider != null)
                {
                    _playerHeadCollider.screenCollisionIndicator = screenCollisionIndicator;
                    _playerHeadCollider.pushbackAnchor = transform;
                }
            }
        }

        [Tooltip("Camera that renders the hud. Should be configured as overlay for the Main Camera of the XR Rig.")]
        [SerializeField]
        private Camera _hudCamera;
        public Camera hudCamera
        {
            get => _hudCamera;
            set
            {
                _hudCamera = value;

                if (_hudCamera != null)
                {
                    _hudCamera.cullingMask = 1 << LayerMask.NameToLayer("UI Always On Top");
                }

                if (_hud != null)
                {
                    _hud.worldCamera = _hudCamera;
                }
            }
        }


        [Tooltip("Canvas that acts as a hud for the rig.")]
        [SerializeField]
        private Canvas _hud;
        public Canvas hud
        {
            get => _hud;
            set
            {
                _hud = value;

                if (_hud != null)
                {
                    if (_hud.gameObject.layer != LayerMask.NameToLayer("UI Always On Top"))
                    {
                        Debug.LogWarning("The Hud's layer (and it's children) must be set to 'UI Always On Top'.");
                    }
                    _hud.worldCamera = _hudCamera;
                }
            }
        }


        [Tooltip("Must be a ScreenCollisionIndicator-Component attached to the Hud.")]
        [SerializeField]
        private ScreenCollisionIndicator _screenCollisionIndicator;
        public ScreenCollisionIndicator screenCollisionIndicator
        {
            get => _screenCollisionIndicator;
            set
            {
                _screenCollisionIndicator = value;

                if (_playerHeadCollider != null)
                {
                    _playerHeadCollider.screenCollisionIndicator = screenCollisionIndicator;
                }
            }
        }


        [Tooltip("Prefab that will be displayed when teleporting to a valid location. Will be overwritten by the teleportation area/anchors reticle.")]
        [SerializeField]
        private GameObject _teleportValidReticle;
        public GameObject teleportValidReticle
        {
            get => _teleportValidReticle;
            set
            {
                bool updateReticles = _teleportValidReticle == value;

                _teleportValidReticle = value;

                if (updateReticles)
                {
                    if (_leftHandController != null && _leftHandController.TeleportInteractor != null
                            && _leftHandController.TeleportInteractor.TryGetComponent(out XRInteractorLineVisual leftLineVisual))
                    {
                        leftLineVisual.reticle = _teleportValidReticle;
                    }

                    if (_rightHandController != null && _rightHandController.TeleportInteractor != null
                            && _rightHandController.TeleportInteractor.TryGetComponent(out XRInteractorLineVisual rightLineVisual))
                    {
                        rightLineVisual.reticle = _teleportValidReticle;
                    }
                }
            }
        }

        [Tooltip("Prefab that will be displayed when teleporting to an invalid location. Will be overwritten by the teleportation area/anchors reticle.")]
        [SerializeField]
        private GameObject _teleportInvalidReticle;
        public GameObject teleportInvalidReticle
        {
            get => _teleportInvalidReticle;
            set
            {
                bool updateReticles = _teleportInvalidReticle == value;
                _teleportInvalidReticle = value;

                if (updateReticles)
                {
                    if (_leftHandController != null && _leftHandController.TeleportInteractor != null
                            && _leftHandController.TeleportInteractor.TryGetComponent(out XRInteractorLineVisual leftLineVisual))
                    {
                        leftLineVisual.blockedReticle = _teleportInvalidReticle;
                    }

                    if (_rightHandController != null && _rightHandController.TeleportInteractor != null
                            && _rightHandController.TeleportInteractor.TryGetComponent(out XRInteractorLineVisual rightLineVisual))
                    {
                        rightLineVisual.blockedReticle = _teleportInvalidReticle;
                    }
                }
            }
        }
        #endregion

        #region General Utility
        [Tooltip("The way the 'Game'-view displays the rig's camera when entering play mode. Can be changed at runtime at the top right in the 'Game'-tab.")]
        [SerializeField]
        private GameTabDisplayMode _gameTabDisplayMode;
        public GameTabDisplayMode gameTabDisplayMode
        {
            get => _gameTabDisplayMode;
            set
            {
                _gameTabDisplayMode = value;
            }
        }


        [Tooltip("Determines how the controllers/hands are rendered in the VR.")]
        [SerializeField]
        private HandModelMode _handModelMode = HandModelMode.Hand;
        public HandModelMode handModelMode
        {
            get => _handModelMode;
            set
            {
                _handModelMode = value;

                if (_leftHandController != null)
                {
                    _leftHandController.handModelMode = handModelMode;
                }

                if (_rightHandController != null)
                {
                    _rightHandController.handModelMode = handModelMode;
                }
            }
        }

        [Tooltip("Enables or disables physical collisions of the controllers/hands with other objects in the VR.")]
        [SerializeField]
        private bool _handModelCollisions = true;
        public bool handModelCollisions
        {
            get => _handModelCollisions;
            set
            {
                _handModelCollisions = value;

                if (_leftHandController != null)
                {
                    _leftHandController.handModelCollisions = _handModelCollisions;
                }

                if (_rightHandController != null)
                {
                    _rightHandController.handModelCollisions = _handModelCollisions;
                }
            }
        }

        // Object containing all necessary references for configuration
        public ConfigData currentConfigData
        {
            get => new(inputMethod, movementPreset, interactionOptions,
                        leftHandController, rightHandController,
                        eyeGazeController, headGazeController, locomotionSystem);
        }
        #endregion


        // Start is called before the first frame update
        private void Awake()
        {
#if UNITY_EDITOR
            RuntimeEditorUtils.ChangeGameTabDisplayMode(gameTabDisplayMode);
#endif
        }

        // Fade
        public void FadeToColor(bool instant = false)
        {
            if (fadeRect != null)
            {
                fadeRect.FadeToColor(instant);
            }
        }

        public void FadeToClear(bool instant = false)
        {
            if (fadeRect != null)
            {
                fadeRect.FadeToClear(instant);
            }
        }

        #region Editor Functions
        private void OnValidate()
        {
            RigConfigurator.ApplyEverything(currentConfigData);

            // Apply Head Collisions
            playerHeadCollider = _playerHeadCollider;
            headCollisionEnabled = _headCollisionEnabled;
            handModelMode = _handModelMode;
            handModelCollisions = _handModelCollisions;
            showCollisionVignetteEffect = _showCollisionVignetteEffect;
        }

        public void EditorRevalidate()
        {
            inputMethod = _inputMethod;

            // Apply Reticles
            teleportValidReticle = _teleportValidReticle;
            teleportInvalidReticle = _teleportInvalidReticle;

            // Setup Hud
            hud = _hud;
            hudCamera = _hudCamera;

            // Update showPokeReticle here as it enables/disables a Component
            if (_leftHandController != null)
            {
                _leftHandController.showPokeReticle = _interactionOptions.HasFlag(InteractionOptions.ShowPokeReticle);
            }

            if (_rightHandController != null)
            {
                _rightHandController.showPokeReticle = _interactionOptions.HasFlag(InteractionOptions.ShowPokeReticle);
            }
        }
        #endregion
    }
}