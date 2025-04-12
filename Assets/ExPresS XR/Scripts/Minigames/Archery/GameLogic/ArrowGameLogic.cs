using ExPresSXR.Misc.Timing;
using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.Archery
{
    [RequireComponent(typeof(AudioSource))]
    public class ArrowGameLogic : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Duration of the game after started")]
        private float _duration = 30.0f;

        [SerializeField]
        [Tooltip("Ref to the Display of the Score with the ScoreCounter script")]
        private ScoreManager _scoreManager;

        [SerializeField]
        [Tooltip("Ref to the Display of the Counter with the Counter script")]
        private Timer _timer;

        [Space]

        [SerializeField]
        [Tooltip("Start Sound")]
        private AudioClip _startSound;

        [SerializeField]
        [Tooltip("End Sound")]
        private AudioClip _endSound;

        private AudioSource _audioSource;

        // Events
        public UnityEvent OnStarted;
        public UnityEvent OnEnded;
        public UnityEvent<int> OnFinalScore;


        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            _timer.OnTimeout.AddListener(StopArrowGame);
        }

        private void OnDisable()
        {
            _timer.OnTimeout.RemoveListener(StopArrowGame);
        }

        [ContextMenu("Start Game")]
        public void StartArrowGame()
        {
            _scoreManager.ResetScore();
            _scoreManager.enabled = false;
            _timer.StartTimer(_duration);

            if (_startSound != null)
            {
                _audioSource.PlayOneShot(_startSound, 2);
            }
            OnStarted.Invoke();
        }

        [ContextMenu("Stop Game")]
        public void StopArrowGame()
        {
            if (_endSound != null)
            {
                _audioSource.PlayOneShot(_endSound, 1);
            }
            _scoreManager.enabled = true;
            _timer.StopTimer();

            OnFinalScore.Invoke(_scoreManager.Score);
            OnEnded.Invoke();
        }
    }
}