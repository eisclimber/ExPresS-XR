/*
    Script Name: ScoreDisplay.cs
    Author: Kevin Koerner
    Refactoring & Integration: Luca Dreiling
    Purpose: Displays and stores a numerical score via ui.
*/
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ExPresSXR.Minigames.CoinThrow
{
    public class ScoreDisplay : MonoBehaviour
    {
        /// <summary>
        /// Reference to the TMP_Text that displays the value.
        /// </summary>
        [SerializeField]
        private TMP_Text _textDisplay;

        private void Start() {
            if (_textDisplay == null || !TryGetComponent(out _textDisplay))
            {
                Debug.LogError("No Text Display was provided.", this);
            }
        }

        /// <summary>
        /// Amount to add to the current score. Can be negative.
        /// </summary>
        /// <param name="score">Amount to add.</param>
        public void AddScore(int score)
        {
            int currentScore = int.Parse(_textDisplay.text);
            currentScore += score;
            _textDisplay.text = currentScore.ToString();
        }

        /// <summary>
        /// Amount to subtract to the current score. Can be negative.
        /// </summary>
        /// <param name="score">Amount to subtract.</param>
        public void SubtractScore(int score)
        {
            int currentScore = int.Parse(_textDisplay.text);
            currentScore -= score;
            _textDisplay.text = currentScore.ToString();
        }
    }
}