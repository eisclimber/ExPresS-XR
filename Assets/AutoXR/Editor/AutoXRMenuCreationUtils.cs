using System;
using UnityEngine;
using UnityEditor;

public static class AutoXRMenuCreationUtils
{
    // Auto XR Rig
    [MenuItem("GameObject/AutoXR/Auto XR Rig/Teleport")]
    static void CreateAutoXRRig(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, AutoXRCreationUtils.TELEPORT_AUTOXR_PREFAB_NAME);
    }

    [MenuItem("GameObject/AutoXR/Auto XR Rig/Continuous Move")]
    static void CreateAutoXRRigContinuousMove(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, AutoXRCreationUtils.CONTINUOUS_MOVE_AUTOXR_PREFAB_NAME);
    }

    [MenuItem("GameObject/AutoXR/Auto XR Rig/Head Gaze")]
    static void CreateAutoXRRigHeadGaze(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, AutoXRCreationUtils.HEAD_GAZE_AUTOXR_PREFAB_NAME);
    }

    [MenuItem("GameObject/AutoXR/Auto XR Rig/Custom")]
    public static void CreateAutoXRRigCustom(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, AutoXRCreationUtils.CUSTOM_AUTOXR_PREFAB_NAME);
        if (go == null)
        {
            Debug.LogWarning("No custom AutoXR-Rig found. Create a new one and mark it as custom in the Inspector.");
        }
    }

    // Interaction
    [MenuItem("GameObject/AutoXR/Interaction/AutoXR Offset Interactable")]
    public static void CreateAutoXROffsetInteractable(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "Interaction/AutoXR Offset Interactable");
    }

    [MenuItem("GameObject/AutoXR/Interaction/Highlightable Socket Interactor")]
    public static void CreateHighlightableSocketInteractor(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "Interaction/Highlightable Socket Interactor");
    }

    [MenuItem("GameObject/AutoXR/Interaction/Put Back Socket Interactor")]
    public static void CreatePutBackSocketInteractor(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "Interaction/Put Back Socket Interactor");
    }

    [MenuItem("GameObject/AutoXR/Interaction/Tag Check Socket Interactor")]
    public static void CreateTagCheckSocketInteractor(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "Interaction/Tag Check Socket Interactor");
    }

    // Buttons
    [MenuItem("GameObject/AutoXR/Buttons/Auto XR Button Empty Text")]
    public static void CreateAutoXRButtonEmptyText(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "Auto XR Buttons/Auto XR Button Empty Text");
    }

    [MenuItem("GameObject/AutoXR/Buttons/Auto XR Button Empty")]
    public static void CreateAutoXRButtonEmpty(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "Auto XR Buttons/Auto XR Button Empty");
    }

    [MenuItem("GameObject/AutoXR/Buttons/Auto XR Button Round Square Text")]
    public static void CreateAutoXRButtonRoundSquareText(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "Auto XR Buttons/Auto XR Button Round Square Text");
    }

    [MenuItem("GameObject/AutoXR/Buttons/Auto XR Button Round Square")]
    public static void CreateAutoXRButtonRoundSquare(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "Auto XR Buttons/Auto XR Button Round Square");
    }

    [MenuItem("GameObject/AutoXR/Buttons/Auto XR Button Round Text")]
    public static void CreateAutoXRButtonRoundText(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "Auto XR Buttons/Auto XR Button Round Text");
    }

    [MenuItem("GameObject/AutoXR/Buttons/Auto XR Button Round")]
    public static void CreateAutoXRButtonRound(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "Auto XR Buttons/Auto XR Button Round");
    }

    [MenuItem("GameObject/AutoXR/Buttons/Auto XR Button Square Text")]
    public static void CreateAutoXRButtonSquareText(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "Auto XR Buttons/Auto XR Button Square Text");
    }

    [MenuItem("GameObject/AutoXR/Buttons/Auto XR Button Square")]
    public static void CreateAutoXRButtonSquare(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "Auto XR Buttons/Auto XR Button Square");
    }

    // Quiz Buttons
    [MenuItem("GameObject/AutoXR/Buttons/Quiz Buttons/Auto XR Quiz Button Empty")]
    public static void CreateAutoXRQuizButtonEmpty(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "Auto XR Buttons/Auto XR Quiz Button/Auto XR Quiz Button Empty");
    }

    [MenuItem("GameObject/AutoXR/Buttons/Quiz Buttons/Auto XR Quiz Button Round Square")]
    public static void CreateAutoXRQuizButtonRoundSquare(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "Auto XR Buttons/Auto XR Quiz Button/Auto XR Quiz Button Round Square");
    }

    [MenuItem("GameObject/AutoXR/Buttons/Quiz Buttons/Auto XR Quiz Button Round")]
    public static void CreateAutoXRQuizButtonRound(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "Auto XR Buttons/Auto XR Quiz Button/Auto XR Quiz Button Round");
    }

    [MenuItem("GameObject/AutoXR/Buttons/Quiz Buttons/Auto XR Quiz Button Square")]
    public static void CreateAutoXRQuizButtonSquare(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, AutoXRCreationUtils.AUTOXR_QUIZ_BUTTON_SQUARE_PREFAB_NAME);
    }


    [MenuItem("GameObject/AutoXR/Buttons/Quiz Buttons/Auto XR Multiple Choice Confirm Button Square")]
    public static void CreateAutoXRMcConfirmButton(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "Auto XR Buttons/Auto XR Quiz Button/Auto XR Multiple Choice Confirm Button Square");
    }

    // Hud
    [MenuItem("GameObject/AutoXR/HUD/HUD")]
    static void CreateHud(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "UI/Hud");
    }

    [MenuItem("GameObject/AutoXR/HUD/Fade Rect")]
    static void CreateFadeRect(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "UI/Fade Rect");
    }

    [MenuItem("GameObject/AutoXR/HUD/Head Gaze Reticle")]
    static void CreateHeadGazeReticle(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "UI/Head Gaze Reticle");
    }
    
    // UI
    [MenuItem("GameObject/AutoXR/UI/World Space Canvas")]
    static void CreateWorldSpaceImage(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "UI/World Space Canvas");
    }

    [MenuItem("GameObject/AutoXR/UI/World Space Canvas (Always On Top)")]
    static void CreateWorldSpaceCanvasAOT(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "UI/World Space Canvas (Always On Top)");
    }

    // Keyboards
    [MenuItem("GameObject/AutoXR/UI/Keyboard - German")]
    static void CreateKeyboardGerman(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "UI/World Space Keyboard German");
    }


    [MenuItem("GameObject/AutoXR/UI/Keyboard - English")]
    static void CreateKeyboardEnglish(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "UI/World Space Keyboard English");
    }

    [MenuItem("GameObject/AutoXR/UI/Keyboard - Numpad")]
    static void CreateKeyboardNumpad(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "UI/World Space Keyboard Numpad");
    }

    // Exhibition
    [MenuItem("GameObject/AutoXR/Exhibition/Exhibition Display - Object")]
    static void CreateExhibitionDisplayObject(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "Exhibition/Exhibition Display Object");
    }


    [MenuItem("GameObject/AutoXR/Exhibition/Exhibition Display - Image")]
    static void CreateExhibitionDisplayImage(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "Exhibition/Exhibition Display Image");
    }


    // Teleportation
    [MenuItem("GameObject/AutoXR/Interaction/Teleportation Area")]
    static void CreateTeleporationArea(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "Interaction/Teleportation Area");
    }


    [MenuItem("GameObject/AutoXR/Interaction/Teleportation Anchor")]
    static void CreateTeleporationAnchor(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "Interaction/Teleportation Anchor");
    }


    // Data
    [MenuItem("GameObject/AutoXR/Data Gatherer")]
    public static void CreateDataGatherer(MenuCommand menuCommand)
    {
        GameObject go = new GameObject("Data Gatherer");
        go.AddComponent<DataGatherer>();
        GameObjectUtility.EnsureUniqueNameForSibling(go);
        Undo.RegisterCreatedObjectUndo(go, "Create Data Gatherer Game Object");
    }


    private static GameObject InstantiatePrefabAtContextTransform(MenuCommand menuCommand, string prefabName)
    {
        Transform parentOfNewGameObject = AutoXRCreationUtils.GetContextTransform(menuCommand);
        return AutoXRCreationUtils.InstantiateAndPlacePrefab(prefabName, parentOfNewGameObject);
    }
}
