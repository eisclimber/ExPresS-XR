using UnityEngine;

namespace ExPresSXR.Misc.Audio
{
    public class AudioSourceCancelGroup : MonoBehaviour
    {
        [SerializeField]
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