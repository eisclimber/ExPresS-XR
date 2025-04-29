using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace ExPresSXR.Interaction
{
    public class PutBackSocketInteractor : HighlightableSocketInteractor
    {
        /// <summary>
        /// The prefab that is displayed at the socket. Will automatically create an instance of the prefab and update the references.
        /// </summary>
        [SerializeField]
        private GameObject _putBackPrefab;
        public GameObject putBackPrefab
        {
            get => _putBackPrefab;
            set
            {
                _putBackPrefab = value;
                UpdatePutBackObject();
            }
        }

        /// <summary>
        /// The current instance of the putBackPrefab. Can not be changed via the editor.
        /// </summary>
        [SerializeField]
        private GameObject _putBackInstance;
        public GameObject putBackObjectInstance
        {
            get => _putBackInstance;
        }

        /// <summary>
        /// The current interactable of the putBackPrefab. Can not be changed via the editor. 
        /// May be null if the `putBackPrefab` has no `XRBaseInteractable`-Component and `allowNonInteractables` is true.
        /// </summary>
        [SerializeField]
        private XRBaseInteractable _putBackInteractable;
        public XRBaseInteractable putBackInteractable
        {
            get => _putBackInteractable;
        }


        /// <summary>
        /// If enabled GameObjects without an `XRGrabInteractable`-Component will be allowed to be set as `putBackPrefab`. Otherwise the provided prefab will be set to null.
        /// </summary>
        [Tooltip("If enabled GameObjects without an `XRGrabInteractable`-Component will be allowed to be set as `putBackPrefab`. Otherwise the provided prefab will be set to null.")]
        [SerializeField]
        private bool _allowNonInteractables;
        public bool allowNonInteractables
        {
            get => _allowNonInteractables;
            set => _allowNonInteractables = value;
        }


        /// <summary>
        /// Compensates the attach off set of the putback interactable. Makes placing interactables with an attach easier but requires an attach transform to be set.
        /// </summary>
        [SerializeField]
        private bool _compensateInteractableAttach;
        public bool CompensateInteractableAttach
        {
            get => _compensateInteractableAttach;
            set
            {
                _compensateInteractableAttach = value;
                UpdatePutbackAttachCompensation();
            }
        }

        /// <summary>
        /// Will set the 'Retain Parent Transform' property of the interactable to false to disable a warning regarding it.
        /// </summary>
        [Tooltip("Will set the 'Retain Parent Transform' property of the interactable to false to disable a warning regarding it.")]
        [SerializeField]
        private bool _disableRetainTransformParent = true;
        public bool DisableRetainTransformParent
        {
            get => _disableRetainTransformParent;
            set
            {
                _disableRetainTransformParent = value;
                TryDisableGrabInteractableRetainTransformParent();
            }
        }

        /// <summary>
        /// The duration in seconds how long the put back object can be unselected outside the socket until being snapped back to the socket. 
        /// If less or equal to 0, the object will snap back instantaneous.
        /// </summary>
        [SerializeField]
        private float _putBackTime = 2.0f;
        public float putBackTime
        {
            get => _putBackTime;
            set => _putBackTime = value;
        }


        /// <summary>
        /// Prevents emitting the initial OnSelectEnter event after the socket is activated.
        /// </summary>
        [SerializeField]
        private bool _omitInitialSelectEnterEvent = true;
        public bool OmitInitialSelectEnterEvent
        {
            get => _omitInitialSelectEnterEvent;
            set => _omitInitialSelectEnterEvent = value;
        }


        /// <summary>
        /// Prevents emitting the initial OnSelectExit event after the socket is activated.
        /// </summary>
        [SerializeField]
        private bool _omitInitialSelectExitEvent = true;
        public bool OmitInitialSelectExitEvent
        {
            get => _omitInitialSelectExitEvent;
            set => _omitInitialSelectExitEvent = value;
        }


        /// <summary>
        /// Hidden in the editor!
        /// Used to disable certain fields in the editor when controlled by an Exhibition Display.
        /// </summary>
        [SerializeField]
        private bool _externallyControlled;
        public bool externallyControlled
        {
            get => _externallyControlled;
            set => _externallyControlled = value;
        }


        protected bool _omitSelectEnterEvent;
        protected bool _omitSelectExitEvent;

        private Coroutine putBackCoroutine;

        /// <summary>
        /// Emitted once an interactable has been put back automatically, but not by placing it back into the socket manually.
        /// </summary>
        public UnityEvent OnPutBack;


        /// <summary>
        /// Resets the putBackObject when rebuilding to prevent errors.
        /// Can be overwritten, but `base.OnEnable()` should be called to ensure correct behavior.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (!ArePutBackReferencesValid())
            {
                putBackPrefab = _putBackPrefab;
            }

            SetHighlighterVisible(_putBackInstance == null);

            if (_putBackInstance != null && _putBackInstance.TryGetComponent(out _putBackInteractable))
            {
                _putBackInteractable.selectExited.AddListener(StartPutBackTimer);
                _putBackInteractable.selectEntered.AddListener(ResetPutBackTimer);

                // Enable putBackInteractable if attached (avoids dropping if attached)
                _putBackInstance.SetActive(true);
            }

            // Prevent select enter event being emitted (must be done before base.Start() is called)
            _omitSelectEnterEvent = _omitInitialSelectEnterEvent;

            selectEntered.AddListener(HideHighlighter);
            selectExited.AddListener(ShowHighlighter);
        }


        protected override void Start()
        {
            base.Start();
            _omitSelectEnterEvent = false;
        }


        /// <summary>
        /// Determines if a `XRGrabInteractable` can hover, e.g. is considered a valid target.
        /// Can be overwritten, but `base.CanHover(interactable)` should be called to ensure correct behavior.
        /// </summary>
        protected override void OnDisable()
        {
            // Prevents select enter event being emitted (Must be done before disabling the interactable)
            _omitSelectExitEvent = _omitInitialSelectExitEvent;

            // Unlink and disable interactable before disabling this socket to avoid dropping
            if (_putBackInstance != null && _putBackInstance.TryGetComponent(out _putBackInteractable))
            {
                _putBackInteractable.selectExited.RemoveListener(StartPutBackTimer);
                _putBackInteractable.selectEntered.RemoveListener(ResetPutBackTimer);

                _putBackInstance.SetActive(false);
            }

            base.OnDisable();

            selectEntered.RemoveListener(HideHighlighter);
            selectExited.RemoveListener(ShowHighlighter);
            _omitSelectExitEvent = false;
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            if (!_omitSelectEnterEvent)
            {
                base.OnSelectEntered(args);
            }
        }


        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            if (!_omitSelectExitEvent)
            {
                base.OnSelectExited(args);
            }
        }

        /// <summary>
        /// Determines if a `XRGrabInteractable` can hover, i.e. is considered a valid target.
        /// Can be overwritten, but `base.CanHover(interactable)` should be called to ensure correct behavior.
        /// </summary>
        /// <param name="interactable">Interactable hovering.</param>
        /// <returns>If the interactable can hover.</returns>
        public override bool CanHover(IXRHoverInteractable interactable) => IsObjectMatch(interactable) && base.CanHover(interactable);

        /// <summary>
        /// Determines if a `XRGrabInteractable` can be selected, i.e. is considered a valid target.
        /// Can be overwritten, but `base.CanSelect(interactable)` should be called to ensure correct behavior.
        /// </summary>
        /// <param name="interactable">Interactable selecting</param>
        /// <returns>If the interactable can select.</returns>
        public override bool CanSelect(IXRSelectInteractable interactable) => IsObjectMatch(interactable) && base.CanSelect(interactable);

        private void StartPutBackTimer(SelectExitEventArgs args)
        {
            if (_putBackPrefab == null || _putBackInteractable == null || args.interactorObject == (IXRSelectInteractor)this)
            {
                // Do nothing if the interactable does not exists or is exiting this object, 
                // e.g. was picked up from socket
                return;
            }

            if (putBackCoroutine != null)
            {
                StopCoroutine(putBackCoroutine);
            }

            if (isActiveAndEnabled && _putBackTime <= 0)
            {
                // Put Object back
                interactionManager.SelectEnter(this, (IXRSelectInteractable)_putBackInteractable);
            }
            else if (isActiveAndEnabled)
            {
                putBackCoroutine = StartCoroutine(CreatePutBackCoroutine(_putBackTime));
            }
        }

        private void ResetPutBackTimer(SelectEnterEventArgs args)
        {
            if (putBackCoroutine != null)
            {
                StopCoroutine(putBackCoroutine);
            }
        }


        private IEnumerator CreatePutBackCoroutine(float duration)
        {
            yield return new WaitForSeconds(duration);
            if (_putBackPrefab != null && _putBackInteractable != null)
            {
                // Put Object back
                interactionManager.SelectEnter(this, (IXRSelectInteractable)_putBackInteractable);
                OnPutBack.Invoke();
            }
            putBackCoroutine = null;
            SetHighlighterVisible(false);
        }

        private bool IsObjectMatch(IXRInteractable interactable)
            => _putBackInteractable != null && interactable.transform.gameObject == _putBackInteractable.transform.gameObject;


        /// <summary>
        /// Updates the `putBackPrefab` by destroying and creating instances, adding/removing listeners and de-/selecting the interactable. 
        /// Will be automatically called when setting `putBackPrefab`.
        /// </summary>
        public void UpdatePutBackObject()
        {
            if (!ValidatePutBackPrefab())
            {
                putBackPrefab = null;
                return;
            }

            UnregisterPutBackInteractable();
            DeleteOldPutBackInstance();
            InstantiatePutBackPrefab();
            RegisterPutBackInteractable();
        }


        private void DeleteOldPutBackInstance()
        {
            if (_putBackInstance != null)
            {
                // Destroy Interactable
                if (Application.isPlaying)
                {
                    interactionManager.UnregisterInteractable((IXRInteractable)_putBackInteractable);
                    // Don't know why we need to wait here
                    Destroy(_putBackInstance, 0.1f);
                }
                else
                {
                    DestroyImmediate(_putBackInstance);
                }
            }
        }

        private void UnregisterPutBackInteractable()
        {
            if (_putBackInteractable != null)
            {
                _putBackInteractable.selectExited.RemoveListener(StartPutBackTimer);
                _putBackInteractable.selectEntered.RemoveListener(ResetPutBackTimer);

                startingSelectedInteractable = null;
                _putBackInteractable = null;
            }
        }


        private void RegisterPutBackInteractable()
        {
            if (_putBackInstance != null && _putBackInstance.TryGetComponent(out _putBackInteractable))
            {
                startingSelectedInteractable = _putBackInteractable;

                if (interactionManager != null && ((IXRSelectInteractable)_putBackInteractable) != null && Application.isPlaying)
                {
                    interactionManager.SelectEnter(this, (IXRSelectInteractable)_putBackInteractable);
                }

                _putBackInteractable.selectExited.AddListener(StartPutBackTimer);
                _putBackInteractable.selectEntered.AddListener(ResetPutBackTimer);
            }
        }


        private void InstantiatePutBackPrefab()
        {
            if (_putBackPrefab != null)
            {
                Transform attachParent = attachTransform != null ? attachTransform : transform;
                _putBackInstance = Instantiate(_putBackPrefab, attachParent);

                if (_putBackInstance != null && _putBackInstance.TryGetComponent(out _putBackInteractable))
                {
                    _putBackInstance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                    UpdatePutbackAttachCompensation();
                    TryDisableGrabInteractableRetainTransformParent();
                }
                else if (_putBackInstance != null && allowNonInteractables)
                {
                    _putBackInstance.transform.SetPositionAndRotation(attachParent.position, attachParent.rotation);
                }
                else
                {
                    Debug.LogError("Can't attach PutBackPrefab, it is not an XRGrabInteractable. "
                                    + "If you want to attach regular GameObjects without being able "
                                    + "to pick them up enable: 'allowNonInteractables'.", this);
                    // Clean up invalid instance if existing
                    if (_putBackInstance != null)
                    {
#if UNITY_EDITOR
                        DestroyImmediate(_putBackInstance);
#else
                        Destroy(_putBackInstance);
#endif
                    }
                    putBackPrefab = null;
                }
            }

            // Hide the highlighter in editor
            SetHighlighterVisible(showHighlighter && _putBackInstance == null);
        }


        public void UpdatePutbackAttachCompensation()
        {
            if (!_compensateInteractableAttach)
            {
                return;
            }

            if (!attachTransform)
            {
                Debug.LogWarning("Can't compensate the attach for the PutBackPrefab because it requires an Attach but none is set.", this);
                return;
            }

            if (_putBackInteractable)
            {
                attachTransform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity); // Reset attach to avoid continuous shifting
                Transform otherAttach = _putBackInteractable.GetAttachTransform(this);
                attachTransform.SetPositionAndRotation(otherAttach.position, otherAttach.rotation);
            }
        }


        private void TryDisableGrabInteractableRetainTransformParent()
        {
            if (_disableRetainTransformParent)
            {
                XRGrabInteractable grabInteractable = _putBackInteractable as XRGrabInteractable;
                if (grabInteractable != null)
                {
                    grabInteractable.retainTransformParent = false;
                }
            }
        }


        /// <summary>
        /// Checks if all references derived from the putBackPrefab are valid.
        /// </summary>
        /// <returns>If all references are valid.</returns>
        public bool ArePutBackReferencesValid()
        {
            bool hasPrefab = _putBackPrefab != null;
            bool hasInstance = _putBackInstance != null;
            bool hasInteractable = _putBackInteractable != null;
            bool interactableMatchesPrefab = hasPrefab && _putBackPrefab.TryGetComponent(out XRBaseInteractable _) == (_putBackInteractable != null);
            bool hasInteractableWhenRequired = allowNonInteractables || interactableMatchesPrefab;

            return !hasPrefab && !hasInstance && !hasInteractable // Does not exist
                || hasPrefab && hasInstance && hasInteractableWhenRequired; // Exist
        }


        /// <summary>
        /// Checks if the putBackPrefab is valid with respect to the current configuration.
        /// </summary>
        /// <returns>If the putBackPrefab is valid.</returns>
        private bool ValidatePutBackPrefab()
        {
#if UNITY_EDITOR
            if (_putBackPrefab != null && !PrefabUtility.IsPartOfPrefabAsset(_putBackPrefab))
            {
                Debug.LogWarning($"It is strongly recommended to set the putBackPrefab '{_putBackPrefab}' "
                    + "to a prefab saved on the disk and not a GameObject from within the scene.");
            }
#endif
            if (_putBackPrefab != null && !_allowNonInteractables && !_putBackPrefab.TryGetComponent<XRBaseInteractable>(out _))
            {
                Debug.LogWarning($"Can't attach '{_putBackPrefab}' to this socket as it is not an interactable and can be used as putBackPrefab. "
                    + "If you want to allow non-interactables enable 'allowInteractables'.", this);
                return false;
            }

            return true;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            CompensateInteractableAttach = CompensateInteractableAttach;
        }
    }
}