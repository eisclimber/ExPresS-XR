/*
    Script Name: ScaleState.cs
    Author: Kevin Koerner
    Refactoring & Integration: Luca Dreiling
    Purpose: Represents the state of a scale (one side heavier or both equal).
*/
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
            leftBowlPosition = left;
            rightBowlPosition = right;
        }
    }
}