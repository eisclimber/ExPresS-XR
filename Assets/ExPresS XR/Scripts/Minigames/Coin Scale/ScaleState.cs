/*
    Script Name: ScaleState.cs
    Author: Kevin Koerner
    Refactoring & Integration: Luca Dreiling
    Purpose: Represents the state of a scale (one side heavier or both equal).
*/
namespace ExPresSXR.Minigames.CoinScale
{
    public class ScaleState
    {
        public enum BowlPosition
        {
            Down,
            Center,
            Up
        }

        /// <summary>
        /// The elevation state of the left bowl.
        /// </summary>
        public BowlPosition LeftBowlPosition { get; }

        /// <summary>
        /// The elevation state of the right bowl.
        /// </summary>
        public BowlPosition RightBowlPosition { get; }

        /// <summary>
        /// Creates a new `ScaleState`-Instance using the weights of two bowls.
        /// </summary>
        /// <param name="leftWeight">Left Weight</param>
        /// <param name="rightWeight">Right Weight</param>
        /// <returns>The resulting scale state.</returns>
        public static ScaleState CreateFromWeights(int leftWeight, int rightWeight)
        {
            if (leftWeight < rightWeight)
            {
                return new ScaleState(BowlPosition.Up, BowlPosition.Down);
            }
            else if (leftWeight > rightWeight)
            {
                return new ScaleState(BowlPosition.Down, BowlPosition.Up);
            }
            return new ScaleState(BowlPosition.Center, BowlPosition.Center);
        }

        private ScaleState(BowlPosition left, BowlPosition right)
        {
            LeftBowlPosition = left;
            RightBowlPosition = right;
        }
    }
}