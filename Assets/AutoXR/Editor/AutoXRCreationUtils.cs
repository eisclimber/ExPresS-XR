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

    // Hud
    [MenuItem("GameObject/AutoXR/Fade Rect")]
    static void CreateFadeRect(MenuCommand menuCommand)
    {
        Transform parentOfNewGameObject = GetContextTransform(menuCommand);
        GameObject go = CreateAndPlaceGameObject("Fade Rect", parentOfNewGameObject);
    }

    [MenuItem("GameObject/AutoXR/Head Gaze Reticle")]
    static void CreateHeadGazeReticle(MenuCommand menuCommand)
    {
        Transform parentOfNewGameObject = GetContextTransform(menuCommand);
        GameObject go = CreateAndPlaceGameObject("Head Gaze Reticle", parentOfNewGameObject);
    }

    // Keyboards
    [MenuItem("GameObject/AutoXR/Keyboard - German")]
    static void CreateKeyboardGerman(MenuCommand menuCommand)
    {
        Transform parentOfNewGameObject = GetContextTransform(menuCommand);
        GameObject go = CreateAndPlaceGameObject("World Space Keyboard German", parentOfNewGameObject);
    }
    
    [MenuItem("GameObject/AutoXR/Keyboard - English")]
    static void CreateKeyboardEnglish(MenuCommand menuCommand)
    {
        Transform parentOfNewGameObject = GetContextTransform(menuCommand);
        GameObject go = CreateAndPlaceGameObject("World Space Keyboard English", parentOfNewGameObject);
    }

    [MenuItem("GameObject/AutoXR/Keyboard - Numpad")]
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
        else {
            Debug.LogError("Prefab not found at: " + path);
        }
        return null;
    }


    /// <summary>
    /// Creates an path that *MAY* be a path to an AutoXR Prefab by using the AUTOXR_PREFAB_FORMAT.
    /// </summary>
    /// <param name="name">The name (or subpath) to an prefab.</param>
    /// <returns>Returns the formatted <see cref="string"/>.</returns>
    public static string MakeAutoXRPrefabPath(string name) => String.Format(AUTOXR_PREFAB_FORMAT, name);


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
