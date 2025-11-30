using System.Collections.Generic;
using Unity.XR.CoreUtils.Bindings;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;

/// <summary>
/// Copy-Pasta from the unity starter assets with slight adjustments to allow inheritance.
/// </summary>
namespace ExPresSXR.Rig
{
    /// <summary>
    /// Use this class to mediate the interactors for a controller under different interaction states
    /// and the input actions used by them.
    /// </summary>
    /// <remarks>
    /// If the teleport ray input is engaged, the Ray Interactor used for distant manipulation is disabled
    /// and the Ray Interactor used for teleportation is enabled. If the Ray Interactor is selecting and it
    /// is configured to allow for attach transform manipulation, all locomotion input actions are disabled
    /// (teleport ray, move, and turn controls) to prevent input collision with the manipulation inputs used
    /// by the ray interactor.
    /// <br />
    /// A typical hierarchy also includes an XR Interaction Group component to mediate between interactors.
    /// The interaction group ensures that the Direct and Ray Interactors cannot interact at the same time,
    /// with the Direct Interactor taking priority over the Ray Interactor.
    /// </remarks>
    [AddComponentMenu("XR/Controller Input Action Manager Base (ExPresS XR)")]
    public class ControllerInputActionManagerBase : MonoBehaviour
    {
        // [Space]
        // [Header("Interactors")]

        [SerializeField]
        [Tooltip("The interactor used for distant/ray manipulation. Use this or Near-Far Interactor, not both.")]
        protected XRRayInteractor _rayInteractor;

        [SerializeField]
        [Tooltip("Near-Far Interactor used for distant/ray manipulation. Use this or Ray Interactor, not both.")]
        protected NearFarInteractor _nearFarInteractor;

        [SerializeField]
        [Tooltip("The interactor used for teleportation.")]
        protected XRRayInteractor _teleportInteractor;

        // [Space]
        // [Header("Controller Actions")]

        [SerializeField]
        [Tooltip("The reference to the action to start the teleport aiming mode for this controller.")]
        protected InputActionReference _teleportMode;

        [SerializeField]
        [Tooltip("The reference to the action to cancel the teleport aiming mode for this controller.")]
        protected InputActionReference _teleportModeCancel;

        [SerializeField]
        [Tooltip("The reference to the action of continuous turning the XR Origin with this controller.")]
        protected InputActionReference _turn;

        [SerializeField]
        [Tooltip("The reference to the action of snap turning the XR Origin with this controller.")]
        protected InputActionReference _snapTurn;

        [SerializeField]
        [Tooltip("The reference to the action of moving the XR Origin with this controller.")]
        protected InputActionReference _move;

        [SerializeField]
        [Tooltip("The reference to the action of scrolling UI with this controller.")]
        protected InputActionReference _uIScroll;

        // [Space]
        // [Header("Locomotion Settings")]

        [SerializeField]
        [Tooltip("If true, continuous movement will be enabled. If false, teleport will be enabled.")]
        protected bool _smoothMotionEnabled;

        [SerializeField]
        [Tooltip("If true, continuous turn will be enabled. If false, snap turn will be enabled. Note: If smooth motion is enabled and enable strafe is enabled on the continuous move provider, turn will be overriden in favor of strafe.")]
        protected bool _smoothTurnEnabled;

        [SerializeField]
        [Tooltip("With the Near-Far Interactor, if true, teleport will be enabled during near interaction. If false, teleport will be disabled during near interaction.")]
        protected bool _nearFarEnableTeleportDuringNearInteraction = true;

        // [Space]
        // [Header("UI Settings")]

        [SerializeField]
        [Tooltip("If true, UI scrolling will be enabled. Locomotion will be disabled when pointing at UI to allow it to be scrolled.")]
        protected bool _uiScrollingEnabled = true;

        // [Space]
        // [Header("Mediation Events")]

        [SerializeField]
        [Tooltip("Event fired when the active ray interactor changes between interaction and teleport.")]
        public UnityEvent<IXRRayProvider> _rayInteractorChanged;

        public bool SmoothMotionEnabled
        {
            get => _smoothMotionEnabled;
            set
            {
                _smoothMotionEnabled = value;
                UpdateLocomotionActions();
            }
        }

        public bool SmoothTurnEnabled
        {
            get => _smoothTurnEnabled;
            set
            {
                _smoothTurnEnabled = value;
                UpdateLocomotionActions();
            }
        }

        public bool NearFarEnableTeleportDuringNearInteraction
        {
            get => _nearFarEnableTeleportDuringNearInteraction;
            set => _nearFarEnableTeleportDuringNearInteraction = value;
        }

        public bool UiScrollingEnabled
        {
            get => _uiScrollingEnabled;
            set
            {
                _uiScrollingEnabled = value;
                UpdateUIActions();
            }
        }

        private bool _startCalled;
        private bool _postponedDeactivateTeleport;
        private bool _postponedNearRegionLocomotion;
        private bool _hoveringScrollableUI;

        protected readonly HashSet<InputAction> _locomotionUsers = new();
        protected readonly BindingsGroup _bindingsGroup = new();

        protected void SetupInteractorEvents()
        {
            if (_nearFarInteractor != null)
            {
                _nearFarInteractor.uiHoverEntered.AddListener(OnUIHoverEntered);
                _nearFarInteractor.uiHoverExited.AddListener(OnUIHoverExited);
                _bindingsGroup.AddBinding(_nearFarInteractor.selectionRegion.Subscribe(OnNearFarSelectionRegionChanged));
            }

            if (_rayInteractor != null)
            {
                _rayInteractor.selectEntered.AddListener(OnRaySelectEntered);
                _rayInteractor.selectExited.AddListener(OnRaySelectExited);
                _rayInteractor.uiHoverEntered.AddListener(OnUIHoverEntered);
                _rayInteractor.uiHoverExited.AddListener(OnUIHoverExited);
            }

            var teleportModeAction = GetInputAction(_teleportMode);
            if (teleportModeAction != null)
            {
                teleportModeAction.performed += OnStartTeleport;
                teleportModeAction.performed += OnStartLocomotion;
                teleportModeAction.canceled += OnCancelTeleport;
                teleportModeAction.canceled += OnStopLocomotion;
            }

            var teleportModeCancelAction = GetInputAction(_teleportModeCancel);
            if (teleportModeCancelAction != null)
            {
                teleportModeCancelAction.performed += OnCancelTeleport;
            }

            var moveAction = GetInputAction(_move);
            if (moveAction != null)
            {
                moveAction.started += OnStartLocomotion;
                moveAction.canceled += OnStopLocomotion;
            }

            var turnAction = GetInputAction(_turn);
            if (turnAction != null)
            {
                turnAction.started += OnStartLocomotion;
                turnAction.canceled += OnStopLocomotion;
            }

            var snapTurnAction = GetInputAction(_snapTurn);
            if (snapTurnAction != null)
            {
                snapTurnAction.started += OnStartLocomotion;
                snapTurnAction.canceled += OnStopLocomotion;
            }
        }

        protected void TeardownInteractorEvents()
        {
            _bindingsGroup.Clear();

            if (_nearFarInteractor != null)
            {
                _nearFarInteractor.uiHoverEntered.RemoveListener(OnUIHoverEntered);
                _nearFarInteractor.uiHoverExited.RemoveListener(OnUIHoverExited);
            }

            if (_rayInteractor != null)
            {
                _rayInteractor.selectEntered.RemoveListener(OnRaySelectEntered);
                _rayInteractor.selectExited.RemoveListener(OnRaySelectExited);
                _rayInteractor.uiHoverEntered.RemoveListener(OnUIHoverEntered);
                _rayInteractor.uiHoverExited.RemoveListener(OnUIHoverExited);
            }

            var teleportModeAction = GetInputAction(_teleportMode);
            if (teleportModeAction != null)
            {
                teleportModeAction.performed -= OnStartTeleport;
                teleportModeAction.performed -= OnStartLocomotion;
                teleportModeAction.canceled -= OnCancelTeleport;
                teleportModeAction.canceled -= OnStopLocomotion;
            }

            var teleportModeCancelAction = GetInputAction(_teleportModeCancel);
            if (teleportModeCancelAction != null)
            {
                teleportModeCancelAction.performed -= OnCancelTeleport;
            }

            var moveAction = GetInputAction(_move);
            if (moveAction != null)
            {
                moveAction.started -= OnStartLocomotion;
                moveAction.canceled -= OnStopLocomotion;
            }

            var turnAction = GetInputAction(_turn);
            if (turnAction != null)
            {
                turnAction.started -= OnStartLocomotion;
                turnAction.canceled -= OnStopLocomotion;
            }

            var snapTurnAction = GetInputAction(_snapTurn);
            if (snapTurnAction != null)
            {
                snapTurnAction.started -= OnStartLocomotion;
                snapTurnAction.canceled -= OnStopLocomotion;
            }
        }

        protected void OnStartTeleport(InputAction.CallbackContext context)
        {
            _postponedDeactivateTeleport = false;

            if (_teleportInteractor != null)
                _teleportInteractor.gameObject.SetActive(true);

            if (_rayInteractor != null)
                _rayInteractor.gameObject.SetActive(false);

            if (_nearFarInteractor != null && _nearFarInteractor.selectionRegion.Value != NearFarInteractor.Region.Near)
                _nearFarInteractor.gameObject.SetActive(false);

            _rayInteractorChanged?.Invoke(_teleportInteractor);
        }

        protected void OnCancelTeleport(InputAction.CallbackContext context)
        {
            // Do not deactivate the teleport interactor in this callback.
            // We delay turning off the teleport interactor in this callback so that
            // the teleport interactor has a chance to complete the teleport if needed.
            // OnAfterInteractionEvents will handle deactivating its GameObject.
            _postponedDeactivateTeleport = true;

            if (_rayInteractor != null)
                _rayInteractor.gameObject.SetActive(true);

            if (_nearFarInteractor != null)
                _nearFarInteractor.gameObject.SetActive(true);

            _rayInteractorChanged?.Invoke(_rayInteractor);
        }

        protected void OnStartLocomotion(InputAction.CallbackContext context)
        {
            _locomotionUsers.Add(context.action);
        }

        protected void OnStopLocomotion(InputAction.CallbackContext context)
        {
            _locomotionUsers.Remove(context.action);

            if (_locomotionUsers.Count == 0 && _hoveringScrollableUI)
            {
                DisableAllLocomotionActions();
                UpdateUIActions();
            }
        }

        protected void OnNearFarSelectionRegionChanged(NearFarInteractor.Region selectionRegion)
        {
            _postponedNearRegionLocomotion = false;

            if (selectionRegion == NearFarInteractor.Region.None)
            {
                UpdateLocomotionActions();
                return;
            }

            var manipulateAttachTransform = false;
            var attachController = _nearFarInteractor.interactionAttachController as InteractionAttachController;
            if (attachController != null)
            {
                manipulateAttachTransform = attachController.useManipulationInput &&
                    (attachController.manipulationInput.inputSourceMode == XRInputValueReader.InputSourceMode.InputActionReference && attachController.manipulationInput.inputActionReference != null) ||
                    (attachController.manipulationInput.inputSourceMode != XRInputValueReader.InputSourceMode.InputActionReference && attachController.manipulationInput.inputSourceMode != XRInputValueReader.InputSourceMode.Unused);
            }

            if (selectionRegion == NearFarInteractor.Region.Far)
            {
                if (manipulateAttachTransform)
                    DisableAllLocomotionActions();
                else
                    DisableTeleportActions();
            }
            else if (selectionRegion == NearFarInteractor.Region.Near)
            {
                // Determine if the user entered the near region due to pulling back on the thumbstick.
                // If so, postpone enabling locomotion until the user releases the thumbstick
                // in order to avoid an immediate snap turn around from triggering on region change.
                var hasStickInput = manipulateAttachTransform && HasStickInput(attachController);
                if (hasStickInput)
                {
                    _postponedNearRegionLocomotion = true;
                    DisableAllLocomotionActions();
                }
                else
                {
                    UpdateLocomotionActions();
                    if (!_nearFarEnableTeleportDuringNearInteraction)
                        DisableTeleportActions();
                }
            }
        }

        protected void OnRaySelectEntered(SelectEnterEventArgs args)
        {
            if (_rayInteractor.manipulateAttachTransform)
            {
                // Disable locomotion and turn actions
                DisableAllLocomotionActions();
            }
        }

        protected void OnRaySelectExited(SelectExitEventArgs args)
        {
            if (_rayInteractor.manipulateAttachTransform)
            {
                // Re-enable the locomotion and turn actions
                UpdateLocomotionActions();
            }
        }

        protected void OnUIHoverEntered(UIHoverEventArgs args)
        {
            _hoveringScrollableUI = _uiScrollingEnabled && args.deviceModel.isScrollable;
            UpdateUIActions();

            // If locomotion is occurring, wait
            if (_hoveringScrollableUI && _locomotionUsers.Count == 0)
            {
                // Disable locomotion and turn actions
                DisableAllLocomotionActions();
            }
        }

        void OnUIHoverExited(UIHoverEventArgs args)
        {
            _hoveringScrollableUI = false;
            UpdateUIActions();

            // Re-enable the locomotion and turn actions
            UpdateLocomotionActions();
        }

        protected void OnEnable()
        {
            if (_rayInteractor != null && _nearFarInteractor != null)
            {
                Debug.LogWarning("Both Ray Interactor and Near-Far Interactor are assigned. Only one should be assigned, not both. Clearing Ray Interactor.", this);
                _rayInteractor = null;
            }

            if (_teleportInteractor != null)
                _teleportInteractor.gameObject.SetActive(false);

            // Allow the actions to be refreshed when this component is re-enabled.
            // See comments in Start for why we wait until Start to enable/disable actions.
            if (_startCalled)
            {
                UpdateLocomotionActions();
                UpdateUIActions();
            }

            SetupInteractorEvents();
        }

        protected void OnDisable()
        {
            TeardownInteractorEvents();
        }

        protected void Start()
        {
            _startCalled = true;

            // Ensure the enabled state of locomotion and turn actions are properly set up.
            // Called in Start so it is done after the InputActionManager enables all input actions earlier in OnEnable.
            UpdateLocomotionActions();
            UpdateUIActions();
        }

        protected void Update()
        {
            // Since this behavior has the default execution order, it runs after the XRInteractionManager,
            // so selection events have been finished by now this frame. This means that the teleport interactor
            // has had a chance to process its select interaction event and teleport if needed.
            if (_postponedDeactivateTeleport)
            {
                if (_teleportInteractor != null)
                    _teleportInteractor.gameObject.SetActive(false);

                _postponedDeactivateTeleport = false;
            }

            // If stick input caused the near region to be entered,
            // wait until the stick is released before enabling locomotion.
            if (_postponedNearRegionLocomotion)
            {
                var hasStickInput = false;
                if (_nearFarInteractor != null &&
                    _nearFarInteractor.interactionAttachController is InteractionAttachController attachController
                    && attachController != null)
                {
                    hasStickInput = HasStickInput(attachController);
                }

                if (!hasStickInput)
                {
                    _postponedNearRegionLocomotion = false;

                    UpdateLocomotionActions();
                    if (!_nearFarEnableTeleportDuringNearInteraction)
                        DisableTeleportActions();
                }
            }
        }

        protected void UpdateLocomotionActions()
        {
            // Disable/enable Teleport and Turn when Move is enabled/disabled.
            SetEnabled(_move, _smoothMotionEnabled);
            SetEnabled(_teleportMode, !_smoothMotionEnabled);
            SetEnabled(_teleportModeCancel, !_smoothMotionEnabled);

            // Disable ability to turn when using continuous movement
            SetEnabled(_turn, !_smoothMotionEnabled && _smoothTurnEnabled);
            SetEnabled(_snapTurn, !_smoothMotionEnabled && !_smoothTurnEnabled);
        }

        protected void DisableTeleportActions()
        {
            DisableAction(_teleportMode);
            DisableAction(_teleportModeCancel);
        }

        protected void DisableMoveAndTurnActions()
        {
            DisableAction(_move);
            DisableAction(_turn);
            DisableAction(_snapTurn);
        }

        protected void DisableAllLocomotionActions()
        {
            DisableTeleportActions();
            DisableMoveAndTurnActions();
        }

        protected void UpdateUIActions()
        {
            SetEnabled(_uIScroll, _uiScrollingEnabled && _hoveringScrollableUI && _locomotionUsers.Count == 0);
        }

        protected static bool HasStickInput(InteractionAttachController attachController)
        {
            // 75% of default 0.5 press threshold
            const float sqrStickReleaseThreshold = 0.375f * 0.375f;

            return attachController.manipulationInput.TryReadValue(out var stickInput) &&
                stickInput.sqrMagnitude > sqrStickReleaseThreshold;
        }

        protected static void SetEnabled(InputActionReference actionReference, bool enabled)
        {
            if (enabled)
                EnableAction(actionReference);
            else
                DisableAction(actionReference);
        }

        protected static void EnableAction(InputActionReference actionReference)
        {
            var action = GetInputAction(actionReference);
            action?.Enable();
        }

        protected static void DisableAction(InputActionReference actionReference)
        {
            var action = GetInputAction(actionReference);
            action?.Disable();
        }

        protected static InputAction GetInputAction(InputActionReference actionReference)
        {
#pragma warning disable IDE0031 // Use null propagation -- Do not use for UnityEngine.Object types
            return actionReference != null ? actionReference.action : null;
#pragma warning restore IDE0031
        }
    }
}
