using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;
using ExPresSXR.UI;

namespace ExPresSXR.Rig
{
    [RequireComponent(typeof(XROrigin))]
    [DisallowMultipleComponent]
    public class ExPresSXRRig : MonoBehaviour
    {
        [SerializeField]
        private InputMethodType _inputMethod;
        public InputMethodType inputMethod
        {
            get => _inputMethod;
            set
            {
                _inputMethod = value;

                _leftHandController.gameObject.SetActive(inputMethod == InputMethodType.Controller);
                _rightHandController.gameObject.SetActive(inputMethod == InputMethodType.Controller);

                _headGazeController.gameObject.SetActive(inputMethod == InputMethodType.HeadGaze);

                if (_headGazeReticle != null)
                {
                    _headGazeReticle.gameObject.SetActive(inputMethod == InputMethodType.HeadGaze);
                }
            }
        }

        [SerializeField]
        private bool _teleportationEnabled;
        public bool teleportationEnabled
        {
            get => _teleportationEnabled;
            set
            {
                _teleportationEnabled = value;

                bool enableAsDriver = _teleportationEnabled && !_joystickMovementEnabled;

                EnableLocomotionProvider<TeleportationProvider>(_teleportationEnabled, enableAsDriver);

                _leftHandController.teleportationEnabled = _teleportationEnabled && (_interactHands & HandCombinations.Left) != 0;
                _rightHandController.teleportationEnabled = _teleportationEnabled && (_interactHands & HandCombinations.Right) != 0;
                _headGazeController.teleportationEnabled = _teleportationEnabled;
            }
        }

        [SerializeField]
        private bool _joystickMovementEnabled;
        public bool joystickMovementEnabled
        {
            get => _joystickMovementEnabled;
            set
            {
                _joystickMovementEnabled = value;

                bool enableAsDriver = _joystickMovementEnabled;

                EnableLocomotionProvider<ActionBasedContinuousMoveProvider>(_joystickMovementEnabled, enableAsDriver);
                EnableLocomotionProvider<ActionBasedContinuousTurnProvider>(_joystickMovementEnabled);

                // Snap Turn and joystick Movement is not allowed
                if (_joystickMovementEnabled && snapTurnEnabled)
                {
                    snapTurnEnabled = false;
                }
            }
        }

        [SerializeField]
        private bool _snapTurnEnabled;
        public bool snapTurnEnabled
        {
            get => _snapTurnEnabled;
            set
            {
                _snapTurnEnabled = value;

                EnableLocomotionProvider<ActionBasedSnapTurnProvider>(_snapTurnEnabled);

                // Snap Turn and joystick Movement is not allowed
                if (_snapTurnEnabled && joystickMovementEnabled)
                {
                    joystickMovementEnabled = false;
                }
            }
        }

        [SerializeField]
        private HandModelMode _handModelMode;
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

        [SerializeField]
        private HandCombinations _interactHands = HandCombinations.Left | HandCombinations.Right;
        public HandCombinations interactHands
        {
            get => _interactHands;
            set
            {
                _interactHands = value;

                _leftHandController.interactionEnabled = (_interactHands & HandCombinations.Left) != 0;
                _rightHandController.interactionEnabled = (_interactHands & HandCombinations.Right) != 0;
            }
        }

        [SerializeField]
        private HandCombinations _teleportHands = HandCombinations.Left | HandCombinations.Right;
        public HandCombinations teleportHands
        {
            get => _teleportHands;
            set
            {
                _teleportHands = value;

                _leftHandController.teleportationEnabled = _teleportationEnabled && (_interactHands & HandCombinations.Left) != 0;
                _rightHandController.teleportationEnabled = _teleportationEnabled && (_interactHands & HandCombinations.Right) != 0;
            }
        }


        [Tooltip("Prevents the players Camera from clipping through Objects and looking inside them.")]
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

        [Tooltip("Shows a vignette effect (corners get blurry) if the players Camera is clipping through Objects and looking inside them.")]
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


        [Tooltip("Gives a visual clue of the bounds of the play area. The play area must be configured via you VR's software, e.g. SteamVR. Be aware that this may change the forward direction and start position depending on your play area")]
        [SerializeField]
        private bool _showPlayAreaBounds;
        public bool showPlayAreaBounds
        {
            get => _showPlayAreaBounds;
            set
            {
                _showPlayAreaBounds = value;

                if (_playAreaBoundingBox != null)
                {
                    _playAreaBoundingBox.showPlayAreaBounds = _showPlayAreaBounds;
                }
            }
        }

        [Tooltip("Uses the material applied to the bounding box GameObject instead of using the system default.")]
        [SerializeField]
        private bool _useCustomPlayAreaMaterial;
        public bool useCustomPlayAreaMaterial
        {
            get => _useCustomPlayAreaMaterial;
            set
            {
                _useCustomPlayAreaMaterial = value;

                if (_playAreaBoundingBox != null)
                {
                    _playAreaBoundingBox.useCustomBoundingBoxMaterial = _useCustomPlayAreaMaterial;
                }
            }
        }


        ///////////////

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


        //////////////////////
        [SerializeField]
        private HandController _leftHandController;
        public HandController leftHandController
        {
            get => _leftHandController;
            set => _leftHandController = value;
        }

        //////////////////
        [SerializeField]
        private HandController _rightHandController;
        public HandController rightHandController
        {
            get => _rightHandController;
            set => _rightHandController = value;
        }

        //////////////////

        [SerializeField]
        private HeadGazeController _headGazeController;
        public HeadGazeController headGazeController
        {
            get => _headGazeController;
            set => _headGazeController = value;
        }

        //////////////////

        [SerializeField]
        private LocomotionSystem _locomotionSystem;
        public LocomotionSystem locomotionSystem
        {
            get => _locomotionSystem;
            set => _locomotionSystem = value;
        }

        //////////////////

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
                    _playerHeadCollider.pushbackAnchor = this.gameObject;
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


        [Tooltip("A Reference to the PlayAreaBoundingBox of the Rig.")]
        [SerializeField]
        private PlayAreaBoundingBox _playAreaBoundingBox;
        public PlayAreaBoundingBox playAreaBoundingBox
        {
            get => _playAreaBoundingBox;
            set => _playAreaBoundingBox = value;
        }

        //////////////

        [Tooltip("A Reference to a Canvas containing most other hud elements. It's mode must be set to 'Screen Space - Camera'.")]
        [SerializeField]
        private Canvas _hud;
        public Canvas hud
        {
            get => _hud;
            set => _hud = value;
        }

        [Tooltip("A FadeRect GameObject that is used to fade the whole screen to black. It should be a child of the hud.")]
        [SerializeField]
        private FadeRect _fadeRect;
        public FadeRect fadeRect
        {
            get => _fadeRect;
            set => _fadeRect = value;
        }

        [Tooltip("The way the 'Game'-view displays the rig's camera when entering play mode. Can be changed at runtime at the top right in the 'Game'-tab.")]
        [SerializeField]
        private GameTabDisplayMode _gameTabDisplayMode = GameTabDisplayMode.SideBySide;
        public GameTabDisplayMode gameTabDisplayMode
        {
            get => _gameTabDisplayMode;
            set => _gameTabDisplayMode = value;
        }

        ///////////
        private void EnableLocomotionProvider<T>(bool enabled, bool updateDriverOnEnable = false) where T : LocomotionProvider
        {
            if (_locomotionSystem != null)
            {
                LocomotionProvider provider = _locomotionSystem.gameObject.GetComponent<T>();
                CharacterControllerDriver driver = _locomotionSystem.gameObject.GetComponent<CharacterControllerDriver>();
                if (provider != null)
                {
                    provider.enabled = enabled;

                    if (driver != null && enabled && updateDriverOnEnable)
                    {
                        driver.locomotionProvider = provider;
                    }
                }
            }
        }

        private void Awake()
        {
            List<XRDisplaySubsystem> displaySubsystems = new List<XRDisplaySubsystem>();
            SubsystemManager.GetInstances<XRDisplaySubsystem>(displaySubsystems);

            if (displaySubsystems.Count > 0)
            {
                displaySubsystems[0].SetPreferredMirrorBlitMode((int) _gameTabDisplayMode);
            }

            // Update the initial position of the teleportation provider
            StartCoroutine(UpdateInitialCharacterPosition());
        }

        // Fade
        public void FadeToColor(bool instant = false)
        {
            if (_fadeRect != null)
            {
                _fadeRect.FadeToColor(instant);
            }
        }

        public void FadeToClear(bool instant = false)
        {
            if (_fadeRect != null)
            {
                _fadeRect.FadeToClear(instant);
            }
        }


        private IEnumerator UpdateInitialCharacterPosition()
        {
            // Wait for a bit to allow the ExPresS XR Rig to update the positions properly
            yield return new WaitForSeconds(0.3f);

            CharacterController characterController = GetComponent<CharacterController>();
            XROrigin xrOrigin = GetComponent<XROrigin>();

            var height = Mathf.Clamp(xrOrigin.CameraInOriginSpaceHeight, 0.0f, float.PositiveInfinity);
            Vector3 center = xrOrigin.CameraInOriginSpacePos;

            center.y = height / 2f + characterController.skinWidth;

            characterController.height = height;
            characterController.center = center;
        }
    }

    public enum InputMethodType
    {
        Controller,
        HeadGaze
    }

    [System.Flags]
    public enum HandCombinations
    {
        None,
        Left,
        Right
    }

    /// <summary>
    /// Reflects the <see cref="XRMirrorViewBlitMode"/> for easier display in the editor.
    /// </summary>
    public enum GameTabDisplayMode
    {
        Default = 0, 
        LeftEye = -1, 
        RightEye = -2, 
        SideBySide = -3, 
        SideBySideOcclusionMesh = -4, 
        Distort = -5,
        None = -6
    }
}