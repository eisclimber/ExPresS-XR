using UnityEditor;
using ExPresSXR.Editor.Utility;

namespace ExPresSXR.Editor.UtilityMenuCreation
{
    public static class ButtonsMenuCreations
    {
        #region Buttons
        [MenuItem("GameObject/ExPresS XR/Interaction/Buttons/Button Empty Text")]
        public static void CreateButtonEmptyText(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Buttons/Button Empty Text");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Buttons/Empty")]
        public static void CreateButtonEmpty(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Buttons/Button Empty");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Buttons/Round Square Text")]
        public static void CreateButtonRoundSquareText(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Buttons/Button Round Square Text");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Buttons/Round Square")]
        public static void CreateButtonRoundSquare(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Buttons/Button Round Square");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Buttons/Round Text")]
        public static void CreateButtonRoundText(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Buttons/Button Round Text");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Buttons/Round")]
        public static void CreateButtonRound(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Buttons/Button Round");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Buttons/Square Text")]
        public static void CreateButtonSquareText(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Buttons/Button Square Text");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Buttons/Square")]
        public static void CreateButtonSquare(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Buttons/Button Square");
        }
        #endregion

        #region Legacy Buttons
        [MenuItem("GameObject/ExPresS XR/Interaction/Buttons (Legacy)/Button Empty Text")]
        public static void CreateLEgacyButtonEmptyText(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Legacy Buttons/Legacy Button Empty Text");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Buttons (Legacy)/Empty")]
        public static void CreateLEgacyButtonEmpty(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Legacy Buttons/Legacy Button Empty");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Buttons (Legacy)/Round Square Text")]
        public static void CreateLEgacyButtonRoundSquareText(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Legacy Buttons/Legacy Button Round Square Text");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Buttons (Legacy)/Round Square")]
        public static void CreateLEgacyButtonRoundSquare(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Legacy Buttons/Legacy Button Round Square");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Buttons (Legacy)/Round Text")]
        public static void CreateLEgacyButtonRoundText(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Legacy Buttons/Legacy Button Round Text");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Buttons (Legacy)/Round")]
        public static void CreateLEgacyButtonRound(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Legacy Buttons/Legacy Button Round");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Buttons (Legacy)/Square Text")]
        public static void CreateLEgacyButtonSquareText(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Legacy Buttons/Legacy Button Square Text");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Buttons (Legacy)/Square")]
        public static void CreateLEgacyButtonSquare(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Legacy Buttons/Legacy Button Square");
        }
        #endregion

        #region Quiz Buttons
        [MenuItem("GameObject/ExPresS XR/Interaction/Buttons/Quiz Buttons/Empty")]
        public static void CreateBaseQuizButtonEmpty(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Buttons/Quiz Buttons/Quiz Button Empty");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Buttons/Quiz Buttons/Round Square")]
        public static void CreateQuizButtonRoundSquare(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Buttons/Quiz Buttons/Quiz Button Round Square");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Buttons/Quiz Buttons/Round")]
        public static void CreateQuizButtonRound(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Buttons/Quiz Buttons/Quiz Button Round");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Buttons/Quiz Buttons/Square")]
        public static void CreateQuizButtonSquare(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, CreationUtils.QUIZ_BUTTON_SQUARE_PREFAB_NAME);
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Buttons/Quiz Buttons/Multiple Choice Confirm")]
        public static void CreateMcConfirmButton(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Buttons/Quiz Buttons/Multiple Choice Confirm Button Square");
        }
        #endregion
    }
}