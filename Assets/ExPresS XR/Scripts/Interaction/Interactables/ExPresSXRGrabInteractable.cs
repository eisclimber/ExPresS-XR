using System.Collections;
using System.Collections.Generic;
using ExPresSXR.Misc;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ExPresSXR.Interaction
{
    /// <summary>
    /// Acts as a wrapper for allowing XRGrabInteractables to be scaled while being held. 
    /// This will **not** scale the GameObject itself but all of it's children. Scaling will be applied to the children using their initial scale.
    /// </summary>
    [AddComponentMenu("ExPresS XR/ExPresS XR Grab Interactable")]
    public class ExPresSXRGrabInteractable : UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable
    {
        [SerializeField]
        [Tooltip("Minimal scale possible, negative values are considered unbound.")]
        private float _minScaleFactor = 1.0f;
        /// <summary>
        /// Minimal scale possible, negative values are considered unbound.
        /// </summary>
        public float MinScaleFactor
        {
            get => _minScaleFactor;
            set => _minScaleFactor = value;
        }

        [SerializeField]
        [Tooltip("Maximal scale possible, negative values are considered unbound.")]
        private float _maxScaleFactor = 1.0f;
        /// <summary>
        /// Maximal scale possible, negative values are considered unbound.
        /// </summary>
        public float MaxScaleFactor
        {
            get => _maxScaleFactor;
            set => _maxScaleFactor = value;
        }

        /// <summary>
        /// Range of scaling. If either the min or max scale is set. Else infinity will be returned. 
        /// </summary>
        public float ScaleRange
        {
            get => _maxScaleFactor > 0.0f && _minScaleFactor > 0.0f ? _maxScaleFactor - _minScaleFactor : Mathf.Infinity;
        }

        /// <summary>
        /// If scaling is possible, that if the ScaleRange is greater than zero.
        /// </summary>
        public bool Scalable
        {
            get => ScaleRange > 0.0f;
        }


        [SerializeField]
        [Tooltip("Override to the scale speed of the `ScalingRayInteractor` or `ScalingDirectInteractor`. \n"
                + "It is recommended to change the scale speed in the interactors themselves and use this override sparingly!")]
        private float _scaleSpeedOverride = -1.0f;
        /// <summary>
        /// Override to the scale speed of the `ScalingRayInteractor` or `ScalingDirectInteractor`. 
        /// It is recommended to change the scale speed in the interactors themselves and use this override sparingly!
        /// </summary>
        public float ScaleSpeedOverride
        {
            get => _scaleSpeedOverride;
            set => _scaleSpeedOverride = value;
        }

        /// <summary>
        /// If the interactable should reset its scale when selected by a socket or retain the current scale.
        /// </summary>
        [SerializeField]
        [Tooltip(" If the interactable should reset its scale when selected by a socket or retain the current scale.")]
        private bool _resetScaleInSockets = true;


        [SerializeField]
        [Tooltip("If all children should be scaled or only those set as `scaledChildren` via the editor.")]
        private bool _scaleAllChildren = true;
        /// <summary>
        /// If all children should be scaled or only those set as `scaledChildren` via the editor.
        /// </summary>
        public bool ScaleAllChildren
        {
            get => _scaleAllChildren;
        }


        [Tooltip("Children affected by scaling. Setting this value during runtime will use the current scales as initial scale.")]
        [SerializeField]
        private Transform[] _scaledChildren;
        /// <summary>
        /// Children affected by scaling. Setting this value during runtime will use the current scales as initial scale.
        /// </summary>
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

        [SerializeField]
        [Tooltip("If false, denies interactions with ray and direct interactors. Can be used to enable interaction after a certain stage or disable it later.")]
        private bool _allowGrab = true;
        /// <summary>
        /// If false, denies interactions with ray and direct interactors. Can be used to enable interaction after a certain stage or disable it later.
        /// </summary>
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
        /// If only direct (i.e. grab) interactions are allowed. For this you'll need a child GameObject with a RigidBody with a collision.
        /// </summary>
        [SerializeField]
        [Tooltip("If only direct (i.e. grab) interactions are allowed. For this you'll need a child GameObject with a RigidBody with a collision.")]
        protected bool _requireDirectInteraction;

        [SerializeField]
        [Tooltip("If enabled allows NearFarInteractors to be treats as valid Direct Interactor. "
                + "It is recommended to set the max interaction distance to the size of near interaction volume, "
                + "as we can not differentiate hovers from it and the ray.")]
        private bool _treatNearFarAsGrab = true;
        /// <summary>
        /// If enabled allows NearFarInteractors to be treats as valid Direct Interactor.
        /// It is recommended to set the max interaction distance to the size of near interaction volume,
        /// as we can not differentiate hovers from it and the ray.
        /// </summary>
        public bool TreatNearFarAsGrab
        {
            get => _treatNearFarAsGrab;
            set => _treatNearFarAsGrab = value;
        }

        [SerializeField]
        [Tooltip("Custom attach used for socket interactors.")]
        private Transform _customSocketAttach;
        /// <summary>
        /// Custom attach used for socket interactors.
        /// </summary>
        public Transform CustomSocketAttach
        {
            get => _customSocketAttach;
            set => _customSocketAttach = value;
        }

        private float _scaleFactor = 1.0f;
        /// <summary>
        /// The current scale to the children, relative to their initial scale.
        /// </summary>
        public float ScaleFactor
        {
            get => _scaleFactor;
            set
            {
                _scaleFactor = GetClampedScaleFactor(value);

                ScaleChildren();
            }
        }


        private Vector3[] _initialScales;
        /// <summary>
        /// The initial scales of the object in `_scaledChildren`. Only available at runtime.
        /// </summary>
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

        private Coroutine _hiddenFromPlayerCoroutine;

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
            if (RuntimeUtils.IsCloseUpHandInteractor(args.interactorObject, _treatNearFarAsGrab))
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
            if (RuntimeUtils.IsCloseUpHandInteractor(args.interactorObject, _treatNearFarAsGrab))
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
            bool isGrabInteractor = RuntimeUtils.IsCloseUpHandInteractor(interactor, _treatNearFarAsGrab);
            bool canGrab = (_allowGrab || !isGrabInteractor) && base.IsSelectableBy(interactor);

            if (isGrabInteractor)
            {
                (canGrab ? OnGrabAllowed : OnGrabDenied).Invoke();
            }
            return canGrab;
        }

        /// <summary>
        /// Return the attach transform for the interactor.
        /// If the interactor is a SocketInteractor and a `_customSocketAttach` is set, it is returned.
        /// </summary>
        /// <param name="interactor"></param>
        /// <returns></returns>
        public override Transform GetAttachTransform(IXRInteractor interactor)
        {
            if (_customSocketAttach != null && interactor is XRSocketInteractor)
            {
                return _customSocketAttach;
            }

            return base.GetAttachTransform(interactor);
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
                if (RuntimeUtils.IsCloseUpHandInteractor(interactor, _treatNearFarAsGrab))
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

        /// <summary>
        /// Disables all visuals during which grabbing is prevented.
        /// </summary>
        /// <param name="duration">Duration to hide.</param>
        public void HideFromPlayerForDuration(float duration)
        {
            AllowGrab = false;

            if (_hiddenFromPlayerCoroutine != null)
            {
                StopCoroutine(_hiddenFromPlayerCoroutine);
                _hiddenFromPlayerCoroutine = null;
            }

            if (duration > 0.0f)
            {
                _hiddenFromPlayerCoroutine = StartCoroutine(ShowHiddenFromPlayerDelayed(duration));
            }
        }

        private void SetVisualsHidden(bool hidden)
        {
            foreach (Renderer render in GetComponentsInChildren<Renderer>())
            {
                // Debug.Log($"Setting render {render} hidden to {hidden}");
                render.enabled = !hidden;
            }
        }

        private IEnumerator ShowHiddenFromPlayerDelayed(float duration)
        {
            SetVisualsHidden(true);
            yield return new WaitForSeconds(duration);
            AllowGrab = true;
            SetVisualsHidden(false);
            _hiddenFromPlayerCoroutine = null;
        }
    }
}