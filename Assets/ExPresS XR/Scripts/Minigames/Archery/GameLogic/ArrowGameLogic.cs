using UnityEngine;

namespace ExPresSXR.Minigames.Archery
{
    [RequireComponent(typeof(AudioSource))]
    public class ArrowGameLogic : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Ref to the Display of the Score with the ScoreCounter script")]
        private ScoreManager _scoreManager;

        [SerializeField]
        [Tooltip("Ref to the Display of the Counter with the Counter script")]
        private TimeCounter _timeCounter;

        [Space]

        [SerializeField]
        [Tooltip("Start Sound")]
        private AudioClip _startSound;

        [SerializeField]
        [Tooltip("End Sound")]
        private AudioClip _endSound;

        private AudioSource _audioSource;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void StartArrowGame()
        {
            _scoreManager.Reset();
            _timeCounter.Reset();

            _scoreManager.game = false;
            _timeCounter.Counter = true;

            if (_startSound != null)
            {
                _audioSource.PlayOneShot(_startSound, 2);
            }
        }

        public void Reset()
        {
            if (_endSound != null)
            {
                _audioSource.PlayOneShot(_endSound, 1);
            }
            _scoreManager.game = true;
            _timeCounter.Counter = false;
        }
    }
}