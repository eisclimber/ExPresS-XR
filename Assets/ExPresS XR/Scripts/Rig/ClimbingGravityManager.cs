using System;
using System.Collections.Generic;
using System.Linq;
using ExPresSXR.Experimentation.DataGathering;
using ExPresSXR.Movement;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace ExPresSXR.Rig
{
    public class ClimbingGravityManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("How many interactables (inclusive) must be held to not fall. SHould be greater than 0.")]
        private int _minGrabbedHolds = 1;


        [SerializeField]
        [Tooltip("The interactors used for climbing. Usually direct interactors.")]
        private XRBaseControllerInteractor[] _climbInteractors;
        public XRBaseControllerInteractor[] climbInteractors
        {
            get => _climbInteractors;
        }

        [Space]

        [SerializeField]
        private bool _applyReleaseVelocity;

        [SerializeField]
        private float _releaseStrengthFactor;

        [Space]

        [SerializeField]
        private PlayerGravity _playerGravity;

        [SerializeField]
        private PlayerRigidForce _playerRigidForce;

        
        private int _grabbedHolds;

        private Vector3[] _interactorPrevPos;
        private Vector3[] _interactorsVelocities;


        public UnityEvent OnGrabHoldSuccess;
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

            RegisterInteractors();
            RegisterGravityProvider();
        }

        private void OnDisable()
        {
            UnregisterInteractors();
            UnregisterGravityProvider();
        }


        private void Update() {
            // Update Velocities
            for (int i = 0; i < _climbInteractors.Length; i++)
            {
                // Debug.Log(" x " + _interactorPrevPos);
                if (_interactorPrevPos[i] != null)
                {
                    _interactorsVelocities[i] = _climbInteractors[i].transform.position - _interactorPrevPos[i];
                }
                _interactorPrevPos[i] = _climbInteractors[i].transform.position;
            }
        }


        public void RegisterInteractors()
        {
            _interactorPrevPos = new Vector3[_climbInteractors.Length];
            _interactorsVelocities = new Vector3[_climbInteractors.Length];

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

                _interactorPrevPos[i] = interactor.transform.position;
                _interactorsVelocities[i] = Vector3.zero;
            }
        }

        public void UnregisterInteractors()
        {
            foreach (XRBaseControllerInteractor interactor in _climbInteractors)
            {
                interactor.selectEntered.RemoveListener(AddGrabbedHold);
                interactor.selectExited.RemoveListener(RemoveGrabbedHold);
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
                    ApplyReleaseVelocity(args.interactorObject);
                    OnGrabHoldFailed.Invoke();
                }
            }
        }


        private void ApplyReleaseVelocity(IXRSelectInteractor interactor)
        {
            if (!_applyReleaseVelocity)
            {
                return;
            }

            int idx = Array.FindIndex(_climbInteractors, x => x == (UnityEngine.Object)interactor);

            // // Ignore interactors that are not used for climbing
            if (idx >= 0)
            {
                Debug.Log(CsvUtility.ArrayToString(_interactorsVelocities) + " x " + idx + " - " + _interactorsVelocities[idx]);
                _playerRigidForce.ApplyImpulseUpperHalfSphere(_interactorsVelocities[idx] * _releaseStrengthFactor);
            }
        }

        private void DisableExternalForces()
        {
            if (_playerGravity != null)
            {
                _playerGravity.applyGravity = false;
            }

            if (_playerRigidForce != null)
            {
                _playerRigidForce.applyForce = false;
            }
        }

        private void EnableExternalForces()
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
    }
}