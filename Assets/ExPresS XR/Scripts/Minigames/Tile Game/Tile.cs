using System;
using ExPresSXR.Misc;
using UnityEngine;

namespace ExPresSXR.Minigames.TileGame
{
    /// <summary>
    /// Represents a tile of the game.
    /// The edges of a tile are programmatically represented by Vector2Ints with a length of 1 and (0, 0) for the center area.  
    /// </summary>
    [Serializable]
    public class Tile
    {
        /// <summary>
        /// Area type of the center area.
        /// </summary>
        [SerializeField]
        [Tooltip("Area type of the center area.")]
        public int CenterAreaId;

        /// <summary>
        /// Area type of the top area.
        /// </summary>
        [SerializeField]
        [Tooltip("Area type of the top area.")]
        public int TopAreaId;

        /// <summary>
        /// Area type of the bottom area.
        /// </summary>
        [SerializeField]
        [Tooltip("Area type of the bottom area.")]
        public int BottomAreaId;

        /// <summary>
        /// Area type of the left area.
        /// </summary>
        [SerializeField]
        [Tooltip("Area type of the left area.")]
        public int LeftAreaId;

        /// <summary>
        /// Area type of the right area.
        /// </summary>
        [SerializeField]
        [Tooltip("Area type of the right area.")]
        public int RightAreaId;

        /// <summary>
        /// Creates a new tile with a random area type or the provided `ìd` one if valid.
        /// </summary>
        /// <param name="numAreas">Number of possible areas.</param>
        /// <param name="id">Optional id to set all areas types to.</param>
        public Tile(int numAreas, int id = -1)
        {
            bool validId = IsValidAreaId(id, numAreas);

            CenterAreaId = validId ? id : GetRandomAreaId(numAreas);
            TopAreaId = validId ? id : GetRandomAreaId(numAreas);
            BottomAreaId = validId ? id : GetRandomAreaId(numAreas);
            LeftAreaId = validId ? id : GetRandomAreaId(numAreas);
            RightAreaId = validId ? id :GetRandomAreaId(numAreas);
        }

        /// <summary>
        /// Creates a new tile with the provided area types or the a random one if invalid.
        /// </summary>
        /// <param name="numAreas">Number of possible areas.</param>
        /// <param name="centerAreaId">Center area type.</param>
        /// <param name="topAreaId">Top area type.</param>
        /// <param name="bottomAreaId">Bottom area type.</param>
        /// <param name="leftAreaId">Left area type.</param>
        /// <param name="rightAreaId">Right area type.</param>
        public Tile(int numAreas, int centerAreaId, int topAreaId, int bottomAreaId, int leftAreaId, int rightAreaId)
        {
            CenterAreaId = IsValidAreaId(centerAreaId, numAreas) ? centerAreaId : GetRandomAreaId(numAreas);
            TopAreaId = IsValidAreaId(topAreaId, numAreas) ? topAreaId : GetRandomAreaId(numAreas);
            BottomAreaId = IsValidAreaId(bottomAreaId, numAreas) ? bottomAreaId : GetRandomAreaId(numAreas);
            LeftAreaId = IsValidAreaId(leftAreaId, numAreas) ? leftAreaId : GetRandomAreaId(numAreas);
            RightAreaId = IsValidAreaId(rightAreaId, numAreas) ? rightAreaId : GetRandomAreaId(numAreas);
        }

        /// <summary>
        /// Whether the tiles are adjacently connected in a direction by the same area type.
        /// </summary>
        /// <param name="other">Tile to check the connection to.</param>
        /// <param name="checkDir">Direction to check in.</param>
        /// <returns>If the tiles area adjacently connected.</returns>
        public bool IsAdjacentConnected(Tile other, Vector2Int checkDir)
        {
            if (other == null || checkDir == Vector2Int.zero)
            {
                return false;
            }

            int ownType = DirectionToAreaId(checkDir);
            int otherType = other.DirectionToAreaId(-checkDir); // Flip direction

            return ownType == otherType;
        }

        /// <summary>
        /// Checks if the edges (or areas) of this tile are connected via a continuous area.
        /// This is the case if they share a corner, are connected 
        /// </summary>
        /// <param name="aDir"></param>
        /// <param name="bDir"></param>
        /// <returns></returns>
        public bool AreEdgesTypeConnected(Vector2Int aDir, Vector2Int bDir)
        {
            if (aDir == bDir)
            {
                return false; // Same are not considered connected
            }

            int aType = DirectionToAreaId(aDir);
            int bType = DirectionToAreaId(bDir);

            bool eitherIsCenter = aDir == Vector2Int.zero || bDir == Vector2Int.zero;
            bool opposing = aDir != -bDir;
            bool directlyAdjacent = eitherIsCenter || !opposing;

            if (directlyAdjacent)
            {
                return aType == bType;
            }
            
            // Lastly we need to check if the areas are connected via two edges
            int cType = DirectionToAreaId(LazyRotate90(aDir, true));
            int dType = DirectionToAreaId(LazyRotate90(aDir, false));

            return (aType == cType || aType == dType) && aType == bType;
        }


        /// <summary>
        /// Rotates the tile data clockwise rounding to steps of 90 degrees.
        /// </summary>
        /// <param name="degrees"></param>
        public void RotateDegrees(float degrees) => Rotate(RuntimeUtils.PosMod(Mathf.RoundToInt(degrees / 90.0f), 4));

        /// <summary>
        /// Rotates the tile data clockwise in steps of 90 degrees.
        /// </summary>
        /// <param name="steps"></param>
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

        /// <summary>
        /// Returns the area type for the direction.
        /// </summary>
        /// <param name="dir">Direction to convert.</param>
        /// <returns>Area type of the direction or center area type if invalid.</returns>
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


        private static bool IsValidAreaId(int id, int numAreas) => id >= 0 && id < numAreas;

        private static Vector2Int LazyRotate90(Vector2Int v, bool flipSigns)
        {
            // Utility to rotate directions by 90 degree.
            // We also don't care about the actual rotation direction since we just want to check both.
            return new(flipSigns ? -v.y : v.y, flipSigns ? -v.x : v.x);
        }
    }
}