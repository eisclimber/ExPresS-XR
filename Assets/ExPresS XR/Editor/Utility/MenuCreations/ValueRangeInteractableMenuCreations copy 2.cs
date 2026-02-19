using UnityEditor;
using ExPresSXR.Editor.Utility;

namespace ExPresSXR.Editor.UtilityMenuCreation
{
    public static class ValueRangeInteractableMenuCreations
    {
        [MenuItem("GameObject/ExPresS XR/Interaction/Value Range Interactables/Lever")]
        public static void CreateLeverInteractable(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Value Range Interactables/Lever");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Value Range Interactables/Slider")]
        public static void CreateSliderInteractable(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Value Range Interactables/Slider");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Value Range Interactables/Pullback Slider")]
        public static void CreatePullbackSliderInteractable(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Value Range Interactables/Pullback Slider");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Value Range Interactables/Joystick")]
        public static void CreatePullbackJoystickInteractable(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Value Range Interactables/Joystick");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Value Range Interactables/Turn Knob")]
        public static void CreatePullbackTurnKnobInteractable(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Value Range Interactables/Turn Knob");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Value Range Interactables/Screw")]
        public static void CreateScrewInteractable(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Value Range Interactables/Screw");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Value Range Interactables/Nut Screw")]
        public static void CreateNutScrewInteractable(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Value Range Interactables/Nut Screw");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Value Range Interactables/Fly Nut Screw")]
        public static void CreateFlyNutScrewInteractable(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Value Range Interactables/Fly Nut Screw");
        }


        [MenuItem("GameObject/ExPresS XR/Interaction/Value Range Interactables/Slider 2D Square Individual")]
        public static void CreateSlider2DSquareIndividualInteractable(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Value Range Interactables/Slider 2D Square Individual");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Value Range Interactables/Slider 2D Square")]
        public static void CreateSlider2DSquareInteractable(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Value Range Interactables/Slider 2D Square");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Value Range Interactables/Slider 2D Round")]
        public static void CreateSlider2DRoundInteractable(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Value Range Interactables/Slider 2D Round");
        }


        [MenuItem("GameObject/ExPresS XR/Interaction/Value Range Interactables/Slider 3D Cubic")]
        public static void CreateSlider3DSquareCubicInteractable(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Value Range Interactables/Slider 3D Cubic");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Value Range Interactables/Slider 3D Spherical")]
        public static void CreateSlider3DSphericalInteractable(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Value Range Interactables/Slider 3D Spherical");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Value Range Interactables/Slider 3D Direction")]
        public static void CreateSlider3DDirectionInteractable(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Value Range Interactables/Slider 3D Direction");
        }
    }
}