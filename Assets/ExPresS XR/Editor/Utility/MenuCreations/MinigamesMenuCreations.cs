using UnityEditor;
using ExPresSXR.Editor.Utility;

namespace ExPresSXR.Editor.UtilityMenuCreation
{
    public static class MinigamesMenuCreations
    {
        [MenuItem("GameObject/ExPresS XR/Minigames/Boxing Minigame")]
        static void CreateBoxingMinigame(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Minigames/Boxing/Boxing Minigame");
        }

        [MenuItem("GameObject/ExPresS XR/Minigames/Coin Scale")]
        static void CreateCoinScaleMinigame(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Minigames/Coin Scale/Coin Scale Minigame");
        }

        [MenuItem("GameObject/ExPresS XR/Minigames/Coin Throw")]
        static void CreateCoinThrowMinigame(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Minigames/Coin Throw/Coin Throw Minigame");
        }

        [MenuItem("GameObject/ExPresS XR/Minigames/Excavation")]
        static void CreateExcavationMinigame(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Minigames/Excavation/Excavation Minigame");
        }

        [MenuItem("GameObject/ExPresS XR/Minigames/Puzzle")]
        static void CreatePuzzleMinigame(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Minigames/Puzzle/Puzzle Minigame");
        }

        [MenuItem("GameObject/ExPresS XR/Minigames/Sword Cleaning")]
        static void CreateSwordCleaningMinigame(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Minigames/Target Area/Sword Cleaning/Sword Cleaning Minigame");
        }

        [MenuItem("GameObject/ExPresS XR/Minigames/Breakable Stones")]
        static void CreateBreakableStonesMinigame(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Minigames/Target Area/Breakable Stone/Breakable Stone Minigame");
        }

        [MenuItem("GameObject/ExPresS XR/Minigames/Tile Game")]
        static void CreateTileMinigame(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Minigames/Tile Game/Tile Minigame");
        }
    }
}