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
        private const string SCENE_TEMPLATE_FORMAT = "Assets/ExPresS XR/Scene Templates/{0}.scenetemplate";

        public const string BASIC_SCENE_NAME = "Basic Scene (ExPresS XR)";
        public const string EXHIBITION_EXPORT_SCENE_NAME = "Exhibition Export Scene";
        public const string EXHIBITION_TUTORIAL_SCENE_NAME = "Exhibition Tutorial Scene";
        public const string EXPERIMENTATION_EXPORT_SCENE_NAME = "Experimentation Export Scene";
        public const string EXPERIMENTATION_TUTORIAL_SCENE_NAME = "Experimentation Tutorial Scene";
        public const string GENERAL_EXPORT_SCENE_NAME = "General Export Scene";
        public const string INTERACTION_TUTORIAL_SCENE_NAME = "Interaction Tutorial Scene";
        public const string MOBILE_EXPORT_SCENE_NAME = "Mobile Export Scene";
        public const string MOVEMENT_TUTORIAL_SCENE_NAME = "Movement Tutorial Scene";


        // Helper variable that stores the ExPresS XR Rig wile a new scene gets created
        private static RigConfigData _rigConfig = null;


        /// <summary>
        /// Instantiates a scene template with the given name and adds a rig to it. 
        /// The rig can be configured using the rigData. If none is provided the users saved rig is used if it exists or else the teleport rig.
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="rigData">The Data specifying the rigs parameters.</param>
        // public static void LoadSceneTemplate(string templateName, RigConfigData rigData = null)
        // {
        //     string path = string.Format(SCENE_TEMPLATE_FORMAT, templateName);
        //     SceneTemplateAsset templateAsset = AssetDatabase.LoadAssetAtPath<SceneTemplateAsset>(path);

        //     if (templateAsset == null)
        //     {
        //         Debug.LogError($"Could not find SceneTemplate at '{path}'.");
        //     }

        //     if (rigData != null && File.Exists(CreationUtils.savedXRRigPath))
        //     {
        //         _rigConfig = rigData;
        //     }
        //     else if (File.Exists(CreationUtils.savedXRRigPath))
        //     {
        //         Debug.LogWarning("No Custom Rig found using your saved Rig instead.");
        //         rigData ??= new RigConfigData();
        //         rigData.basePrefabPath = CreationUtils.SAVED_RIG_PREFAB_NAME;
        //         _rigConfig = rigData;
        //     }
        //     else
        //     {
        //         Debug.LogWarning("No Custom Rig found using the 'Teleportation'-Rig instead.");
        //         rigData ??= new RigConfigData();
        //         rigData.basePrefabPath = CreationUtils.TELEPORT_RIG_PREFAB_NAME;
        //         _rigConfig = rigData;
        //     }

        //     EditorSceneManager.sceneOpened += OneShotAddXRRigCallback;

        //     InstantiationResult result = SceneTemplateService.Instantiate(templateAsset, false);

        //     if (result != null && result.scene != null)
        //     {
        //         SceneManager.SetActiveScene(result.scene);
        //     }
        // }


        private static void OneShotAddXRRigCallback(Scene scene, OpenSceneMode mode)
        {
            AddRigWithConfigData(_rigConfig);

            // Cleanup
            _rigConfig = null;
            EditorSceneManager.sceneOpened -= OneShotAddXRRigCallback;
        }


        public static void AddRigWithConfigData(RigConfigData rigData)
        {
            GameObject rigObject = null;
            string rigPath = rigData?.basePrefabPath ?? "";
            string fullRigPath = string.Format(CreationUtils.EXPRESS_XR_PREFAB_FORMAT, rigPath);
            if (AssetDatabase.LoadAssetAtPath<GameObject>(fullRigPath) != null)
            {
                rigObject = CreationUtils.InstantiateAndPlacePrefab(rigPath, null);
            }

            if (rigObject == null)
            {
                Debug.LogWarning("There may not be an ExPresS XR Rig in the scene. Please create one via the hierarchy.");
            }
            else if (!rigObject.TryGetComponent(out ExPresSXRRig rig))
            {
                UnityEngine.Object.Destroy(rig);
                Debug.LogError($"Could load ExPresS XR-prefab '{rigPath}', but it has no ExPresSXRRig-Component! "
                    + "Please add an ExPresSXRRig via the hierarchy.");
            }
            else
            {
                // Apply rig Data Config if it exists
                rigData.ApplyConfigToRig(rig);
            }
        }

        public class RigConfigData
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



            public RigConfigData() => new RigConfigData(CreationUtils.SAVED_RIG_PREFAB_NAME);

            public RigConfigData(string basePrefabPath)
            {
                this.basePrefabPath = basePrefabPath;

                useInputMethod = false;
                useMovementPreset = false;
                useInteractionOptions = false;
            }

            public RigConfigData(string basePrefabPath, InputMethod inputMethod, MovementPreset movementPreset, InteractionOptions interactionOptions)
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