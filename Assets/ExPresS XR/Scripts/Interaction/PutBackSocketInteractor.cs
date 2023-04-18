using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;

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

        [Tooltip("If true GameObjects will be added to the socket but won't be able to be picked up")]
        [SerializeField]
        private bool _allowNonInteractables;
        public bool allowNonInteractables
        {
            get => _allowNonInteractables;
            set => _allowNonInteractables = value;
        }

        [SerializeField]
        private GameObject _putBackObjectInstance;
        public GameObject putBackObjectInstance
        {
            get => _putBackObjectInstance;
        }

        private XRGrabInteractable _putBackInteractable;
        public XRGrabInteractable putBackInteractable
        {
            get => _putBackInteractable;
        }


        [SerializeField]
        private float _putBackTime;
        public float putBackTime
        {
            get => _putBackTime;
            set => _putBackTime = value;
        }

        private Coroutine putBackCoroutine;


        protected override void Awake()
        {
            base.Awake();

            socketActive = _putBackObjectInstance != null;
        }



        protected override void OnEnable()
        {
            // Calling this in OnEnable (instead of Awake) will also reset the putBackObject when rebuilding
            putBackPrefab = _putBackPrefab;
            socketActive = _putBackObjectInstance != null;
            SetHighlighterVisible(_putBackObjectInstance == null);
        }

        public override bool CanHover(IXRHoverInteractable interactable)
        {
            return base.CanHover(interactable) && IsObjectMatch(interactable);
        }

        public override bool CanSelect(IXRSelectInteractable interactable)
        {
            return base.CanSelect(interactable) && IsObjectMatch(interactable);
        }

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
            else
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
            // EndManualInteraction();
            putBackCoroutine = null;
        }

        private bool IsObjectMatch(IXRInteractable interactable)
        {
            return _putBackInteractable != null && interactable.transform.gameObject == _putBackInteractable.gameObject;
        }

        public override void SetHighlighterVisible(bool visible)
        {
            base.SetHighlighterVisible(_putBackObjectInstance == null && visible);
        }


        public void UpdatePutBackObject()
        {
            DeletePutBackObjectInstance();
            UnregisterPutBackInteractable();
            InstantiateAndRegisterPutBackPrefab();
        }


        private void DeletePutBackObjectInstance()
        {
            if (_putBackObjectInstance != null)
            {
                // Destroy Interactable
                if (Application.isPlaying)
                {
                    Destroy(_putBackObjectInstance);
                }
                else
                {
                    DestroyImmediate(_putBackObjectInstance);
                }
            }
        }

        private void UnregisterPutBackInteractable()
        {
            if (_putBackInteractable != null)
            {
                // Unregister interactable
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

        private void InstantiateAndRegisterPutBackPrefab()
        {
            if (_putBackPrefab != null)
            {
                _putBackObjectInstance = Instantiate(_putBackPrefab, transform);

                if (_putBackObjectInstance.TryGetComponent(out _putBackInteractable))
                {
                    startingSelectedInteractable = _putBackInteractable;
                    if (Application.isPlaying)
                    {
                        interactionManager.SelectEnter(this, (IXRSelectInteractable)_putBackInteractable);
                    }

                    _putBackObjectInstance.transform.position = transform.position;
                    _putBackObjectInstance.transform.SetParent(transform);

                    _putBackInteractable.selectExited.AddListener(StartPutBackTimer);
                    _putBackInteractable.selectEntered.AddListener(ResetPutBackTimer);
                }
                else if (allowNonInteractables)
                {
                    // Debug.Log("PutBackPrefab it is not an XRGrabInteractable, you won't be able to pick it up");
                    _putBackObjectInstance.transform.position = transform.position;
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
            SetHighlighterVisible(showHighlighter && _putBackObjectInstance == null);
        }
    }
}