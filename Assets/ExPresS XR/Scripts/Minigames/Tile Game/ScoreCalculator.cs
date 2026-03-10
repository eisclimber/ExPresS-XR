using UnityEngine;
using static ExPresSXR.Minigames.TileGame.TileGame;

namespace ExPresSXR.Minigames.TileGame
{
    /// <summary>
    /// An abstract class to serve as interface for Unity's Component system,
    /// allowing you to implement scoring logic via inheritance.
    /// </summary>
    public abstract class ScoreCalculator : MonoBehaviour
    {
        /// <summary>
        /// Calculates the score based on the PlacementData.
        /// </summary>
        /// <param name="data">Context of the placed tile.</param>
        /// <returns>Score for that placement.</returns>
        public abstract ScoreResults CalculateScore(PlacementData data);

        /// <summary>
        /// A default implementation of deriving the score of a PlacementData used as fallback.
        /// </summary>
        /// <param name="data">Context of the placed tile.</param>
        /// <returns>Score for that placement.</returns>
        public static ScoreResults CalculateDefaultScore(PlacementData data)
        {
            if (data == null || !data.Valid)
            {
                Debug.LogError($"Invalid PlacementData ({data}) provided, returning zero-score.");
                return new();
            }

            Tile placedTile = data.Tile;

            return new(
                1, placedTile.CenterAreaId,
                data.Up.NumAreas, placedTile.UpAreaId,
                data.Down.NumAreas, placedTile.DownAreaId,
                data.Left.NumAreas, placedTile.LeftAreaId,
                data.Right.NumAreas, placedTile.RightAreaId
            );
        }
    }
}