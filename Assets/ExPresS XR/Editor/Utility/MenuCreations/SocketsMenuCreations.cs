using UnityEditor;
using ExPresSXR.Editor.Utility;

namespace ExPresSXR.Editor.UtilityMenuCreation
{
    public static class SocketsMenuCreations
    {
        [MenuItem("GameObject/ExPresS XR/Interaction/Socket Interactors/Highlightable")]
        public static void CreateHighlightableSocketInteractor(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Socket Interactors/Highlightable Socket Interactor");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Socket Interactors/Put Back")]
        public static void CreatePutBackSocketInteractor(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Socket Interactors/Put Back Socket Interactor");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Socket Interactors/Tag Check")]
        public static void CreateTagCheckSocketInteractor(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Socket Interactors/Tag Check Socket Interactor");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Socket Interactors/Object Check")]
        public static void CreateObjectCheckSocketInteractor(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Socket Interactors/Object Check Socket Interactor");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Socket Interactors/Tag Check Submit")]
        public static void CreateTagCheckSubmitSocketInteractor(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Socket Interactors/Tag Check Submit Socket Interactor");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Socket Interactors/Object Submit")]
        public static void CreateObjectCheckSubmitSocketInteractor(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Socket Interactors/Object Submit Socket Interactor");
        }
    }
}