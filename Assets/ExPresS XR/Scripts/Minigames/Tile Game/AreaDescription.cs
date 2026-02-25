using System;
using UnityEngine;

namespace ExPresSXR.Minigames.TileGame
{
    /// <summary>
    /// Description of an excavation area.
    /// </summary>
    [Serializable]
    public class AreaDescription
    {
        /// <summary>
        /// Id of the area
        /// </summary>
        public int Id;

        /// <summary>
        /// Display name of the area.
        /// </summary>
        public string Name;

        /// <summary>
        /// Color (for the scores of the area).
        /// </summary>
        public Color Color;

        /// <summary>
        /// Material representing the area.
        /// </summary>
        public Material Material;

        public AreaDescription(int id, string name, Color color, Material material)
        {
            Id = id;
            Name = name;
            Color = color;
            Material = material;
        }
    }
}