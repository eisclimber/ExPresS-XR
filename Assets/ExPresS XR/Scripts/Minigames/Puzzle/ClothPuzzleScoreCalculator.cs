using ExPresSXR.Misc.Timing;
using UnityEngine;

namespace ExPresSXR.Minigames.Puzzle
{
    public class ClothPuzzleScoreCalculator : MonoBehaviour
    {
        [SerializeField]
        private int _baseTilePoints = 100;

        [SerializeField]
        private AnimationCurve _tileTimeBonusDistribution;

        [SerializeField]
        private float _maxTileTimeBonusCutoff = 10.0f;

        [SerializeField]
        private int _maxTileTimeBonusPoints = 250;

        [Space]

        [SerializeField]
        private int _puzzleCompletionPoints = 250;
        public int PuzzleCompletionPoints
        {
            get => _puzzleCompletionPoints;
        }

        [SerializeField]
        private int _gameCompletionPoints = 500;

        [SerializeField]
        private AnimationCurve _timeBonusDistribution;

        [SerializeField]
        private int _maxTimeBonusPoints = 2500;

        public CompositePoints CalculateTilePlacementScore(float currentTime, float lastTime)
        {
            float timeDiff = currentTime - lastTime;
            float timePct = Mathf.Clamp01(timeDiff / _maxTileTimeBonusCutoff);
            float bonusPct = _tileTimeBonusDistribution.Evaluate(timePct);
            int timeBonus = (int)(bonusPct * _maxTileTimeBonusPoints);
            // Debug.Log($"Calculating tile placement score: curr: {currentTime} x last {lastTime} x bonusPct {bonusPct} x timeBonus {timeBonus}");
            return new(_baseTilePoints, timeBonus);
        }

        public CompositePoints CalculatePuzzleCompletionBonus() => new(_puzzleCompletionPoints, 0);
        public CompositePoints CalculateAllCompletionBonus(float remainingTime, float maxTime)
        {
            if (remainingTime <= 0.0f)
            {
                return new(0, 0); // Time out -> not completed
            }

            float timePct = Mathf.Clamp01(remainingTime / maxTime);
            float bonusPct = _timeBonusDistribution.Evaluate(timePct);
            int completionBonus = (int)(bonusPct * _maxTimeBonusPoints);
            // Debug.Log($"Calculating tile placement score: tpct: {timePct} x bonusPct {bonusPct} x timeBonus {completionBonus}");
            return new(_puzzleCompletionPoints + _gameCompletionPoints, completionBonus);
        }

        public class CompositePoints
        {
            public int BasePoints;
            public int TimeBonus;

            public int TotalPoints { get => BasePoints + TimeBonus; }

            public CompositePoints(int tilePoints, int timeBonus)
            {
                BasePoints = tilePoints;
                TimeBonus = timeBonus;
            }
        }
    }
}