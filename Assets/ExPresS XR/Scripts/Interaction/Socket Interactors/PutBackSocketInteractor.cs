using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEditor;
using ExPresSXR.Misc;

namespace ExPresSXR.Interaction
{
    public class PutBackSocketInteractor : HighlightableSocketInteractor
    {
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

        [SerializeField]
        private GameObject _putBackInstance;
        public GameObject putBackObjectInstance
        {
            get => _putBackInstance;
        }

        [SerializeField]
        private XRBaseInteractable _putBackInteractable;
        public XRBaseInteractable putBackInteractable
        {
            get => _putBackInteractable;
        }


        [Tooltip("If true GameObjects will be added to the socket but won't be able to be picked up")]
        [SerializeField]
        private bool _allowNonInteractables;
        public bool allowNonInteractables
        {
            get => _allowNonInteractables;
            set => _allowNonInteractables = value;
        }


        [SerializeField]
        private float _putBackTime = 1.0f;
        public float putBackTime
        {
            get => _putBackTime;
            set => _putBackTime = value;
        }

        private Coroutine putBackCoroutine;


        protected override void Awake()
        {
            base.Awake();

            socketActive = _putBackInstance != null;
        }

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

        protected override void OnDisable()
        {
            base.OnDisable();
            UnregisterPutBackInteractable();

            selectEntered.RemoveListener(HideHighlighter);
            selectExited.RemoveListener(ShowHighlighter);
        }

        public override bool CanHover(IXRHoverInteractable interactable) => IsObjectMatch(interactable) && base.CanHover(interactable);

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