
using UnityEngine;

namespace ExPresSXR.Minigames.TileGame
{
    public class ScoreResults
    {
        public int TotalScore;

        public int CenterScore;
        public int CenterAreaId;

        public int TopScore;
        public int TopAreaId;

        public int BottomScore;
        public int BottomAreaId;

        public int LeftScore;
        public int LeftAreaId;
        
        public int RightScore;
        public int RightAreaId;

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