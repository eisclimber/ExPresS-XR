using UnityEngine;

namespace ExPresSXR.Misc.Audio
{
    public class AudioSourcePitchRandomizer : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _audioSource;

        [SerializeField]
        [Tooltip("Min pitch (inclusive)")]
        private float _minPict = 0.95f;

        [SerializeField]
        [Tooltip("Max pitch (exclusive)")]
        private float _maxPict = 1.05f;


        private void Start()
        {
            if (!_audioSource && !TryGetComponent(out _audioSource))
            {
                Debug.Log("No AudioSource could be found to randomize the pitch.", this);
            }

            if (_minPict > _maxPict)
            {
                Debug.LogWarning("Min pitch is greater than max pitch. Setting min to max pitch!");
            }
        }


        public void RandomizePitch()
        {
            if (_audioSource)
            {
                _audioSource.pitch = Random.Range(_minPict, _maxPict);
            }
            else
            {
                Debug.LogWarning("No AudioSource set to randomize the pitch.", this);
            }
        }
    }
}