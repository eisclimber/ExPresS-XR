using System;
using ExPresSXR.Misc;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.TileGame
{
    public class TileGame : MonoBehaviour
    {
        public const int DEFAULT_BOARD_WIDTH = 5;
        public const int DEFAULT_BOARD_HEIGHT = 3;

        
        [SerializeField]
        private bool _autoStart;
        

        [SerializeField]
        private Vector2Int _boardSize = new(DEFAULT_BOARD_WIDTH, DEFAULT_BOARD_HEIGHT);
        public Vector2Int BoardSize
        {
            get => _boardSize;
            set
            {
                _boardSize = value;
                _board = new Tile[_boardSize.x, _boardSize.y];
            }
        }

        [SerializeField]
        private Transform _boardSocketsParent;
        public Transform BoardSocketsParent
        {
            get => _boardSocketsParent;
            set => _boardSocketsParent = value;
        }

        [SerializeField]
        private TileRespawnSocket[] _tileRespawnSockets;
        public TileRespawnSocket[] TileRespawnSockets
        {
            get => _tileRespawnSockets;
            set
            {
                _tileRespawnSockets = value;

                UpdateAreasVisuals();
            }
        }

        [SerializeField]
        private AreaDescription[] _areas;


        [SerializeField]
        [ReadonlyInInspector]
        private int _totalScore;
        public int TotalScore
        {
            get => _totalScore;
            private set
            {
                _totalScore = value;
                OnScoreChanged.Invoke(_totalScore);
            }
        }

        [SerializeField]
        [ReadonlyInInspector]
        private int _placedTiles;
        public int PlacedTiles
        {
            get => _placedTiles;
            set
            {
                _placedTiles = value;

                if (_placedTiles >= NumBoardSlots)
                {
                    EndGame();
                }
            }
        }

        public int NumBoardSlots { get => _boardSize.x * _boardSize.y; }

        public int NumAreas { get => _areas.Length; }


        private Tile[,] _board = new Tile[DEFAULT_BOARD_WIDTH, DEFAULT_BOARD_HEIGHT];

        public UnityEvent<int> OnStarted;
        public UnityEvent<Vector2Int> OnTileAdded;
        public UnityEvent<int> OnScoreChanged;
        public UnityEvent<int> OnCompleted;

        private void OnEnable()
        {
            UpdateAreasVisuals();
            
            if (_autoStart)
            {
                StartGame();
            }
        }


        [ContextMenu("Start Game")]
        private void StartGame()
        {
            _board = new Tile[_boardSize.x, _boardSize.y];
        }

        [ContextMenu("End Game")]
        private void EndGame()
        {
            Debug.Log($"Completed with a score of: {_totalScore}.", this);
            OnCompleted.Invoke(_totalScore);
        }

        public void AddTileFromBoardSubmission(TileSubmitSocket.BoardSubmitContext ctx)
        {
            TileVisuals display = ctx.TileVisuals;
            ScoreResults score = AddTileAt(display.DisplayedTile, ctx.BoardPos);
            display.DisplayScore(score);
            TotalScore += score.TotalScore;
        }

        public ScoreResults AddTileAt(Tile tile, Vector2Int pos)
        {
            if (!IsPosInBounds(pos))
            {
                Debug.Log($"Starting pos out of bounds at {pos}.", this);
                return new();
            }

            if (IsTileOccupied(pos))
            {
                Debug.LogWarning($"Can't place tile {tile} pos {pos} (either occupied or out of bounds).", this);
                return new();
            }

            _board[pos.x, pos.y] = tile;
            PlacedTiles++;
            OnTileAdded.Invoke(pos);

            // Dirty but we want fresh copies for each direction to avoid one side not awarding any points
            bool[,] visitedTop = new bool[_boardSize.x, _boardSize.y];
            visitedTop[pos.x, pos.y] = true;
            bool[,] visitedBottom = new bool[_boardSize.x, _boardSize.y];
            visitedBottom[pos.x, pos.y] = true;
            bool[,] visitedLeft = new bool[_boardSize.x, _boardSize.y];
            visitedLeft[pos.x, pos.y] = true;
            bool[,] visitedRight = new bool[_boardSize.x, _boardSize.y];
            visitedRight[pos.x, pos.y] = true;

            ScoreResults score = new(
                1,
                tile.CenterAreaId,
                CheckNeighbor(pos, Vector2Int.down, visitedTop), // Invert neighbor up/down dir since were using different axis
                tile.TopAreaId,
                CheckNeighbor(pos, Vector2Int.up, visitedBottom), // Invert neighbor up/down dir since were using different axis
                tile.BottomAreaId,
                CheckNeighbor(pos, Vector2Int.left, visitedLeft),
                tile.LeftAreaId,
                CheckNeighbor(pos, Vector2Int.right, visitedRight),
                tile.RightAreaId
            );
            return score;
        }

        public int EvaluatePointsFrom(Vector2Int pos, int score, bool[,] visited)
        {
            if (!IsPosInBounds(pos) && !IsTileOccupied(pos) && visited[pos.x, pos.y])
            {
                return score;
            }

            visited[pos.x, pos.y] = true;
            score++;

            score += CheckNeighbor(pos, Vector2Int.down, visited); // Invert neighbor up/down dir since were using different axis
            score += CheckNeighbor(pos, Vector2Int.up, visited); // Invert neighbor up/down dir since were using different axis
            score += CheckNeighbor(pos, Vector2Int.left, visited);
            score += CheckNeighbor(pos, Vector2Int.right, visited);
            return score;
        }

        public int CheckNeighbor(Vector2Int pos, Vector2Int checkDir, bool[,] visited)
        {
            Vector2Int nextPos = pos + checkDir;
            if (!IsPosInBounds(nextPos))
            {
                return 0;
            }

            Tile currentTile = _board[pos.x, pos.y];
            Tile nextTile = _board[nextPos.x, nextPos.y];

            if (currentTile.IsAdjacentConnected(nextTile, checkDir))
            {
                return EvaluatePointsFrom(nextPos, 0, visited);
            }
            return 0;
        }

        public bool IsTileOccupied(Vector2Int pos) => IsPosInBounds(pos) && _board[pos.x, pos.y] != null;

        public bool IsPosInBounds(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < DEFAULT_BOARD_WIDTH
                && pos.y >= 0 && pos.y < DEFAULT_BOARD_HEIGHT;
        }

        [ContextMenu("Update Area Visuals")]
        public void UpdateAreasVisuals()
        {
            foreach (TileRespawnSocket socket in _tileRespawnSockets)
            {
                socket.Areas = _areas;
            }
        }

        [ContextMenu("Add Test Score")]
        private void AddTestScore() => TotalScore += UnityEngine.Random.Range(1, 20);

        private void OnDrawGizmosSelected()
        {
            if (_boardSocketsParent == null)
            {
                GizmoUtils.DrawLabel($"No Board Socket Parent set.", Vector3.zero, Color.red, transform);
                return; // Can't draw if no parent is provided
            }

            for (int y = 0; y < DEFAULT_BOARD_HEIGHT; y++)
            {
                for (int x = 0; x < DEFAULT_BOARD_WIDTH; x++)
                {
                    Transform tile = _boardSocketsParent.GetChild(y * DEFAULT_BOARD_WIDTH + x);
                    if (tile == null)
                    {
                        continue;
                    }
                    Color occupiedColor = _board[x, y] == null ? Color.white : Color.green;
                    GizmoUtils.DrawLabel($"({x}, {y})", Vector3.zero, occupiedColor, tile);
                }
            }
        }
    }
}