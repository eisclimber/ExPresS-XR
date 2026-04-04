namespace ExPresSXR.Minigames.CoinScale
{
    /// <summary>
    /// Represents the state of a scale (one side heavier or both equal).
    /// </summary>
    public class ScaleState
    {
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

        /// <summary>
        /// The position of the bowl during weighing.
        /// </summary>
        public enum BowlPosition
        {
            /// <summary> Down position when weighing. </summary>
            Down,
            /// <summary> Center position when weighing. </summary>
            Center,
            /// <summary> Up position when weighing. </summary>
            Up
        }
    }
}