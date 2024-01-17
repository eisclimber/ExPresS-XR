using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor.SceneTemplate;
using ExPresSXR.Rig;


namespace ExPresSXR.Editor
{
    public static class SceneUtils
    {
        public const string BASIC_SCENE_NAME = "Basic Scene (ExPresS XR)";
        public const string EXHIBITION_EXPORT_SCENE_NAME = "Exhibition Export Scene";
        public const string EXHIBITION_TUTORIAL_SCENE_NAME = "Exhibition Tutorial Scene";
        public const string EXPERIMENTATION_EXPORT_SCENE_NAME = "Experimentation Export Scene";
        public const string EXPERIMENTATION_TUTORIAL_SCENE_NAME = "Experimentation Tutorial Scene";
        public const string GENERAL_EXPORT_SCENE_NAME = "General Export Scene";
        public const string INTERACTION_TUTORIAL_SCENE_NAME = "Interaction Tutorial Scene";
        public const string MOBILE_EXPORT_SCENE_NAME = "Mobile Export Scene";
        public const string MOVEMENT_TUTORIAL_SCENE_NAME = "Movement Tutorial Scene";


        public static void AddRigWithConfigData(RigCreationData rigData)
        {
            GameObject rigObject = null;
            string rigPath = rigData?.basePrefabPath ?? "";
            string fullRigPath = string.Format(CreationUtils.EXPRESS_XR_PREFAB_FORMAT, rigPath);
            if (AssetDatabase.LoadAssetAtPath<GameObject>(fullRigPath) != null)
            {
                rigObject = CreationUtils.InstantiateAndPlaceGameObject(rigPath, null);
            }

            if (rigObject == null)
            {
                Debug.LogWarning("There may not be an ExPresS XR Rig in the scene. Please create one via the hierarchy.");
            }
            else if (!rigObject.TryGetComponent(out ExPresSXRRig rig))
            {
                Debug.LogError($"Could load ExPresS XR-prefab at '{rigPath}', but it has no ExPresSXRRig-Component! "
                                + "Please add an ExPresSXRRig via the hierarchy.");
                UnityEngine.Object.Destroy(rig);
            }
            else
            {
                // Apply rig Data Config if it exists
                rigData.ApplyConfigToRig(rig);
            }
        }

        public class RigCreationData
        {
            /// <summary>
            /// Path to the prefab that shall be used as base for the rig.
            /// Default: CreationUtils.SAVED_RIG_PREFAB_NAME
            /// </summary>
            public string basePrefabPath = CreationUtils.SAVED_RIG_PREFAB_NAME;

            public bool useInputMethod;
            public InputMethod inputMethod;

            public bool useMovementPreset;
            public MovementPreset movementPreset;

            public bool useInteractionOptions;
            public InteractionOptions interactionOptions;

    
            public RigCreationData() => new RigCreationData(CreationUtils.SAVED_RIG_PREFAB_NAME);

            public RigCreationData(string basePrefabPath)
            {
                this.basePrefabPath = basePrefabPath;

                useInputMethod = false;
                useMovementPreset = false;
                useInteractionOptions = false;
            }

            public RigCreationData(string basePrefabPath, InputMethod inputMethod, MovementPreset movementPreset, InteractionOptions interactionOptions)
            {
                this.basePrefabPath = basePrefabPath;

                useInputMethod = true;
                this.inputMethod = inputMethod;

                useMovementPreset = true;
                this.movementPreset = movementPreset;

                useInteractionOptions = true;
                this.interactionOptions = interactionOptions;
            }

            public void ApplyConfigToRig(ExPresSXRRig rig)
            {
                if (useInputMethod)
                {
                    rig.inputMethod = inputMethod;
                }

                if (useMovementPreset)
                {
                    rig.movementPreset = movementPreset;
                }

                if (useInteractionOptions)
                {
                    rig.interactionOptions = interactionOptions;
                }
            }
        }
    }
}