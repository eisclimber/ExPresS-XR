using UnityEngine;

namespace ExPresSXR.Minigames.TileGame
{
    /// <summary>
    /// Data about the results of a placement, grouping the directional discoveries.
    /// </summary>
    public class PlacementData
    {
        /// <summary>
        /// Tile placed.
        /// </summary>
        public Tile Tile;
        /// <summary>
        /// Position the tile was placed.
        /// </summary>
        public Vector2Int Pos;
        /// <summary>
        /// How many tiles have already been placed.
        /// </summary>
        public int Number;

        /// <summary>
        /// Data gathered for the up-direction.
        /// </summary>
        public AreaDiscoveryData Up;

        /// <summary>
        /// Data gathered for the down-direction.
        /// </summary>
        public AreaDiscoveryData Down;

        /// <summary>
        /// Data gathered for the left-direction.
        /// </summary>
        public AreaDiscoveryData Left;

        /// <summary>
        /// Data gathered for the right-direction.
        /// </summary>
        public AreaDiscoveryData Right;


        /// <summary>
        /// Checks if all necessary values have been provided (are not null).
        /// </summary>
        public bool Valid
        {
            get => Tile != null && Up != null && Down != null && Left != null && Right != null;
        }

        /// <summary>
        /// Creates a new placement data object.
        /// </summary>
        /// <param name="tile">Tile placed.</param>
        /// <param name="pos">Placement position.</param>
        /// <param name="number">When the board was placed.</param>
        /// <param name="left">Left discovery data.</param>
        /// <param name="right">Right discovery data.</param>
        /// <param name="up">Up discovery data.</param>
        /// <param name="down">Down discovery data.</param>
        public PlacementData(Tile tile, Vector2Int pos, int number,
                                AreaDiscoveryData up, AreaDiscoveryData down,
                                AreaDiscoveryData left, AreaDiscoveryData right)
        {
            Tile = tile;
            Pos = pos;
            Number = number;

            Left = left;
            Right = right;
            Up = up;
            Down = down;
        }

        /// <summary>
        /// Creates an invalid placement data object if placement is invalid.
        /// </summary>
        public PlacementData() { }
    }
}