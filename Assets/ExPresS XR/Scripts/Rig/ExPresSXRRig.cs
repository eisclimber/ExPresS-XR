using UnityEngine;
using Unity.XR.CoreUtils;
using ExPresSXR.UI;
using ExPresSXR.Misc;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

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
        public InputMethod InputMethod
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
        public MovementPreset MovementPreset
        {
            get => _movementPreset;
            set
            {
                _movementPreset = value;
                RigConfigurator.ApplyMovementPreset(CurrentConfigData);
            }
        }

        [Tooltip("Flags for enabling different movement options.")]
        [SerializeField]
        private MovementOptions _movementOptions;
        public MovementOptions MovementOptions
        {
            get => _movementOptions;
            set
            {
                _movementOptions = value;
                RigConfigurator.ApplyMovementOptions(CurrentConfigData);
            }
        }

        [Tooltip("Flags for enabling different interaction options with controllers.")]
        [SerializeField]
        private InteractionOptions _interactionOptions;
        public InteractionOptions InteractionOptions
        {
            get => _interactionOptions;
            set
            {
                _interactionOptions = value;
                RigConfigurator.ApplyInteractionsOptions(CurrentConfigData);
            }
        }
        #endregion

        #region Head Gaze
        [Tooltip("Allow reselection of currently hovered Interactable with HeadGaze.")]
        [SerializeField]
        private bool _headGazeCanReselect;
        public bool HeadGazeCanReselect
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
        public float HeadGazeTimeToSelect
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
        public HeadGazeReticle HeadGazeReticle
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
        public HandControllerManager LeftHandController
        {
            get => _leftHandController;
            set
            {
                if (_leftHandController != null)
                {
                    _leftHandController.ExternallyControlled = true;
                }

                _leftHandController = value;

                if (_leftHandController != null)
                {
                    _leftHandController.ExternallyControlled = true;
                }

                RigConfigurator.ApplyConfigData(CurrentConfigData);
            }
        }

        [Tooltip("Reference to the *left* AutoHandModel of the ExPresS XR Rig.")]
        [SerializeField]
        private AutoHandModel _leftAutoHand;
        public AutoHandModel LeftAutoHand
        {
            get => _leftAutoHand;
            set
            {
                _leftAutoHand = value;
                UpdateAutoHands();
            }
        }

        [Tooltip("Reference to the *right* HandControllerManager of the ExPresS XR Rig.")]
        [SerializeField]
        private HandControllerManager _rightHandController;
        public HandControllerManager RightHandController
        {
            get => _rightHandController;
            set
            {
                if (_rightHandController != null)
                {
                    _rightHandController.ExternallyControlled = false;
                }

                _rightHandController = value;

                if (_rightHandController != null)
                {
                    _rightHandController.ExternallyControlled = true;
                }
                RigConfigurator.ApplyConfigData(CurrentConfigData);
            }
        }

        [Tooltip("Reference to the *right* AutoHandModel of the ExPresS XR Rig.")]
        [SerializeField]
        private AutoHandModel _rightAutoHand;
        public AutoHandModel RightAutoHand
        {
            get => _rightAutoHand;
            set
            {
                _rightAutoHand = value;
                UpdateAutoHands();
            }
        }

        [Tooltip("Reference to the HeadGazeController of the ExPresS XR Rig.")]
        [SerializeField]
        private HeadGazeController _headGazeController;
        public HeadGazeController HeadGazeController
        {
            get => _headGazeController;
            set
            {
                _headGazeController = value;
                RigConfigurator.ApplyConfigData(CurrentConfigData);
            }
        }
        #endregion

        #region Head Collisions
        [Tooltip("Prevents the players Camera from clipping through Objects and looking inside them by actively puhing the player back.")]
        [SerializeField]
        private bool _headCollisionPushback;
        public bool HeadCollisionPushback
        {
            get => _headCollisionPushback;
            set
            {
                _headCollisionPushback = value;

                if (_playerHeadCollider != null)
                {
                    _playerHeadCollider.collisionPushbackEnabled = _headCollisionPushback;
                }
            }
        }

        [Tooltip("Shows a vignette effect (corners get blurry) if the players Camera is clipping through Objects and looking inside them."
                    + " Does not require headCollisionPushback to be enabled to work")]
        [SerializeField]
        private bool _showCollisionVignetteEffect;
        public bool ShowCollisionVignetteEffect
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
        [Tooltip("Reference to the LocomotionMediator of the ExPresS XR Rig.")]
        [SerializeField]
        private LocomotionMediator _locomotionMediator;
        public LocomotionMediator LocomotionMediator
        {
            get => _locomotionMediator;
            set
            {
                _locomotionMediator = value;
            }
        }

        [Tooltip("Reference to the fadeRect of the ExPresS XR Rig.")]
        [SerializeField]
        private FadeRect _fadeRect;
        public FadeRect FadeRect
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
        public PlayerHeadCollider PlayerHeadCollider
        {
            get => _playerHeadCollider;
            set
            {
                _playerHeadCollider = value;

                if (_playerHeadCollider != null)
                {
                    _playerHeadCollider.screenCollisionIndicator = ScreenCollisionIndicator;
                    _playerHeadCollider.pushbackAnchor = transform;
                }
            }
        }

        [Tooltip("Camera that renders the hud. Should be configured as overlay for the Main Camera of the XR Rig.")]
        [SerializeField]
        private Camera _hudCamera;
        public Camera HudCamera
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
        public Canvas Hud
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
        public ScreenCollisionIndicator ScreenCollisionIndicator
        {
            get => _screenCollisionIndicator;
            set
            {
                _screenCollisionIndicator = value;

                if (_playerHeadCollider != null)
                {
                    _playerHeadCollider.screenCollisionIndicator = ScreenCollisionIndicator;
                }
            }
        }


        [Tooltip("Prefab that will be displayed when teleporting to a valid location. Will be overwritten by the teleportation area/anchors reticle.")]
        [SerializeField]
        private GameObject _teleportValidReticle;
        public GameObject TeleportValidReticle
        {
            get => _teleportValidReticle;
            set
            {
                _teleportValidReticle = value;

                if (_leftHandController != null)
                {
                    _leftHandController.TeleportValidReticle = _teleportValidReticle;
                }

                if (_rightHandController != null)
                {
                    _rightHandController.TeleportValidReticle = _teleportValidReticle;
                }
            }
        }

        [Tooltip("Prefab that will be displayed when teleporting to an invalid location. Will be overwritten by the teleportation area/anchors reticle.")]
        [SerializeField]
        private GameObject _teleportInvalidReticle;
        public GameObject TeleportInvalidReticle
        {
            get => _teleportInvalidReticle;
            set
            {
                _teleportInvalidReticle = value;

                if (_leftHandController != null)
                {
                    _leftHandController.TeleportInvalidReticle = _teleportInvalidReticle;
                }

                if (_rightHandController != null)
                {
                    _rightHandController.TeleportInvalidReticle = _teleportInvalidReticle;
                }
            }
        }
        #endregion

        #region General Utility
        [Tooltip("The way the 'Game'-view displays the rig's camera when entering play mode. Can be changed at runtime at the top right in the 'Game'-tab.")]
        [SerializeField]
        private GameTabDisplayMode _gameTabDisplayMode;
        public GameTabDisplayMode GameTabDisplayMode
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
        public HandModelMode HandModelMode
        {
            get => _handModelMode;
            set
            {
                _handModelMode = value;
                UpdateAutoHands();
            }
        }

        [Tooltip("Enables or disables physical collisions of the controllers/hands with other objects in the VR.")]
        [SerializeField]
        private bool _handModelCollisions = true;
        public bool HandModelCollisions
        {
            get => _handModelCollisions;
            set
            {
                _handModelCollisions = value;
                UpdateAutoHands();
            }
        }

        // Object containing all necessary references for configuration
        public ConfigData CurrentConfigData
        {
            get => new(this, _inputMethod, _movementPreset, _movementOptions, _interactionOptions,
                        _leftHandController, _rightHandController, _headGazeController,
                        _locomotionMediator);
        }
        #endregion


        // Start is called before the first frame update
        private void Awake()
        {
#if UNITY_EDITOR
            RuntimeEditorUtils.ChangeGameTabDisplayMode(GameTabDisplayMode);
#endif
        }

        #region Helper Functions
        // Fade
        public void FadeToColor()
        {
            if (_fadeRect != null)
            {
                _fadeRect.FadeToColor();
            }
        }

        public void FadeToColorInstant()
        {
            if (_fadeRect != null)
            {
                _fadeRect.FadeToColorInstant();
            }
        }

        public void FadeToClear()
        {
            if (_fadeRect != null)
            {
                _fadeRect.FadeToClear();
            }
        }

        public void FadeToClearInstant()
        {
            if (_fadeRect != null)
            {
                _fadeRect.FadeToClearInstant();
            }
        }

        public void SetControllerInputEnabled(bool enabled)
        {
            InputMethod = enabled ? InputMethod.Controller : InputMethod.None;
        }
        #endregion

        #region Editor Functions

        /// <summary>
        /// Silently applies the general config values avoiding the getters and setters.
        /// </summary>
        /// <param name="inputMethod">Input Method to be set.</param>
        /// <param name="movementPreset">Movement Preset to be set.</param>
        /// <param name="movementOptions">Movement Options to be set.</param>
        /// <param name="interactionOptions">Interaction Options to be set.</param>
        public void ApplyConfigValues(InputMethod inputMethod, MovementPreset movementPreset, 
                                        MovementOptions movementOptions, InteractionOptions interactionOptions)
        {
            _inputMethod = inputMethod;
            _movementPreset = movementPreset;
            _movementOptions = movementOptions;
            _interactionOptions = interactionOptions;
        }

        private void UpdateAutoHands()
        {
            if (_leftAutoHand != null)
            {
                _leftAutoHand.HandModelMode = _handModelMode;
                _leftAutoHand.ModelCollisionsEnabled = _handModelCollisions;
            }

            if (_rightAutoHand != null)
            {
                _rightAutoHand.HandModelMode = _handModelMode;
                _rightAutoHand.ModelCollisionsEnabled = _handModelCollisions;
            }
        }


        private void OnValidate()
        {
            // Make sure hands are set externally controlled
            LeftHandController = _leftHandController;
            RightHandController = _rightHandController;

            PlayerHeadCollider = _playerHeadCollider;
            HeadCollisionPushback = _headCollisionPushback;
            HandModelCollisions = _handModelCollisions;
            ShowCollisionVignetteEffect = _showCollisionVignetteEffect;

            // Not necessary but just to be sure
            RigConfigurator.ApplyConfigData(CurrentConfigData);
        }

        public void EditorRevalidate()
        {
            InputMethod = _inputMethod;

            // Apply Reticles
            TeleportValidReticle = _teleportValidReticle;
            TeleportInvalidReticle = _teleportInvalidReticle;

            // Setup Hud
            Hud = _hud;
            HudCamera = _hudCamera;

            // Set Hand Model mode
            HandModelMode = _handModelMode;
        }
        #endregion
    }
}