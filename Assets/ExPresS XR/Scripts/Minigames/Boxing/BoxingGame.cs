using ExPresSXR.Minigames.TargetArea;
using ExPresSXR.Misc;
using ExPresSXR.Misc.Timing;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.Boxing
{
    public class BoxingGame : MonoBehaviour
    {
        [SerializeField]
        private TargetAreaTriggerer[] _targetTriggerers;

        [SerializeField]
        private BoxingTargetArea[] _targets;

        [SerializeField]
        private BoxingTargetRandomizer _targetRandomizer;

        [SerializeField]
        private Timer _timer;

        [SerializeField]
        private bool _autoStart;

        [SerializeField]
        [ReadonlyInInspector]
        private int _currentScore;
        public int CurrentScore
        {
            get => _currentScore;
            protected set
            {
                _currentScore = value;
                OnScoreChanged.Invoke(_currentScore);
            }
        }

        public UnityEvent OnStarted;
        public UnityEvent<int> OnScoreChanged;
        public UnityEvent OnCompleted;

        public void OnEnable()
        {
            if (_targetTriggerers.Length <= 0)
            {
                Debug.LogError("No TargetAreaTriggerers set up to trigger the boxing areas.", this);
            }

            if (_targets.Length <= 0)
            {
                Debug.LogError("No TargeAreas provided. Can't hit anything.", this);
            }

            foreach (TargetAreaTriggerer triggerer in _targetTriggerers)
            {
                triggerer.Targets = _targets;
            }

            foreach (BoxingTargetArea target in _targets)
            {
                target.OnPointsScored.AddListener(HandlePointsScored);
            }

            if (_targetRandomizer != null)
            {
                _targetRandomizer.Targets = _targets;

                if (_targetRandomizer.AutoStart)
                {
                    Debug.LogWarning("AutoStart was set enabled on the BoxingTargetRandomizer. Deactivating it to prevent interference...");
                    _targetRandomizer.AutoStart = false;
                }
            }
            else
            {
                Debug.LogError("No TargeAreaRandomizer provided which is needed for reactivating the targets.", this);
            }

            if (_timer != null)
            {
                _timer.OnTimeout.AddListener(EndGame);
            }
        }

        public void OnDisable()
        {
            foreach (BoxingTargetArea target in _targets)
            {
                target.OnPointsScored.RemoveListener(HandlePointsScored);
            }

            if (_timer != null)
            {
                _timer.OnTimeout.RemoveListener(EndGame);
            }
            EndGame();
        }

        public void Start()
        {
            if (_autoStart)
            {
                StartGame();
            }
        }

        [ContextMenu("Start Game")]
        public void StartGame()
        {
            _targetRandomizer.StartTargetRandomization();
            _timer.StartTimerDefault();
            OnStarted.Invoke();
        }

        [ContextMenu("End Game")]
        public void EndGame()
        {
            _targetRandomizer.StopTargetRandomization();
            _timer.StopTimer();
            OnCompleted.Invoke();
        }

        private void HandlePointsScored(int points) => CurrentScore += points;

        private void OnValidate()
        {
            if (_targetRandomizer != null && _targetRandomizer.AutoStart)
            {
                Debug.LogWarning("AutoStart was set enabled on the BoxingTargetRandomizer. Deactivating it to prevent interference...");
                _targetRandomizer.AutoStart = false;
            }
        }
    }
}