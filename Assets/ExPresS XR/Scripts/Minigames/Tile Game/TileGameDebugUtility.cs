using UnityEngine;

namespace ExPresSXR.Minigames.TileGame
{
    /// <summary>
    /// Debug utility class for manually adding tile to and reading from a tile game.
    /// Most functionality is provided via the context menu (three dots in the header of the component).
    /// </summary>
    public class TileGameDebugUtility : MonoBehaviour
    {
        /// <summary>
        /// Center area id used for debugging features.
        /// </summary>
        [SerializeField]
        [Tooltip("Center area id used for debugging features.")]
        private int _centerAreaId;
        public int CenterAreaId
        {
            get => _centerAreaId;
        }

        [Space]

        /// <summary>
        /// Top area id used for debugging features.
        /// </summary>
        [SerializeField]
        [Tooltip("Top area id used for debugging features.")]
        private int _topAreaId;
        public int TopAreaId
        {
            get => _topAreaId;
        }

        /// <summary>
        /// Bottom area id used for debugging features.
        /// </summary>
        [SerializeField]
        [Tooltip("Bottom area id used for debugging features.")]
        private int _bottomAreaId;
        public int BottomAreaId
        {
            get => _bottomAreaId;
        }

        /// <summary>
        /// Left area id used for debugging features.
        /// </summary>
        [SerializeField]
        [Tooltip("Left area id used for debugging features.")]
        private int _leftAreaId;
        public int LeftAreaId
        {
            get => _leftAreaId;
        }

        /// <summary>
        /// Right area id used for debugging features.
        /// </summary>
        [SerializeField]
        [Tooltip("Right area id used for debugging features.")]
        private int _rightAreaId;
        public int RightAreaId
        {
            get => _rightAreaId;
        }

        [Space]

        /// <summary>
        /// Board position to insert a new tile in.
        /// </summary>
        [SerializeField]
        [Tooltip("Board position to insert a new tile in.")]
        private Vector2Int _insertPos;

        [Space]

        /// <summary>
        /// Reference to the TileGame targeted.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the TileGame targeted.")]
        private TileGame _game;

        /// <summary>
        /// Tile visuals instance to show debug scores in.
        /// </summary>
        [SerializeField]
        [Tooltip("Tile visuals instance to show debug scores in.")]
        private TileVisuals _targetVisualsInstance;


        [ContextMenu("Add Tile From Board Submission")]
        private void AddTileFromBoardSubmission()
        {
            _targetVisualsInstance.DisplayedTile = new(_game.NumAreas, _centerAreaId, _topAreaId, _bottomAreaId, _leftAreaId, _rightAreaId);
            _game.AddTileFromBoardSubmission(new(_targetVisualsInstance, _insertPos));
        }

        [ContextMenu("Add Tile Manually")]
        private void AddTile()
        {
            Tile tile = new(_game.NumAreas, _centerAreaId, _topAreaId, _bottomAreaId, _leftAreaId, _rightAreaId);
            ScoreResults score = _game.AddTileAt(tile, _insertPos);
            score.PrintScore();
        }

        [ContextMenu("Evaluate Board")]
        private void Evaluate()
        {
            Vector2Int size = _game.BoardSize;
            bool[,] visited = new bool[size.x, size.y];
            _game.EvaluatePointsFrom(_insertPos, 0, visited);
        }
    }
}