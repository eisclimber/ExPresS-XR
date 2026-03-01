using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using ExPresSXR.UI;
using ExPresSXR.Rig.HeadGazeInputDevice;


namespace ExPresSXR.Rig
{
    /// <summary>
    /// Wraps an `XRRayInteractor` to be able to interact within the VR without controllers only by focusing the head's direction on an interactable object.  
    /// 
    /// It is recommended to only use UI and teleportation interactions with this controller as there is no way of physically interacting with or holding onto objects.
    /// 
    /// Interactions are performed by focusing the head on an interactable object for the duration of `timeToSelect`. During this time the `HeadGazeReticle` will be shown to give a visual indication of the existence and progress of an interaction.
    /// To cancel or reset an active interaction the head can be moved which will also hide the reticle.
    /// 
    /// Unity's input system requires an input device to create a `InputAction` that can be interpreted as a select action for the `Action Base XR Origin`.
    /// This is done via a "fake" InputDevice the "HeadGazeInputDevice" and the "HeadGazeSelect"-InputAction. For testing purposes, the InputAction also supports triggering it via the Mouse Forward/Button 4 and the enter key on your keyboard. For this to work you'll need to have your game focussed and a mouse/keyboard must be connected. So this option might not be available with your setup.
    /// </summary>
    public class HeadGazeController : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Whether or not the head gaze can be used to teleport.")]
        private bool _TeleportationEnabled = true;
        /// <summary>
        /// Whether or not the head gaze can be used to teleport.
        /// </summary>
        public bool TeleportationEnabled
        {
            get => _TeleportationEnabled;
            set
            {
                _TeleportationEnabled = value;

                if (_TeleportationEnabled)
                {
                    _rayInteractor.interactionLayers |= 1 << InteractionLayerMask.NameToLayer("Teleport");
                }
                else
                {
                    _rayInteractor.interactionLayers &= ~(1 << InteractionLayerMask.NameToLayer("Teleport"));
                }
            }
        }

        [SerializeField]
        [Tooltip("Whether or not multiple interactions can be performed when keeping the focus on an interactable.")]
        private bool _canReselect = true;
        /// <summary>
        /// Whether or not multiple interactions can be performed when keeping the focus on an interactable.
        /// </summary>
        public bool CanReselect
        {
            get => _canReselect;
            set => _canReselect = value;
        }

        [SerializeField]
        [Tooltip("The time in seconds needed to focus the head gaze to interact.")]
        private float _timeToSelect = 1.0f;
        /// <summary>
        /// The time in seconds needed to focus the head gaze to interact.
        /// </summary>
        public float TimeToSelect
        {
            get => _timeToSelect;
            set
            {
                _timeToSelect = value;

                if (_headGazeReticle != null)
                {
                    _headGazeReticle.HintDuration = _timeToSelect;
                }
            }
        }

        /// <summary>
        /// The time in seconds that the reticle will not show after interacting. Only takes effect if reselect is enabled.
        /// </summary>
        [SerializeField]
        [Tooltip("The time in seconds that the reticle will not show after interacting. Only takes effect if reselect is enabled.")]
        private float _postInteractionCooldown = 0.5f;

        /// <summary>
        /// The time in seconds the reticle will not show after heavy head movement.
        /// The intensity-threshold can be set by changing the 'Head/Head Gaze Prevent Interaction'-Input-Mapping.
        /// </summary>
        [SerializeField]
        [Tooltip("The time in seconds the reticle will not show after heavy head movement. \nThe intensity-threshold can be set by changing the 'Head/Head Gaze Prevent Interaction'-Input-Mapping.")]
        private float _timeInteractionPrevented = 0.2f;

        [SerializeField]
        [Tooltip("The reticle used to indicate head gaze interactions.")]
        private HeadGazeReticle _headGazeReticle;
        /// <summary>
        /// The reticle used to indicate head gaze interactions.
        /// </summary>
        public HeadGazeReticle HeadGazeReticle
        {
            get => _headGazeReticle;
            set => _headGazeReticle = value;
        }

        /// <summary>
        /// The XRRayInteractor that is used to perform the head gaze interactions.
        /// </summary>
        [SerializeField]
        [Tooltip("The XRRayInteractor that is used to perform the head gaze interactions.")]
        private XRRayInteractor _rayInteractor;


        /// <summary>
        /// Should be an input interaction that is used for performing any interaction
        /// </summary>
        public InputActionReference PerformInteractionReference;


        /// <summary>
        /// Should be an input action that is used for canceling any interaction
        /// </summary>
        public InputActionReference PreventInteractionReference;


        private HeadGazeDevice _headGazeDevice;

        private GameObject _hoverTarget;
        private float _timeLeftTillSelect;
        private float _timeLeftTillHoverBlocked;

        private bool _hoverTimePassed;


        private void Awake()
        {
            TeleportationEnabled = _TeleportationEnabled;
            PreventInteractionReference.action.performed += TeleportModeReset;

            if (_rayInteractor == null && !TryGetComponent(out _rayInteractor))
            {
                Debug.Log("Could not find a RayInteractor-Component. Provide one for HeadGaze to work!");
            }

            if (_headGazeReticle != null)
            {
                _headGazeReticle.HintDuration = _timeToSelect;
                TryHideReticle();
            }
        }

        private void OnEnable()
        {
            // Add a fake mouse that will be used to trigger the action (by pressing mouse_forward)
            _headGazeDevice = InputSystem.AddDevice<HeadGazeDevice>();
            _headGazeDevice.MakeCurrent();
        }

        private void OnDisable()
        {
            // Add a fake mouse that will be used to trigger the action (by pressing mouse_forward)
            InputSystem.RemoveDevice(_headGazeDevice);
        }

        private void FixedUpdate()
        {
            GameObject newTarget = TryGetNewTarget();

            if (newTarget != _hoverTarget)
            {
                // New Target Found
                _hoverTarget = newTarget;
                _timeLeftTillSelect = _timeToSelect;
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
                    out var _,
                    out var uiRaycastResult,
                    out _,
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
                    bool ignoreTeleportation = !TeleportationEnabled
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
                _timeLeftTillSelect = _timeToSelect;

                TryHideReticle();
            }
        }

        private void PerformFakeButtonPress()
        {
            _headGazeDevice.SetHeadGazeSelectPressed();
            // Release button press after a short while
            StartCoroutine(ReleaseButtonPress());
        }

        private IEnumerator ReleaseButtonPress()
        {
            yield return new WaitForSeconds(0.05f);
            _headGazeDevice.SetHeadGazeSelectReleased();
            using (StateEvent.From(_headGazeDevice, out var eventPtr))
            {
                _headGazeDevice.HeadGazeSelect.WriteValueIntoEvent(0.0f, eventPtr);
                InputSystem.QueueEvent(eventPtr);
            }

            TryHideReticle();

            if (_canReselect)
            {
                _timeLeftTillHoverBlocked = _postInteractionCooldown;
                _timeLeftTillSelect = _timeToSelect;
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
    }
}