using UnityEngine;

namespace ExPresSXR.Minigames.TileGame
{
    public class TileGameDebugUtility : MonoBehaviour
    {
        [SerializeField]
        private int _centerAreaId;
        public int CenterAreaId
        {
            get => _centerAreaId;
        }

        [Space]

        [SerializeField]
        private int _topAreaId;
        public int TopAreaId
        {
            get => _topAreaId;
        }


        [SerializeField]
        private int _bottomAreaId;
        public int BottomAreaId
        {
            get => _bottomAreaId;
        }

        [SerializeField]
        private int _leftAreaId;
        public int LeftAreaId
        {
            get => _leftAreaId;
        }

        [SerializeField]
        private int _rightAreaId;
        public int RightAreaId
        {
            get => _rightAreaId;
        }

        [Space]

        [SerializeField]
        private Vector2Int _insertPos;

        [Space]

        [SerializeField]
        private TileGame _game;

        [SerializeField]
        private TileVisuals _targetVisualsInstance;


        [ContextMenu("Add Tile From Board Submission")]
        public void AddTileFromBoardSubmission()
        {
            _targetVisualsInstance.DisplayedTile = new(_game.NumAreas, _centerAreaId, _topAreaId, _bottomAreaId, _leftAreaId, _rightAreaId);
            _game.AddTileFromBoardSubmission(new(_targetVisualsInstance, _insertPos));
        }

        [ContextMenu("Add Tile Manually")]
        public void AddTile()
        {
            Tile tile = new(_game.NumAreas, _centerAreaId, _topAreaId, _bottomAreaId, _leftAreaId, _rightAreaId);
            ScoreResults score = _game.AddTileAt(tile, _insertPos);
            score.PrintScore();
        }

        [ContextMenu("Evaluate Board")]
        public void Evaluate()
        {
            Vector2Int size = _game.BoardSize;
            bool[,] visited = new bool[size.x, size.y];
            _game.EvaluatePointsFrom(_insertPos, 0, visited);
        }
    }
}