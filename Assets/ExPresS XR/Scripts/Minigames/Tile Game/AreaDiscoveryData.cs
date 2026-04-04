using UnityEngine;

namespace ExPresSXR.Minigames.TileGame
{
    /// <summary>
    /// Data asset combining relevant information used to discover adjacent tiles for the board game.
    /// </summary>
    public class AreaDiscoveryData
    {
        /// <summary>
        /// Area id used for adjacency.
        /// </summary>
        public readonly int AreaId;

        /// <summary>
        /// Board size used for validity checks.
        /// </summary>
        public readonly Vector2Int BoardSize;

        /// <summary>
        /// keeps track of if a tile was visited.
        /// </summary>
        public bool[,] Visited;

        /// <summary>
        /// Number of tiles found.
        /// </summary>
        public int NumTiles;

        /// <summary>
        /// Number of tiles found.
        /// </summary>
        public int NumAreas;


        /// <summary>
        /// Creates a new AreaDiscoveryData instance.
        /// </summary>
        /// <param name="areaId">Area Id to be searched with.</param>
        /// <param name="startPos">Position to start from.</param>
        /// <param name="boardSize">Size of the board to search.</param>
        public AreaDiscoveryData(int areaId, Vector2Int startPos, Vector2Int boardSize)
        {
            AreaId = areaId;

            BoardSize = boardSize;
            Visited = new bool[boardSize.x, boardSize.y];
            RecordTileVisit(startPos);
        }

        /// <summary>
        /// Checks if the tile is valid in undiscovered.
        /// </summary>
        /// <param name="pos">Position to check.</param>
        /// <returns>Position is valid and undiscovered.</returns>
        public bool IsDiscoverable(Vector2Int pos) => IsPosInBounds(pos) && !Visited[pos.x, pos.y];

        /// <summary>
        /// Records a visit of a new tile, increasing both NumTiles and NumAreas by 1. 
        /// </summary>
        /// <param name="pos">Position to visit.</param>
        public void RecordTileVisit(Vector2Int pos)
        {
            if (!IsPosInBounds(pos))
            {
                Debug.LogError($"Invalid board pos {pos} for board size {BoardSize}. Skipping setting tile visited.");
                return;
            }
            Visited[pos.x, pos.y] = true;
            NumTiles++;
            NumAreas++;
        }

        private bool IsPosInBounds(Vector2Int pos) => TileGame.IsValidBoardPosition(pos, BoardSize);

        /// <summary>
        /// Prints the relevant information.
        /// </summary>
        /// <returns>String representation of a AreaDiscoveryData.</returns>
        public override string ToString()
        {
            return $"[AreaId: {AreaId}, NumTiles: {NumTiles}, NumAreas: {NumAreas}]";
        }
    }
}