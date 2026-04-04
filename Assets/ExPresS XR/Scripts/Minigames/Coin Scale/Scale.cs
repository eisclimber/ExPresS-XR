using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.CoinScale
{
    /// <summary>
    /// Compares the checkt in both bowls.
    /// </summary>
    public class Scale : MonoBehaviour
    {
        /// <summary>
        /// Reference to the left bowl.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the left bowl.")]
        private Bowl _leftBowl;

        /// <summary>
        /// Reference to the right bowl.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the right bowl.")]
        private Bowl _rightBowl;

        /// <summary>
        /// Audio clip played the bowls are checked.
        /// </summary>
        [SerializeField]
        [Tooltip("Audio clip played the bowls are checked.")]
        private AudioClip _checkSound;

        /// <summary>
        /// Audio source used to play the sound when checking the bowls.
        /// </summary>
        [SerializeField]
        [Tooltip("Audio source used to play the sound when checking the bowls.")]

        private AudioSource _checkAudioPlayer;

        /// <summary>
        /// Emitted when `CheckBowls()` is called, returning the current state of the scale.
        /// </summary>
        public UnityEvent<ScaleState> OnScaleCheck;

        /// <summary>
        /// Checks and calculates the current state of the scale (which side is lower), emits the result via the Event `OnScaleCheck`.
        /// </summary>
        public void CheckBowls()
        {
            PlayCheckedSound();
            OnScaleCheck.Invoke(ScaleState.CreateFromWeights(_leftBowl.GetWeight(), _rightBowl.GetWeight()));
        }


        private void PlayCheckedSound()
        {
            if (_checkSound)
            {
                if (_checkAudioPlayer != null)
                {
                    _checkAudioPlayer.PlayOneShot(_checkSound);
                }
                else
                {
                    Debug.LogWarning("Can't play check sound. No AudioSource provided.", this);
                }
            }
        }

        /// <summary>
        /// Resets the state of the scale, by resetting both bowls.
        /// </summary>
        public void ResetScale()
        {
            _leftBowl.ResetBowl();
            _rightBowl.ResetBowl();
        }
    }
}