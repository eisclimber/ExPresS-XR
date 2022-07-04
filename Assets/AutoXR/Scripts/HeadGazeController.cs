using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Controls;

public class HeadGazeController : MonoBehaviour
{
    [Tooltip("Wether or not the head gaze can be used to teleport.")]
    [SerializeField]
    private bool _teleportationEnabled = true;
    public bool teleportationEnabled
    {
        get => _teleportationEnabled;
        set
        {
            _teleportationEnabled = value;

            if (_teleportationEnabled)
            {
                _rayInteractor.interactionLayers |= (1 << InteractionLayerMask.NameToLayer("Teleportation"));
            }
            else
            {
                _rayInteractor.interactionLayers = ~(1 << InteractionLayerMask.NameToLayer("Teleportation"));
            }
        }
    }

    [Tooltip("Wether or not multiple interactions can be performed when keeping the focus on an interactable.")]
    [SerializeField]
    private bool _canReselect = true;
    public bool canReselect
    {
        get => _canReselect;
        set => _canReselect = value;
    }

    [Tooltip("The time in seconds needed to focus the head gaze to interact.")]
    [SerializeField]
    private float _timeToSelect = 1.0f;

    [Tooltip("The time in seconds that the reticle will not show after interacting. Only takes effect if reselect is enabled.")]
    [SerializeField]
    private float _postInteractionCooldown = 0.2f;

    [Tooltip("The time in seconds the reticle will not show after heavy head movement. The intensity-threshold can be set by changing the 'Head/Head Gaze Prevent Interaction'-Input-Mapping.")]
    [SerializeField]
    private float _timeInteractionPrevented = 0.2f;

    public float timeToSelect
    {
        get => _timeToSelect;
        set
        {
            _timeToSelect = value;

            if (_headGazeReticle != null)
            {
                _headGazeReticle.hintDuration = _timeToSelect;
            }
        }
    }


    [SerializeField]
    private HeadGazeReticle _headGazeReticle;
    public HeadGazeReticle headGazeReticle
    {
        get => _headGazeReticle;
        set => _headGazeReticle = value;
    }

    [SerializeField]
    private XRRayInteractor _rayInteractor;


    // Should be an input interaction that is used for performing any interaction
    public InputActionReference performInteractionReference;


    // Should be an input action that is used for canceling

    public InputActionReference preventInteractionReference;


    private Mouse fakeInputDevice;

    private GameObject hoverTarget;
    private float timeLeftTillSelect;
    private float timeLeftTillHoverBlocked;

    private bool hoverTimePassed;



    private void Awake()
    {
        preventInteractionReference.action.performed += TeleportModeReset;
        if (_headGazeReticle != null)
        {
            _headGazeReticle.hintDuration = _timeToSelect;
            TryHideReticle();
        }
        // Add a fake mouse that will be used to trigger the action (by pressing mouse_forward)
        fakeInputDevice = InputSystem.AddDevice<Mouse>();
    }

    private void Update()
    {
        GameObject newTarget = TryGetNewTarget();

        if (newTarget != hoverTarget)
        {
            // New Target Found
            hoverTarget = newTarget;
            timeLeftTillSelect = timeToSelect;
            hoverTimePassed = false;

            if (hoverTarget != null)
            {
                TryShowReticle();
            }
            else
            {
                timeLeftTillHoverBlocked = 0.0f;
                TryHideReticle();
            }
        }
        else if (newTarget != null && timeLeftTillHoverBlocked > 0.0f)
        {
            // Timeout after canceling
            timeLeftTillHoverBlocked = Mathf.Max(timeLeftTillHoverBlocked - Time.deltaTime, 0.0f);

            if (timeLeftTillHoverBlocked <= 0.0f)
            {
                TryShowReticle();
            }
        }
        else if (newTarget != null && !hoverTimePassed)
        {
            // Wait till hovered long enough
            timeLeftTillSelect = Mathf.Max(timeLeftTillSelect - Time.deltaTime, 0.0f);
            if (timeLeftTillSelect <= 0.0f)
            {
                hoverTimePassed = true;
                PerformFakeButtonPress();
            }
        }
    }


    private GameObject TryGetNewTarget()
    {
        if (_rayInteractor.TryGetCurrentRaycast(
                out var raycastHit,
                out var raycastHitIndex,
                out var uiRaycastResult,
                out var uiRaycastHitIndex,
                out var isUIHitClosest))
        {
            if (uiRaycastResult.HasValue && isUIHitClosest)
            {
                // UI hit or Interactor Hit
                return uiRaycastResult.Value.gameObject;
            }
            else if (raycastHit.HasValue)
            {
                // Interactor Hit
                return raycastHit.Value.transform.gameObject;
            }
        }
        return null;
    }


    private void TeleportModeReset(InputAction.CallbackContext callback)
    {
        if (hoverTarget != null && !hoverTimePassed)
        {
            // Reset Hovertimer and set cooldown
            timeLeftTillHoverBlocked = _timeInteractionPrevented;
            timeLeftTillSelect = timeToSelect;

            TryHideReticle();
        }
    }

    private void PerformFakeButtonPress()
    {
        using (StateEvent.From(fakeInputDevice, out var eventPtr))
        {
            ((ButtonControl)fakeInputDevice.forwardButton).WriteValueIntoEvent(1.0f, eventPtr);
            InputSystem.QueueEvent(eventPtr);
            // Release button press after a short while
            StartCoroutine(ReleaseButtonPress());
        }
    }

    private IEnumerator ReleaseButtonPress()
    {
        yield return new WaitForSeconds(0.05f);
        using (StateEvent.From(fakeInputDevice, out var eventPtr))
        {
            ((ButtonControl)fakeInputDevice.forwardButton).WriteValueIntoEvent(0.0f, eventPtr);
            InputSystem.QueueEvent(eventPtr);
        }

        TryHideReticle();

        if (_canReselect)
        {
            timeLeftTillHoverBlocked = _postInteractionCooldown;
            timeLeftTillSelect = timeToSelect;
            hoverTimePassed = false;
        }
    }

    private void TryShowReticle()
    {
        if (_headGazeReticle != null)
        {
            _headGazeReticle.ShowHint();
        }
    }

    private void TryHideReticle()
    {
        if (_headGazeReticle != null)
        {
            _headGazeReticle.HideHint();
        }
    }


    // Allows in-editor changes
    private void OnValidate()
    {
        teleportationEnabled = _teleportationEnabled;
    }
}
