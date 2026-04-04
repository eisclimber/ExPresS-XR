using UnityEditor;
using ExPresSXR.Editor.Utility;

namespace ExPresSXR.Editor.UtilityMenuCreation
{
    public static class UiMenuCreations
    {
        #region Hud
        [MenuItem("GameObject/ExPresS XR/UI/HUD/HUD")]
        static void CreateHud(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "UI/Hud");
        }

        [MenuItem("GameObject/ExPresS XR/UI/HUD/Fade Rect")]
        static void CreateFadeRect(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "UI/Fade Rect");
        }

        [MenuItem("GameObject/ExPresS XR/UI/HUD/Head Gaze Reticle")]
        static void CreateHeadGazeReticle(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Reticles/Head Gaze Reticle");
        }
        #endregion

        #region UI
        [MenuItem("GameObject/ExPresS XR/UI/World Space Canvas")]
        static void CreateWorldSpaceImage(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "UI/World Space Canvas");
        }

        [MenuItem("GameObject/ExPresS XR/UI/World Space Canvas (Not Interactable)")]
        static void CreateWorldSpaceImageNI(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "UI/World Space Canvas (Not Interactable)");
        }

        [MenuItem("GameObject/ExPresS XR/UI/World Space Canvas (Always On Top)")]
        static void CreateWorldSpaceCanvasAOT(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "UI/World Space Canvas (Always On Top)");
        }
        #endregion

        #region Keyboards
        [MenuItem("GameObject/ExPresS XR/UI/Keyboards/German")]
        static void CreateKeyboardGerman(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "UI/Keyboards/World Space Keyboard German");
        }


        [MenuItem("GameObject/ExPresS XR/UI/Keyboards/English")]
        static void CreateKeyboardEnglish(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "UI/Keyboards/World Space Keyboard English");
        }

        [MenuItem("GameObject/ExPresS XR/UI/Keyboards/Numpad")]
        static void CreateKeyboardNumpad(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "UI/Keyboards/World Space Keyboard Numpad");
        }
        #endregion


        #region  Misc Menus
        [MenuItem("GameObject/ExPresS XR/UI/Misc/Main Menu UI")]
        static void CreateMainMenuUI(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "UI/Main Menu UI");
        }

        [MenuItem("GameObject/ExPresS XR/UI/Circular Timer")]
        static void CreateCircularTimerUI(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "UI/Circular Timer UI");
        }

        [MenuItem("GameObject/ExPresS XR/UI/Misc/After Quiz Dialog")]
        static void CreateCakeAfterQuizMenu(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Misc/After Quiz Dialog");
        }


        [MenuItem("GameObject/ExPresS XR/UI/Misc/Cake Demo UI")]
        static void CreateCakeDemoUi(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Misc/Cake Demo UI");
        }

        [MenuItem("GameObject/ExPresS XR/UI/Misc/Console To UI")]
        static void CreateConsoleToUi(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "UI/Console To Ui");
        }


        [MenuItem("GameObject/ExPresS XR/UI/Misc/Change Movement Menu")]
        static void CreateChangeMovementMenu(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, "Misc/Change Movement Menu");
        }
        #endregion
    }
}