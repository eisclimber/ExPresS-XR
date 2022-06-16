using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

[RequireComponent(typeof(XROrigin))]
[DisallowMultipleComponent]
[AddComponentMenu("AutoXR/Auto XR Rig")]
public class AutoXRRig : MonoBehaviour
{
    [SerializeField]
    private InputMethodeType _inputMethode;
    public InputMethodeType inputMethode
    {
        get => _inputMethode;
        set
        {
            _inputMethode = value;

            _leftHandController.gameObject.SetActive(inputMethode == InputMethodeType.Controller);
            _rightHandController.gameObject.SetActive(inputMethode == InputMethodeType.Controller);

            _headGazeController.gameObject.SetActive(inputMethode == InputMethodeType.HeadGaze);

            if (_headGazeReticle != null)
            {
                _headGazeReticle.gameObject.SetActive(inputMethode == InputMethodeType.HeadGaze);
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

            EnableLocomotionProvider<TeleportationProvider>(_teleportationEnabled);

            _leftHandController.teleportationEnabled = _teleportationEnabled;
            _rightHandController.teleportationEnabled = _teleportationEnabled && (_interactHands & HandCombinations.Left) != 0;
            _headGazeController.teleportationEnabled = _teleportationEnabled && (_interactHands & HandCombinations.Right) != 0;
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

            EnableLocomotionProvider<ActionBasedContinuousMoveProvider>(_joystickMovementEnabled);
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
                _playerHeadCollider.collisionScreenFadeEnabled = _showCollisionVignetteEffect;
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
                _playAreaBoundingBox.enabled = _showPlayAreaBounds;
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

            if (_headGazeController != null) {
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
            if (_headGazeController != null) {
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
        set => _playerHeadCollider = value;
    }


    [Tooltip("A Reference to the PlayAreaBoundingBox of the Rig")]
    [SerializeField]
    private PlayAreaBoundingBox _playAreaBoundingBox;
    public PlayAreaBoundingBox playAreaBoundingBox
    {
        get => _playAreaBoundingBox;
        set => _playAreaBoundingBox = value;
    }

    //////////////

    [SerializeField]
    private Canvas _hud;
    public Canvas hud
    {
        get => _hud;
        set => _hud = value;
    }

    [SerializeField]
    private FadeRect _fadeRect;
    public FadeRect fadeRect
    {
        get => _fadeRect;
        set => _fadeRect = value;
    }


    ///////////
    private void EnableLocomotionProvider<T>(bool enabled) where T : LocomotionProvider
    {
        if (_locomotionSystem != null)
        {
            LocomotionProvider provider = _locomotionSystem.gameObject.GetComponent<T>();
            if (provider != null)
            {
                provider.enabled = enabled;
            }
        }
    }

    private void Awake() {
        List<XRDisplaySubsystem> displaySubsystems = new List<XRDisplaySubsystem>();
        SubsystemManager.GetInstances<XRDisplaySubsystem>(displaySubsystems);

        if (displaySubsystems.Count > 0)
        {
            displaySubsystems[0].SetPreferredMirrorBlitMode(XRMirrorViewBlitMode.SideBySide);
        }
    }
}

public enum InputMethodeType
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