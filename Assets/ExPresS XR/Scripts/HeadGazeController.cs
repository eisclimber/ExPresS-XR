using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Controls;
using ExPresSXR.UI;


namespace ExPresSXR.Rig
{
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
                    _rayInteractor.interactionLayers |= 1 << InteractionLayerMask.NameToLayer("Teleport");
                }
                else
                {
                    _rayInteractor.interactionLayers &= ~(1 << InteractionLayerMask.NameToLayer("Teleport"));
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

        [Tooltip("The time in seconds that the reticle will not show after interacting. Only takes effect if reselect is enabled.")]
        [SerializeField]
        private float _postInteractionCooldown = 0.5f;

        [Tooltip("The time in seconds the reticle will not show after heavy head movement. The intensity-threshold can be set by changing the 'Head/Head Gaze Prevent Interaction'-Input-Mapping.")]
        [SerializeField]
        private float _timeInteractionPrevented = 0.2f;


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


        private Mouse _fakeInputDevice;

        private GameObject _hoverTarget;
        private float _timeLeftTillSelect;
        private float _timeLeftTillHoverBlocked;

        private bool _hoverTimePassed;



        private void Awake()
        {
            teleportationEnabled = _teleportationEnabled;
            preventInteractionReference.action.performed += TeleportModeReset;
            if (_headGazeReticle != null)
            {
                _headGazeReticle.hintDuration = _timeToSelect;
                TryHideReticle();
            }
            // Add a fake mouse that will be used to trigger the action (by pressing mouse_forward)
            _fakeInputDevice = InputSystem.AddDevice<Mouse>();
        }

        private void Update()
        {
            GameObject newTarget = TryGetNewTarget();

            if (newTarget != _hoverTarget)
            {
                // New Target Found
                _hoverTarget = newTarget;
                _timeLeftTillSelect = timeToSelect;
                _hoverTimePassed = false;

                if (_hoverTarget != null)
                {
                    TryShowReticle();
                }
                else
                {
                    _timeLeftTillHoverBlocked = 0.0f;
                    TryHideReticle();
                }
            }
            else if (newTarget != null && _timeLeftTillHoverBlocked > 0.0f)
            {
                // Timeout after canceling
                _timeLeftTillHoverBlocked = Mathf.Max(_timeLeftTillHoverBlocked - Time.deltaTime, 0.0f);

                if (_timeLeftTillHoverBlocked <= 0.0f)
                {
                    TryShowReticle();
                }
            }
            else if (newTarget != null && !_hoverTimePassed)
            {
                // Wait till hovered long enough
                _timeLeftTillSelect = Mathf.Max(_timeLeftTillSelect - Time.deltaTime, 0.0f);
                if (_timeLeftTillSelect <= 0.0f)
                {
                    _hoverTimePassed = true;
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
                if (uiRaycastResult != null && uiRaycastResult.HasValue && isUIHitClosest)
                {
                    // UI hit or Interactor Hit
                    return uiRaycastResult.Value.gameObject;
                }
                else if (raycastHit != null && raycastHit.HasValue)
                {
                    bool hasInteractor = raycastHit.Value.transform.GetComponent<IXRInteractable>() != null;
                    bool ignoreTeleportation = !teleportationEnabled 
                        && (raycastHit.Value.transform.GetComponent<TeleportationAnchor>()
                            || raycastHit.Value.transform.GetComponent<TeleportationArea>());
                    // Any Hit (might be not an XRInteractor though)
                    if (hasInteractor && !ignoreTeleportation) 
                    {
                        // Interactor Hit
                        return raycastHit.Value.transform.gameObject;
                    }
                }
            }
            return null;
        }


        private void TeleportModeReset(InputAction.CallbackContext callback)
        {
            if (_hoverTarget != null && !_hoverTimePassed)
            {
                // Reset hover timer and set cooldown
                _timeLeftTillHoverBlocked = _timeInteractionPrevented;
                _timeLeftTillSelect = timeToSelect;

                TryHideReticle();
            }
        }

        private void PerformFakeButtonPress()
        {
            using (StateEvent.From(_fakeInputDevice, out var eventPtr))
            {
                ((ButtonControl)_fakeInputDevice.forwardButton).WriteValueIntoEvent(1.0f, eventPtr);
                InputSystem.QueueEvent(eventPtr);
                // Release button press after a short while
                StartCoroutine(ReleaseButtonPress());
            }
        }

        private IEnumerator ReleaseButtonPress()
        {
            yield return new WaitForSeconds(0.05f);
            using (StateEvent.From(_fakeInputDevice, out var eventPtr))
            {
                ((ButtonControl)_fakeInputDevice.forwardButton).WriteValueIntoEvent(0.0f, eventPtr);
                InputSystem.QueueEvent(eventPtr);
            }

            TryHideReticle();

            if (_canReselect)
            {
                _timeLeftTillHoverBlocked = _postInteractionCooldown;
                _timeLeftTillSelect = timeToSelect;
                _hoverTimePassed = false;
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


        // // Allows in-editor changes
        // private void OnValidate()
        // {
        //     teleportationEnabled = _teleportationEnabled;
        // }
    }
}