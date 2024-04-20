/*
    Script Name: Scale.cs
    Author: Kevin Koerner
    Refactoring & Integration: Luca Dreiling
    Purpose: Compares the weight in both bowls.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.CoinScale
{
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
        /// Emitted when `CheckBowls()` is called, returning the current state of the scale.
        /// </summary>
        public UnityEvent<ScaleState> OnScaleCheck;

        /// <summary>
        /// Checks and calculates the current state of the scale (which side is lower), emits the result via the Event `OnScaleCheck`.
        /// </summary>
        public void CheckBowls() => OnScaleCheck.Invoke(ScaleState.CreateFromWeights(_leftBowl.GetWeight(), _rightBowl.GetWeight()));

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