
using System.Collections;
using ExPresSXR.Misc;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace ExPresSXR.Minigames.Archery.Bow
{
    public class BowString : XRBaseInteractable
    {
        /*------------------------------------------------------------------------------------
        This Script handles the String-Bow interaction.
        The original idea is inspired by https://fistfullofshrimp.com/unity-vr-bow-and-arrow-part-1/
        -----------------------------------------------------------------------------------
        */

        /// <summary>
        /// Start position of the draw. I.e. minimal draw position.
        /// </summary>
        [SerializeField]
        [Tooltip("Start position of the draw. I.e. minimal draw position.")]
        private Transform _startPosition;

        /// <summary>
        /// End position of the draw. I.e. maximal draw position.
        /// </summary>
        [SerializeField]
        [Tooltip("End position of the draw. I.e. maximal draw position.")]
        private Transform _endPosition;

        /// <summary>
        /// Anchor for the arrow and bend point on the line.
        /// </summary>
        [SerializeField]
        [Tooltip("Anchor for the arrow and bend point on the line.")]
        private Transform _anchor;

        /// <summary>
        /// LineRender representing the bow string.
        /// </summary>
        [SerializeField]
        [Tooltip("LineRender representing the bow string.")]
        private LineRenderer _lineRenderer;

        /// <summary>
        /// Reference for the loaded arrow visuals.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference for the loaded arrow visuals.")]
        private GameObject _arrowLoaded;

        /// <summary>
        /// Pull string sound.
        /// </summary>
        [SerializeField]
        [Tooltip("Pull string sound.")]
        private AudioClip _pullStringSound;

        /// <summary>
        /// AudioSource to play the pull sound.
        /// </summary>
        [SerializeField]
        [Tooltip("AudioSource to play the pull sound.")]
        private AudioSource _audioSource;

        // For debugging
        [Space]

        /// <summary>
        /// Will shoot arrows automatically. For debugging.
        /// </summary>
        [SerializeField]
        [Tooltip("Will shoot arrows automatically. For debugging.")]
        private bool _autoShot;
        public bool AutoShot
        {
            get => _autoShot;
            set
            {
                _autoShot = value;
                UpdateAutoShot();
            }
        }

        /// <summary>
        /// Time of arrows shot automatically, if enabled. For debugging.
        /// </summary>
        [SerializeField]
        [Tooltip("Time of arrows shot automatically, if enabled. For debugging.")]
        private float _autoShotFrequency = 1.0f;

        /// <summary>
        /// Current pull strength. For debugging.
        /// </summary>
        [ReadonlyInInspector]
        [SerializeField]
        [Tooltip("Current pull strength. For debugging.")]
        private float _pullStrength = 0.0f;
        public float PullStrength
        {
            get => _pullStrength;
        }


        private IXRSelectInteractor _stringInteractor;
        private Coroutine _autoShotCoroutine;

        // Events

        /// <summary>
        /// Emitted when the string is released.
        /// </summary>
        public UnityEvent<float> OnStringReleased;

        /// <summary>
        /// Loads the required component and evaluates the LineRenderers setup.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            if (_lineRenderer == null && !TryGetComponent(out _lineRenderer))
            {
                Debug.LogError("No LineRender provided, bowstring will not be shown nor updated!", this);
            }
            else if (_lineRenderer.positionCount != 3)
            {
                Debug.LogWarning("The LineRenderer does not have at least the suggested three points to represent the bowstring.", this);
            }

            if (_audioSource == null && !TryGetComponent(out _audioSource))
            {
                Debug.LogError("No AudioSource to play the pull sound found.");
            }
        }

        /// <summary>
        /// Start auto shooting if enabled.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateAutoShot();
        }


        /// <summary>
        /// Stop auto shooting if enabled.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            UpdateAutoShot();
        }

        /// <summary>
        /// Shows the drawn arrow and plays the draw sound. 
        /// </summary>
        /// <param name="args">Args of the select enter.</param>
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            _stringInteractor = args.interactorObject;
            if (_pullStringSound != null)
            {
                _audioSource.PlayOneShot(_pullStringSound, 1.0f);
            }
            _arrowLoaded.SetActive(true);
        }

        /// <summary>
        /// Resets the visuals and emits the string released event with the given strength.
        /// </summary>
        /// <param name="args">Args of the select exit.</param>
        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            _stringInteractor = null;
            OnStringReleased?.Invoke(_pullStrength);
            _arrowLoaded.SetActive(false);
            _pullStrength = 0.0f;
            UpdateVisuals();
        }

        /// <summary>
        /// Update the current pullStrength and visuals of the bow string.
        /// </summary>
        /// <param name="updatePhase">Update phase when the interactor is update.</param>
        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic && isSelected)
            {
                Vector3 pullPosition = _stringInteractor.transform.position;
                _pullStrength = CalculatePull(pullPosition);
                UpdateVisuals();
            }
        }

        private float CalculatePull(Vector3 pullPosition)
        {
            Vector3 pullDir = pullPosition - _startPosition.position;
            Vector3 targetDir = (_endPosition.position - _startPosition.position).normalized;
            float pullValue = Vector3.Dot(pullDir, targetDir) / targetDir.magnitude;
            return Mathf.Clamp(pullValue, 0, 1);
        }

        private void UpdateVisuals()
        {
            float pullZPos = Mathf.Lerp(_startPosition.localPosition.z, _endPosition.localPosition.z, _pullStrength);
            float bendYPos = _lineRenderer.GetPosition(1).y;
            Vector3 line = new(0, bendYPos, pullZPos);
            _lineRenderer.SetPosition(1, line);
            _anchor.localPosition = new Vector3(line.x, line.y, pullZPos);
        }

        // Auto shot
        private void EmitStringReleasedEvent() => OnStringReleased?.Invoke(1.0f);

        private void UpdateAutoShot()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            if (!_autoShot && _autoShotCoroutine != null)
            {
                StopCoroutine(_autoShotCoroutine);
            }
            
            if (_autoShot)
            {
                _autoShotCoroutine = StartCoroutine(AutoShootLoop());
            }
        }

        private IEnumerator AutoShootLoop()
        {
            yield return new WaitForSeconds(_autoShotFrequency);
            EmitStringReleasedEvent();

            if (_autoShot)
            {
                _autoShotCoroutine = StartCoroutine(AutoShootLoop());
            }
        }
    }
}
