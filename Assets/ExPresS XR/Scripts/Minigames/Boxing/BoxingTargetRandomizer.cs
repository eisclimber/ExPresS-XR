using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.Boxing
{
    /// <summary>
    /// Randomly activates one of the BoxingTargetAreas.
    /// </summary>
    public class BoxingTargetRandomizer : MonoBehaviour
    {
        /// <summary>
        /// Targets to randomize.
        /// </summary>
        [SerializeField]
        [Tooltip("Targets to randomize.")]
        private BoxingTargetArea[] _targets;
        public BoxingTargetArea[] Targets
        {
            get => _targets;
            set => _targets = value;
        }

        [Space]

        /// <summary>
        /// If the randomization should start automatically.
        /// </summary>
        [SerializeField]
        [Tooltip("If the randomization should start automatically.")]
        private bool _autoStart;
        public bool AutoStart
        {
            get => _autoStart;
            set => _autoStart = value;
        }

        /// <summary>
        /// If active targets should be canceled when activating a new one.
        /// </summary>
        [SerializeField]
        [Tooltip("If active targets should be canceled when activating a new one.")]
        private bool _cancelActiveTargets = true;

        /// <summary>
        /// If all targets should be set hidden initially.
        /// </summary>
        [SerializeField]
        [Tooltip("If all targets should be set hidden initially.")]
        private bool _hideTargetsInitially = true;

        /// <summary>
        /// If all targets should be set hidden when the randomizer gets disabled.
        /// </summary>
        [SerializeField]
        [Tooltip("If all targets should be set hidden when the randomizer gets disabled.")]
        private bool _hideTargetsOnDisabled = true;

        /// <summary>
        /// Minimum delay for randomization.
        /// </summary>
        [SerializeField]
        [Tooltip("Minimum delay for randomization.")]
        private float _minDelay = 1.0f;

        /// <summary>
        /// Maximum delay for randomization.
        /// </summary>
        [SerializeField]
        [Tooltip("Maximum delay for randomization.")]
        private float _maxDelay = 3.0f;

        private Coroutine _waitForSpawnCoroutine;

        public UnityEvent<int> OnTargetActivate;


        private void Awake()
        {
            if (_hideTargetsInitially)
            {
                if (!_autoStart)
                {
                    SetTargetsVisible(true);
                }
                else
                {
                    Debug.LogWarning("Ignoring 'Hide Targets Initially' since auto start is enabled.");
                }
            }

            if (_autoStart)
            {
                StartTargetRandomization();
            }
        }

        private void OnEnable()
        {
            foreach (BoxingTargetArea targetArea in _targets)
            {
                targetArea.OnActionPerformed.AddListener(StartTargetRandomization);
                targetArea.OnFailed.AddListener(StartTargetRandomization);
            }
        }

        private void OnDisable()
        {
            foreach (BoxingTargetArea targetArea in _targets)
            {
                targetArea.OnActionPerformed.RemoveListener(StartTargetRandomization);
                targetArea.OnFailed.RemoveListener(StartTargetRandomization);
            }

            StopTargetRandomization();

            if (_hideTargetsOnDisabled)
            {
                SetTargetsVisible(true);
            }
        }

        /// <summary>
        /// Starts randomization.
        /// </summary>
        [ContextMenu("Start Target Randomization")]
        public void StartTargetRandomization()
        {
            float delay = Random.Range(_minDelay, _maxDelay);
            SetTargetsVisible(true);
            StopTargetRandomization();
            _waitForSpawnCoroutine = StartCoroutine(RandomizeNextActivatedTargetDelayed(delay));
        }

        /// <summary>
        /// Stosp randomization.
        /// </summary>
        [ContextMenu("Stop Target Randomization")]
        public void StopTargetRandomization()
        {
            if (_waitForSpawnCoroutine != null)
            {
                StopCoroutine(_waitForSpawnCoroutine);
            }
        }

        private void RandomizeActivatedTarget()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            int nextIdx = Random.Range(0, _targets.Length);
            for (int i = 0; i < _targets.Length; i++)
            {
                bool isNewActivation = i == nextIdx;
                // Change only to activate or if they should be canceled
                if (isNewActivation || _cancelActiveTargets)
                {
                    _targets[i].TargetActive = isNewActivation;
                }
            }
            OnTargetActivate.Invoke(nextIdx);
        }

        /// <summary>
        /// Forcefully deactivates all targets. No events will be emitted.
        /// </summary>
        public void DeactivateAllTargets()
        {
            for (int i = 0; i < _targets.Length; i++)
            {
                _targets[i].TargetActive = false;
            }
        }

        /// <summary>
        /// Controlls the visibility of all targets
        /// </summary>
        /// <param name="visible">If it should be set to visible or not.</param>
        public void SetTargetsVisible(bool visible)
        {
            for (int i = 0; i < _targets.Length; i++)
            {
                _targets[i].gameObject.SetActive(visible);
            }
        }

        private IEnumerator RandomizeNextActivatedTargetDelayed(float delay)
        {
            RandomizeActivatedTarget();
            yield return new WaitForSeconds(delay);
            StartTargetRandomization();
        }
    }
}