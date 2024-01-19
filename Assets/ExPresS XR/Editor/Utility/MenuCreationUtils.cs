using System.IO;
using UnityEngine;
using UnityEditor;
using ExPresSXR.Experimentation.DataGathering;


namespace ExPresSXR.Editor.Utility
{
    public static class MenuCreationUtils
    {
        // ExPresS XR Rig
        [MenuItem("GameObject/ExPresS XR/XR Rig/Teleport")]
        static void CreateXRRig(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, CreationUtils.TELEPORT_RIG_PREFAB_NAME);
        }

        [MenuItem("GameObject/ExPresS XR/XR Rig/Joystick")]
        static void CreateXRRigContinuousMove(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, CreationUtils.JOYSTICK_RIG_PREFAB_NAME);
        }

        [MenuItem("GameObject/ExPresS XR/XR Rig/Grab Motion")]
        static void CreateXRRigGrabMove(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, CreationUtils.GRAB_MOTION_RIG_PREFAB_NAME);
        }

        [MenuItem("GameObject/ExPresS XR/XR Rig/Grab Manipulation")]
        static void CreateXRRigGrabManipulation(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, CreationUtils.GRAB_MANIPULATION_RIG_PREFAB_NAME);
        }

        [MenuItem("GameObject/ExPresS XR/XR Rig/Head Gaze")]
        static void CreateXRRigHeadGaze(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, CreationUtils.HEAD_GAZE_RIG_PREFAB_NAME);
        }

        [MenuItem("GameObject/ExPresS XR/XR Rig/Eye Gaze")]
        static void CreateXRRigEyeGaze(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, CreationUtils.EYE_GAZE_RIG_PREFAB_NAME);
        }


        [MenuItem("GameObject/ExPresS XR/XR Rig/None")]
        static void CreateXRRigNone(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, CreationUtils.NONE_RIG_PREFAB_NAME);
        }

        [MenuItem("GameObject/ExPresS XR/XR Rig/Custom")]
        static void CreateXRRigCustom(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, CreationUtils.CUSTOM_RIG_PREFAB_NAME);
        }


        [MenuItem("GameObject/ExPresS XR/XR Rig/Custom (Saved)")]
        public static void CreateXRRigSaved(MenuCommand menuCommand)
        {
            GameObject go = null;
            if (File.Exists(CreationUtils.savedXRRigPath))
            {
                go = InstantiateGameObjectAtContextTransform(menuCommand, CreationUtils.SAVED_RIG_PREFAB_NAME);
            }

            if (go == null)
            {
                Debug.LogError("No custom XR Rig found. Create a new one and save it from the rig's inspector.");
            }
        }


        // Inverse Kinematics
        [MenuItem("GameObject/ExPresS XR/Inverse Kinematics/Sample - Empty")]
        static void CreateIKSampleEmpty(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "IK/IK Sample - Empty");
        }


        [MenuItem("GameObject/ExPresS XR/Inverse Kinematics/Sample - Character")]
        static void CreateIKSampleCharacter(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "IK/IK Sample - Character");
        }


        // Interaction (Interactables)
        [MenuItem("GameObject/ExPresS XR/Interaction/Interactables/Dynamic Attach")]
        public static void CreateXROffsetInteractable(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Interactables/Dynamic Attach Interactable");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Interactables/Scalable")]
        public static void CreateScalableInteractable(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Interactables/Dynamic Attach Interactable");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Interactables/Grab Trigger")]
        public static void CreateXRGrabTriggerInteractable(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Interactables/Grab Trigger Interactable");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Interactables/Climb")]
        public static void CreateXRClimbInteractable(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Interactables/Climb Interactable");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Interactables/Exit Game")]
        public static void CreateXRExitGameInteractable(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Interactables/Exit Game Interactable");
        }


        // Interaction (Sockets)
        [MenuItem("GameObject/ExPresS XR/Interaction/Socket Interactors/Highlightable")]
        public static void CreateHighlightableSocketInteractor(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Socket Interactors/Highlightable Socket Interactor");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Socket Interactors/Put Back")]
        public static void CreatePutBackSocketInteractor(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Socket Interactors/Put Back Socket Interactor");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Socket Interactors/Tag Check")]
        public static void CreateTagCheckSocketInteractor(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Socket Interactors/Tag Check Socket Interactor");
        }

        [MenuItem("GameObject/ExPresS XR/Interaction/Socket Interactors/Object Check")]
        public static void CreateObjectCheckSocketInteractor(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Interaction/Socket Interactors/Object Check Socket Interactor");
        }


        // Buttons
        [MenuItem("GameObject/ExPresS XR/Buttons/Button Empty Text")]
        public static void CreateBaseButtonEmptyText(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Buttons/Base Button Empty Text");
        }

        [MenuItem("GameObject/ExPresS XR/Buttons/Base Button Empty")]
        public static void CreateBaseButtonEmpty(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Buttons/Base Button Empty");
        }

        [MenuItem("GameObject/ExPresS XR/Buttons/Base Button Round Square Text")]
        public static void CreateBaseButtonRoundSquareText(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Buttons/Base Button Round Square Text");
        }

        [MenuItem("GameObject/ExPresS XR/Buttons/Base Button Round Square")]
        public static void CreateBaseButtonRoundSquare(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Buttons/Base Button Round Square");
        }

        [MenuItem("GameObject/ExPresS XR/Buttons/Base Button Round Text")]
        public static void CreateBaseButtonRoundText(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Buttons/Base Button Round Text");
        }

        [MenuItem("GameObject/ExPresS XR/Buttons/Base Button Round")]
        public static void CreateBaseButtonRound(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Buttons/Base Button Round");
        }

        [MenuItem("GameObject/ExPresS XR/Buttons/Base Button Square Text")]
        public static void CreateBaseButtonSquareText(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Buttons/Base Button Square Text");
        }

        [MenuItem("GameObject/ExPresS XR/Buttons/Base Button Square")]
        public static void CreateBaseButtonSquare(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Buttons/Base Button Square");
        }

        // Quiz Buttons
        [MenuItem("GameObject/ExPresS XR/Buttons/Quiz Buttons/Quiz Button Empty")]
        public static void CreateBaseQuizButtonEmpty(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Buttons/Quiz Buttons/Quiz Button Empty");
        }

        [MenuItem("GameObject/ExPresS XR/Buttons/Quiz Buttons/Quiz Button Round Square")]
        public static void CreateQuizButtonRoundSquare(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Buttons/Quiz Buttons/Quiz Button Round Square");
        }

        [MenuItem("GameObject/ExPresS XR/Buttons/Quiz Buttons/Quiz Button Round")]
        public static void CreateQuizButtonRound(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Buttons/Quiz Buttons/Quiz Button Round");
        }

        [MenuItem("GameObject/ExPresS XR/Buttons/Quiz Buttons/Quiz Button Square")]
        public static void CreateQuizButtonSquare(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, CreationUtils.QUIZ_BUTTON_SQUARE_PREFAB_NAME);
        }

        [MenuItem("GameObject/ExPresS XR/Buttons/Quiz Buttons/Multiple Choice Confirm Button Square")]
        public static void CreateMcConfirmButton(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Buttons/Quiz Buttons/Multiple Choice Confirm Button Square");
        }

        // Quiz Buttons
        [MenuItem("GameObject/ExPresS XR/Button Quiz/Differing Types Single Choice")]
        public static void CreateDifferingTypesSingleChoiceButtonQuiz(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Button Quiz/Differing Types Single Choice Quiz");
        }

        [MenuItem("GameObject/ExPresS XR/Button Quiz/Fruit Video")]
        public static void CreateFruitVideoButtonQuiz(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Button Quiz/Fruit Video Quiz");
        }

        [MenuItem("GameObject/ExPresS XR/Button Quiz/Random Feedback")]
        public static void CreateRandomFeedbackButtonQuiz(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Button Quiz/Random Feedback Quiz");
        }

        [MenuItem("GameObject/ExPresS XR/Button Quiz/Shadow Objects")]
        public static void CreateShadowObjectsButtonQuiz(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Button Quiz/Shadow Objects Quiz");
        }

        [MenuItem("GameObject/ExPresS XR/Button Quiz/Single Choice Text")]
        public static void CreateSingleChoiceTextButtonQuiz(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Button Quiz/Single Choice Text Quiz");
        }

        [MenuItem("GameObject/ExPresS XR/Button Quiz/Sockets Single Choice")]
        public static void CreateSocketsSingleChoiceButtonQuiz(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Button Quiz/Sockets Single Choice Quiz");
        }

        [MenuItem("GameObject/ExPresS XR/Button Quiz/Test Multiple Choice Text")]
        public static void CreateTestMultipleChoiceTextButtonQuiz(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Button Quiz/Test Multiple Choice Text Quiz");
        }

        [MenuItem("GameObject/ExPresS XR/Button Quiz/Uni Trivia")]
        public static void CreateUniTriviaQuizButtonQuiz(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Button Quiz/Uni Trivia Quiz");
        }

        [MenuItem("GameObject/ExPresS XR/Button Quiz/Wrong Feedback")]
        public static void CreateWrongFeedbackButtonQuiz(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Button Quiz/Wrong Feedback Quiz");
        }

        [MenuItem("GameObject/ExPresS XR/Button Quiz/Wrong Feedback Test Multiple Choice Text")]
        public static void CreateWrongFeedbackTestMultipleChoiceTextButtonQuiz(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Button Quiz/Wrong Feedback Test Multiple Choice Text Quiz");
        }

        // Hud
        [MenuItem("GameObject/ExPresS XR/UI/HUD/HUD")]
        static void CreateHud(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "UI/Hud");
        }

        [MenuItem("GameObject/ExPresS XR/UI/HUD/Fade Rect")]
        static void CreateFadeRect(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "UI/Fade Rect");
        }

        [MenuItem("GameObject/ExPresS XR/UI/HUD/Head Gaze Reticle")]
        static void CreateHeadGazeReticle(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Reticles/Head Gaze Reticle");
        }

        // UI
        [MenuItem("GameObject/ExPresS XR/UI/World Space Canvas")]
        static void CreateWorldSpaceImage(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "UI/World Space Canvas");
        }

        [MenuItem("GameObject/ExPresS XR/UI/World Space Canvas (Not Interactable)")]
        static void CreateWorldSpaceImageNI(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "UI/World Space Canvas (Not Interactable)");
        }

        [MenuItem("GameObject/ExPresS XR/UI/World Space Canvas (Always On Top)")]
        static void CreateWorldSpaceCanvasAOT(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "UI/World Space Canvas (Always On Top)");
        }

        // Keyboards
        [MenuItem("GameObject/ExPresS XR/UI/Keyboards/German")]
        static void CreateKeyboardGerman(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "UI/Keyboards/World Space Keyboard German");
        }


        [MenuItem("GameObject/ExPresS XR/UI/Keyboards/English")]
        static void CreateKeyboardEnglish(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "UI/Keyboards/World Space Keyboard English");
        }

        [MenuItem("GameObject/ExPresS XR/UI/Keyboards/Numpad")]
        static void CreateKeyboardNumpad(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "UI/Keyboards/World Space Keyboard Numpad");
        }


        // Misc Menus
        [MenuItem("GameObject/ExPresS XR/UI/Main Menu UI")]
        static void CreateMainMenuUI(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "UI/Main Menu UI");
        }

        [MenuItem("GameObject/ExPresS XR/UI/Misc/After Quiz Dialog")]
        static void CreateCakeAfterQuizMenu(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Misc/After Quiz Dialog");
        }


        [MenuItem("GameObject/ExPresS XR/UI/Misc/Cake Demo UI")]
        static void CreateCakeDemoUi(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Misc/Cake Demo UI");
        }


        [MenuItem("GameObject/ExPresS XR/UI/Misc/Change Movement Menu")]
        static void CreateChangeMovementMenu(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Misc/Change Movement Menu");
        }


        // Exhibition Displays
        [MenuItem("GameObject/ExPresS XR/Presentation/Exhibition Displays/Object")]
        static void CreateExhibitionDisplayObject(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Exhibition Displays/Exhibition Display - Object");
        }

        [MenuItem("GameObject/ExPresS XR/Presentation/Exhibition Displays/Object Small")]
        static void CreateExhibitionDisplayObjectSmall(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Exhibition Displays/Exhibition Display - Object Small");
        }

        [MenuItem("GameObject/ExPresS XR/Presentation/Exhibition Displays/Image")]
        static void CreateExhibitionDisplayImage(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Exhibition Displays/Exhibition Display - Image");
        }

        [MenuItem("GameObject/ExPresS XR/Presentation/Exhibition Displays/Info Stand")]
        static void CreateExhibitionDisplayInfoStand(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Exhibition Displays/Exhibition Display - Info Stand");
        }


        [MenuItem("GameObject/ExPresS XR/Presentation/Exhibition Displays/Empty")]
        static void CreateExhibitionDisplayEmpty(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Exhibition Displays/Exhibition Display - Empty");
        }

        // Mirror
        [MenuItem("GameObject/ExPresS XR/Presentation/Mirror")]
        static void CreateMirror(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Mirror/Mirror");
        }

        // Misc Interaction
        [MenuItem("GameObject/ExPresS XR/Presentation/Highlighter Area")]
        static void CreateHighlighterArea(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Misc/Highlighter Area");
        }


        // Eye Tracking
        [MenuItem("GameObject/ExPresS XR/Eye Tracking/Area Of Interest")]
        static void CreateAOI(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Eye Tracking/Area Of Interest");
        }

        [MenuItem("GameObject/ExPresS XR/Eye Tracking/Area Of Interest Ray")]
        static void CreateAOIRay(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Eye Tracking/Area Of Interest Ray");
        }


        // Teleportation
        [MenuItem("GameObject/ExPresS XR/Movement/Teleportation Area")]
        static void CreateTeleportationArea(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Movement/Teleportation Area");
        }

        [MenuItem("GameObject/ExPresS XR/Movement/Teleportation Anchor")]
        static void CreateTeleportationAnchor(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Movement/Teleportation Anchor");
        }


        // Map Point
        [MenuItem("GameObject/ExPresS XR/Movement/Map Point Teleport/Basic Setup")]
        static void CreateBasicMapPointSetup(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Movement/Map Point Basic Setup");
        }

        [MenuItem("GameObject/ExPresS XR/Movement/Map Point Teleport/Map Point")]
        static void CreateMapPoint(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Movement/Map Point");
        }

        [MenuItem("GameObject/ExPresS XR/Movement/Map Point Teleport/Teleport Option")]
        static void CreateMapMapPointTeleportOption(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Movement/Map Point Teleport Option");
        }

        [MenuItem("GameObject/ExPresS XR/Movement/Map Point Teleport/Manager")]
        static void CreateMapPointManager(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Movement/Map Point Manager");
        }


        // Minigames
        [MenuItem("GameObject/ExPresS XR/Minigames/Sword Cleaning")]
        static void CreateSwordCleaningMinigame(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Minigames/Sword Cleaning/Sword Cleaning Minigame");
        }

        [MenuItem("GameObject/ExPresS XR/Minigames/Coin Throw")]
        static void CreateCoinThrowMinigame(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Minigames/Coin Throw/Coin Throw Minigame");
        }

        [MenuItem("GameObject/ExPresS XR/Minigames/Coin Scale")]
        static void CreateCoinScaleMinigame(MenuCommand menuCommand)
        {
            InstantiateGameObjectAtContextTransform(menuCommand, "Minigames/Coin Scale/Coin Scale Minigame");
        }

        // Data
        [MenuItem("GameObject/ExPresS XR/Data Gatherer")]
        public static void CreateDataGatherer(MenuCommand menuCommand)
        {
            GameObject go = new("Data Gatherer");
            go.AddComponent<DataGatherer>();
            GameObjectUtility.EnsureUniqueNameForSibling(go);
            Undo.RegisterCreatedObjectUndo(go, "Create Data Gatherer Game Object");
        }


        private static GameObject InstantiateGameObjectAtContextTransform(MenuCommand menuCommand, string prefabName)
        {
            Transform parent = CreationUtils.GetContextTransform(menuCommand);
            return CreationUtils.InstantiateAndPlaceGameObject(prefabName, parent);
        }
    }
}