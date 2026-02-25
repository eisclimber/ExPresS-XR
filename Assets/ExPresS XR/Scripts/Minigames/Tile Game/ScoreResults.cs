
using UnityEngine;

namespace ExPresSXR.Minigames.TileGame
{
    public class ScoreResults
    {
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
        /// Score of the top area.
        /// </summary>
        public int TopScore;

        /// <summary>
        /// Type of the top area.
        /// </summary>
        public int TopAreaId;

        /// <summary>
        /// Score of the bottom area.
        /// </summary>
        public int BottomScore;

        /// <summary>
        /// Type of the bottom area.
        /// </summary>
        public int BottomAreaId;

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
        /// <param name="topScore">Top Score.</param>
        /// <param name="bottomScore">Bottom Score.</param>
        /// <param name="leftScore">Left Score.</param>
        /// <param name="rightScore">Right Score.</param>
        public ScoreResults(int centerScore, int topScore, int bottomScore, int leftScore, int rightScore)
        {
            TotalScore = centerScore + topScore + bottomScore + leftScore + rightScore;

            CenterScore = centerScore;
            TopScore = topScore;
            BottomScore = bottomScore;
            LeftScore = leftScore;
            RightScore = rightScore;
        }

        /// <summary>
        /// Creates a score with area types.
        /// </summary>
        /// <param name="centerScore">Center Score.</param>
        /// <param name="centerAreaId">Center area type.</param>
        /// <param name="topScore">Top Score.</param>
        /// <param name="topAreaId">Top area type.</param>
        /// <param name="bottomScore">Bottom Score.</param>
        /// <param name="bottomAreaId">Bottom Score.</param>
        /// <param name="leftScore">Left Score.</param>
        /// <param name="leftAreaId">Left area type.</param>
        /// <param name="rightScore">Right Score.</param>
        /// <param name="rightAreaId">Right area type.</param>
        public ScoreResults(int centerScore, int centerAreaId, int topScore, int topAreaId, int bottomScore,
                            int bottomAreaId, int leftScore, int leftAreaId, int rightScore, int rightAreaId)
        {
            TotalScore = centerScore + topScore + bottomScore + leftScore + rightScore;

            CenterScore = centerScore;
            CenterAreaId = centerAreaId;
            TopScore = topScore;
            TopAreaId = topAreaId;
            BottomScore = bottomScore;
            BottomAreaId = bottomAreaId;
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
                + $"- Center ({CenterAreaId}): {CenterScore}\n"
                + $"- Top ({TopAreaId}): {TopScore}\n"
                + $"- Bottom ({BottomAreaId}): {BottomScore}\n"
                + $"- Left ({LeftAreaId}): {LeftScore}\n"
                + $"- Right ({RightAreaId}): {RightScore})");
        }
    }
}