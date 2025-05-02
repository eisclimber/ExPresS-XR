using UnityEngine;

namespace ExPresSXR.Misc.Audio
{
    public class AudioSourcePitchRandomizer : MonoBehaviour
    {
        /// <summary>
        /// Audio Source affected.
        /// </summary>
        [SerializeField]
        private AudioSource _audioSource;

        /// <summary>
        /// Minimum possible pitch (inclusive).
        /// </summary>&
        [SerializeField]
        [Tooltip("Minimum possible pitch (inclusive).")]
        private float _minPitch = 0.95f;

        [SerializeField]
        [Tooltip("Maximum possible pitch (exclusive).")]
        private float _maxPitch = 1.05f;


        private void Start()
        {
            if (!_audioSource && !TryGetComponent(out _audioSource))
            {
                Debug.Log("No AudioSource could be found to randomize the pitch.", this);
            }

            if (_minPitch > _maxPitch)
            {
                Debug.LogWarning("Min pitch is greater than max pitch. Setting min to max pitch!");
            }
        }

        /// <summary>
        /// Randomizes the pitch of the audio source.
        /// </summary>        
        public void RandomizePitch()
        {
            if (_audioSource)
            {
                _audioSource.pitch = Random.Range(_minPitch, _maxPitch);
            }
            else
            {
                Debug.LogWarning("No AudioSource set to randomize the pitch.", this);
            }
        }
    }
}