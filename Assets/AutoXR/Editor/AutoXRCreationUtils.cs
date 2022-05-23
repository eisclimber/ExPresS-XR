using System;
using UnityEngine;
using UnityEditor;

public static class AutoXRCreationUtils
{
    private const string AUTOXR_PREFABS_PATH = "Assets/AutoXR/Prefabs/";
    public const string AUTOXR_PREFAB_FORMAT = AUTOXR_PREFABS_PATH + "{0}.prefab";
    public const string TELEPORT_AUTOXR_PREFAB_NAME = "Auto XR Rigs/Auto XR Rig - Teleport";
    public const string CONTINUOUS_MOVE_AUTOXR_PREFAB_NAME = "Auto XR Rigs/Auto XR Rig - Continuous Move";
    public const string HEAD_GAZE_AUTOXR_PREFAB_NAME = "Auto XR Rigs/Auto XR Rig - Head Gaze";
    public const string CUSTOM_AUTOXR_PREFAB_NAME = "Auto XR Rigs/Auto XR Rig - Custom";
    public const string AUTOXR_QUIZ_BUTTON_SQUARE_PREFAB_NAME = "Auto XR Buttons/Auto XR Quiz Button/Auto XR Quiz Button Square";

    // Auto XR Rig
    [MenuItem("GameObject/AutoXR/Auto XR Rig/Teleport")]
    static void CreateAutoXRRig(MenuCommand menuCommand)
    {
        Transform parentOfNewGameObject = GetContextTransform(menuCommand);
        GameObject go = CreateAndPlaceGameObject(TELEPORT_AUTOXR_PREFAB_NAME, parentOfNewGameObject);
    }

    [MenuItem("GameObject/AutoXR/Auto XR Rig/Continuous Move")]
    static void CreateAutoXRRigContinuousMove(MenuCommand menuCommand)
    {
        Transform parentOfNewGameObject = GetContextTransform(menuCommand);
        GameObject go = CreateAndPlaceGameObject(CONTINUOUS_MOVE_AUTOXR_PREFAB_NAME, parentOfNewGameObject);
    }

    [MenuItem("GameObject/AutoXR/Auto XR Rig/Head Gaze")]
    static void CreateAutoXRRigHeadGaze(MenuCommand menuCommand)
    {
        Transform parentOfNewGameObject = GetContextTransform(menuCommand);
        GameObject go = CreateAndPlaceGameObject(HEAD_GAZE_AUTOXR_PREFAB_NAME, parentOfNewGameObject);
    }

    [MenuItem("GameObject/AutoXR/Auto XR Rig/Custom")]
    public static void CreateAutoXRRigCustom(MenuCommand menuCommand)
    {
        Transform parentOfNewGameObject = GetContextTransform(menuCommand);
        GameObject go = CreateAndPlaceGameObject(CUSTOM_AUTOXR_PREFAB_NAME, parentOfNewGameObject);
        if (go == null)
        {
            Debug.LogWarning("No custom AutoXR-Rig found. Create a new one and mark it as custom in the Inspector.");
        }
    }

    // Buttons
    [MenuItem("GameObject/AutoXR/Buttons/Auto XR Button Empty Text")]
    public static void CreateAutoXRButtonEmptyText(MenuCommand menuCommand)
    {
        Transform parentOfNewGameObject = GetContextTransform(menuCommand);
        GameObject go = CreateAndPlaceGameObject("Auto XR Buttons/Auto XR Button Empty Text", parentOfNewGameObject);
    }

    [MenuItem("GameObject/AutoXR/Buttons/Auto XR Button Empty")]
    public static void CreateAutoXRButtonEmpty(MenuCommand menuCommand)
    {
        Transform parentOfNewGameObject = GetContextTransform(menuCommand);
        GameObject go = CreateAndPlaceGameObject("Auto XR Buttons/Auto XR Button Empty", parentOfNewGameObject);
    }

    [MenuItem("GameObject/AutoXR/Buttons/Auto XR Button Round Square Text")]
    public static void CreateAutoXRButtonRoundSquareText(MenuCommand menuCommand)
    {
        Transform parentOfNewGameObject = GetContextTransform(menuCommand);
        GameObject go = CreateAndPlaceGameObject("Auto XR Buttons/Auto XR Button Round Square Text", parentOfNewGameObject);
    }

    [MenuItem("GameObject/AutoXR/Buttons/Auto XR Button Round Square")]
    public static void CreateAutoXRButtonRoundSquare(MenuCommand menuCommand)
    {
        Transform parentOfNewGameObject = GetContextTransform(menuCommand);
        GameObject go = CreateAndPlaceGameObject("Auto XR Buttons/Auto XR Button Round Square", parentOfNewGameObject);
    }

    [MenuItem("GameObject/AutoXR/Buttons/Auto XR Button Round Text")]
    public static void CreateAutoXRButtonRoundText(MenuCommand menuCommand)
    {
        Transform parentOfNewGameObject = GetContextTransform(menuCommand);
        GameObject go = CreateAndPlaceGameObject("Auto XR Buttons/Auto XR Button Round Text", parentOfNewGameObject);
    }

    [MenuItem("GameObject/AutoXR/Buttons/Auto XR Button Round")]
    public static void CreateAutoXRButtonRound(MenuCommand menuCommand)
    {
        Transform parentOfNewGameObject = GetContextTransform(menuCommand);
        GameObject go = CreateAndPlaceGameObject("Auto XR Buttons/Auto XR Button Round", parentOfNewGameObject);
    }

    [MenuItem("GameObject/AutoXR/Buttons/Auto XR Button Square Text")]
    public static void CreateAutoXRButtonSquareText(MenuCommand menuCommand)
    {
        Transform parentOfNewGameObject = GetContextTransform(menuCommand);
        GameObject go = CreateAndPlaceGameObject("Auto XR Buttons/Auto XR Button Square Text", parentOfNewGameObject);
    }

    [MenuItem("GameObject/AutoXR/Buttons/Auto XR Button Square")]
    public static void CreateAutoXRButtonSquare(MenuCommand menuCommand)
    {
        Transform parentOfNewGameObject = GetContextTransform(menuCommand);
        GameObject go = CreateAndPlaceGameObject("Auto XR Buttons/Auto XR Button Square", parentOfNewGameObject);
    }

    // Quiz Buttons
    [MenuItem("GameObject/AutoXR/Buttons/Quiz Buttons/Auto XR Quiz Button Empty")]
    public static void CreateAutoXRQuizButtonEmpty(MenuCommand menuCommand)
    {
        Transform parentOfNewGameObject = GetContextTransform(menuCommand);
        GameObject go = CreateAndPlaceGameObject("Auto XR Buttons/Auto XR Quiz Button/Auto XR Quiz Button Empty", parentOfNewGameObject);
    }

    [MenuItem("GameObject/AutoXR/Buttons/Quiz Buttons/Auto XR Quiz Button Round Square")]
    public static void CreateAutoXRQuizButtonRoundSquare(MenuCommand menuCommand)
    {
        Transform parentOfNewGameObject = GetContextTransform(menuCommand);
        GameObject go = CreateAndPlaceGameObject("Auto XR Buttons/Auto XR Quiz Button/Auto XR Quiz Button Round Square", parentOfNewGameObject);
    }

    [MenuItem("GameObject/AutoXR/Buttons/Quiz Buttons/Auto XR Quiz Button Round")]
    public static void CreateAutoXRQuizButtonRound(MenuCommand menuCommand)
    {
        Transform parentOfNewGameObject = GetContextTransform(menuCommand);
        GameObject go = CreateAndPlaceGameObject("Auto XR Buttons/Auto XR Quiz Button/Auto XR Quiz Button Round", parentOfNewGameObject);
    }

    [MenuItem("GameObject/AutoXR/Buttons/Quiz Buttons/Auto XR Quiz Button Square")]
    public static void CreateAutoXRQuizButtonSquare(MenuCommand menuCommand)
    {
        Transform parentOfNewGameObject = GetContextTransform(menuCommand);
        GameObject go = CreateAndPlaceGameObject(AUTOXR_QUIZ_BUTTON_SQUARE_PREFAB_NAME, parentOfNewGameObject);
    }


    // Hud
    [MenuItem("GameObject/AutoXR/HUD/Fade Rect")]
    static void CreateFadeRect(MenuCommand menuCommand)
    {
        Transform parentOfNewGameObject = GetContextTransform(menuCommand);
        GameObject go = CreateAndPlaceGameObject("Fade Rect", parentOfNewGameObject);
    }

    [MenuItem("GameObject/AutoXR/HUD/Head Gaze Reticle")]
    static void CreateHeadGazeReticle(MenuCommand menuCommand)
    {
        Transform parentOfNewGameObject = GetContextTransform(menuCommand);
        GameObject go = CreateAndPlaceGameObject("Head Gaze Reticle", parentOfNewGameObject);
    }

    // Keyboards
    [MenuItem("GameObject/AutoXR/UI/Keyboard - German")]
    static void CreateKeyboardGerman(MenuCommand menuCommand)
    {
        Transform parentOfNewGameObject = GetContextTransform(menuCommand);
        GameObject go = CreateAndPlaceGameObject("World Space Keyboard German", parentOfNewGameObject);
    }

    [MenuItem("GameObject/AutoXR/UI/Keyboard - English")]
    static void CreateKeyboardEnglish(MenuCommand menuCommand)
    {
        Transform parentOfNewGameObject = GetContextTransform(menuCommand);
        GameObject go = CreateAndPlaceGameObject("World Space Keyboard English", parentOfNewGameObject);
    }

    [MenuItem("GameObject/AutoXR/UI/Keyboard - Numpad")]
    static void CreateKeyboardNumpad(MenuCommand menuCommand)
    {
        Transform parentOfNewGameObject = GetContextTransform(menuCommand);
        GameObject go = CreateAndPlaceGameObject("World Space Keyboard Numpad", parentOfNewGameObject);
    }


    // Helpers

    /// <summary>
    /// Creates an <see cref="GameObject"> from a given prefab and adds it under the current selection
    /// </summary>
    /// <param name="path">The object passed to custom menu item functions to operate on.</param>
    /// <param name="menuCommand">The object passed to custom menu item functions to operate on.</param>
    /// <returns> A Reference the object that was created or <see langword="null"/> if zhe prefab was 
    /// not found.</returns>
    public static GameObject CreateAndPlaceGameObject(string name, Transform parent)
    {
        string path = MakeAutoXRPrefabPath(name);
        UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

        if (prefab != null)
        {
            GameObject go = (GameObject)GameObject.Instantiate(prefab, parent);

            go.name = prefab.name;

            Undo.RegisterCreatedObjectUndo(go, "Created " + name);

            Selection.activeGameObject = go;

            GameObjectUtility.EnsureUniqueNameForSibling(go);

            return go;
        }
        else
        {
            Debug.LogError("Prefab not found at: " + path);
        }
        return null;
    }


    /// <summary>
    /// Creates an path that *MAY* be a path to an AutoXR Prefab by using the AUTOXR_PREFAB_FORMAT.
    /// </summary>
    /// <param name="name">The name (or subpath) to an prefab.</param>
    /// <returns>Returns the formatted <see cref="string"/>.</returns>
    public static string MakeAutoXRPrefabPath(string name) {
        if (name.StartsWith(AUTOXR_PREFABS_PATH)) {
            Debug.LogWarning("Do not add the AutoXRPrefabPath to the name. We're accounting for that already.");
            name = name.Substring(AUTOXR_PREFABS_PATH.Length);
        }
        if (name.EndsWith(".prefab")) {
            Debug.LogWarning("Do not add the suffix '.prefab' to the name. We're accounting for that already.");
            name = name.Substring(0, name.Length - ".prefab".Length);
        }
        return String.Format(AUTOXR_PREFAB_FORMAT, name);
    }


    public static string customAutoXRRigPath
    {
        get => MakeAutoXRPrefabPath(CUSTOM_AUTOXR_PREFAB_NAME);
    }

    /// <summary>
    /// Gets the <see cref="Transform"/> associated with the <see cref="MenuCommand.context"/>.
    /// </summary>
    /// <param name="menuCommand">The object passed to custom menu item functions to operate on.</param>
    /// <returns>Returns the <see cref="Transform"/> of the object that is the target of a menu command,
    /// or <see langword="null"/> if there is no context.</returns>
    private static Transform GetContextTransform(this MenuCommand menuCommand)
    {
        var context = menuCommand.context as GameObject;
        if (context == null)
            return null;

        return context.transform;
    }
}
