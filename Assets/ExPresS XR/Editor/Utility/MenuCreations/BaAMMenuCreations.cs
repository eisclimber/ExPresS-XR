using UnityEditor;
using ExPresSXR.Editor.Utility;

namespace ExPresSXR.Editor.UtilityMenuCreation
{
    public static class BaAMMenuCreations
    {
        [MenuItem("GameObject/ExPresS XR/Minigames/Archery/Game Logic")]
        static void CreateArcheryGameLogic(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Minigames/Archery/Archery Games/Archery Game Logic");
        }

        [MenuItem("GameObject/ExPresS XR/Minigames/Archery/Object Pool Manager")]
        static void CreateArcheryObjectPoolManager(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Minigames/Archery/Archery Games/Object Pool Manager");
        }

        [MenuItem("GameObject/ExPresS XR/Minigames/Archery/Classic Archery Game")]
        static void CreateArcheryClassicMinigame(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Minigames/Archery/Archery Games/Classic Archery Game");
        }

        [MenuItem("GameObject/ExPresS XR/Minigames/Archery/Throw Archery Game")]
        static void CreateArcheryThrowerMinigame(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Minigames/Archery/Archery Games/Throw Archery Game");
        }

        [MenuItem("GameObject/ExPresS XR/Minigames/Archery/Line Archery Game")]
        static void CreateArcheryLineMinigame(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Minigames/Archery/Archery Games/Line Archery Game");
        }

    }
}