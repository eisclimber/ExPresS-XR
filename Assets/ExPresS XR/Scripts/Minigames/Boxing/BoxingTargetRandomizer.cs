using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.Boxing
{
    public class BoxingTargetRandomizer : MonoBehaviour
    {
        [SerializeField]
        private BoxingTargetArea[] _targets;
        public BoxingTargetArea[] Targets
        {
            get => _targets;
            set => _targets = value;
        }

        [Space]

        [SerializeField]
        private bool _autoStart;
        public bool AutoStart
        {
            get => _autoStart;
            set => _autoStart = value;
        }

        [SerializeField]
        private bool _cancelActiveTargets = true;

        [SerializeField]
        private bool _hideTargetsInitially = true;

        [SerializeField]
        private bool _hideTargetsOnDisabled = true;

        [SerializeField]
        private float _minDelay = 1.0f;

        [SerializeField]
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

        [ContextMenu("Start Target Randomization")]
        public void StartTargetRandomization()
        {
            float delay = Random.Range(_minDelay, _maxDelay);
            SetTargetsVisible(true);
            StopTargetRandomization();
            _waitForSpawnCoroutine = StartCoroutine(RandomizeNextActivatedTargetDelayed(delay));
        }

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

        public void DeactivateAllTargets()
        {
            for (int i = 0; i < _targets.Length; i++)
            {
                _targets[i].TargetActive = false;
            }
        }

        public void SetTargetsVisible(bool visible)
        {
            for (int i = 0; i < _targets.Length; i++)
            {
                _targets[i].gameObject.SetActive(visible);
            }
        }

        public IEnumerator RandomizeNextActivatedTargetDelayed(float delay)
        {
            RandomizeActivatedTarget();
            yield return new WaitForSeconds(delay);
            StartTargetRandomization();
        }
    }
}