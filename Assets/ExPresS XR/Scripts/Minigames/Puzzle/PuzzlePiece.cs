using TMPro;
using UnityEngine;

namespace ExPresSXR.Minigames.Puzzle
{
    public class PuzzlePiece : MonoBehaviour
    {
        [SerializeField]
        private Vector2Int _puzzlePosition;
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

        [SerializeField]
        private TMP_Text _tileText;
    }
}