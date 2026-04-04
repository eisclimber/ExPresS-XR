using System.Linq;
using UnityEngine;
using static ExPresSXR.Minigames.TileGame.TileGame;

namespace ExPresSXR.Minigames.TileGame
{
    /// <summary>
    /// A configurable implementation of a scoring logic.
    /// An in depth description can be found in the Readme placed alongside the TileGame Prefabs.
    /// </summary>
    public class BasicScoreCalculator : ScoreCalculator
    {
        /// <summary>
        /// Points granted when placing a tile.
        /// </summary>
        [SerializeField]
        [Tooltip("Points granted when placing a tile.")]
        private int _tilePlacementPoints = 0;

        [Space]

        /// <summary>
        /// How or what is counted when checking the connected tiles.
        /// </summary>
        [SerializeField]
        [Tooltip("How or what is counted when checking the connected tiles.")]
        private CountType _countType = CountType.Areas;

        /// <summary>
        /// Multiplier to for the counted objects. Will always be rounded up.
        /// </summary>
        [SerializeField]
        [Tooltip("Multiplier to for the counted objects. Will always be rounded up.")]
        private float _countMultiplier = 1.0f;

        [Space]

        /// <summary>
        /// Defines how scores of the placed tile get accumulated.
        /// </summary>
        [SerializeField]
        [Tooltip("Defines how scores of the placed tile get accumulated.")]
        private ScoreAccumulation _scoreAccumulation = ScoreAccumulation.Individual;

        /// <summary>
        /// Multiplier to for the accumulated scores. Will always be rounded up.
        /// </summary>
        [SerializeField]
        [Tooltip("Multiplier to for the accumulated scores. Will always be rounded up.")]
        private float _scoreAccumulationMultiplier = 1.0f;

        /// <summary>
        /// Calculates the score based on the PlacementData.
        /// </summary>
        /// <param name="data">Context of the placed tile.</param>
        /// <returns>Score for that placement.</returns>
        public override ScoreResults CalculateScore(PlacementData data)
        {
            if (data == null || !data.Valid)
            {
                Debug.LogError($"Invalid PlacementData ({data}) provided, returning zero-score.");
                return new();
            }

            Tile placedTile = data.Tile;

            int numUp = GetAdjacencyCount(data.Up.NumTiles, data.Up.NumAreas);
            int numDown = GetAdjacencyCount(data.Down.NumTiles, data.Down.NumAreas);
            int numLeft = GetAdjacencyCount(data.Left.NumTiles, data.Left.NumAreas);
            int numRight = GetAdjacencyCount(data.Right.NumTiles, data.Right.NumAreas);

            int[] numArray = new int[] { _tilePlacementPoints, numUp, numDown, numLeft, numRight };

            int upScore = AccumulateDirectionalScore(placedTile, Vector2Int.up, numUp, numArray);
            int downScore = AccumulateDirectionalScore(placedTile, Vector2Int.down, numDown, numArray);
            int leftScore = AccumulateDirectionalScore(placedTile, Vector2Int.left, numLeft, numArray);
            int rightScore = AccumulateDirectionalScore(placedTile, Vector2Int.right, numRight, numArray);

            return new(
                _tilePlacementPoints, placedTile.CenterAreaId,
                upScore, placedTile.UpAreaId,
                downScore, placedTile.DownAreaId,
                leftScore, placedTile.LeftAreaId,
                rightScore, placedTile.RightAreaId
            );
        }

        private int GetAdjacencyCount(int numTiles, int numAreas)
        {
            int count = _countType == CountType.Tiles ? numTiles : numAreas;
            return Mathf.CeilToInt(_countMultiplier * count);
        }

        private int AccumulateDirectionalScore(Tile tile, Vector2Int dir, int individualScore, int[] numArray)
        {
            int score = individualScore;
            if (_scoreAccumulation == ScoreAccumulation.ConnectedGroups)
            {
                score = GetConnectedGroupCount(tile, dir, numArray);
            }
            return Mathf.CeilToInt(_scoreAccumulationMultiplier * score);
        }

        private int GetConnectedGroupCount(Tile tile, Vector2Int dir, int[] numArray)
        {
            bool[] connections = tile.GetAreaConnections(dir, true);
            // Sums up all the points per direction if there is a connection
            return numArray.Zip(connections, (i, b) => new { i, b })
                            .Where(x => x.b)
                            .Sum(x => x.i);
        }

        /// <summary>
        /// How the scores of adjacent areas are accumulated on the placed tile.
        /// </summary>
        public enum ScoreAccumulation
        {
            /// <summary> Handles the scores of each side separately. </summary>
            Individual,
            /// <summary> Accumulates the scores of adjacent scores sides per direction. </summary>
            ConnectedGroups
        }

        /// <summary>
        /// What is counted when calculating the score.
        /// </summary>
        public enum CountType
        {
            /// <summary> Counts connected tiles. </summary>
            Tiles,
            /// <summary> Counts connected areas. </summary>
            Areas
        }
    }
}