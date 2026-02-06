using System;
using ExPresSXR.Misc;
using UnityEngine;

namespace ExPresSXR.Minigames.TileGame
{
    [Serializable]
    public class Tile
    {
        [SerializeField]
        public int CenterAreaId;

        [SerializeField]
        public int TopAreaId;

        [SerializeField]
        public int BottomAreaId;

        [SerializeField]
        public int LeftAreaId;

        [SerializeField]
        public int RightAreaId;

        public Tile(int numAreas, int id = -1)
        {
            bool validId = IsValidAreaId(id, numAreas);

            CenterAreaId = validId ? id : GetRandomAreaId(numAreas);
            TopAreaId = validId ? id : GetRandomAreaId(numAreas);
            BottomAreaId = validId ? id : GetRandomAreaId(numAreas);
            LeftAreaId = validId ? id : GetRandomAreaId(numAreas);
            RightAreaId = validId ? id :GetRandomAreaId(numAreas);
        }

        public Tile(int numAreas, int centerAreaId, int topAreaId, int bottomAreaId, int leftAreaId, int rightAreaId)
        {
            CenterAreaId = IsValidAreaId(centerAreaId, numAreas) ? centerAreaId : GetRandomAreaId(numAreas);
            TopAreaId = IsValidAreaId(topAreaId, numAreas) ? topAreaId : GetRandomAreaId(numAreas);
            BottomAreaId = IsValidAreaId(bottomAreaId, numAreas) ? bottomAreaId : GetRandomAreaId(numAreas);
            LeftAreaId = IsValidAreaId(leftAreaId, numAreas) ? leftAreaId : GetRandomAreaId(numAreas);
            RightAreaId = IsValidAreaId(rightAreaId, numAreas) ? rightAreaId : GetRandomAreaId(numAreas);
        }

        public bool IsAdjacentConnected(Tile other, Vector2Int checkDir)
        {
            if (other == null)
            {
                return false;
            }

            int ownType = DirectionToAreaId(checkDir);
            int otherType = other.DirectionToAreaId(-checkDir); // Flip direction

            return ownType == otherType;
        }

        public bool AreEdgesTypeConnected(Vector2Int aDir, Vector2Int bDir)
        {
            if (aDir == bDir)
            {
                return false; // Same are not considered connected
            }

            int aType = DirectionToAreaId(aDir);
            int bType = DirectionToAreaId(bDir);

            if (aDir == Vector2Int.zero || bDir == Vector2Int.zero)
            {
                return aType == bType; // Either on is center
            }
            else if (aDir != -bDir)
            {
                return aType == bType; // Assuming they are connected via a corner (i.e. not on opposite sides)
            }

            return aType == CenterAreaId&& aType == bType; // If opposing must be connected via center too
        }


        /// <summary>
        /// Rotates clockwise in steps of 90 degrees.
        /// </summary>
        /// <param name="degrees"></param>
        public void RotateDegrees(float degrees) => Rotate(RuntimeUtils.PosMod(Mathf.RoundToInt(degrees / 90.0f), 90));

        public void Rotate(int steps)
        {
            if (steps < 0 || steps > 3)
            {
                Debug.Log($"Negative or more than one revelation provided: {steps}. This should be avoided.");
            }

            for (int i = 0; i < steps; i++)
            {
                int oldTop = TopAreaId;
                TopAreaId = RightAreaId;
                RightAreaId = BottomAreaId;
                BottomAreaId = LeftAreaId;
                LeftAreaId = oldTop;
            }
        }

        public int DirectionToAreaId(Vector2Int dir)
        {
            if (dir == Vector2Int.zero)
            {
                return CenterAreaId;
            }
            else if (dir == Vector2Int.down)
            {
                return TopAreaId;
            }
            else if (dir == Vector2Int.up)
            {
                return BottomAreaId;
            }
            else if (dir == Vector2Int.left)
            {
                return LeftAreaId;
            }
            else if (dir == Vector2Int.right)
            {
                return RightAreaId;
            }
            Debug.LogWarning($"Could not determine int for dir {dir}. Returning center int instead.");
            return CenterAreaId;
        }

        private int GetRandomAreaId(int numAreas) => UnityEngine.Random.Range(0, numAreas);

        private bool IsValidAreaId(int id, int numAreas) => id >= 0 && id < numAreas;
    }
}