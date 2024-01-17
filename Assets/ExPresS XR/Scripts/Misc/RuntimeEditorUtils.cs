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

namespace ExPresSXR.Misc
{
    /// <summary>
    /// This class aims to implement common actions that aims to provide some actions only available in the editor during runtime.
    /// </summary>
    public static class RuntimeEditorUtils
    {
        /// <summary>
        /// Changes how the editor is 
        /// </summary>
        /// <param name="gameTabDisplayMode"></param>
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
    }

    /// <summary>
    /// Determines the modes how to display VR in the editor during play mode
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