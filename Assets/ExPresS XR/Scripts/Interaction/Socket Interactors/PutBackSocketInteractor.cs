using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEditor;
using ExPresSXR.Misc;

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
        /// The duration in seconds how long the put back object can be unselected outside the socket until being snapped back to the socket. 
        /// If less or equal to 0, the object will snap back instantaneous.
        /// </summary>
        [SerializeField]
        private float _putBackTime = 1.0f;
        public float putBackTime
        {
            get => _putBackTime;
            set => _putBackTime = value;
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


        private Coroutine putBackCoroutine;


        /// <summary>
        /// Deactivates the socket if no `putBackObjectInstance` could be created and sets up the inherited socket classes. 
        /// Can be overwritten, but `base.Awake()` should be called to ensure correct behavior.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            socketActive = _putBackInstance != null;
        }


        /// <summary>
        /// Resets the putBackObject when rebuilding to prevent errors.
        /// Can be overwritten, but `base.OnEnable()` should be called to ensure correct behavior.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            // Calling this in OnEnable (instead of Awake) will also reset the putBackObject when rebuilding
            if (!ArePutBackReferencesValid())
            {
                putBackPrefab = _putBackPrefab;
            }

            socketActive = _putBackInstance != null;
            SetHighlighterVisible(_putBackInstance == null);

            selectEntered.AddListener(HideHighlighter);
            selectExited.AddListener(ShowHighlighter);
        }


        /// <summary>
        /// Determines if a `XRGrabInteractable` can hover, e.g. is considered a valid target.
        /// Can be overwritten, but `base.CanHover(interactable)` should be called to ensure correct behavior.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            UnregisterPutBackInteractable();

            selectEntered.RemoveListener(HideHighlighter);
            selectExited.RemoveListener(ShowHighlighter);
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
            if (_putBackInteractable == null || args.interactorObject == (IXRSelectInteractor)this)
            {
                // Do nothing if the interactable does not exists or is exiting this object, 
                // e.g. was picked up from socket
                return;
            }

            if (putBackCoroutine != null)
            {
                StopCoroutine(putBackCoroutine);
            }

            if (_putBackTime <= 0)
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
            // Put Object back
            interactionManager.SelectEnter(this, (IXRSelectInteractable)_putBackInteractable);
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
                    Destroy(_putBackInstance);
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

                if (Application.isPlaying && _putBackInteractable.isSelected)
                {
                    interactionManager.SelectExit(this, (IXRSelectInteractable)_putBackInteractable);
                }

                startingSelectedInteractable = null;
                _putBackInteractable = null;
            }
        }


        private void RegisterPutBackInteractable()
        {
            if (_putBackInstance != null && _putBackInstance.TryGetComponent(out _putBackInteractable))
            {
                _putBackInteractable.selectExited.AddListener(StartPutBackTimer);
                _putBackInteractable.selectEntered.AddListener(ResetPutBackTimer);

                if (interactionManager != null && Application.isPlaying)
                {
                    interactionManager.SelectEnter(this, (IXRSelectInteractable)_putBackInteractable);
                }

                startingSelectedInteractable = _putBackInteractable;
            }
        }


        private void InstantiatePutBackPrefab()
        {
            if (_putBackPrefab != null)
            {
                Transform attachParent = attachTransform != null ? attachTransform : transform;
                _putBackInstance = Instantiate(_putBackPrefab, attachParent);

                if (_putBackInstance.TryGetComponent(out _putBackInteractable))
                {
                    if (interactionManager != null && Application.isPlaying)
                    {
                        interactionManager.SelectEnter(this, (IXRSelectInteractable)_putBackInteractable);
                    }

                    _putBackInstance.transform.SetPositionAndRotation(attachParent.position, Quaternion.identity);
                }
                else if (allowNonInteractables)
                {
                    _putBackInstance.transform.SetPositionAndRotation(attachParent.position, attachParent.rotation);
                }
                else
                {
                    Debug.LogError("Can't attach PutBackPrefab, it is not an XRGrabInteractable. "
                                    + "If you want to attach regular GameObjects without being able "
                                    + "to pick them up enable: 'allowNonInteractables'.");
                    putBackPrefab = null;
                }
            }

            // Hide the highlighter in editor
            SetHighlighterVisible(showHighlighter && _putBackInstance == null);
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
            bool hasInteractableWhenRequired = allowNonInteractables && interactableMatchesPrefab;

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
    }
}