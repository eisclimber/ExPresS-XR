/*
    Script Name: ScoreDisplay.cs
    Author: Kevin Koerner
    Refactoring & Integration: Luca Dreiling
    Purpose: Displays a numerical score on a text.
*/
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ExPresSXR.Minigames.CoinThrow
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _textDisplay;

        private void Start() {
            if (_textDisplay == null || !TryGetComponent(out _textDisplay))
            {
                Debug.LogError("No Text Display was provided.", this);
            }
        }

        public void AddScore(int score)
        {
            int currentScore = int.Parse(_textDisplay.text);
            currentScore += score;
            _textDisplay.text = currentScore.ToString();
        }

        public void SubtractScore(int score)
        {
            int currentScore = int.Parse(_textDisplay.text);
            currentScore -= score;
            _textDisplay.text = currentScore.ToString();
        }
    }
}