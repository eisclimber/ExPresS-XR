using TMPro;
using UnityEngine;


namespace ExPresSXR.Minigames.Archery
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("How fast the score in-/decreases per default.")]
        private int _stepSize = 1;

        [SerializeField]
        [Tooltip("If the score can be negative.")]
        private bool _allowNegative;

        [SerializeField]
        [Tooltip("Reference to the text displaying the score.")]
        public TMP_Text _scoreText;

        private int _score = 0;
        public int Score
        {
            get => _score;
            set
            {
                _score = _allowNegative ? value : Mathf.Max(value, 0);

                if (_scoreText != null)
                {
                    _scoreText.text = Score.ToString();
                }
            }
        }

        public void AlterPointsFromCollision(Collision col)
        {
            if (isActiveAndEnabled)
            {
                if (col.gameObject.CompareTag("Target"))
                {
                    IncreaseScore();
                }
                else if (col.gameObject.CompareTag("BadTarget"))
                {
                    DecreaseScore();
                }
            }
        }

        [ContextMenu("Increase Score")]
        public void IncreaseScore() => Score += _stepSize;
        public void IncreaseScore(int amount) => Score += amount;

        [ContextMenu("Decrease Score")]
        public void DecreaseScore() => Score -= _stepSize;
        public void DecreaseScore(int amount) => Score -= amount;

        [ContextMenu("Reset Score")]
        public void ResetScore() => Score = 0;
    }
}