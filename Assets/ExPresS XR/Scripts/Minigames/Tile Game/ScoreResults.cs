
using UnityEngine;

namespace ExPresSXR.Minigames.TileGame
{
    /// <summary>
    /// The individual results of placing a tile in a tile game. 
    /// </summary>
    public class ScoreResults
    {
        /// <summary>
        /// Total score for placing the tile (sum of all other scores).
        /// </summary>
        public int TotalScore;

        /// <summary>
        /// Score of the center area.
        /// </summary>
        public int CenterScore;

        /// <summary>
        /// Type of the center area.
        /// </summary>
        public int CenterAreaId;

        /// <summary>
        /// Score of the up area.
        /// </summary>
        public int UpScore;

        /// <summary>
        /// Type of the up area.
        /// </summary>
        public int UpAreaId;

        /// <summary>
        /// Score of the down area.
        /// </summary>
        public int DownScore;

        /// <summary>
        /// Type of the down area.
        /// </summary>
        public int DownAreaId;

        /// <summary>
        /// Score of the left area.
        /// </summary>
        public int LeftScore;

        /// <summary>
        /// Type of the left area.
        /// </summary>
        public int LeftAreaId;

        /// <summary>
        /// Score of the right area.
        /// </summary>
        public int RightScore;

        /// <summary>
        /// Score of the right area.
        /// </summary>
        public int RightAreaId;

        /// <summary>
        /// Creates a zero-score.
        /// </summary>
        public ScoreResults() { }
        
        /// <summary>
        /// Creates a score without area types.
        /// </summary>
        /// <param name="centerScore">Center Score.</param>
        /// <param name="upScore">Up Score.</param>
        /// <param name="downScore">Down Score.</param>
        /// <param name="leftScore">Left Score.</param>
        /// <param name="rightScore">Right Score.</param>
        public ScoreResults(int centerScore, int upScore, int downScore, int leftScore, int rightScore)
        {
            TotalScore = centerScore + upScore + downScore + leftScore + rightScore;

            CenterScore = centerScore;
            UpScore = upScore;
            DownScore = downScore;
            LeftScore = leftScore;
            RightScore = rightScore;
        }

        /// <summary>
        /// Creates a score with area types.
        /// </summary>
        /// <param name="centerScore">Center Score.</param>
        /// <param name="centerAreaId">Center area type.</param>
        /// <param name="upScore">Up Score.</param>
        /// <param name="upAreaId">Up area type.</param>
        /// <param name="downScore">Down Score.</param>
        /// <param name="downAreaId">Down Score.</param>
        /// <param name="leftScore">Left Score.</param>
        /// <param name="leftAreaId">Left area type.</param>
        /// <param name="rightScore">Right Score.</param>
        /// <param name="rightAreaId">Right area type.</param>
        public ScoreResults(int centerScore, int centerAreaId, int upScore, int upAreaId, int downScore,
                            int downAreaId, int leftScore, int leftAreaId, int rightScore, int rightAreaId)
        {
            TotalScore = centerScore + upScore + downScore + leftScore + rightScore;

            CenterScore = centerScore;
            CenterAreaId = centerAreaId;
            UpScore = upScore;
            UpAreaId = upAreaId;
            DownScore = downScore;
            DownAreaId = downAreaId;
            LeftScore = leftScore;
            LeftAreaId = leftAreaId;
            RightScore = rightScore;
            RightAreaId = rightAreaId;
        }

        /// <summary>
        /// Prints the score to the console for debugging.
        /// </summary>
        public void PrintScore()
        {
            Debug.Log($"Final Score: {TotalScore}\n"
                + $"- Center (ID: {CenterAreaId}): {CenterScore}\n"
                + $"- Up (ID: {UpAreaId}): {UpScore}\n"
                + $"- Down (ID: {DownAreaId}): {DownScore}\n"
                + $"- Left (ID: {LeftAreaId}): {LeftScore}\n"
                + $"- Right (ID: {RightAreaId}): {RightScore}");
        }
    }
}