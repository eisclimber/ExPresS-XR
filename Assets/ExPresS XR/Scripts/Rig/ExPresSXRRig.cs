using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;
using ExPresSXR.UI;

namespace ExPresSXR.Rig
{
    [AddComponentMenu("ExPresS XR/ExPresS XR Rig")]
    [RequireComponent(typeof(XROrigin))]
    public class ExPresSXRRig : MonoBehaviour
    {
        // Input and Movement
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
            }
        }


        [SerializeField]
        private MovementPreset _movementPreset = MovementPreset.Teleport;
        public MovementPreset movementPreset
        {
            get => _movementPreset;
            set
            {
                _movementPreset = value;

                ApplyCurrentMovementPreset();
            }
        }


        [SerializeField]
        private InteractionOptions _interactionOptions;
        public InteractionOptions interactionOptions
        {
            get => _interactionOptions;
            set
            {
                _interactionOptions = value;

                ApplyCurrentInteractions();
            }
        }


        // Head Gaze controls
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


        [Tooltip("Duration after which HeadGaze reselects an hovered interaction if enabled.")]
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


        [Tooltip("Reference to the HeadGazeReticle of the ExPresS XR Rig.")]
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


        // XR Controllers
        [Tooltip("Reference to the *left* HandController of the ExPresS XR Rig.")]
        [SerializeField]
        private HandControllerManager _leftHandController;
        public HandControllerManager leftHandController
        {
            get => _leftHandController;
            set
            {
                _leftHandController = value;

                ApplyCurrentMovementPreset();
                ApplyCurrentInteractions();
            }
        }

        [Tooltip("Reference to the *right* HandController of the ExPresS XR Rig.")]
        [SerializeField]
        private HandControllerManager _rightHandController;
        public HandControllerManager rightHandController
        {
            get => _rightHandController;
            set
            {
                _rightHandController = value;

                ApplyCurrentMovementPreset();
                ApplyCurrentInteractions();
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

                ApplyCurrentMovementPreset();
                ApplyCurrentInteractions();
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

                ApplyCurrentMovementPreset();
                ApplyCurrentInteractions();
            }
        }


        // Head Collisions
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

        // Component References
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

        // Utility
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

        [Tooltip("Determines how the controllers/hands are rendered in the VR.")]
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


        // Start is called before the first frame update
        void Awake()
        {
            List<XRDisplaySubsystem> displaySubsystems = new();
            SubsystemManager.GetInstances(displaySubsystems);

            if (displaySubsystems.Count > 0)
            {
                displaySubsystems[0].SetPreferredMirrorBlitMode((int)gameTabDisplayMode);
            }
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

        // Config
        public void ApplyCurrentMovementPreset()
        {
            if (_inputMethod != InputMethod.Controller
                && !(_movementPreset == MovementPreset.Teleport 
                        || movementPreset == MovementPreset.None
                        || movementPreset == MovementPreset.Custom))
            {
                Debug.LogWarning("InputPresets other than 'None', 'Teleport', 'Custom' will be ignored with InputMethod set to 'Controller'.");
            }

            if (_movementPreset == MovementPreset.Custom)
            {
                // Do not change anything for custom preset
                return;
            }

            ApplyLeftHandPreset();
            ApplyRightHandPreset();
            ApplyEyeGazePreset();
            ApplyHeadGazePreset();
            ApplyLocomotionSystemPreset();
        }

        private void ApplyLeftHandPreset()
        {
            if (_leftHandController != null)
            {
                _leftHandController.teleportationEnabled = _movementPreset == MovementPreset.Teleport;
                _leftHandController.snapTurnEnabled = _movementPreset == MovementPreset.Teleport;
                _leftHandController.smoothMoveEnabled = _movementPreset == MovementPreset.Joystick
                                                        || movementPreset == MovementPreset.JoystickNoTurn;
                _leftHandController.smoothTurnEnabled = _movementPreset == MovementPreset.JoystickInverse;
                _leftHandController.grabMoveEnabled = _movementPreset == MovementPreset.GrabWorldMotion
                                                    || _movementPreset == MovementPreset.GrabWorldManipulation;
            }
        }

        private void ApplyRightHandPreset()
        {
            if (_rightHandController != null)
            {
                _rightHandController.teleportationEnabled = _movementPreset == MovementPreset.Teleport;
                _rightHandController.snapTurnEnabled = _movementPreset == MovementPreset.Teleport;
                _rightHandController.smoothMoveEnabled = _movementPreset == MovementPreset.JoystickInverse
                                                        || movementPreset == MovementPreset.JoystickNoTurn;
                _rightHandController.smoothTurnEnabled = _movementPreset == MovementPreset.Joystick;
                _rightHandController.grabMoveEnabled = _movementPreset == MovementPreset.GrabWorldMotion
                                                    || _movementPreset == MovementPreset.GrabWorldManipulation;
            }
        }

        private void ApplyEyeGazePreset()
        {
            if (_eyeGazeController != null)
            {
                if (_movementPreset == MovementPreset.Teleport)
                {
                    _eyeGazeController.interactionLayers |= 1 << InteractionLayerMask.NameToLayer("Teleport");
                }
                else
                {
                    _eyeGazeController.interactionLayers &= ~(1 << InteractionLayerMask.NameToLayer("Teleport"));
                }
            }
        }

        private void ApplyHeadGazePreset()
        {
            if (_headGazeController != null)
            {
                _headGazeController.teleportationEnabled = _movementPreset == MovementPreset.Teleport;
            }
        }

        private void ApplyLocomotionSystemPreset()
        {
            if (_locomotionSystem != null)
            {
                if (_locomotionSystem.TryGetComponent(out TwoHandedGrabMoveProvider grabProvider))
                {
                    grabProvider.enabled = _movementPreset == MovementPreset.GrabWorldManipulation;
                }
            }
        }


        // Interactions
        public void ApplyCurrentInteractions()
        {
            if (_leftHandController != null)
            {
                _leftHandController.directInteractionEnabled = _interactionOptions.HasFlag(InteractionOptions.Direct);
                _leftHandController.pokeInteractionEnabled = _interactionOptions.HasFlag(InteractionOptions.Poke);
                _leftHandController.rayInteractionEnabled = _interactionOptions.HasFlag(InteractionOptions.Ray);
                _leftHandController.rayAnchorControlEnabled = _interactionOptions.HasFlag(InteractionOptions.RayAnchorControl);
                _leftHandController.uiRayInteractionEnabled = _interactionOptions.HasFlag(InteractionOptions.UiRay);
                _leftHandController.uiPokeInteractionEnabled = _interactionOptions.HasFlag(InteractionOptions.UiPoke);
                _leftHandController.chooseTeleportForwardEnabled = _interactionOptions.HasFlag(InteractionOptions.ChooseTeleportForward);
            }

            if (_rightHandController != null)
            {
                _rightHandController.directInteractionEnabled = _interactionOptions.HasFlag(InteractionOptions.Direct);
                _rightHandController.pokeInteractionEnabled = _interactionOptions.HasFlag(InteractionOptions.Poke);
                _rightHandController.rayInteractionEnabled = _interactionOptions.HasFlag(InteractionOptions.Ray);
                _rightHandController.rayAnchorControlEnabled = _interactionOptions.HasFlag(InteractionOptions.RayAnchorControl);
                _rightHandController.uiRayInteractionEnabled = _interactionOptions.HasFlag(InteractionOptions.UiRay);
                _rightHandController.uiPokeInteractionEnabled = _interactionOptions.HasFlag(InteractionOptions.UiPoke);
                _rightHandController.chooseTeleportForwardEnabled = _interactionOptions.HasFlag(InteractionOptions.ChooseTeleportForward);
            }
        }

        private void OnValidate()
        {
            ApplyCurrentMovementPreset();
            ApplyCurrentInteractions();
        }

        public void RevalidateInputMethod()
        {
            inputMethod = _inputMethod;
        }
    }


    public enum InputMethod
    {
        None,
        Controller,
        HeadGaze,
        EyeGaze
    }

    public enum MovementPreset
    {
        None,
        Teleport,
        Joystick,
        JoystickInverse,
        JoystickNoTurn,
        GrabWorldMotion,
        GrabWorldManipulation,
        Custom
    }


    [System.Flags]
    public enum InteractionOptions
    {
        None = 0,
        Direct = 1,
        Poke = 2,
        UiPoke = 4,
        Ray = 8,
        RayAnchorControl = 16,
        UiRay = 32,
        ChooseTeleportForward = 64
    }

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