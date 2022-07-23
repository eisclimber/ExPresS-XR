using System;
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
        private static string _xRRigPrefabName = "";


        [MenuItem("ExPresS XR/Scenes.../Create New Basic Scene (ExPresS XR)")]
        static void CreateEmptyScene() => LoadSceneTemplate(BASIC_SCENE_NAME);

        [MenuItem("ExPresS XR/Scenes.../Create New Exhibition Export Scene")]
        static void CreateExhibitionExportScene() => LoadSceneTemplate(EXHIBITION_EXPORT_SCENE_NAME);

        [MenuItem("ExPresS XR/Scenes.../Create New Exhibition Tutorial Scene")]
        static void CreateExhibitionTutorialScene() => LoadSceneTemplate(EXHIBITION_TUTORIAL_SCENE_NAME);

        [MenuItem("ExPresS XR/Scenes.../Create New Experimentation Export Scene")]
        static void CreateExperimentationTutorialScene() => LoadSceneTemplate(EXPERIMENTATION_EXPORT_SCENE_NAME);

        [MenuItem("ExPresS XR/Scenes.../Create New Experimentation Tutorial Scene")]
        static void CreateExperimentationExportScene() => LoadSceneTemplate(EXPERIMENTATION_TUTORIAL_SCENE_NAME);

        [MenuItem("ExPresS XR/Scenes.../Create New General Export Scene")]
        static void CreateGeneralExportScene() => LoadSceneTemplate(GENERAL_EXPORT_SCENE_NAME);

        [MenuItem("ExPresS XR/Scenes.../Create New Interaction Tutorial Scene")]
        static void CreateInteractionTutorialScene() => LoadSceneTemplate(INTERACTION_TUTORIAL_SCENE_NAME);

        [MenuItem("ExPresS XR/Scenes.../Create New Mobile Export Scene")]
        static void CreateMobileExportScene() => LoadSceneTemplate(MOBILE_EXPORT_SCENE_NAME);

        [MenuItem("ExPresS XR/Scenes.../Create New Movement Tutorial Scene")]
        static void CreateMovementTutorialScene() => LoadSceneTemplate(MOVEMENT_TUTORIAL_SCENE_NAME);



        public static void LoadSceneTemplate(string templateName, string rigName = CreationUtils.CUSTOM_EXPRESS_XR_RIG_PREFAB_NAME)
        {
            string path = string.Format(SCENE_TEMPLATE_FORMAT, templateName);
            SceneTemplateAsset templateAsset = AssetDatabase.LoadAssetAtPath<SceneTemplateAsset>(path);

            if (templateAsset == null)
            {
                Debug.LogErrorFormat("Could not find SceneTemplate at '{0}'.", path);
            }

            _xRRigPrefabName = rigName;
            EditorSceneManager.sceneOpened += OneShotAddXRRigCallback;

            InstantiationResult result = SceneTemplateService.Instantiate(templateAsset, false);

            if (result != null && result.scene != null)
            {
                SceneManager.SetActiveScene(result.scene);
            }
        }

        private static void OneShotAddXRRigCallback(UnityEngine.SceneManagement.Scene scene, UnityEditor.SceneManagement.OpenSceneMode mode)
        {
            GameObject rig = CreationUtils.InstantiateAndPlacePrefab(_xRRigPrefabName, null);
            if (rig == null)
            {
                Debug.LogWarning("There is currently no ExPresSXRRig in the scene. Please create one via the hierarchy.");
            }
            if (rig != null && rig.GetComponent<ExPresSXRRig>() == null)
            {
                UnityEngine.Object.Destroy(rig);
                Debug.LogError(string.Format("Could load ExPresS XR-prefab '{0}', but it has no ExPresSXRRig-Component! Please add an ExPresSXRRig via the hierarchy.", _xRRigPrefabName));
            }

            // Cleanup
            _xRRigPrefabName = "";
            EditorSceneManager.sceneOpened -= OneShotAddXRRigCallback;
        }
    }
}