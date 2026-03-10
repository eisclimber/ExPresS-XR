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
        /// String representation of a row of an empty tile.
        /// </summary>
        public const string EMPTY_TILE_ROW = "XXXXX";

        /// <summary>
        /// String representation of the spacer rows of a tile.
        /// </summary>
        public const string TILE_ROW_SPACER = "       ";

        /// <summary>
        /// Steps of rotations (90 degrees each).
        /// </summary>
        private const int NUM_STEPS = 4;

        /// <summary>
        /// Area type of the center area.
        /// </summary>
        [SerializeField]
        [Tooltip("Area type of the center area.")]
        public int CenterAreaId;

        /// <summary>
        /// Area type of the up area.
        /// </summary>
        [SerializeField]
        [Tooltip("Area type of the up area.")]
        public int UpAreaId;

        /// <summary>
        /// Area type of the down area.
        /// </summary>
        [SerializeField]
        [Tooltip("Area type of the down area.")]
        public int DownAreaId;

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
            UpAreaId = validId ? id : GetRandomAreaId(numAreas);
            DownAreaId = validId ? id : GetRandomAreaId(numAreas);
            LeftAreaId = validId ? id : GetRandomAreaId(numAreas);
            RightAreaId = validId ? id : GetRandomAreaId(numAreas);
        }

        /// <summary>
        /// Creates a new tile with the provided area types or the a random one if invalid.
        /// </summary>
        /// <param name="numAreas">Number of possible areas.</param>
        /// <param name="centerAreaId">Center area type.</param>
        /// <param name="upAreaId">Up area type.</param>
        /// <param name="downAreaId">Down area type.</param>
        /// <param name="leftAreaId">Left area type.</param>
        /// <param name="rightAreaId">Right area type.</param>
        public Tile(int numAreas, int centerAreaId, int upAreaId, int downAreaId, int leftAreaId, int rightAreaId)
        {
            CenterAreaId = IsValidAreaId(centerAreaId, numAreas) ? centerAreaId : GetRandomAreaId(numAreas);
            UpAreaId = IsValidAreaId(upAreaId, numAreas) ? upAreaId : GetRandomAreaId(numAreas);
            DownAreaId = IsValidAreaId(downAreaId, numAreas) ? downAreaId : GetRandomAreaId(numAreas);
            LeftAreaId = IsValidAreaId(leftAreaId, numAreas) ? leftAreaId : GetRandomAreaId(numAreas);
            RightAreaId = IsValidAreaId(rightAreaId, numAreas) ? rightAreaId : GetRandomAreaId(numAreas);
        }

        /// <summary>
        /// Whether the this tile has the same areaId in one direction as the other tile in the other direction.
        /// Does not check if the tiles are actually touching on the board!
        /// </summary>
        /// <param name="other">Tile to check the connection to.</param>
        /// <param name="checkDir">Direction to check in.</param>
        /// <returns>If the tiles area adjacently connected.</returns>
        public bool DoOpposingSidesMatch(Tile other, Vector2Int checkDir)
        {
            if (other == null || checkDir == Vector2Int.zero)
            {
                Debug.Log($"{other == null} || {checkDir == Vector2Int.zero}");
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
        public bool AreaConnectionExists(Vector2Int aDir, Vector2Int bDir)
        {
            if (aDir == bDir)
            {
                // Debug.Log($"{aDir} x {bDir}: Same dir");
                return false; // Same directions are not considered connected
            }

            int aType = DirectionToAreaId(aDir);
            int bType = DirectionToAreaId(bDir);

            if (aType != bType)
            {
                // Debug.Log($"{aDir} x {bDir}: Different types");
                return false; // Different types cant be connected
            }

            bool eitherIsCenter = aDir == Vector2Int.zero || bDir == Vector2Int.zero;
            bool opposing = aDir != -bDir;
            bool directlyAdjacent = eitherIsCenter || !opposing;

            if (directlyAdjacent)
            {
                // Debug.Log($"{aDir} x {bDir}: Directly adjacent");
                return aType == bType;
            }

            // Check if connected via center
            int cType = DirectionToAreaId(Vector2Int.zero);
            if (aType == cType)
            {
                // Debug.Log($"{aDir} x {bDir}: Center adjacent");
                return true; // Types connected via center
            }

            // Lastly we need to check if the areas are connected via two edges
            int dType = DirectionToAreaId(LazyRotate90(aDir, true));
            int eType = DirectionToAreaId(LazyRotate90(aDir, false));

            // Debug.Log($"{aDir} x {bDir}: Edge adjacent {aType == dType || aType == eType}");
            return aType == dType || aType == eType;
        }

        /// <summary>
        /// Returns an array containing true if there is a connection from `dir` to the direction.
        /// The order of entries is: [center, up, down, left, right].
        /// </summary>
        /// <param name="dir">Dir to check.</param>
        /// <param name="considerSelfConnected">If connections to the same direction are considered connected.</param>
        /// <returns>Array of bools if a connection area exists.</returns>
        public bool[] GetAreaConnections(Vector2Int dir, bool considerSelfConnected = true)
        {
            return new[] {
                (considerSelfConnected && dir == Vector2Int.zero) || AreaConnectionExists(dir, Vector2Int.zero),
                (considerSelfConnected && dir == Vector2Int.up) || AreaConnectionExists(dir, Vector2Int.up),
                (considerSelfConnected && dir == Vector2Int.down) || AreaConnectionExists(dir, Vector2Int.down),
                (considerSelfConnected && dir == Vector2Int.left) || AreaConnectionExists(dir, Vector2Int.left),
                (considerSelfConnected && dir == Vector2Int.right) || AreaConnectionExists(dir, Vector2Int.right)
            };
        }


        /// <summary>
        /// Rotates the tile data clockwise rounding to steps of 90 degrees.
        /// </summary>
        /// <param name="degrees"></param>
        public void RotateDegrees(float degrees) => Rotate(RuntimeUtils.PosMod(Mathf.RoundToInt(degrees / 90.0f), NUM_STEPS));

        /// <summary>
        /// Rotates the tile data counterclockwise in steps of 90 degrees.
        /// </summary>
        /// <param name="steps"></param>
        public void Rotate(int steps)
        {
            if (steps < 0 || steps > 3)
            {
                Debug.Log($"Negative or more than one revelation provided: {steps}. This should be avoided.");
                steps = RuntimeUtils.PosMod(steps, NUM_STEPS);
            }

            for (int i = 0; i < steps; i++)
            {
                int oldUp = UpAreaId;
                UpAreaId = LeftAreaId;
                LeftAreaId = DownAreaId;
                DownAreaId = RightAreaId;
                RightAreaId = oldUp;
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
                return UpAreaId;
            }
            else if (dir == Vector2Int.up)
            {
                return DownAreaId;
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

        /// <summary>
        /// Utility for printing the tile as three row string (row 1).
        /// </summary>
        /// <returns>First row of the tiles string representation.</returns>
        public string ToStringStringRow1() => $"  {UpAreaId}  ";

        /// <summary>
        /// Utility for printing the tile as three row string (row 2).
        /// </summary>
        /// <returns>Second row of the tiles string representation.</returns>
        public string ToStringStringRow2() => $"{LeftAreaId}{CenterAreaId}{RightAreaId}";

        /// <summary>
        /// Utility for printing the tile as three row string (row 3).
        /// </summary>
        /// <returns>Third row of the tiles string representation.</returns>
        public string ToStringStringRow3() => $"  {DownAreaId}  ";

        /// <summary>
        /// Prints the tile as 3x3 character string.
        /// </summary>
        /// <returns>String representation of the tile.</returns>
        public override string ToString() =>  $"{ToStringStringRow1()}\n{ToStringStringRow2()}\n{ToStringStringRow3()}";

        /// <summary>
        /// Utility to rotate directions by 90 degree by switching x and y coordinates.
        /// We also don't care about the actual rotation direction since we just want to check both in our code.
        /// </summary>
        /// <param name="v">Vector to rotate.</param>
        /// <param name="flipSigns">I the signs should be flipped (+ or - 90 degrees).</param>
        /// <returns>The vector rotated by 90 degrees.</returns>
        private static Vector2Int LazyRotate90(Vector2Int v, bool flipSigns)
        {
            return new(flipSigns ? -v.y : v.y, flipSigns ? -v.x : v.x);
        }

        private static bool IsValidAreaId(int id, int numAreas) => id >= 0 && id < numAreas;
    }
}