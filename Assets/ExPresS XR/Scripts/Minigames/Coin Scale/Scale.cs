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
        /// Reference to the left Bowl.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the left Bowl.")]
        private Bowl _leftBowl;

        /// <summary>
        /// Reference to the right Bowl.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the right Bowl.")]
        private Bowl _rightBowl;

        public UnityEvent<ScaleState> OnScaleCheck;

        public void CheckBowls() => OnScaleCheck.Invoke(ScaleState.CreateFromWeights(_leftBowl.GetWeight(), _rightBowl.GetWeight()));

        public void ResetScale()
        {
            _leftBowl.ResetBowl();
            _rightBowl.ResetBowl();
        }
    }
}