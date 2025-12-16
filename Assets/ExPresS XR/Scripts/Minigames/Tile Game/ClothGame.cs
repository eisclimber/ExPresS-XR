using ExPresSXR.Misc;
using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.TileGame
{
    public class ClothGame : MonoBehaviour
    {
        public const int FIELD_WIDTH = 5;
        public const int FIELD_HEIGHT = 3;

        [SerializeField]
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
        private int _placedTiles;
        public int PlacedTiles
        {
            get => _placedTiles;
            set
            {
                int diff = value - _placedTiles;
                _placedTiles = value;

                if (_placedTiles >= NumBoardSlot)
                {
                    Invoke(nameof(NotifyCompletion), _completionDelay);
                }
            }
        }

        [SerializeField]
        private float _completionDelay = 1.0f;


        [SerializeField]
        private Color[] _clothTypeScoreColors = new Color[3];


        public int NumBoardSlot
        {
            get => FIELD_WIDTH * FIELD_HEIGHT;
        }


        private ClothTile[,] _board = new ClothTile[FIELD_WIDTH, FIELD_HEIGHT];

        public UnityEvent<Vector2Int> OnTileAdded;
        public UnityEvent<int> OnScoreChanged;
        public UnityEvent<int> OnGameCompleted;

        public void AddClothTileFromDisplay(ClothTileSubmitSocket.BoardSubmitContext ctx)
        {
            ClothTileDisplay display = ctx.TileDisplay;
            ScoreResults score = AddClothTileAt(display.DisplayedTile, ctx.BoardPos);
            display.DisplayScore(score);
            TotalScore += score.TotalScore;
        }

        public ScoreResults AddClothTileAt(ClothTile tile, Vector2Int pos)
        {
            if (!IsPosInbounds(pos))
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
            bool[,] visitedTop = new bool[FIELD_WIDTH, FIELD_HEIGHT];
            visitedTop[pos.x, pos.y] = true;
            bool[,] visitedBottom = new bool[FIELD_WIDTH, FIELD_HEIGHT];
            visitedBottom[pos.x, pos.y] = true;
            bool[,] visitedLeft = new bool[FIELD_WIDTH, FIELD_HEIGHT];
            visitedLeft[pos.x, pos.y] = true;
            bool[,] visitedRight = new bool[FIELD_WIDTH, FIELD_HEIGHT];
            visitedRight[pos.x, pos.y] = true;

            ScoreResults score = new(
                1,
                ClothTypeToColor(tile.CenterCloth),
                CheckNeighbor(pos, Vector2Int.down, visitedTop), // Invert neighbor up/down dir since were using different origins
                ClothTypeToColor(tile.TopCloth),
                CheckNeighbor(pos, Vector2Int.up, visitedBottom), // Invert neighbor up/down dir since were using different origins
                ClothTypeToColor(tile.BottomCloth),
                CheckNeighbor(pos, Vector2Int.left, visitedLeft),
                ClothTypeToColor(tile.LeftCloth),
                CheckNeighbor(pos, Vector2Int.right, visitedRight),
                ClothTypeToColor(tile.RightCloth)
            );
            return score;
        }

        public int EvaluatePointsFrom(Vector2Int pos, int score, bool[,] visited)
        {
            if (!IsPosInbounds(pos))
            {
                Debug.Log($"Start pos out of bounds at {pos}.", this);
                return score;
            }

            if (!IsTileOccupied(pos))
            {
                Debug.Log($"Tile at {pos} not occupied.", this);
                return score;
            }

            if (visited[pos.x, pos.y])
            {
                return score;
            }

            visited[pos.x, pos.y] = true;
            score++;

            score += CheckNeighbor(pos, Vector2Int.down, visited); // Invert neighbor up/down dir since were using different origins
            score += CheckNeighbor(pos, Vector2Int.up, visited); // Invert neighbor up/down dir since were using different origins
            score += CheckNeighbor(pos, Vector2Int.left, visited);
            score += CheckNeighbor(pos, Vector2Int.right, visited);
            return score;
        }

        public int CheckNeighbor(Vector2Int pos, Vector2Int checkDir, bool[,] visited)
        {
            Vector2Int nextPos = pos + checkDir;
            if (!IsPosInbounds(nextPos))
            {
                return 0;
            }

            ClothTile currentTile = _board[pos.x, pos.y];
            ClothTile nextTile = _board[nextPos.x, nextPos.y];

            if (currentTile.IsAdjacentConnected(nextTile, checkDir))
            {
                return EvaluatePointsFrom(nextPos, 0, visited);
            }
            return 0;
        }

        [ContextMenu("Complete Game")]
        private void NotifyCompletion()
        {
            OnGameCompleted.Invoke(_totalScore);
            Debug.Log($"Completed with a score of: {_totalScore}.");
        }

        public bool IsTileOccupied(Vector2Int pos)
        {
            return IsPosInbounds(pos) && _board[pos.x, pos.y] != null;
        }

        public bool IsPosInbounds(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < FIELD_WIDTH
                && pos.y >= 0 && pos.y < FIELD_HEIGHT;
        }

        [ContextMenu("Add Test Score")]
        public void AddTestScore() => TotalScore += Random.Range(1, 20);

        private Color ClothTypeToColor(ClothType type)
        {
            int idx = (int)type;
            return idx >= 0 && idx < _clothTypeScoreColors.Length ? _clothTypeScoreColors[idx] : Color.black;
        }

        public class ScoreResults
        {
            public int TotalScore;

            public int CenterScore;
            public Color CenterColor;
            public int TopScore;
            public Color TopColor;
            public int BottomScore;
            public Color BottomColor;
            public int LeftScore;
            public Color LeftColor;
            public int RightScore;
            public Color RightColor;

            public ScoreResults() { } // Empty/Zero score
            public ScoreResults(int centerScore, int topScore, int bottomScore, int leftScore, int rightScore)
            {
                TotalScore = centerScore + topScore + bottomScore + leftScore + rightScore;

                CenterScore = centerScore;
                TopScore = topScore;
                BottomScore = bottomScore;
                LeftScore = leftScore;
                RightScore = rightScore;
            }

            public ScoreResults(int centerScore, Color centerColor, int topScore, Color topColor, int bottomScore,
                                Color bottomColor, int leftScore, Color leftColor, int rightScore, Color rightColor)
            {
                TotalScore = centerScore + topScore + bottomScore + leftScore + rightScore;

                CenterScore = centerScore;
                CenterColor = centerColor;
                TopScore = topScore;
                TopColor = topColor;
                BottomScore = bottomScore;
                BottomColor = bottomColor;
                LeftScore = leftScore;
                LeftColor = leftColor;
                RightScore = rightScore;
                RightColor = rightColor;
            }

            public void PrintScore()
            {
                Debug.Log($"Final Score: {TotalScore} ({CenterScore} x {TopScore} x {BottomScore} x {LeftScore} x {RightScore})");
            }
        }
    }

    public enum ClothType
    {
        LightBurlap,
        DarkBurlap,
        Leather
    }
}