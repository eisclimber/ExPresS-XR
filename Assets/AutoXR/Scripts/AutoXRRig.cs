using System.Collections.Generic;
using UnityEngine;
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
                _playerHeadCollider.enabled = _headCollisionEnabled;
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

    [Tooltip("Should be an Component attached to the Main Camera.")]
    [SerializeField]
    private PlayerHeadCollider _playerHeadCollider;
    public PlayerHeadCollider playerHeadCollider
    {
        get => _playerHeadCollider;
        set => _playerHeadCollider = value;
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


    // Assign all variables to their to properties
    // Or use a custom Editor
    // private void OnValidate()
    // {
    //     inputMethode = _inputMethode;
    //     teleportationEnabled = _teleportationEnabled;
    //     joystickMovementEnabled = _joystickMovementEnabled;
    //     snapTurnEnabled = _snapTurnEnabled;
    //     handModelMode = _handModelMode;
    //     interactHands = _interactHands;
    //     headGazeReticle = _headGazeReticle;
    // }
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