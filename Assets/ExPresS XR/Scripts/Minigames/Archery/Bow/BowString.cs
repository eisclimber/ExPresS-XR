
using ExPresSXR.Misc;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace ExPresSXR.Minigames.Archery
{
    public class BowString : XRBaseInteractable
    {
        /*------------------------------------------------------------------------------------
        This Script handles the String-Bow interaction.
        The original idea is inspired by https://fistfullofshrimp.com/unity-vr-bow-and-arrow-part-1/
        -----------------------------------------------------------------------------------
        */
        // setting the pull amount for the bow
        [SerializeField]
        [Tooltip("Start of the draw.")]
        private Transform _startPosition;

        [SerializeField]
        [Tooltip("End position of the draw.")]
        private Transform _endPosition;

        [SerializeField]
        [Tooltip("Anchor on the line.")]
        private Transform _anchor;

        [SerializeField]
        [Tooltip("LineRender for the bowstring.")]
        private LineRenderer _lineRenderer;

        [SerializeField]
        [Tooltip("Reference for the loaded arrow visuals.")]
        private GameObject _arrowLoaded;

        [SerializeField]
        [Tooltip("Pull String Sound")]
        private AudioClip _pullStringSound;

        [SerializeField]
        [Tooltip("AudioSource to play the pull sound.")]
        private AudioSource _audioSource;
        
        // For debugging
        [Space]

        [SerializeField]
        [Tooltip("Will shoot arrows automatically. For debugging.")]
        private bool _autoShot;

        [ReadonlyInInspector]
        [SerializeField]
        private float _pullStrength = 0.0f;
        public float PullStrength
        {
            get => _pullStrength;
        }

        
        private IXRSelectInteractor _stringInteractor;

        [Tooltip("Event when String is released")]
        public UnityEvent<float> OnStringReleased;


        protected override void Awake()
        {
            base.Awake();
            if (!_lineRenderer && !TryGetComponent(out _lineRenderer))
            {
                Debug.LogError("No LineRender provided, bowstring will not be shown nor updated!", this);
            }
            else if (_lineRenderer.positionCount != 3)
            {
                Debug.LogWarning("The LineRenderer does not have at least the suggested three points to represent the bowstring.", this);
            }

            if (!TryGetComponent(out _audioSource))
            {
                Debug.LogError("No AudioSource to play the pull sound found.");
            }

            if (_autoShot)
            {
                InvokeRepeating(nameof(EmitStringReleasedEvent), 0.0f, 1.0f);
            }
        }

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

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            _stringInteractor = null;
            OnStringReleased?.Invoke(_pullStrength);
            _arrowLoaded.SetActive(false);
            _pullStrength = 0.0f;
            UpdateVisuals();
        }

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

        private void EmitStringReleasedEvent() => OnStringReleased?.Invoke(1.0f);
        private void EmitStringReleasedEvent(float strength) => OnStringReleased?.Invoke(strength);
    }
}
