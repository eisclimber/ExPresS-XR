using ExPresSXR.Misc.Timing;
using UnityEngine;
using UnityEngine.Events;
using ExPresSXR.Minigames.Archery.TargetSpawner;

namespace ExPresSXR.Minigames.Archery.GameLogic
{
    [RequireComponent(typeof(AudioSource))]
    public class ArrowGameLogic : MonoBehaviour
    {
        /// <summary>
        /// Duration of the game after started.
        /// </summary>
        [SerializeField]
        [Tooltip("Duration of the game after started.")]
        private float _duration = 30.0f;

        /// <summary>
        /// Reference to the Display of the Score with the ScoreCounter script.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the Display of the Score with the ScoreCounter script.")]
        private ScoreManager _scoreManager;

        /// <summary>
        /// Targets that get associated automatically with the score manager.
        /// </summary>
        [SerializeField]
        [Tooltip("Targets that get associated automatically with the score manager.")]
        private Target[] _targets;

        /// <summary>
        /// Spawners automatically controlled and associated with the score manager.
        /// </summary>
        [SerializeField]
        [Tooltip("Spawners automatically controlled and associated with the score manager.")]
        private TargetSpawnerBase[] _spawners;

        /// <summary>
        /// Reference to the Display of the Counter with the Counter script.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the Display of the Counter with the Counter script.")]
        private Timer _timer;

        [Space]

        /// <summary>
        /// Sound played at the start of the game.
        /// </summary>
        [SerializeField]
        [Tooltip("Sound played at the start of the game.")]
        private AudioClip _startSound;

        /// <summary>
        /// Sound played at the end of the game.
        /// </summary>
        [SerializeField]
        [Tooltip("Sound played at the end of the game.")]
        private AudioClip _endSound;

        [SerializeField]
        [Tooltip("AudioSource used to be played the start and end sound.")]
        private AudioSource _audioSource;

        // Events
        /// <summary>
        /// Emitted when the game is started.
        /// </summary>
        public UnityEvent OnStarted;

        /// <summary>
        /// Emitted when the game finished.
        /// </summary>
        public UnityEvent OnEnded;

        /// <summary>
        /// Emitted with the final score when the game finished.
        /// </summary>
        public UnityEvent<int> OnFinalScore;


        private void Start()
        {
            if (_audioSource == null && !TryGetComponent(out _audioSource))
            {
                Debug.LogError("GameLogic does not have a AudioSource configured. Can not play start and end sounds.", this);
            }
        }

        private void OnEnable()
        {
            _timer.OnTimeout.AddListener(StopArrowGame);
        }

        private void OnDisable()
        {
            _timer.OnTimeout.RemoveListener(StopArrowGame);
        }

        /// <summary>
        /// (Re)-starts the game.
        /// </summary>
        [ContextMenu("Start Game")]
        public void StartArrowGame()
        {
            _scoreManager.ResetScore();

            _scoreManager.enabled = true;
            _timer.StartTimer(_duration);

            if (_startSound != null)
            {
                _audioSource.PlayOneShot(_startSound, 2);
            }

            foreach (Target target in _targets)
            {
                target.ScoreManagers = ScoreManager.MergeWithGlobalManagers(_scoreManager);
            }

            foreach (TargetSpawnerBase spawner in _spawners)
            {
                spawner.AddScoreManager(_scoreManager);
                spawner.StartSpawning();
            }

            OnStarted.Invoke();
        }

        /// <summary>
        /// Stops the game.
        /// </summary>
        [ContextMenu("Stop Game")]
        public void StopArrowGame()
        {
            if (_endSound != null)
            {
                _audioSource.PlayOneShot(_endSound, 1);
            }
            _scoreManager.enabled = false;
            _timer.StopTimer();

            foreach (Target target in _targets)
            {
                target.ScoreManagers = null;
            }

            foreach (TargetSpawnerBase spawner in _spawners)
            {
                spawner.RemoveScoreManager(_scoreManager);
                spawner.StopSpawning();
            }

            OnFinalScore.Invoke(_scoreManager.Score);
            OnEnded.Invoke();
        }
    }
}