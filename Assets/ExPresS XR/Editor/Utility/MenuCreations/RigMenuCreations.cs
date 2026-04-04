using System.IO;
using UnityEngine;
using UnityEditor;
using ExPresSXR.Editor.Utility;

namespace ExPresSXR.Editor.UtilityMenuCreation
{
    public static class RigMenuCreations
    {
        [MenuItem("GameObject/ExPresS XR/XR Rig/Teleport")]
        static void CreateXRRig(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, CreationUtils.TELEPORT_RIG_PREFAB_NAME);
        }

        [MenuItem("GameObject/ExPresS XR/XR Rig/Joystick")]
        static void CreateXRRigContinuousMove(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, CreationUtils.JOYSTICK_RIG_PREFAB_NAME);
        }

        [MenuItem("GameObject/ExPresS XR/XR Rig/Grab Motion")]
        static void CreateXRRigGrabMove(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, CreationUtils.GRAB_MOTION_RIG_PREFAB_NAME);
        }

        [MenuItem("GameObject/ExPresS XR/XR Rig/Grab Manipulation")]
        static void CreateXRRigGrabManipulation(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, CreationUtils.GRAB_MANIPULATION_RIG_PREFAB_NAME);
        }

        [MenuItem("GameObject/ExPresS XR/XR Rig/Head Gaze")]
        static void CreateXRRigHeadGaze(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, CreationUtils.HEAD_GAZE_RIG_PREFAB_NAME);
        }

        [MenuItem("GameObject/ExPresS XR/XR Rig/None")]
        static void CreateXRRigNone(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, CreationUtils.NONE_RIG_PREFAB_NAME);
        }

        [MenuItem("GameObject/ExPresS XR/XR Rig/Custom")]
        static void CreateXRRigCustom(MenuCommand menuCommand)
        {
            CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, CreationUtils.CUSTOM_RIG_PREFAB_NAME);
        }


        [MenuItem("GameObject/ExPresS XR/XR Rig/Custom (Saved)")]
        public static void CreateXRRigSaved(MenuCommand menuCommand)
        {
            GameObject go = null;
            if (File.Exists(CreationUtils.SavedXRRigPath))
            {
                go = CreationUtils.InstantiateGameObjectAtContextTransform(menuCommand, CreationUtils.SAVED_RIG_PREFAB_NAME);
            }

            if (go == null)
            {
                Debug.LogError("No custom XR Rig found. Create a new one and save it from the rig's inspector.");
            }
        }
    }
}