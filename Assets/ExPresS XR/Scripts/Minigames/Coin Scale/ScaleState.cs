using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        public BowlPosition leftBowlPosition { get; }
        public BowlPosition rightBowlPosition { get; }


        public ScaleState(int leftWeight, int rightWeight)
        {
            if (leftWeight < rightWeight)
            {
                new ScaleState(BowlPosition.Up, BowlPosition.Down);
            }
            else if (leftWeight > rightWeight)
            {
                new ScaleState(BowlPosition.Down, BowlPosition.Up);
            }
            else
            {
                new ScaleState(BowlPosition.Center, BowlPosition.Center);
            }
        }

        private ScaleState(BowlPosition left, BowlPosition right)
        {
            leftBowlPosition = left;
            rightBowlPosition = right;
        }
    }
}