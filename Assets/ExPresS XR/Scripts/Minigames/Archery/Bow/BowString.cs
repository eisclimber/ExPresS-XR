
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
        [SerializeField]
        private float _pullStrength = 0.0f;
        public float PullStrength {
            get => _pullStrength;
        }

        // setting the pull amount for the bow
        [SerializeField]
        [Tooltip("startposition")]
        private Transform startPosition;

        [SerializeField]
        [Tooltip("endposition")]
        private Transform endPosition;

        [SerializeField]
        [Tooltip("Anchor on the line")]
        private Transform anchor;

        [SerializeField]
        [Tooltip("Prefab for the locked Arrow (locked in bow)")]
        private GameObject _arrowLockedPrefab;

        [SerializeField]
        [Tooltip("Pull String Sound")]
        private AudioClip _pullStringSound;

        [SerializeField]
        [Tooltip("Event when String is released")]
        public UnityEvent<float> OnStringReleased;

        private IXRSelectInteractor _stringInteractor = null;
        private AudioSource _audioSource;

        // For debugging
        [SerializeField]
        [Tooltip("Debugging -> arrows will fly automatically")]
        private bool _debug;
        private int _badTimer = 0;

        private void Update()
        {
            if (_debug && _badTimer > 100)
            {
                OnStringReleased?.Invoke(0.5f);
                _badTimer = 0;
            }
            else
            {
                _badTimer += 1;
            }
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            _stringInteractor = args.interactorObject;
            _audioSource = GetComponent<AudioSource>();
            if (_pullStringSound != null)
            {
                _audioSource.PlayOneShot(_pullStringSound, 0.5f);
            }
            _arrowLockedPrefab.SetActive(true);
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            _stringInteractor = null;
            OnStringReleased?.Invoke(_pullStrength);
            _arrowLockedPrefab.SetActive(false);
            _pullStrength = 0f;
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
            Vector3 pullDir = pullPosition - startPosition.position;
            Vector3 targetDir = endPosition.position - startPosition.position;
            float max = targetDir.magnitude;

            targetDir.Normalize();

            float pullValue = Vector3.Dot(pullDir, targetDir) / max;
            return Mathf.Clamp(pullValue, 0, 1);
        }

        private void UpdateVisuals()
        {
            Vector3 line = new Vector3(1, 0, 0) * Mathf.Lerp(startPosition.localPosition.x, endPosition.localPosition.x, _pullStrength);

            LineRenderer _lineRenderer = GetComponent<LineRenderer>();
            line += new Vector3(0, _lineRenderer.GetPosition(1).y, 0);
            _lineRenderer.SetPosition(1, line);
            anchor.localPosition = new Vector3(line.x, anchor.transform.localPosition.y, line.z);
        }
    }
}
