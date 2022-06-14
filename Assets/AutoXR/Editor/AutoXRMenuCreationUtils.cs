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


    // Hud
    [MenuItem("GameObject/AutoXR/HUD/Fade Rect")]
    static void CreateFadeRect(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "Fade Rect");
    }

    [MenuItem("GameObject/AutoXR/HUD/Head Gaze Reticle")]
    static void CreateHeadGazeReticle(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "Head Gaze Reticle");
    }
    
    // UI
    [MenuItem("GameObject/AutoXR/UI/World Space Image")]
    static void CreateWorldSpaceImage(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "World Space Image");
    }

    // Keyboards
    [MenuItem("GameObject/AutoXR/UI/Keyboard - German")]
    static void CreateKeyboardGerman(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "World Space Keyboard German");
    }

    [MenuItem("GameObject/AutoXR/UI/Keyboard - English")]
    static void CreateKeyboardEnglish(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "World Space Keyboard English");
    }

    [MenuItem("GameObject/AutoXR/UI/Keyboard - Numpad")]
    static void CreateKeyboardNumpad(MenuCommand menuCommand)
    {
        GameObject go = InstantiatePrefabAtContextTransform(menuCommand, "World Space Keyboard Numpad");
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
