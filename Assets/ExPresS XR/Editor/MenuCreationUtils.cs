using System;
using UnityEngine;
using UnityEditor;
using ExPresSXR.Experimentation.DataGathering;


namespace ExPresSXR.Editor
{
    public static class MenuCreationUtils
    {
        // ExPresS XR Rig
        [MenuItem("GameObject/ExPresS XR/XR Rig/Teleport")]
        static void CreateXRRig(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, CreationUtils.TELEPORT_EXPRESS_XR_RIG_PREFAB_NAME);
        }

        [MenuItem("GameObject/ExPresS XR/XR Rig/Continuous Move")]
        static void CreateXRRigContinuousMove(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, CreationUtils.CONTINUOUS_MOVE_EXPRESS_XR_RIG_PREFAB_NAME);
        }

        [MenuItem("GameObject/ExPresS XR/XR Rig/Head Gaze")]
        static void CreateXRRigHeadGaze(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, CreationUtils.HEAD_GAZE_EXPRESS_XR_RIG_PREFAB_NAME);
        }

        [MenuItem("GameObject/ExPresS XR/XR Rig/Head Gaze Teleport")]
        static void CreateXRRigHeadGazeTeleport(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, CreationUtils.HEAD_GAZE_TELEPORT_EXPRESS_XR_RIG_PREFAB_NAME);
        }

        [MenuItem("GameObject/ExPresS XR/XR Rig/Custom")]
        public static void CreateXRRigCustom(MenuCommand menuCommand)
        {
            GameObject go = InstantiatePrefabAtContextTransform(menuCommand, CreationUtils.CUSTOM_EXPRESS_XR_RIG_PREFAB_NAME);
            if (go == null)
            {
                Debug.LogWarning("No custom XR Rig found. Create a new one and mark it as custom in the Inspector.");
            }
        }

        // Interaction
        [MenuItem("GameObject/ExPresS XR/Interaction/Offset Interactable")]
        public static void CreateXROffsetInteractable(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Interaction/Offset Interactable");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Highlightable Socket Interactor")]
        public static void CreateHighlightableSocketInteractor(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Interaction/Highlightable Socket Interactor");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Put Back Socket Interactor")]
        public static void CreatePutBackSocketInteractor(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Interaction/Put Back Socket Interactor");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Tag Check Socket Interactor")]
        public static void CreateTagCheckSocketInteractor(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Interaction/Tag Check Socket Interactor");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Object Check Socket Interactor")]
        public static void CreateObjectCheckSocketInteractor(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Interaction/Object Check Socket Interactor");
        }

        // Buttons
        [MenuItem("GameObject/ExPresS XR/Buttons/Button Empty Text")]
        public static void CreateBaseButtonEmptyText(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Buttons/Base Button Empty Text");
        }

        [MenuItem("GameObject/ExPresS XR/Buttons/Base Button Empty")]
        public static void CreateBaseButtonEmpty(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Buttons/Base Button Empty");
        }

        [MenuItem("GameObject/ExPresS XR/Buttons/Base Button Round Square Text")]
        public static void CreateBaseButtonRoundSquareText(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Buttons/Base Button Round Square Text");
        }

        [MenuItem("GameObject/ExPresS XR/Buttons/Base Button Round Square")]
        public static void CreateBaseButtonRoundSquare(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Buttons/Base Button Round Square");
        }

        [MenuItem("GameObject/ExPresS XR/Buttons/Base Button Round Text")]
        public static void CreateBaseButtonRoundText(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Buttons/Base Button Round Text");
        }

        [MenuItem("GameObject/ExPresS XR/Buttons/Base Button Round")]
        public static void CreateBaseButtonRound(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Buttons/Base Button Round");
        }

        [MenuItem("GameObject/ExPresS XR/Buttons/Base Button Square Text")]
        public static void CreateBaseButtonSquareText(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Buttons/Base Button Square Text");
        }

        [MenuItem("GameObject/ExPresS XR/Buttons/Base Button Square")]
        public static void CreateBaseButtonSquare(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Buttons/Base Button Square");
        }

        // Quiz Buttons
        [MenuItem("GameObject/ExPresS XR/Buttons/Quiz Buttons/Quiz Button Empty")]
        public static void CreateBaseQuizButtonEmpty(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Buttons/Quiz Buttons/Quiz Button Empty");
        }

        [MenuItem("GameObject/ExPresS XR/Buttons/Quiz Buttons/Quiz Button Round Square")]
        public static void CreateQuizButtonRoundSquare(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Buttons/Quiz Buttons/Quiz Button Round Square");
        }

        [MenuItem("GameObject/ExPresS XR/Buttons/Quiz Buttons/Quiz Button Round")]
        public static void CreateQuizButtonRound(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Buttons/Quiz Buttons/Quiz Button Round");
        }

        [MenuItem("GameObject/ExPresS XR/Buttons/Quiz Buttons/Quiz Button Square")]
        public static void CreateQuizButtonSquare(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, CreationUtils.QUIZ_BUTTON_SQUARE_PREFAB_NAME);
        }

        [MenuItem("GameObject/ExPresS XR/Buttons/Quiz Buttons/Multiple Choice Confirm Button Square")]
        public static void CreateMcConfirmButton(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Buttons/Quiz Buttons/Multiple Choice Confirm Button Square");
        }

        // Quiz Buttons
        [MenuItem("GameObject/ExPresS XR/Button Quiz/Differing Types Single Choice")]
        public static void CreateDifferingTypesSingleChoiceButtonQuiz(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Button Quiz/Differing Types Single Choice Quiz");
        }

        [MenuItem("GameObject/ExPresS XR/Button Quiz/Fruit Video")]
        public static void CreateFruitVideoButtonQuiz(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Button Quiz/Fruit Video Quiz");
        }

        [MenuItem("GameObject/ExPresS XR/Button Quiz/Random Feedback")]
        public static void CreateRandomFeedbackButtonQuiz(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Button Quiz/Random Feedback Quiz");
        }

        [MenuItem("GameObject/ExPresS XR/Button Quiz/Shadow Objects")]
        public static void CreateShadowObjectsButtonQuiz(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Button Quiz/Shadow Objects Quiz");
        }

        [MenuItem("GameObject/ExPresS XR/Button Quiz/Single Choice Text")]
        public static void CreateSingleChoiceTextButtonQuiz(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Button Quiz/Single Choice Text Quiz");
        }

        [MenuItem("GameObject/ExPresS XR/Button Quiz/Sockets Single Choice")]
        public static void CreateSocketsSingleChoiceButtonQuiz(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Button Quiz/Sockets Single Choice Quiz");
        }

        [MenuItem("GameObject/ExPresS XR/Button Quiz/Test Multiple Choice Text")]
        public static void CreateTestMultipleChoiceTextButtonQuiz(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Button Quiz/Test Multiple Choice Text Quiz");
        }

        [MenuItem("GameObject/ExPresS XR/Button Quiz/Uni Trivia")]
        public static void CreateUniTriviaQuizButtonQuiz(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Button Quiz/Uni Trivia Quiz");
        }

        [MenuItem("GameObject/ExPresS XR/Button Quiz/Wrong Feedback")]
        public static void CreateWrongFeedbackButtonQuiz(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Button Quiz/Wrong Feedback Quiz");
        }

        [MenuItem("GameObject/ExPresS XR/Button Quiz/Wrong Feedback Test Multiple Choice Text")]
        public static void CreateWrongFeedbackTestMultipleChoiceTextButtonQuiz(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Button Quiz/Wrong Feedback Test Multiple Choice Text Quiz");
        }

        // Hud
        [MenuItem("GameObject/ExPresS XR/UI/HUD/HUD")]
        static void CreateHud(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "UI/Hud");
        }

        [MenuItem("GameObject/ExPresS XR/UI/HUD/Fade Rect")]
        static void CreateFadeRect(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "UI/Fade Rect");
        }

        [MenuItem("GameObject/ExPresS XR/UI/HUD/Head Gaze Reticle")]
        static void CreateHeadGazeReticle(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Reticles/Head Gaze Reticle");
        }

        // UI
        [MenuItem("GameObject/ExPresS XR/UI/World Space Canvas")]
        static void CreateWorldSpaceImage(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "UI/World Space Canvas");
        }

        [MenuItem("GameObject/ExPresS XR/UI/World Space Canvas (Always On Top)")]
        static void CreateWorldSpaceCanvasAOT(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "UI/World Space Canvas (Always On Top)");
        }

        // Keyboards
        [MenuItem("GameObject/ExPresS XR/UI/Keyboards/German")]
        static void CreateKeyboardGerman(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "UI/World Space Keyboard German");
        }


        [MenuItem("GameObject/ExPresS XR/UI/Keyboards/English")]
        static void CreateKeyboardEnglish(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "UI/World Space Keyboard English");
        }

        [MenuItem("GameObject/ExPresS XR/UI/Keyboards/Numpad")]
        static void CreateKeyboardNumpad(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "UI/World Space Keyboard Numpad");
        }


        // Misc Menus
        [MenuItem("GameObject/ExPresS XR/Misc/After Quiz Dialog")]
        static void CreateCakeAfterQuizMenu(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Misc/After Quiz Dialog");
        }


        [MenuItem("GameObject/ExPresS XR/Misc/Cake Demo UI")]
        static void CreateCakeDemoUi(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Misc/Cake Demo UI");
        }


        // Exhibition
        [MenuItem("GameObject/ExPresS XR/Exhibition/Exhibition Display - Object")]
        static void CreateExhibitionDisplayObject(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Exhibition/Exhibition Display - Object");
        }


        [MenuItem("GameObject/ExPresS XR/Exhibition/Exhibition Display - Image")]
        static void CreateExhibitionDisplayImage(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Exhibition/Exhibition Display - Image");
        }


        [MenuItem("GameObject/ExPresS XR/Exhibition/Exhibition Display - Empty")]
        static void CreateExhibitionDisplayEmpty(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Exhibition/Exhibition Display - Empty");
        }


        // Teleportation
        [MenuItem("GameObject/ExPresS XR/Interaction/Teleportation Area")]
        static void CreateTeleportationArea(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Interaction/Teleportation Area");
        }


        [MenuItem("GameObject/ExPresS XR/Interaction/Teleportation Anchor")]
        static void CreateTeleportationAnchor(MenuCommand menuCommand)
        {
            InstantiatePrefabAtContextTransform(menuCommand, "Interaction/Teleportation Anchor");
        }


        // Data
        [MenuItem("GameObject/ExPresS XR/Data Gatherer")]
        public static void CreateDataGatherer(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Data Gatherer");
            go.AddComponent<DataGatherer>();
            GameObjectUtility.EnsureUniqueNameForSibling(go);
            Undo.RegisterCreatedObjectUndo(go, "Create Data Gatherer Game Object");
        }


        private static GameObject InstantiatePrefabAtContextTransform(MenuCommand menuCommand, string prefabName)
        {
            Transform parent = CreationUtils.GetContextTransform(menuCommand);
            return CreationUtils.InstantiateAndPlacePrefab(prefabName, parent);
        }
    }
}