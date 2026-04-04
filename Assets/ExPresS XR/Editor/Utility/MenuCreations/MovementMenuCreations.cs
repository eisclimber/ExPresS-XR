using UnityEditor;
using ExPresSXR.Editor.Utility;

namespace ExPresSXR.Editor.UtilityMenuCreation
{
    public static class MovementMenuCreations
    {
        #region Teleportation
        [MenuItem("GameObject/ExPresS XR/Movement/Teleportation Area")]
        static void CreateTeleportationArea(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Movement/Teleportation Area");
        }

        [MenuItem("GameObject/ExPresS XR/Movement/Teleportation Anchor")]
        static void CreateTeleportationAnchor(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Movement/Teleportation Anchor");
        }
        #endregion

        #region Map Point
        [MenuItem("GameObject/ExPresS XR/Movement/Map Point Teleport/Basic Setup")]
        static void CreateBasicMapPointSetup(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Movement/Basic Map Point Setup");
        }

        [MenuItem("GameObject/ExPresS XR/Movement/Map Point Teleport/Map Point")]
        static void CreateMapPoint(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Movement/Map Point");
        }

        [MenuItem("GameObject/ExPresS XR/Movement/Map Point Teleport/Teleport Option")]
        static void CreateMapMapPointTeleportOption(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Movement/Teleport Option");
        }

        [MenuItem("GameObject/ExPresS XR/Movement/Map Point Teleport/Manager")]
        static void CreateMapPointManager(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Movement/Map Point Manager");
        }
        #endregion
    }
}