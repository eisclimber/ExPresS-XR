using UnityEditor;
using ExPresSXR.Editor.Utility;

namespace ExPresSXR.Editor.UtilityMenuCreation
{
    public static class InteractablesMenuCreations
    {
        [MenuItem("GameObject/ExPresS XR/Interaction/Interactables/Dynamic Attach")]
        public static void CreateXROffsetInteractable(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Interactables/Dynamic Attach Interactable");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Interactables/ExPresS")]
        public static void CreateExPresSGrabInteractable(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Interactables/ExPresS XR Grab Interactable");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Interactables/Grab Trigger")]
        public static void CreateXRGrabTriggerInteractable(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Interactables/Grab Trigger Interactable");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Interactables/Climb")]
        public static void CreateXRClimbInteractable(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Interactables/Climb Interactable");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Interactables/Exit Game")]
        public static void CreateXRExitGameInteractable(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Interactables/Exit Game Interactable");
        }
    }
}