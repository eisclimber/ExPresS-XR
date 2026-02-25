using TMPro;
using UnityEngine;

namespace ExPresSXR.Minigames.Common
{
    /// <summary>
    /// Helper for displaying an integer score as text indicating changes via score numbers.
    /// </summary>
    public class TotalScoreText : MonoBehaviour
    {
        /// <summary>
        /// Text to display the score.
        /// </summary>
        [SerializeField]
        [Tooltip("Text to display the score.")]
        private TMP_Text _text;

        /// <summary>
        /// Prefab (i.e. ScoreNumbers) to be spawned when scoring.
        /// </summary>
        [SerializeField]
        [Tooltip("Prefab (i.e. ScoreNumbers) to be spawned when scoring.")]
        private GameObject _diffDisplayPrefab;

        /// <summary>
        /// Scale with which the `_pointsDisplayOffset` is spawned.
        /// </summary>
        [SerializeField]
        [Tooltip("Scale with which the `_pointsDisplayOffset` is spawned.")]
        private float _pointsDisplayScale = 0.35f;

        /// <summary>
        /// Scale with which the `_pointsDisplayOffset` is spawned.
        /// </summary>
        [SerializeField]
        [Tooltip("Offset with which the `_pointsDisplayOffset` is spawned.")]
        private Vector3 _pointsDisplayOffset;

        /// <summary>
        /// The currently displayed score. Creates a difference display when changed.
        /// </summary>
        [SerializeField]
        [Tooltip("The currently displayed score. Creates a difference display when changed.")]
        private int _displayedScore;
        public int DisplayedScore
        {
            get => _displayedScore;
            set
            {
                int diff = value - _displayedScore;
                _displayedScore = value;
                _text.text = value.ToString();
                ShowDiffDisplay(diff);
            }
        }


        private void OnEnable()
        {
            if (_text == null && !TryGetComponent(out _text))
            {
                Debug.LogError("Did not find a text component to display the score.", this);
            }
        }

        private void ShowDiffDisplay(int diff)
        {
            if (diff == 0 || _diffDisplayPrefab == null)
            {
                return;
            }

            GameObject scoreNumbersGo = Instantiate(_diffDisplayPrefab); // Instantiate without parent to avoid canvas rotation problems
            scoreNumbersGo.transform.SetPositionAndRotation(transform.position + _pointsDisplayOffset, transform.rotation);
            scoreNumbersGo.transform.localScale = Vector3.one * _pointsDisplayScale;

            if (scoreNumbersGo.TryGetComponent(out ScoreNumbers scoreNumbers))
            {
                scoreNumbers.Score = diff;
            }
        }
    }
}