using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace ExPresSXR.Interaction
{
    /// <summary>
    /// Acts as a wrapper for allowing XRGrabInteractables to be scaled while being held. 
    /// This will **not** scale the GameObject itself but all of it's children. Scaling will be applied to the children using their initial scale.
    /// </summary>
    [AddComponentMenu("ExPresS XR/ExPresS XR Grab Interactable")]
    public class ExPresSXRGrabInteractable : XRGrabInteractable
    {
        /// <summary>
        /// Minimal scale possible, negative values are considered unbound.
        /// </summary>
        [SerializeField]
        private float _minScaleFactor = 1.0f;
        public float MinScaleFactor
        {
            get => _minScaleFactor;
            set => _minScaleFactor = value;
        }

        /// <summary>
        /// Maximal scale possible, negative values are considered unbound.
        /// </summary>
        [SerializeField]
        private float _maxScaleFactor = 1.0f;
        public float MaxScaleFactor
        {
            get => _maxScaleFactor;
            set => _maxScaleFactor = value;
        }

        /// <summary>
        /// Range of scaling. If either the min or max scale is unbound, infinity will be returned. 
        /// </summary>
        public float ScaleRange
        {
            get => _maxScaleFactor > 0.0f & _minScaleFactor > 0.0f ? Mathf.Max(_maxScaleFactor - _minScaleFactor) : Mathf.Infinity;
        }


        /// <summary>
        /// Override to the scale speed of the `ScalingRayInteractor` or `ScalingDirectInteractor`. 
        /// It is recommended to change the scale speed in the interactors themselves and use this override sparingly!
        /// </summary>
        [SerializeField]
        [Tooltip("Override to the scale speed of the `ScalingRayInteractor` or `ScalingDirectInteractor`. \n"
                + "It is recommended to change the scale speed in the interactors themselves and use this override sparingly!")]
        private float _scaleSpeedOverride = -1.0f;
        public float ScaleSpeedOverride
        {
            get => _scaleSpeedOverride;
            set => _scaleSpeedOverride = value;
        }

        /// <summary>
        /// If the interactable should reset its scale when selected by a socket or retain the current scale.
        /// </summary>
        [SerializeField]
        private bool _resetScaleInSockets = true;


        /// <summary>
        /// If all children should be scaled or only those set as `scaledChildren` via the editor.
        /// </summary>
        [SerializeField]
        private bool _scaleAllChildren = true;
        public bool ScaleAllChildren
        {
            get => _scaleAllChildren;
        }


        /// <summary>
        /// Children affected by scaling. Setting this value during runtime will use the current scales as initial scale.
        /// </summary>
        [Tooltip("Children affected by scaling. Setting this value during runtime will use the current scales as initial scale.")]
        [SerializeField]
        private Transform[] _scaledChildren;
        public Transform[] ScaledChildren
        {
            get => _scaledChildren;
            set
            {
                _scaledChildren = value;

                // Load scales
                _initialScales = new Vector3[_scaledChildren.Length];

                for (int i = 0; i < _scaledChildren.Length; i++)
                {
                    _initialScales[i] = _scaledChildren[i] != null ? _scaledChildren[i].localScale : Vector3.one;
                }
            }
        }


        /// <summary>
        /// If false, denies interactions with ray and direct interactors. Can be used to enable interaction after a certain stage or disable it later.
        /// </summary>
        [SerializeField]
        private bool _allowGrab = true;
        public bool AllowGrab
        {
            get => _allowGrab;
            set
            {
                _allowGrab = value;

                if (!_allowGrab)
                {
                    ClearSelectingInteractors();
                }
            }
        }


        /// <summary>
        /// The current scale to the children, relative to their initial scale.
        /// </summary>
        private float _scaleFactor = 1.0f;
        public float ScaleFactor
        {
            get => _scaleFactor;
            set
            {
                _scaleFactor = GetClampedScaleFactor(value);

                ScaleChildren();
            }
        }


        /// <summary>
        /// The initial scales of the object in `_scaledChildren`. Only available at runtime.
        /// </summary>
        private Vector3[] _initialScales;
        public Vector3[] InitialScales
        {
            get => _initialScales;
        }


        /// <summary>
        /// If the interactable has a speed scale override.
        /// </summary>
        public bool HasScaleSpeedOverride
        {
            get => _scaleSpeedOverride > 0.0f;
        }

        // Events

        /// <summary>
        /// Emitted when a grab is started.
        /// </summary>
        public UnityEvent OnGrabStarted;

        /// <summary>
        /// Emitted when a grab is ended.
        /// </summary>
        public UnityEvent OnGrabReleased;

        /// <summary>
        /// Emitted when a grab is allowed.
        /// </summary>
        public UnityEvent OnGrabAllowed;

        /// <summary>
        /// Emitted when a grab is denied.
        /// </summary>
        public UnityEvent OnGrabDenied;

        /// <summary>
        /// Emitted when the scale of this interactable is reset.
        /// </summary>
        public UnityEvent OnScaleReset;



        /// <summary>
        /// Performs general setup for the interactable, load the initial scales and connects the scale reset event.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            // Load initial scales
            LoadInitialScales();
            // Add Listener to reset scale
            selectEntered.AddListener(TryResetScaleInSockets);
        }


        /// <summary>
        /// Performs general teardown for the interactable and disconnects the scale reset event.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            // Remove Listener to reset scale
            selectEntered.RemoveListener(TryResetScaleInSockets);
        }

        /// <summary>
        /// Emits the OnGrabStarted-Event alongside performing the rest of the select enter.
        /// </summary>
        /// <param name="args">Args of the select enter.</param>
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            if (args.interactorObject is XRDirectInteractor or XRRayInteractor)
            {
                OnGrabStarted.Invoke();
            }
        }

        /// <summary>
        /// Emits the OnGrabReleased-Event alongside performing the rest of the select exit.
        /// </summary>
        /// <param name="args">Args of the select exit.</param>
        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            if (args.interactorObject is XRDirectInteractor or XRRayInteractor)
            {
                OnGrabReleased.Invoke();
            }
        }

        /// <summary>
        /// Denies selection of direct and ray interactors if grabbing is not allowed, emitting the respective events.
        /// </summary>
        /// <param name="interactor">Interactor trying to select</param>
        /// <returns>Whether or not selection is allowed.</returns>
        public override bool IsSelectableBy(IXRSelectInteractor interactor)
        {
            // Allow Direct and ray only if grab allowed and add parent checks
            bool canGrab = (_allowGrab || interactor is not (XRDirectInteractor or XRRayInteractor)) && base.IsSelectableBy(interactor);

            if (interactor is XRDirectInteractor || interactor is XRRayInteractor)
            {
                (canGrab ? OnGrabAllowed : OnGrabDenied).Invoke();
            }
            return canGrab;
        }

        /// <summary>
        /// Resets the scale of all (scaled) children to 1.0f
        /// </summary>
        public void ResetScale()
        {
            ScaleFactor = 1.0f;
            OnScaleReset.Invoke();
        }

        private void LoadInitialScales()
        {
            List<Transform> children = new();
            if (_scaleAllChildren)
            {
                foreach (Transform t in transform)
                {
                    children.Add(t);
                }
            }
            else if (ScaledChildren.Length > 0 && transform.childCount > 0)
            {
                children.AddRange(ScaledChildren);
            }
            else
            {
                Debug.LogError("ExPresS XR Grab interactable has no children to scale. Either enable '_scaleAllChildren' or add objects to the 'scaledChildren' array.");
            }

            ScaledChildren = children.ToArray();
        }

        private void ScaleChildren()
        {
            for (int i = 0; i < _scaledChildren.Length; i++)
            {
                _scaledChildren[i].transform.localScale = _initialScales[i] * ScaleFactor;
            }
        }

        private void ClearSelectingInteractors()
        {
            // Don't use a for each here, because the list will shrink
            for (int i = 0; i < interactorsSelecting.Count; i++)
            {
                IXRSelectInteractor interactor = interactorsSelecting[i];
                if (interactor is XRDirectInteractor || interactor is XRRayInteractor)
                {
                    interactionManager.SelectExit(interactor, this);
                    i--; // Decrement as the next element will take the removed interactors place
                }
            }
        }

        private void TryResetScaleInSockets(SelectEnterEventArgs args)
        {
            if (_resetScaleInSockets && args.interactorObject is XRSocketInteractor)
            {
                ResetScale();
            }
        }

        private float GetClampedScaleFactor(float value)
        {
            float minValue = _minScaleFactor >= 0 ? _minScaleFactor : value;
            float maxValue = _maxScaleFactor >= 0 ? _maxScaleFactor : value;

            return Mathf.Clamp(value, minValue, maxValue);
        }
    }
}