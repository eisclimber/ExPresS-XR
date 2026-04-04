using System.Net.Sockets;
using UnityEngine;
using static ExPresSXR.Minigames.TileGame.TileGame;

namespace ExPresSXR.Minigames.TileGame
{
    /// <summary>
    /// Debug utility class for manually adding tile to and reading from a tile game.
    /// Most functionality is provided via the context menu (three dots in the header of the component).
    /// </summary>
    public class TileGameDebugUtility : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Center area id used for debugging features.")]
        private int _centerAreaId;
        /// <summary>
        /// Center area id used for debugging features.
        /// </summary>
        public int CenterAreaId
        {
            get => _centerAreaId;
        }

        [Space]

        [SerializeField]
        [Tooltip("Top area id used for debugging features.")]
        private int _topAreaId;
        /// <summary>
        /// Top area id used for debugging features.
        /// </summary>
        public int TopAreaId
        {
            get => _topAreaId;
        }

        [SerializeField]
        [Tooltip("Bottom area id used for debugging features.")]
        private int _bottomAreaId;
        /// <summary>
        /// Bottom area id used for debugging features.
        /// </summary>
        public int BottomAreaId
        {
            get => _bottomAreaId;
        }

        [SerializeField]
        [Tooltip("Left area id used for debugging features.")]
        private int _leftAreaId;
        /// <summary>
        /// Left area id used for debugging features.
        /// </summary>
        public int LeftAreaId
        {
            get => _leftAreaId;
        }

        [SerializeField]
        [Tooltip("Right area id used for debugging features.")]
        private int _rightAreaId;
        /// <summary>
        /// Right area id used for debugging features.
        /// </summary>
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

        /// <summary>
        /// Parent GameObject to retrieve TileSubmitSockets from.
        /// </summary>
        [SerializeField]
        [Tooltip("Parent GameObject to retrieve TileSubmitSockets from.")]
        private Transform _boardSocketParent;


        [ContextMenu("Add Tile From Board Submission")]
        private void AddTileFromBoardSubmission()
        {
            int socketIdx = _insertPos.y * _game.BoardSize.x + _insertPos.x;
            _targetVisualsInstance.DisplayedTile = new(_game.NumAreas, _centerAreaId, _topAreaId, _bottomAreaId, _leftAreaId, _rightAreaId);
            Transform socketTransform = _boardSocketParent.GetChild(socketIdx);
            if (socketTransform != null && socketTransform.TryGetComponent(out TileSubmitSocket targetSocket))
            {
                _game.AddTileFromBoardSubmission(new(targetSocket, _targetVisualsInstance));
            }
        }

        [ContextMenu("Add Tile Manually")]
        private void AddTile()
        {
            Tile tile = new(_game.NumAreas, _centerAreaId, _topAreaId, _bottomAreaId, _leftAreaId, _rightAreaId);
            PlacementData data = _game.AddTileAt(tile, _insertPos);
            ScoreResults score = _game.ScoreCalculator != null ? _game.ScoreCalculator.CalculateScore(data) : ScoreCalculator.CalculateDefaultScore(data);
            score.PrintScore();
        }
    }
}