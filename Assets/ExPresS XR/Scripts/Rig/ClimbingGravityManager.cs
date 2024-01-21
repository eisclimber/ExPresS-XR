using System;
using System.Collections.Generic;
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
        private List<XRBaseControllerInteractor> _climbInteractors;
        public List<XRBaseControllerInteractor> climbInteractors
        {
            get => _climbInteractors;
            set => _climbInteractors = value;
        }

        [SerializeField]
        private ContinuousMoveProviderBase _gravityProvider;

        private int _grabbedHolds;


        public UnityEvent OnGrabHoldSuccess;
        public UnityEvent OnGrabHoldFailed;


        private void OnEnable()
        {
            if (_climbInteractors.Count <= 0)
            {
                Debug.LogWarning("No Climb Interactors were provided.", this);
            }

            if (_minGrabbedHolds <= 0 || _minGrabbedHolds > _climbInteractors.Count)
            {
                Debug.LogWarning("MinGrabbedHolds was either below 1 or more than the number of Climb Interactors. This won't work!", this);
            }

            if (_gravityProvider == null)
            {
                Debug.LogWarning("No Gravity Provider was provided, can't control gravity!", this);
            }

            RegisterInteractors();
            RegisterGravityProvider();
        }

        private void OnDisable()
        {
            UnregisterInteractors();
            UnregisterGravityProvider();
        }


        public void RegisterInteractors()
        {
            foreach (XRBaseControllerInteractor interactor in _climbInteractors)
            {
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
            OnGrabHoldSuccess.AddListener(DisableGravity);
            OnGrabHoldFailed.AddListener(EnableGravity);
        }

        private void UnregisterGravityProvider()
        {
            OnGrabHoldSuccess.RemoveListener(DisableGravity);
            OnGrabHoldFailed.RemoveListener(EnableGravity);
        }


        private void AddGrabbedHold(SelectEnterEventArgs args)
        {
            if (args.interactableObject is ClimbInteractable)
            {
                _grabbedHolds = Math.Max(_grabbedHolds + 1, 0);
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
                    OnGrabHoldFailed.Invoke();
                }
            }
        }

        private void DisableGravity()
        {
            if (_gravityProvider)
            {
                // Only manipulate enableFly as it will override useGravity
                _gravityProvider.enableFly = true;
            }
        }

        private void EnableGravity()
        {
            if (_gravityProvider)
            {
                // Only manipulate enableFly as it will override useGravity
                _gravityProvider.enableFly = false;
            }
        }
    }
}