using UnityEditor;
using ExPresSXR.Editor.Utility;
using ExPresSXR.Presentation.Pictures;

namespace ExPresSXR.Editor.UtilityMenuCreation
{
    public static class UizMenuCreations
    {
        [MenuItem("GameObject/ExPresS XR/Presentation/Exhibition Displays/Object")]
        static void CreateExhibitionDisplayObject(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Presentation/Exhibition Displays/Exhibition Display - Object");
        }

        [MenuItem("GameObject/ExPresS XR/Presentation/Exhibition Displays/Object Small")]
        static void CreateExhibitionDisplayObjectSmall(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Presentation/Exhibition Displays/Exhibition Display - Object Small");
        }

        [MenuItem("GameObject/ExPresS XR/Presentation/Exhibition Displays/Image")]
        static void CreateExhibitionDisplayImage(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Presentation/Exhibition Displays/Exhibition Display - Image");
        }

        [MenuItem("GameObject/ExPresS XR/Presentation/Exhibition Displays/Info Stand")]
        static void CreateExhibitionDisplayInfoStand(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Presentation/Exhibition Displays/Exhibition Display - Info Stand");
        }

        [MenuItem("GameObject/ExPresS XR/Presentation/Exhibition Displays/Empty")]
        static void CreateExhibitionDisplayEmpty(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Presentation/Exhibition Displays/Exhibition Display - Empty");
        }

        // Picture Presentation
        [MenuItem("GameObject/ExPresS XR/Presentation/Pictures/Picture Scroll Viewer")]
        static void CreatePictureScrollViewer(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Presentation/Pictures/Picture Scroll Viewer");
        }

        [MenuItem("GameObject/ExPresS XR/Presentation/Pictures/Picture Scroll Viewer Table")]
        static void CreatePictureScrollViewerTable(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Presentation/Pictures/Picture Scroll Viewer Table");
        }

        [MenuItem("GameObject/ExPresS XR/Presentation/Pictures/Polaroids Table")]
        static void CreatePolaroidsTable(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Presentation/Pictures/Polaroids Table");
        }

        [MenuItem("GameObject/ExPresS XR/Presentation/Pictures/Picture Wall")]
        static void CreatePictureWall(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Presentation/Pictures/Picture Wall");
        }

        // Mirror
        [MenuItem("GameObject/ExPresS XR/Presentation/Mirror")]
        static void CreateMirror(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Mirror/Mirror");
        }

        // Epi Sphere
        [MenuItem("GameObject/ExPresS XR/Presentation/Epi Sphere/Epi Sphere")]
        static void CreateEpiSphere(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Epi Sphere/Epi Sphere");
        }

        [MenuItem("GameObject/ExPresS XR/Presentation/Epi Sphere/Epi Dome")]
        static void CreateEpiDome(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Epi Sphere/Epi Dome");
        }

        [MenuItem("GameObject/ExPresS XR/Presentation/Epi Sphere/Epi Sphere Video")]
        static void CreateEpiSphereVideo(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Epi Sphere/Epi Sphere Video");
        }

        // Picture Data
        [MenuItem("Assets/Create/ExPresS XR/Picture Data")]
        public static void CreatePictureData() => CreationUtils.CreateScriptableObject<PictureData>("Picture Data");
    }
}