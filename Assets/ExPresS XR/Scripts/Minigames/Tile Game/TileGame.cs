using System;
using ExPresSXR.Misc;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.TileGame
{
    /// <summary>
    /// The main game logic for the tile game.
    /// </summary>
    public class TileGame : MonoBehaviour
    {
        /// <summary>
        /// Default width of the board.
        /// </summary>
        public const int DEFAULT_BOARD_WIDTH = 5;

        /// <summary>
        /// Default height of the board.
        /// </summary>
        public const int DEFAULT_BOARD_HEIGHT = 3;

        /// <summary>
        /// If the game should start automatically.
        /// </summary>
        [SerializeField]
        [Tooltip("")]
        private bool _autoStart;

        /// <summary>
        /// Size of the board.
        /// </summary>
        [SerializeField]
        [Tooltip("")]
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
        [Tooltip("")]
        private Transform _boardSocketsParent;
        public Transform BoardSocketsParent
        {
            get => _boardSocketsParent;
            set => _boardSocketsParent = value;
        }

        [SerializeField]
        [Tooltip("")]
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
        [Tooltip("")]
        private AreaDescription[] _areas;


        [SerializeField]
        [Tooltip("")]
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
        [Tooltip("")]
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
        
        /// <summary>
        /// Number of slots on the board.
        /// </summary>
        public int NumBoardSlots { get => _boardSize.x * _boardSize.y; }

        /// <summary>
        /// Number of area types configured to the game.
        /// </summary>
        public int NumAreas { get => _areas.Length; }


        /// <summary>
        /// Current tiles set for the board.
        /// </summary>
        private Tile[,] _board = new Tile[DEFAULT_BOARD_WIDTH, DEFAULT_BOARD_HEIGHT];

        /// <summary>
        /// Emitted when the game is started.
        /// </summary>
        public UnityEvent OnStarted;

        /// <summary>
        /// Emitted when a tile is added, providing its location on the board.
        /// </summary>
        public UnityEvent<Vector2Int> OnTileAdded;

        /// <summary>
        /// Emitted when the score changes with the score received.
        /// </summary>
        public UnityEvent<int> OnScoreChanged;

        /// <summary>
        /// Emitted when the game was completed with the final score.
        /// </summary>
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
            ResetGame();
            OnStarted.Invoke();
        }

        [ContextMenu("End Game")]
        private void EndGame()
        {
            Debug.Log($"Completed with a score of: {_totalScore}.", this);
            OnCompleted.Invoke(_totalScore);
        }

        [ContextMenu("Reset Game")]
        private void ResetGame()
        {
            _board = new Tile[_boardSize.x, _boardSize.y];
            _totalScore = 0;
            _placedTiles = 0;

            foreach (Transform child in _boardSocketsParent)
            {
                if (child.TryGetComponent(out TileSubmitSocket socket))
                {
                    socket.ClearSelection();
                }
            }
        }

        /// <summary>
        /// Adds a tile from a socket on the board.
        /// </summary>
        /// <param name="ctx">Board submission context provided.</param>
        public void AddTileFromBoardSubmission(TileSubmitSocket.BoardSubmitContext ctx)
        {
            TileVisuals display = ctx.TileVisuals;
            ScoreResults score = AddTileAt(display.DisplayedTile, ctx.BoardPos);
            display.DisplayScore(score);
            TotalScore += score.TotalScore;
        }

        /// <summary>
        /// Adds a tile at the specified location.
        /// </summary>
        /// <param name="tile">Tile to add.</param>
        /// <param name="pos">Board position to add it at.</param>
        /// <returns>Score for the tile added.</returns>
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
            OnTileAdded.Invoke(pos);
            PlacedTiles++;

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

        /// <summary>
        /// Checks the points received from creating a matching area with a neighboring tile position in the specified direction recursively.
        /// </summary>
        /// <param name="pos">Position to check.</param>
        /// <param name="checkDir">Direction to check in.</param>
        /// <param name="visited">Already visited tiles.</param>
        /// <returns>Points received in that direction.</returns>
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
                return EvaluatePointsFrom(nextPos, 0, visited) + 1;
            }
            return 0;
        }


        /// <summary>
        /// Recursive step for checking the point for creating matching areas relative to the position of a tile tile.
        /// </summary>
        /// <param name="pos">Position to check.</param>
        /// <param name="score">Accumulative score.</param>
        /// <param name="visited">Already visited tiles.</param>
        /// <returns>Points received in that tile.</returns>
        public int EvaluatePointsFrom(Vector2Int pos, int score, bool[,] visited)
        {
            if (!IsPosInBounds(pos) || !IsTileOccupied(pos) || visited[pos.x, pos.y])
            {
                return score;
            }
            // Mark tile visited and add increase score
            visited[pos.x, pos.y] = true;
            score++;

            // Add Scores from neighbors
            score += CheckNeighbor(pos, Vector2Int.down, visited); // Invert neighbor up/down dir since were using different axis
            score += CheckNeighbor(pos, Vector2Int.up, visited); // Invert neighbor up/down dir since were using different axis
            score += CheckNeighbor(pos, Vector2Int.left, visited);
            score += CheckNeighbor(pos, Vector2Int.right, visited);
            return score;
        }

        /// <summary>
        /// Check if the tile is occupied.
        /// </summary>
        /// <param name="pos">Position to check.</param>
        /// <returns>Tile is occupied or empty.</returns>
        public bool IsTileOccupied(Vector2Int pos) => IsPosInBounds(pos) && _board[pos.x, pos.y] != null;

        /// <summary>
        /// Check if the position is a valid board position. 
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool IsPosInBounds(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < _boardSize.x
                && pos.y >= 0 && pos.y < _boardSize.y;
        }

        /// <summary>
        /// Updates the references to the available areas of the sockets creating new tiles.
        /// </summary>
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