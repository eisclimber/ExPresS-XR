using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;


namespace ExPresSXR.Rig
{
    [AddComponentMenu("ExPresS XR/Hand Controller")]
    public class HandControllerManager : ControllerManagerBase
    {
        #region Movement Configuration
        /// <summary>
        /// Whether or not teleportation is enabled.
        /// </summary>
        [SerializeField]
        private bool _teleportationEnabled;
        public bool teleportationEnabled
        {
            get => _teleportationEnabled;
            set
            {
                _teleportationEnabled = value;
                UpdateLocomotionActions();
            }
        }

        /// <summary>
        /// Whether or not teleportation can be canceled with the configured InputAction (usually the Grab-Input).
        /// </summary>
        [SerializeField]
        private bool _teleportCancelEnabled;
        public bool teleportCancelEnabled
        {
            get => _teleportCancelEnabled;
            set
            {
                _teleportCancelEnabled = value;
                UpdateLocomotionActions();
            }
        }

        /// <summary>
        /// Whether or not the forwards direction after teleporting can be chosen when rotating the joystick.
        /// The TeleportationAreas must have `matchDirectionalInput` enabled for it to work.
        /// </summary>
        [SerializeField]
        private bool _chooseTeleportForwardEnabled;
        public bool chooseTeleportForwardEnabled
        {
            get => chooseTeleportForwardEnabled;
            set
            {
                _chooseTeleportForwardEnabled = value;

                if (m_TeleportInteractor != null)
                {
                    m_TeleportInteractor.allowAnchorControl = _chooseTeleportForwardEnabled;
                }
            }
        }

        /// <summary>
        /// Whether or not smooth movement (with the joystick) is enabled.
        /// Overrides SmoothTurn and Teleportation.
        /// </summary>
        [SerializeField]
        private bool _smoothMoveEnabled;
        public bool smoothMoveEnabled
        {
            get => _smoothMoveEnabled;
            set
            {
                _smoothMoveEnabled = value;
                UpdateLocomotionActions();
            }
        }

        /// <summary>
        /// Whether or not smooth turn (with the joystick) is enabled.
        /// Overrides Teleportation.
        /// </summary>
        [SerializeField]
        private bool _smoothTurnEnabled;
        public bool smoothTurnEnabled
        {
            get => _smoothTurnEnabled;
            set
            {
                _smoothTurnEnabled = value;
                UpdateLocomotionActions();
            }
        }

        /// <summary>
        /// Whether or not snap turn (with the joystick) is enabled.
        /// The turn amount in degrees can be configured in the SnapTurn-Provider of the LocomotionSystem.
        /// </summary>
        [SerializeField]
        private bool _snapTurnEnabled;
        public bool snapTurnEnabled
        {
            get => _snapTurnEnabled;
            set
            {
                _snapTurnEnabled = value;
                UpdateLocomotionActions();
            }
        }


        /// <summary>
        /// Whether or not (single hand) grab movement for this hand. 
        /// </summary>
        [SerializeField]
        private bool _grabMoveEnabled;
        public bool grabMoveEnabled
        {
            get => _grabMoveEnabled;
            set
            {
                _grabMoveEnabled = value;

                if (TryGetComponent(out GrabMoveProvider provider))
                {
                    provider.enabled = _grabMoveEnabled;
                }
            }
        }
        #endregion

        #region Interaction Config
        /// <summary>
        /// Whether or not direct interaction (i.e. grabbing) is enabled.
        /// </summary>
        [SerializeField]
        private bool _directInteractionEnabled;
        public bool directInteractionEnabled
        {
            get => _directInteractionEnabled;
            set
            {
                _directInteractionEnabled = value;

                if (m_DirectInteractor)
                {
                    m_DirectInteractor.enabled = _directInteractionEnabled;
                }
            }
        }

        /// <summary>
        /// Whether or not poke interaction is enabled.
        /// </summary>
        [SerializeField]
        private bool _pokeInteractionEnabled;
        public bool pokeInteractionEnabled
        {
            get => _pokeInteractionEnabled;
            set
            {
                _pokeInteractionEnabled = value;

                if (m_PokeInteractor != null)
                {
                    // Not ideal but should be fine for now...
                    // -1 = Everything; 0 = Nothing
                    m_PokeInteractor.physicsLayerMask = _pokeInteractionEnabled ? -1 : 0;
                }
            }
        }

        /// <summary>
        /// Whether or not ray interaction is enabled.
        /// </summary>
        [SerializeField]
        private bool _rayInteractionEnabled;
        public bool rayInteractionEnabled
        {
            get => _rayInteractionEnabled;
            set
            {
                _rayInteractionEnabled = value;

                if (m_RayInteractor != null)
                {
                    m_RayInteractor.enabled = _rayInteractionEnabled;
                }
            }
        }


        /// <summary>
        /// Whether or not ray anchor control (i.e. using the joystick to rotate/move the grabbed objects) is enabled.
        /// </summary>
        [SerializeField]
        private bool _rayAnchorControlEnabled;
        public bool rayAnchorControlEnabled
        {
            get => _rayAnchorControlEnabled;
            set
            {
                _rayAnchorControlEnabled = value;

                if (m_RayInteractor != null)
                {
                    m_RayInteractor.allowAnchorControl = value;
                }
            }
        }

        /// <summary>
        /// Whether or not the ray can also interact with UI.
        /// </summary>
        [SerializeField]
        private bool _uiRayInteractionEnabled;
        public bool uiRayInteractionEnabled
        {
            get => _uiRayInteractionEnabled;
            set
            {
                _uiRayInteractionEnabled = value;

                if (m_RayInteractor != null)
                {
                    m_RayInteractor.enableUIInteraction = _uiRayInteractionEnabled;
                }
            }
        }


        /// <summary>
        /// Whether or not poking can be used with UI.
        /// </summary>
        [SerializeField]
        private bool _uiPokeInteractionEnabled;
        public bool uiPokeInteractionEnabled
        {
            get => _uiPokeInteractionEnabled;
            set
            {
                _uiPokeInteractionEnabled = value;

                if (m_PokeInteractor != null)
                {
                    m_PokeInteractor.enableUIInteraction = _uiPokeInteractionEnabled;
                }
            }
        }

        /// <summary>
        /// Whether or not the poke reticle (i.e. all Renderer-Components in the children of the PokeInteractor) is shown.
        /// </summary>
        [Tooltip("Turns all mesh renderers in children of the PokeInteractor on or off.")]
        [SerializeField]
        private bool _showPokeReticle;
        public bool showPokeReticle
        {
            get => _showPokeReticle;
            set
            {
                _showPokeReticle = value;

                if (m_PokeInteractor != null)
                {
                    foreach (Renderer renderer in m_PokeInteractor.GetComponentsInChildren<Renderer>(true))
                    {
                        renderer.enabled = _showPokeReticle;
                    }
                }
            }
        }

        /// <summary>
        /// Enables scaling grabbed objects by pushing the joystick back and forward.
        /// Requires Scaling[Direct/Ray]Interactors and AnchorControl to be enabled on the Ray.
        /// </summary>
        [Tooltip("Enables scaling grabbed objects by pushing the joystick back and forward. (Requires Scaling[Direct/Ray]Interactors and AnchorControl to be enabled on the Ray).")]
        [SerializeField]
        private bool _scaleGrabbedObjects;
        public bool scaleGrabbedObjects
        {
            get => _scaleGrabbedObjects;
            set
            {
                _scaleGrabbedObjects = value;

                if (m_DirectInteractor != null)
                {
                    ScalingDirectInteractor scalingDirect = m_DirectInteractor as ScalingDirectInteractor;

                    if (scalingDirect != null)
                    {
                        scalingDirect.scalingEnabled = _scaleGrabbedObjects;
                    }
                }

                if (m_RayInteractor != null)
                {
                    ScalingRayInteractor scalingRay = m_RayInteractor as ScalingRayInteractor;

                    if (scalingRay != null)
                    {
                        scalingRay.anchorControlMode = _scaleGrabbedObjects ? AnchorControlMode.ScaleWithTranslateFallback : AnchorControlMode.Translate;
                    }
                }
            }
        }

        /// <summary>
        /// Duration in seconds for which the hand collisions are disabled after grabbing an object to allow it to be thrown.
        /// If set to `0.0f` hand model collisions will be turned on immediately.
        /// </summary>
        [Tooltip("Duration for which the hand collisions are disabled after grabbing an object to allow it to be thrown.")]
        [SerializeField]
        private float _afterGrabWaitDuration = 0.3f;
        public float afterGrabWaitDuration
        {
            get => _afterGrabWaitDuration;
            set => _afterGrabWaitDuration = value;
        }
        #endregion


        #region Hand Models
        /// <summary>
        /// How the Hand Model is displayed.
        /// </summary>
        [SerializeField]
        private HandModelMode _handModelMode;
        public HandModelMode handModelMode
        {
            get => _handModelMode;
            set
            {
                _handModelMode = value;

                if (TryGetAutoHand(out AutoHandModel autoHand))
                {
                    autoHand.handModelMode = _handModelMode;
                }
            }
        }

        /// <summary>
        /// Whether or not the hand models have collisions to push objects. They are disabled when hovering an object.
        /// Does not affect collisions when teleporting, these are always disabled.
        /// Change the TeleportInteractors AutoHandModel to the one with collision to enable them.
        /// </summary>
        [SerializeField]
        private bool _handModelCollisions;
        public bool handModelCollisions
        {
            get => _handModelCollisions;
            set
            {
                _handModelCollisions = value;

                if (TryGetAutoHand(out AutoHandModel autoHand))
                {
                    autoHand.modelCollisionsEnabled = _handModelCollisions;
                }
            }
        }
        #endregion


        private Coroutine _afterGrabCoroutine;

        /// <summary>
        /// Connects additional events.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (m_DirectInteractor != null)
            {
                m_DirectInteractor.selectExited.AddListener(OnDirectInteractorSelectExited);
                m_DirectInteractor.selectEntered.AddListener(OnDirectInteractorSelectEntered);
            }

            // Reticles (AutoHandModels) need to be instantiated so they are available in the next frame
            StartCoroutine(AutoHandReticleCreationTimer());
        }


        /// <summary>
        /// Removes connected additional events.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            if (m_DirectInteractor != null)
            {
                m_DirectInteractor.selectExited.RemoveListener(OnDirectInteractorSelectExited);
                m_DirectInteractor.selectEntered.RemoveListener(OnDirectInteractorSelectEntered);
            }
        }

        /// <summary>
        /// Expands the base function by disabling AutoHand-Collisions during teleport.
        /// </summary>
        /// <param name="context">Callback Context of the InputAction.</param>
        protected override void OnStartTeleport(InputAction.CallbackContext context)
        {
            base.OnStartTeleport(context);

            SetAutoHandCollisionsCurrentlyEnabled(false);
        }

        /// <summary>
        /// Expands the base function by enabling AutoHand-Collisions after teleport.
        /// </summary>
        /// <param name="context">Callback Context of the InputAction.</param>
        protected override void OnCancelTeleport(InputAction.CallbackContext context)
        {
            base.OnCancelTeleport(context);

            SetAutoHandCollisionsCurrentlyEnabled(true);
        }


        private bool TryGetAutoHand(out AutoHandModel autoHand)
        {
            autoHand = null;
            if (TryGetComponent(out XRBaseController controller) && controller.model != null)
            {
                return controller.model.TryGetComponent(out autoHand);
            }
            return false;
        }

        private void SetAutoHandCollisionsCurrentlyEnabled(bool enabled)
        {
            if (TryGetAutoHand(out AutoHandModel autoHand))
            {
                autoHand.collisionsCurrentlyEnabled = enabled;
            }
        }

        /// <summary>
        /// Manages which InputActions are available.
        /// This is slightly different to how the base function handles it.
        /// </summary>
        protected override void UpdateLocomotionActions()
        {
            // Disable/enable Teleport and Turn when Move is enabled/disabled.
            SetEnabled(m_Move, smoothMoveEnabled);
            SetEnabled(m_TeleportModeActivate, !smoothMoveEnabled && teleportationEnabled);
            SetEnabled(m_TeleportModeCancel, !smoothMoveEnabled && teleportationEnabled && teleportCancelEnabled);

            // Disable ability to turn when using continuous movement
            SetEnabled(m_Turn, !smoothMoveEnabled && smoothTurnEnabled);
            SetEnabled(m_SnapTurn, !smoothMoveEnabled && !smoothTurnEnabled && snapTurnEnabled);
        }

        private void NotifyOverwrittenBehavior()
        {
            if (smoothMoveEnabled && teleportationEnabled)
            {
                Debug.LogWarning("SmoothMove and Teleportation are both enabled on this hand. Teleportation is disabled, as it is overwritten by SmoothMove.");
            }

            if (smoothMoveEnabled && smoothTurnEnabled)
            {
                Debug.LogWarning("SmoothMove and SmoothTurn are both enabled on this hand. SmoothTurn is disabled, as it is overwritten by SmoothMove.");
            }

            if ((smoothMoveEnabled || smoothTurnEnabled) && snapTurnEnabled)
            {
                Debug.LogWarning("SmoothMove and/or SmoothTurn are both enabled with SnapTurn on this hand. SnapTurn is disabled, as it is overwritten by SmoothMove/SmoothTurn.");
            }

            // Disabling the comment as to not spam this warning if that behavior is not desired
            // Is reflected in the Inspector anyways
            // if (!_rayAnchorControlEnabled && scaleGrabbedObjects)
            // {
            //     Debug.LogWarning("Scaling objects using the RayInteractor will require 'RayAnchorControl' to be enabled.");
            // }
        }


        private void OnDirectInteractorSelectExited(SelectExitEventArgs args)
        {
            // Start wait timer 
            if (gameObject.activeInHierarchy && isActiveAndEnabled)
            {
                _afterGrabCoroutine = StartCoroutine(AfterGrabWaitTimer());
            }

            // Enable the snap turn InputAction if it was previously enabled
            SetEnabled(m_SnapTurn, !smoothMoveEnabled && !smoothTurnEnabled && snapTurnEnabled);
            SetEnabled(m_TeleportModeActivate, !smoothMoveEnabled && teleportationEnabled);
        }


        private void OnDirectInteractorSelectEntered(SelectEnterEventArgs args)
        {
            // Stop wait timer
            if (_afterGrabCoroutine != null)
            {
                StopCoroutine(_afterGrabCoroutine);
            }
            _afterGrabCoroutine = null;

            // Disable the snap turn InputAction turn while grabbing when the interactor has anchor control enabled
            if (hasDirectInteractorScalingSelection)
            {
                SetEnabled(m_SnapTurn, false);
                SetEnabled(m_TeleportModeActivate, false);
            }
        }


        private bool hasDirectInteractorScalingSelection
        {
            get
            {
                ScalingDirectInteractor scalableInteractor = m_DirectInteractor as ScalingDirectInteractor;
                if (scalableInteractor != null)
                {
                    return scalableInteractor.hasScalingSelection;
                }
                return false;
            }
        }


        private IEnumerator AutoHandReticleCreationTimer()
        {
            yield return new WaitForEndOfFrame();
            handModelMode = _handModelMode;
            handModelCollisions = _handModelCollisions;
        }


        private IEnumerator AfterGrabWaitTimer()
        {
            // Disable Collisions
            SetAutoHandCollisionsCurrentlyEnabled(false);

            yield return new WaitForSeconds(_afterGrabWaitDuration);

            // Enable auto hand Collisions
            SetAutoHandCollisionsCurrentlyEnabled(handModelCollisions);

            _afterGrabCoroutine = null;
        }


        private void OnValidate()
        {
            teleportationEnabled = _teleportationEnabled;
            teleportCancelEnabled = _teleportCancelEnabled;
            chooseTeleportForwardEnabled = _chooseTeleportForwardEnabled;
            smoothMoveEnabled = _smoothMoveEnabled;
            smoothTurnEnabled = _smoothTurnEnabled;
            grabMoveEnabled = _grabMoveEnabled;
            directInteractionEnabled = _directInteractionEnabled;
            pokeInteractionEnabled = _pokeInteractionEnabled;
            rayInteractionEnabled = _rayInteractionEnabled;
            rayAnchorControlEnabled = _rayAnchorControlEnabled;
            uiRayInteractionEnabled = _uiRayInteractionEnabled;
            uiPokeInteractionEnabled = _uiPokeInteractionEnabled;
            handModelMode = _handModelMode;
            handModelCollisions = _handModelCollisions;
            scaleGrabbedObjects = _scaleGrabbedObjects;

            NotifyOverwrittenBehavior();
        }

        public void EditorRevalidate()
        {
            showPokeReticle = _showPokeReticle;
        }
    }
}