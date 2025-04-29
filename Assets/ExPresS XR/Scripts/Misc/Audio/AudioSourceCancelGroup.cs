using UnityEngine;

namespace ExPresSXR.Misc.Audio
{
    public class AudioSourceCancelGroup : MonoBehaviour
    {
        /// <summary>
        /// AudioSources to be canceled.
        /// </summary>
        [SerializeField]
        [Tooltip("AudioSources to be canceled.")]
        private AudioSource[] _audioSources;

        /// <summary>
        /// Cancels playback of all configured AudioSource.
        /// </summary>
        public void CancelActiveAudio()
        {
            foreach (AudioSource audioSource in _audioSources)
            {
                audioSource.Stop();
            }
        }
    }
}