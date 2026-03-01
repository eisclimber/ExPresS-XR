using ExPresSXR.Minigames.TargetArea;
using ExPresSXR.Misc;
using ExPresSXR.Misc.Timing;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.Boxing
{
    /// <summary>
    /// Main logic for the boxing game.
    /// </summary>
    public class BoxingGame : MonoBehaviour
    {
        /// <summary>
        /// TargetAreaTriggerers used by this game (i.e. the hands).
        /// </summary>
        [SerializeField]
        [Tooltip("TargetAreaTriggerers used by this game (i.e. the hands).")]
        private TargetAreaTriggerer[] _targetTriggerers;

        /// <summary>
        /// Boxing targets for this game.
        /// </summary>
        [SerializeField]
        [Tooltip("Boxing targets for this game.")]
        private BoxingTargetArea[] _targets;

        /// <summary>
        /// Randomizer for managing the activation of the boxing targets.
        /// </summary>
        [SerializeField]
        [Tooltip("Randomizer for managing the activation of the boxing targets.")]
        private BoxingTargetRandomizer _targetRandomizer;

        /// <summary>
        /// Timer for for limiting the maximum boxing time.
        /// </summary>
        [SerializeField]
        [Tooltip("Timer for for limiting the maximum boxing time.")]
        private Timer _timer;

        /// <summary>
        /// If the game should start automatically.
        /// </summary>
        [SerializeField]
        [Tooltip("If the game should start automatically.")]
        private bool _autoStart;

        /// <summary>
        /// Current score for the game.
        /// </summary>
        [SerializeField]
        [ReadonlyInInspector]
        [Tooltip("Current score for the game.")]
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

        /// <summary>
        /// Emitted when the boxing starts.
        /// </summary>
        public UnityEvent OnStarted;

        /// <summary>
        /// Emitted when the score changes, providing the scored points.
        /// </summary>
        public UnityEvent<int> OnScoreChanged;

        /// <summary>
        /// Emitted when the boxing ends.
        /// </summary>
        public UnityEvent OnCompleted;

        private void OnEnable()
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

        private void OnDisable()
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

        private void Start()
        {
            if (_autoStart)
            {
                StartGame();
            }
        }

        /// <summary>
        /// Starts the game.
        /// </summary>
        [ContextMenu("Start Game")]
        public void StartGame()
        {
            _targetRandomizer.StartTargetRandomization();
            _timer.StartTimerDefault();
            OnStarted.Invoke();
        }

        /// <summary>
        /// Ends the game.
        /// </summary>
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