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
        // Movement
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

        // Rotation
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


        // Grab Move
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


        ////////
        // Interaction
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
        /// Requires rayInteractionEnabled to be true.
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


        ////////
        

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


        ////////

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

        [Tooltip("Duration for which the hand collisions are disabled after grabbing an object to allow it to be thrown.")]
        [SerializeField]
        private float _afterGrabWaitDuration = 0.3f;
        public float afterGrabWaitDuration
        {
            get => _afterGrabWaitDuration;
            set => _afterGrabWaitDuration = value;
        }


        private Coroutine _afterGrabCoroutine;


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


        protected override void OnDisable()
        {
            base.OnDisable();

            if (m_DirectInteractor != null)
            {
                m_DirectInteractor.selectExited.RemoveListener(OnDirectInteractorSelectExited);
                m_DirectInteractor.selectEntered.RemoveListener(OnDirectInteractorSelectEntered);
            }
        }


        protected override void OnStartTeleport(InputAction.CallbackContext context)
        {
            base.OnStartTeleport(context);

            SetAutoHandCollisionsCurrentlyEnabled(false);
        }

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

            if (!_rayAnchorControlEnabled && scaleGrabbedObjects)
            {
                Debug.LogWarning("Scaling objects using the RayInteractor will require 'RayAnchorControl' to be enabled.");
            }
        }


        private void OnDirectInteractorSelectExited(SelectExitEventArgs _)
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


        private void OnDirectInteractorSelectEntered(SelectEnterEventArgs _)
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