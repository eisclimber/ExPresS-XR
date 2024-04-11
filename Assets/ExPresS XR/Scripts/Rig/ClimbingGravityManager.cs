using System;
using ExPresSXR.Movement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace ExPresSXR.Rig
{
    public class ClimbingGravityManager : MonoBehaviour
    {
        /// <summary>
        /// How many interactables (inclusive) must be held to not fall. Should be greater than 0.
        /// </summary>
        [SerializeField]
        [Tooltip("How many interactables (inclusive) must be held to not fall. Should be greater than 0.")]
        private int _minGrabbedHolds = 1;

        /// <summary>
        /// The interactors used for climbing. Usually direct interactors.
        /// </summary>
        [SerializeField]
        [Tooltip("The interactors used for climbing. Usually direct interactors.")]
        private XRBaseControllerInteractor[] _climbInteractors;
        public XRBaseControllerInteractor[] climbInteractors
        {
            get => _climbInteractors;
        }

        [Space]

        /// <summary>
        /// If enabled, will apply an impulse to the player when climbing is ended. This is based on its velocity scaled by `_releaseStrengthFactor`.
        /// </summary>
        [SerializeField]
        private bool _applyReleaseVelocity;

        /// <summary>
        /// Factor to the velocity that is then applied as an impulse to the player when releasing a climb. 
        /// </summary>
        [SerializeField]
        private float _releaseStrengthFactor;

        [Space]

        /// <summary>
        /// Applies the gravity to the player.
        /// </summary>
        [SerializeField]
        private PlayerGravity _playerGravity;

        /// <summary>
        /// Applies the impulse to the player when ending a climb.
        /// </summary>
        [SerializeField]
        private PlayerRigidForce _playerRigidForce;

        /// <summary>
        /// Used to read the players velocity. Should be set to the GameObject that is driven by locomotion (i.e. the root of your ExPresS XR Rig)
        /// </summary>
        [SerializeField]
        private AverageVelocity _playerAverageVelocity;

        /// <summary>
        /// Number of holds (ClimbInteractors) currently grabbed.
        /// </summary>
        private int _grabbedHolds;
        public int grabbedHolds
        {
            get => _grabbedHolds;
        }

        /// <summary>
        /// UnityEvent emitted, when the player interacts with enough ClimbInteractables to start climbing.
        /// </summary>
        public UnityEvent OnGrabHoldSuccess;
        /// <summary>
        /// UnityEvent emitted, when the player does not interacts with enough ClimbInteractables to continue climbing.
        /// </summary>
        public UnityEvent OnGrabHoldFailed;


        private void OnEnable()
        {
            if (_climbInteractors.Length <= 0)
            {
                Debug.LogWarning("No Climb Interactors were provided.", this);
            }

            if (_minGrabbedHolds <= 0 || _minGrabbedHolds > _climbInteractors.Length)
            {
                Debug.LogWarning("MinGrabbedHolds was either below 1 or more than the number of Climb Interactors. This won't work!", this);
            }

            if (_applyReleaseVelocity && _playerAverageVelocity == null)
            {
                Debug.LogError("Trying to apply a velocity on climb release but no AverageVelocity-Component was provided to retrieve the velocity.", this);
            }

            RegisterInteractors();
            RegisterGravityProvider();
        }

        private void OnDisable()
        {
            UnregisterInteractors();
            UnregisterGravityProvider();
        }


        /// <summary>
        /// Sets up and checks the configured ClimbInteractors.
        /// </summary>
        public void RegisterInteractors()
        {
            for (int i = 0; i < _climbInteractors.Length; i++)
            {
                XRBaseControllerInteractor interactor = _climbInteractors[i];
                // Check if the interactors can interact
                if ((interactor.interactionLayers & InteractionLayerMask.NameToLayer("Climb")) != 0)
                {
                    Debug.LogWarning($"The climbing interactor { interactor.name } does not interact with the InteractionLayer 'Climb'. "
                        + "Using this layer for interactors and interactables is recommended to prevent interactions of other interactors with ClimbInteractables.", this);
                }

                interactor.selectEntered.AddListener(AddGrabbedHold);
                interactor.selectExited.AddListener(RemoveGrabbedHold);
            }
        }

        /// <summary>
        /// Unregister the configured ClimbInteractors.
        /// </summary>
        public void UnregisterInteractors()
        {
            foreach (XRBaseControllerInteractor interactor in _climbInteractors)
            {
                interactor.selectEntered.RemoveListener(AddGrabbedHold);
                interactor.selectExited.RemoveListener(RemoveGrabbedHold);
            }
        }

        /// <summary>
        /// Called when a climb is released and applies the release velocity if enabled.
        /// </summary>
        protected virtual void ApplyReleaseVelocity()
        {
            if (!_applyReleaseVelocity)
            {
                return;
            }
            _playerRigidForce.ApplyImpulseUpperHalfSphere(_playerAverageVelocity.GetAverageVelocity() * _releaseStrengthFactor);
        }

        /// <summary>
        /// Called when a climb is stared, disabling external forces (gravity and rigid forces).
        /// </summary>
        protected virtual void DisableExternalForces()
        {
            if (_playerGravity != null)
            {
                _playerGravity.applyGravity = false;
            }

            if (_playerRigidForce != null)
            {
                _playerRigidForce.applyForce = false;
                _playerRigidForce.ClearVelocity();
            }
        }

        /// <summary>
        /// Called when a climb ends, enabling external forces (gravity and rigid forces).
        /// </summary>
        protected virtual void EnableExternalForces()
        {
            if (_playerGravity != null)
            {
                _playerGravity.applyGravity = true;
            }

            if (_playerRigidForce != null)
            {
                _playerRigidForce.applyForce = true;
            }
        }

        private void RegisterGravityProvider()
        {
            OnGrabHoldSuccess.AddListener(DisableExternalForces);
            OnGrabHoldFailed.AddListener(EnableExternalForces);
        }

        private void UnregisterGravityProvider()
        {
            OnGrabHoldSuccess.RemoveListener(DisableExternalForces);
            OnGrabHoldFailed.RemoveListener(EnableExternalForces);
        }


        private void AddGrabbedHold(SelectEnterEventArgs args)
        {
            if (args.interactableObject is ClimbInteractable)
            {
                _grabbedHolds = Math.Max(_grabbedHolds + 1, 0);
                // Only emit the event when the threshold is met 
                if (_grabbedHolds == _minGrabbedHolds)
                {
                    // Added the the required min hold right now
                    OnGrabHoldSuccess.Invoke();
                }
            }
        }

        private void RemoveGrabbedHold(SelectExitEventArgs args)
        {
            if (args.interactableObject is ClimbInteractable)
            {
                _grabbedHolds = Math.Max(_grabbedHolds - 1, 0);
                if (_grabbedHolds == _minGrabbedHolds - 1)
                {
                    ApplyReleaseVelocity();
                    OnGrabHoldFailed.Invoke();
                }
            }
        }
    }
}