using TMPro;
using UnityEngine;

namespace ExPresSXR.Minigames.Puzzle
{
    /// <summary>
    /// Represents a puzzle piece at a specific board position.
    /// </summary>
    public class PuzzlePiece : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Board position of the piece.")]
        private Vector2Int _puzzlePosition;
        /// <summary>
        /// Board position of the piece.
        /// </summary>
        public Vector2Int PuzzlePosition
        {
            get => _puzzlePosition;
            set
            {
                _puzzlePosition = value;

                if (_tileText != null)
                {
                    _tileText.text = $"{_puzzlePosition}";
                }
            }
        }

        /// <summary>
        /// Allows displaying the board position in text object for debugging.
        /// </summary>
        [SerializeField]
        [Tooltip("Allows displaying the board position in text object for debugging.")]
        private TMP_Text _tileText;
    }
}