using UnityEngine;
using Unity.XR.CoreUtils;
using ExPresSXR.UI;
using ExPresSXR.Misc;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

namespace ExPresSXR.Rig
{
    /// <summary>
    /// The main configurable GameObject for XR, having all necessary components for any form of interaction as children of this one GameObject.
    /// 
    /// The rig supports three different types of input methods:
    /// 
    /// - Controller: Used with a complete VR Headset and controllers. Using this allows for the most immersive XR experience.
    /// - Head Gaze: Used with for Smartphone applications but also compatible with VR headsets without requiring controllers.
    /// - Eye Gaze: Used with for Smartphone applications but also compatible with VR headsets without requiring controllers. (As we do not have a compatible headset we are unable to test it. It is included as it is part of Unity's samples and was not changed by ExPresS XR.)
    /// 
    /// Movement is dependent on the pselected Input Method. While Head- and Eye-Gaze only allow for teleportation or no movement, there are the following possibilities when using controllers:
    /// 
    /// - Teleportation: Casts a ray and teleports the rig to the target position of the ray. At the end of the ray is a visual indicator (reticle) that highlight teleportation targets. These targets are defined by the GameObjects with a `TeleportationArea`- and `TeleportationAnchor`-Component.
    ///     *Note:* In case of HeadGaze the ray will not be rendered, only the reticle will be shown. The reticles must be added to the teleportation areas and anchors as otherwise they will be shown with all interactions).
    /// - Continuous Move: Moves the rig continuous as if the user would move in a video game. *This might cause motion sickness for some people!*
    /// - Continuous Turn: Turns the rig continuous in the direction the direction the joystick was moved. This can not be combined with Snap Turn and is usually paired with Continuous Move.
    /// - Snap Turn: Turns the rig in 45 degree (can be changed) steps in the direction the joystick was moved. This can not be combined with Continuous Turn and is usually paired with Teleportation.
    /// - (Single Hand) Grab Motion: Users can grab the air and pull the world towards them.
    /// - (Two Handed) Grab Motion / Grab Manipulation: Expansion of "Single Hand Grab Motion"-Movement but both controllers can be used together to rotate and scale the environment.
    /// - No Movement: If nothing is enabled the rig won't move, but the whole rig itself can still be moved externally.
    /// 
    /// These movement options can be configured by selecting one of the rig's `Movement Presets`. Per default the controls are split between hands in a way known from gamepads or most common VR games (e.g. turn with the right joystick and move with the left). For a detailed description on the controls please check the docs at `Workflow/MovementAndControls.md`.  
    /// Choosing `Custom` allows for full control over the underlying `HandControllers` to create a custom behavior.
    /// 
    /// The application will be rendered directly to the VR headset via Unity's `XROrigin`. There is an additional (UI-)Camera for rendering anything that should be rendered as overlay, most commonly the some sort of HUD.
    /// 
    /// It is also possible to create and save a custom XR Rig as a prefab.
    /// After editing an XR Rig to you your likings the `Save as Custom XR Rig`-Button in the inspector can be pressed.
    /// This will save (and override) the Custom XR Rig making it instantiable via *ExPresS XR/XR Rig/Custom (Saved)*.
    /// 
    /// *Hint:* For further descriptions of the components have a look at the documentation of the properties in this document, the components from `ExPresS XR` and [Unity XR](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.1/api/UnityEngine.XR.Interaction.Toolkit.html).
    /// </summary>
    [AddComponentMenu("ExPresS XR/ExPresS XR Rig")]
    [RequireComponent(typeof(XROrigin))]
    public class ExPresSXRRig : MonoBehaviour
    {
        #region Config
        [SerializeField]
        [Tooltip("How the rig is controlled, either per controller, via Head Gaze or Eye Gaze.")]
        private InputMethod _inputMethod = InputMethod.Controller;
        /// <summary>
        /// How the rig is controlled, either per controller, via Head Gaze or Eye Gaze.
        /// </summary>
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

        [SerializeField]
        [Tooltip("Presets of how the player can move through space.")]
        private MovementPreset _movementPreset = MovementPreset.Teleport;
        /// <summary>
        /// Presets of how the player can move through space.
        /// </summary>
        public MovementPreset MovementPreset
        {
            get => _movementPreset;
            set
            {
                _movementPreset = value;
                RigConfigurator.ApplyMovementPreset(CurrentConfigData);
            }
        }

        [SerializeField]
        [Tooltip("Flags for enabling different movement options.")]
        private MovementOptions _movementOptions;
        /// <summary>
        /// Flags for enabling different movement options.
        /// </summary>
        public MovementOptions MovementOptions
        {
            get => _movementOptions;
            set
            {
                _movementOptions = value;
                RigConfigurator.ApplyMovementOptions(CurrentConfigData);
            }
        }

        [SerializeField]
        [Tooltip("Flags for enabling different interaction options with controllers.")]
        private InteractionOptions _interactionOptions;
        /// <summary>
        /// Flags for enabling different interaction options with controllers.
        /// </summary>
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
        [SerializeField]
        [Tooltip("Allow reselection of currently hovered Interactable with HeadGaze.")]
        private bool _headGazeCanReselect;
        /// <summary>
        /// Allow reselection of currently hovered Interactable with HeadGaze.
        /// </summary>
        public bool HeadGazeCanReselect
        {
            get => _headGazeCanReselect;
            set
            {
                _headGazeCanReselect = value;

                if (_headGazeController != null)
                {
                    _headGazeController.CanReselect = _headGazeCanReselect;
                }
            }
        }

        [SerializeField]
        [Tooltip("Determines how long in seconds the head must be kept focussed on an interaction for it to be (re-)selected.")]
        private float _headGazeTimeToSelect;
        /// <summary>
        /// Determines how long in seconds the head must be kept focussed on an interaction for it to be (re-)selected.
        /// </summary>
        public float HeadGazeTimeToSelect
        {
            get => _headGazeTimeToSelect;
            set
            {
                _headGazeTimeToSelect = value;

                if (_headGazeController != null)
                {
                    _headGazeController.TimeToSelect = _headGazeTimeToSelect;
                }
            }
        }

        [SerializeField]
        [Tooltip("Reference to the HeadGazeReticle that is displayed as interaction indicator and crosshair for Head Gaze.")]
        private HeadGazeReticle _headGazeReticle;
        /// <summary>
        /// Reference to the HeadGazeReticle that is displayed as interaction indicator and crosshair for Head Gaze.
        /// </summary>
        public HeadGazeReticle HeadGazeReticle
        {
            get => _headGazeReticle;
            set
            {
                _headGazeReticle = value;

                // Must be already in the inspector!!!
                if (_headGazeController != null)
                {
                    _headGazeController.HeadGazeReticle = _headGazeReticle;
                }
            }
        }
        #endregion

        #region XR Controllers
        [SerializeField]
        [Tooltip("Reference to the *left* HandControllerManager of the ExPresS XR Rig.")]
        private HandControllerManager _leftHandController;
        /// <summary>
        /// Reference to the *left* HandControllerManager of the ExPresS XR Rig.
        /// </summary>
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

        [SerializeField]
        [Tooltip("Reference to the *left* AutoHandModel of the ExPresS XR Rig.")]
        private AutoHandModel _leftAutoHand;
        /// <summary>
        /// Reference to the *left* AutoHandModel of the ExPresS XR Rig.
        /// </summary>
        public AutoHandModel LeftAutoHand
        {
            get => _leftAutoHand;
            set
            {
                _leftAutoHand = value;
                UpdateAutoHands();
            }
        }

        [SerializeField]
        [Tooltip("Reference to the *right* HandControllerManager of the ExPresS XR Rig.")]
        private HandControllerManager _rightHandController;
        /// <summary>
        /// Reference to the *right* HandControllerManager of the ExPresS XR Rig.
        /// </summary>
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

        [SerializeField]
        [Tooltip("Reference to the *right* AutoHandModel of the ExPresS XR Rig.")]
        private AutoHandModel _rightAutoHand;
        /// <summary>
        /// Reference to the *right* AutoHandModel of the ExPresS XR Rig.
        /// </summary>
        public AutoHandModel RightAutoHand
        {
            get => _rightAutoHand;
            set
            {
                _rightAutoHand = value;
                UpdateAutoHands();
            }
        }

        [SerializeField]
        [Tooltip("Reference to the HeadGazeController of the ExPresS XR Rig.")]
        private HeadGazeController _headGazeController;
        /// <summary>
        /// Reference to the HeadGazeController of the ExPresS XR Rig.
        /// </summary>
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
        [SerializeField]
        [Tooltip("Prevents the players Camera from clipping through Objects and looking inside them by actively pushing the player back.")]
        private bool _headCollisionPushback;
        /// <summary>
        /// Prevents the players Camera from clipping through Objects and looking inside them by actively pushing the player back.
        /// </summary>
        public bool HeadCollisionPushback
        {
            get => _headCollisionPushback;
            set
            {
                _headCollisionPushback = value;

                if (_playerHeadCollider != null)
                {
                    _playerHeadCollider.CollisionPushbackEnabled = _headCollisionPushback;
                }
            }
        }

        [SerializeField]
        [Tooltip("Shows a vignette effect (corners get blurry) if the players Camera is clipping through Objects and looking inside them."
                    + " Does not require headCollisionPushback to be enabled to work.")]
        private bool _showCollisionVignetteEffect;
        /// <summary>
        /// Shows a vignette effect (corners get blurry) if the players Camera is clipping through Objects and looking inside them.
        /// Does not require headCollisionPushback to be enabled to work.
        /// </summary>
        public bool ShowCollisionVignetteEffect
        {
            get => _showCollisionVignetteEffect;
            set
            {
                _showCollisionVignetteEffect = value;

                if (_playerHeadCollider != null)
                {
                    _playerHeadCollider.ShowCollisionVignetteEffect = _showCollisionVignetteEffect;
                }
            }
        }
        #endregion

        #region Misc References
        [SerializeField]
        [Tooltip("Reference to the LocomotionMediator of the ExPresS XR Rig.")]
        private LocomotionMediator _locomotionMediator;
        /// <summary>
        /// Reference to the LocomotionMediator of the ExPresS XR Rig.
        /// </summary>
        public LocomotionMediator LocomotionMediator
        {
            get => _locomotionMediator;
            set
            {
                _locomotionMediator = value;
            }
        }

        [SerializeField]
        [Tooltip("Reference to the fadeRect of the ExPresS XR Rig.")]
        private FadeRect _fadeRect;
        /// <summary>
        /// Reference to the fadeRect of the ExPresS XR Rig.
        /// </summary>
        public FadeRect FadeRect
        {
            get => _fadeRect;
            set
            {
                _fadeRect = value;
            }
        }

        [SerializeField]
        [Tooltip("Must be a PlayerHeadCollider-Component attached to the Main Camera GameObject.")]
        private PlayerHeadCollider _playerHeadCollider;
        /// <summary>
        /// Must be a PlayerHeadCollider-Component attached to the Main Camera GameObject.
        /// </summary>
        public PlayerHeadCollider PlayerHeadCollider
        {
            get => _playerHeadCollider;
            set
            {
                _playerHeadCollider = value;

                if (_playerHeadCollider != null)
                {
                    _playerHeadCollider.screenCollisionIndicator = ScreenCollisionIndicator;
                    _playerHeadCollider.PushbackAnchor = transform;
                }
            }
        }

        [SerializeField]
        [Tooltip("The camera that renders the hud. Should be configured as overlay for the Main Camera of the XR Rig.")]
        private Camera _hudCamera;
        /// <summary>
        /// The camera that renders the hud. Should be configured as overlay for the Main Camera of the XR Rig.
        /// </summary>
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

        [SerializeField]
        [Tooltip("Canvas that acts as a hud for the rig.")]
        private Canvas _hud;
        /// <summary>
        /// Canvas that acts as a hud for the rig.
        /// </summary>
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

        [SerializeField]
        [Tooltip("Must be a ScreenCollisionIndicator-Component attached to the Hud.")]
        private ScreenCollisionIndicator _screenCollisionIndicator;
        /// <summary>
        /// Must be a ScreenCollisionIndicator-Component attached to the Hud.
        /// </summary>
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

        [SerializeField]
        [Tooltip("Prefab that will be displayed when teleporting to a valid location. Will be overwritten by the teleportation area/anchors reticle.")]
        private GameObject _teleportValidReticle;
        /// <summary>
        /// Prefab that will be displayed when teleporting to a valid location. Will be overwritten by the teleportation area/anchors reticle.
        /// </summary>
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

        [SerializeField]
        [Tooltip("Prefab that will be displayed when teleporting to an invalid location. Will be overwritten by the teleportation area/anchors reticle.")]
        private GameObject _teleportInvalidReticle;
        /// <summary>
        /// Prefab that will be displayed when teleporting to an invalid location. Will be overwritten by the teleportation area/anchors reticle.
        /// </summary>
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
        [SerializeField]
        [Tooltip("The way the 'Game'-view displays the rig's camera when entering play mode. Can be changed at runtime at the top right in the 'Game'-tab.")]
        private GameTabDisplayMode _gameTabDisplayMode;
        /// <summary>
        /// The way the 'Game'-view displays the rig's camera when entering play mode. Can be changed at runtime at the top right in the 'Game'-tab.
        /// </summary>
        public GameTabDisplayMode GameTabDisplayMode
        {
            get => _gameTabDisplayMode;
            set
            {
                _gameTabDisplayMode = value;
            }
        }

        [SerializeField]
        [Tooltip("Determines how the controllers/hands are rendered in the VR.")]
        private HandModelMode _handModelMode = HandModelMode.Hand;
        /// <summary>
        /// Determines how the controllers/hands are rendered in the VR.
        /// </summary>
        public HandModelMode HandModelMode
        {
            get => _handModelMode;
            set
            {
                _handModelMode = value;
                UpdateAutoHands();
            }
        }

        [SerializeField]
        [Tooltip("Enables or disables physical collisions of the controllers/hands with other objects in the VR.")]
        private bool _handModelCollisions = true;
        /// <summary>
        /// Enables or disables physical collisions of the controllers/hands with other objects in the VR.
        /// </summary>
        public bool HandModelCollisions
        {
            get => _handModelCollisions;
            set
            {
                _handModelCollisions = value;
                UpdateAutoHands();
            }
        }

        /// <summary>
        /// Object containing all necessary references for configuration
        /// </summary>
        public ConfigData CurrentConfigData
        {
            get => new(this, _inputMethod, _movementPreset, _movementOptions, _interactionOptions,
                        _leftHandController, _rightHandController, _headGazeController,
                        _locomotionMediator);
        }
        #endregion

        private void Awake()
        {
#if UNITY_EDITOR
            RuntimeEditorUtils.ChangeGameTabDisplayMode(GameTabDisplayMode);
#endif
        }

        #region Helper Functions
        /// <summary>
        /// Fades the view to black. Proxy function for the FadeRect of the rig.
        /// </summary>
        public void FadeToColor()
        {
            if (_fadeRect != null)
            {
                _fadeRect.FadeToColor();
            }
        }

        /// <summary>
        /// Fades the view to black instantly. Proxy function for the FadeRect of the rig.
        /// </summary>
        public void FadeToColorInstant()
        {
            if (_fadeRect != null)
            {
                _fadeRect.FadeToColorInstant();
            }
        }

        /// <summary>
        /// Fades the view to black. Proxy function for the FadeRect of the rig.
        /// </summary>
        public void FadeToClear()
        {
            if (_fadeRect != null)
            {
                _fadeRect.FadeToClear();
            }
        }

        /// <summary>
        /// Fades the view to black instantly. Proxy function for the FadeRect of the rig.
        /// </summary>
        public void FadeToClearInstant()
        {
            if (_fadeRect != null)
            {
                _fadeRect.FadeToClearInstant();
            }
        }

        /// <summary>
        /// Temporarily enables/disables controller input by switching the input method.
        /// </summary>
        /// <param name="enabled">If input should be enabled.</param>
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

        /// <summary>
        /// Used internally for applying changes made in the inspector.
        /// </summary>
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