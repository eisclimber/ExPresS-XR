using UnityEngine;

namespace ExPresSXR.Minigames.Archery.DojoExample
{
    public class RobotSoundLogic : MonoBehaviour
    {
        /// <summary>
        /// Welcome message played at the start.
        /// </summary>
        [SerializeField]
        [Tooltip("Welcome message played at the start.")]
        private AudioClip _welcomeMessage;

        /// <summary>
        /// Delay of the welcome message after starting.
        /// </summary>
        [SerializeField]
        [Tooltip("Delay of the welcome message after starting.")]
        private float _welcomeMessageDelay = 10.0f;

        /// <summary>
        /// Kyudo explanation played shortly after the welcome message.
        /// </summary>
        [SerializeField]
        [Tooltip("Kyudo explanation played shortly after the welcome message.")]
        private AudioClip _kyudoExplanation;

        /// <summary>
        /// Delay of the welcome message after finishing the welcome message.
        /// </summary>
        [SerializeField]
        [Tooltip("Delay of the welcome message after finishing the welcome message.")]
        private float _kyudoExplanationDelay = 2.0f;

        /// <summary>
        /// The sound played when the robot is hit by an object.
        /// </summary>
        [SerializeField]
        [Tooltip("The sound played when the robot is hit by an object.")]
        private AudioClip _heySound;

        /// <summary>
        /// AudioSource used to play the sounds.
        /// </summary>
        [SerializeField]
        [Tooltip("AudioSource used to play the sounds.")]
        private AudioSource _audioSource;

        private bool _first = true;


        private void Start()
        {
            if (_audioSource != null || TryGetComponent(out _audioSource))
            {
                _audioSource.clip = _welcomeMessage;
                _audioSource.PlayDelayed(_welcomeMessageDelay);
            }
            else
            {
                Debug.LogError("No AudioSource found. Can not play any sounds.", this);
            }
        }

        private void Update()
        {
            if (_first && !_audioSource.isPlaying)
            {
                _audioSource.clip = _kyudoExplanation;
                _audioSource.PlayDelayed(_kyudoExplanationDelay);
                _first = false;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            _audioSource.clip = _heySound;
            _audioSource.Play();
        }
    }
}
