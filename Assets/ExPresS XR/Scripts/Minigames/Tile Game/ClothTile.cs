using System;
using ExPresSXR.Misc;
using UnityEngine;

namespace ExPresSXR.Minigames.TileGame
{
    [Serializable]
    public class ClothTile
    {
        [SerializeField]
        public ClothType CenterCloth;

        [SerializeField]
        public ClothType TopCloth;

        [SerializeField]
        public ClothType BottomCloth;

        [SerializeField]
        public ClothType LeftCloth;

        [SerializeField]
        public ClothType RightCloth;

        public ClothTile()
        {
            CenterCloth = GetRandomClothType();
            TopCloth = GetRandomClothType();
            BottomCloth = GetRandomClothType();
            LeftCloth = GetRandomClothType();
            RightCloth = GetRandomClothType();
        }

        public ClothTile(ClothType clothType)
        {
            CenterCloth = clothType;
            TopCloth = clothType;
            BottomCloth = clothType;
            LeftCloth = clothType;
            RightCloth = clothType;
        }

        public ClothTile(ClothType centerCloth, ClothType topCloth, ClothType bottomCloth, ClothType leftCloth, ClothType rightCloth)
        {
            CenterCloth = centerCloth;
            TopCloth = topCloth;
            BottomCloth = bottomCloth;
            LeftCloth = leftCloth;
            RightCloth = rightCloth;
        }

        public bool IsAdjacentConnected(ClothTile other, Vector2Int checkDir)
        {
            if (other == null)
            {
                return false;
            }

            ClothType ownType = DirectionToClothType(checkDir);
            ClothType otherType = other.DirectionToClothType(-checkDir); // Flip direction

            return ownType == otherType;
        }

        public bool AreEdgesTypeConnected(Vector2Int aDir, Vector2Int bDir)
        {
            if (aDir == bDir)
            {
                return false; // Same are not considered connected
            }

            ClothType aType = DirectionToClothType(aDir);
            ClothType bType = DirectionToClothType(bDir);

            if (aDir == Vector2Int.zero || bDir == Vector2Int.zero)
            {
                return aType == bType; // Either on is center
            }
            else if (aDir != -bDir)
            {
                return aType == bType; // Assuming they are connected via a corner (i.e. not on opposite sides)
            }

            return aType == CenterCloth && aType == bType; // If opposing must be connected via center too
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
                ClothType oldTop = TopCloth;
                TopCloth = RightCloth;
                RightCloth = BottomCloth;
                BottomCloth = LeftCloth;
                LeftCloth = oldTop;
            }
        }


        public ClothType DirectionToClothType(Vector2Int dir)
        {
            if (dir == Vector2Int.zero)
            {
                return CenterCloth;
            }
            else if (dir == Vector2Int.down)
            {
                return TopCloth;
            }
            else if (dir == Vector2Int.up)
            {
                return BottomCloth;
            }
            else if (dir == Vector2Int.left)
            {
                return LeftCloth;
            }
            else if (dir == Vector2Int.right)
            {
                return RightCloth;
            }
            Debug.LogWarning($"Could not determine ClothType for dir {dir}. Returning center ClothType instead.");
            return CenterCloth;
        }

        private ClothType GetRandomClothType()
        {
            int numValues = Enum.GetValues(typeof(ClothType)).Length;
            System.Random rng = new();
            return (ClothType)rng.Next(0, numValues);
        }
    }
}