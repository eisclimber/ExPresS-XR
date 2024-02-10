using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using ExPresSXR.UI;
using ExPresSXR.Rig;
using UnityEditor;
using UnityEngine.XR;
using System.Reflection;
using UnityEngine.Events;

namespace ExPresSXR.Misc
{
    /// <summary>
    /// This class aims to implement common actions that aims to provide some actions exclusively available in the UnityEditor editor during runtime.
    /// </summary>
    public static class RuntimeEditorUtils
    {
        public const string EXPRESS_XR_PREFABS_PATH = "Assets/ExPresS XR/Prefabs/";
        public const string EXPRESS_XR_PREFAB_FORMAT = EXPRESS_XR_PREFABS_PATH + "{0}.prefab";

        /// <summary>
        /// Changes how the editor displays the VR view in the Game-Tab.
        /// </summary>
        /// <param name="gameTabDisplayMode">How the game is shown.</param>
        public static void ChangeGameTabDisplayMode(GameTabDisplayMode gameTabDisplayMode)
        {
#if UNITY_EDITOR
            List<XRDisplaySubsystem> displaySubsystems = new();
            SubsystemManager.GetInstances(displaySubsystems);

            // Update in-editor display mode
            if (displaySubsystems.Count > 0)
            {
                displaySubsystems[0].SetPreferredMirrorBlitMode((int)gameTabDisplayMode);
            }
#else
            Debug.LogError("Changing the DisplayTab Mode is only allowed in the editor.");
#endif
        }
        /// <summary>
        /// Creates an path that *MAY* be a path to an ExPresSXR Prefab by using the EXPRESS_XR_PREFAB_FORMAT.
        /// The parameter name correlates to the relative location inside ExPresS XR's prefabs directory 
        /// and usually matches with the structure in the GameObject-Creation-Menu.
        /// </summary>
        /// <param name="name">The name of (or subpath to) a prefab.</param>
        /// <returns>Returns the formatted <see cref="string"/>.</returns>
        public static string MakeExPresSXRPrefabPath(string name)
        {
#if UNITY_EDITOR
            if (name.StartsWith(EXPRESS_XR_PREFABS_PATH))
            {
                Debug.LogWarning("Do not add the ExPresSXRPrefabPath to the name. We're accounting for that already.");
                name = name[EXPRESS_XR_PREFABS_PATH.Length..];
            }
            if (name.EndsWith(".prefab"))
            {
                Debug.LogWarning("Do not add the suffix '.prefab' to the name. We're accounting for that already.");
                name = name[..^".prefab".Length];
            }
            return string.Format(EXPRESS_XR_PREFAB_FORMAT, name);
#else
            Debug.LogError("Prefabs should not be loaded during runtime using paths. Use a reference to the prefab instead!");
#endif
        }
    }

    /// <summary>
    /// Determines the modes how to display VR in the editor during play mode.
    /// </summary>
    public enum GameTabDisplayMode
    {
        Default = 0,
        LeftEye = -1,
        RightEye = -2,
        SideBySide = -3,
        SideBySideOcclusionMesh = -4,
        Distort = -5,
        None = -6
    }
}