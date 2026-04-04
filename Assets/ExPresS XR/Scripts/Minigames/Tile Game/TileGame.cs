using System;
using ExPresSXR.Misc;
using UnityEngine;
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
        [Tooltip("If the game should start automatically.")]
        private bool _autoStart;

        [SerializeField]
        [Tooltip("Size of the board.")]
        private Vector2Int _boardSize = new(DEFAULT_BOARD_WIDTH, DEFAULT_BOARD_HEIGHT);

        /// <summary>
        /// Size of the board.
        /// </summary>
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
        [Tooltip("Parent transform for creating new board sockets in.")]
        private Transform _boardSocketsParent;
        /// <summary>
        /// Parent transform for creating new board sockets in.
        /// </summary>
        public Transform BoardSocketsParent
        {
            get => _boardSocketsParent;
            set => _boardSocketsParent = value;
        }

        [SerializeField]
        [Tooltip("Sockets for spawning new tiles after submitting one.")]
        private TileRespawnSocket[] _tileRespawnSockets;
        /// <summary>
        /// Sockets for spawning new tiles after submitting one.
        /// </summary>
        public TileRespawnSocket[] TileRespawnSockets
        {
            get => _tileRespawnSockets;
            set
            {
                _tileRespawnSockets = value;

                UpdateAreasVisuals();
            }
        }

        /// <summary>
        /// Areas available in the game.
        /// </summary>
        [SerializeField]
        [Tooltip("Areas available in the game.")]
        private AreaDescription[] _areas;

        [SerializeField]
        [Tooltip("Optional Score calculator. If none is provided, a default score calculation is used.")]
        private ScoreCalculator _scoreCalculator;
        /// <summary>
        /// Optional Score calculator. If none is provided, a default score calculation is used.
        /// </summary>
        public ScoreCalculator ScoreCalculator
        {
            get => _scoreCalculator;
            set => _scoreCalculator = value;
        }

        [SerializeField]
        [Tooltip("Total score of the current game.")]
        [ReadonlyInInspector]
        private int _totalScore;
        /// <summary>
        /// Total score of the current game.
        /// </summary>
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
        [Tooltip("Number of placed tiles in the current game.")]
        [ReadonlyInInspector]
        private int _placedTiles;
        /// <summary>
        /// Number of placed tiles in the current game.
        /// </summary>
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
            TileVisuals visuals = ctx.TileVisuals;
            TileSubmitSocket socket = ctx.SubmittedSocket;
            PlacementData data = AddTileAt(visuals.DisplayedTile, socket.BoardPos);
            ScoreResults score = _scoreCalculator != null ? _scoreCalculator.CalculateScore(data) : ScoreCalculator.CalculateDefaultScore(data);
            ctx.SubmittedSocket.DisplayScore(score, _areas);
            TotalScore += score.TotalScore;
        }

        /// <summary>
        /// Adds a tile at the specified location.
        /// </summary>
        /// <param name="tile">Tile to add.</param>
        /// <param name="pos">Board position to add it at.</param>
        /// <returns>Score for the tile added.</returns>
        public PlacementData AddTileAt(Tile tile, Vector2Int pos)
        {
            if (!IsPosInBounds(pos))
            {
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

            // Discover tiles using flood fill (per direction)
            AreaDiscoveryData upDiscovery = FindConnectedAreasInDirection(pos, Vector2Int.down); // Our up/down is flipped
            AreaDiscoveryData downDiscovery = FindConnectedAreasInDirection(pos, Vector2Int.up);
            AreaDiscoveryData leftDiscovery = FindConnectedAreasInDirection(pos, Vector2Int.left);
            AreaDiscoveryData rightDiscovery = FindConnectedAreasInDirection(pos, Vector2Int.right);

            return new(tile, pos, PlacedTiles, upDiscovery, downDiscovery, leftDiscovery, rightDiscovery);
        }

        private AreaDiscoveryData FindConnectedAreasInDirection(Vector2Int pos, Vector2Int dir)
        {
            if (!IsPosInBounds(pos))
            {
                Debug.LogError($"Initial position {pos} is outside the board (size: {BoardSize})!", this);
                return null;
            }
            else if (_board[pos.x, pos.y] == null)
            {
                Debug.LogError($"No tile at {pos} to start searching from. Make sure to add the tile to the board first!", this);
                return null;
            }

            int areaId = _board[pos.x, pos.y].DirectionToAreaId(dir);
            AreaDiscoveryData data = new(areaId, pos, _boardSize);
            return DiscoverTileInDirection(pos, dir, data);
        }

        private AreaDiscoveryData DiscoverTileInDirection(Vector2Int fromPos, Vector2Int checkDir, AreaDiscoveryData data)
        {
            // First calculate where we're going and from which direction
            Vector2Int discoverPos = fromPos + checkDir;
            Vector2Int discoverDir = -checkDir;

            if (!IsTileOccupied(discoverPos) || !data.IsDiscoverable(discoverPos))
            {
                // We either reached the border, an empty or a visited tile -> end recursion
                return data;
            }

            Tile fromTile = _board[fromPos.x, fromPos.y];
            Tile discoveredTile = _board[discoverPos.x, discoverPos.y];

            if (!fromTile.DoOpposingSidesMatch(discoveredTile, checkDir))
            {
                // Areas of the touching sides do not match -> Also end the recursion
                return data;
            }

            // We can now visit the tile
            data.RecordTileVisit(discoverPos);

            // ... and then visit & search from the areas on that tile
            if (discoveredTile.AreaConnectionExists(discoverDir, Vector2Int.zero))
            {
                // Do not search from this one, as there is no connection. Duh!
                data.NumAreas++;
            }

            if (discoveredTile.AreaConnectionExists(discoverDir, Vector2Int.left))
            {
                data.NumAreas++;
                DiscoverTileInDirection(discoverPos, Vector2Int.left, data);
            }

            if (discoveredTile.AreaConnectionExists(discoverDir, Vector2Int.right))
            {
                data.NumAreas++;
                DiscoverTileInDirection(discoverPos, Vector2Int.right, data);
            }

            if (discoveredTile.AreaConnectionExists(discoverDir, Vector2Int.down))
            {
                data.NumAreas++;
                DiscoverTileInDirection(discoverPos, Vector2Int.up, data);
            }

            if (discoveredTile.AreaConnectionExists(discoverDir, Vector2Int.down))
            {
                data.NumAreas++;
                DiscoverTileInDirection(discoverPos, Vector2Int.down, data);
            }
            return data;
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
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool IsPosInBounds(Vector2Int pos) => IsValidBoardPosition(pos, _boardSize);

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

        [ContextMenu("Print Board")]
        private void PrintBoard() => Debug.Log(ToString());

        /// <summary>
        /// Prints the tiles of the board as string.
        /// </summary>
        /// <returns>String representation of the board with its tiles.</returns>
        public override string ToString()
        {
            string board = "";
            for (int y = 0; y < BoardSize.y; y++)
            {
                for (int x = 0; x < BoardSize.x; x++)
                {
                    board += _board[x, y] != null ? _board[x, y].ToStringStringRow1() : Tile.EMPTY_TILE_ROW;
                }
                board += "\n";
                for (int x = 0; x < BoardSize.x; x++)
                {
                    board += _board[x, y] != null ? _board[x, y].ToStringStringRow2() : Tile.EMPTY_TILE_ROW;
                }
                board += "\n";
                for (int x = 0; x < BoardSize.x; x++)
                {
                    board += _board[x, y] != null ? _board[x, y].ToStringStringRow3() : Tile.EMPTY_TILE_ROW;
                }
                board += "\n\n";
            }
            return board;
        }


        /// <summary>
        /// Check if the position is a valid position for the provided board size. 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="boardSize"></param>
        /// <returns></returns>
        public static bool IsValidBoardPosition(Vector2Int pos, Vector2Int boardSize)
        {
            return pos.x >= 0 && pos.x < boardSize.x
                && pos.y >= 0 && pos.y < boardSize.y;
        }


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