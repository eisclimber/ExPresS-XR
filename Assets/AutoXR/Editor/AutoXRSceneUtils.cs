using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor.SceneTemplate;

public static class AutoXRSceneUtils
{
    private const string AUTOXR_SCENE_TEMPLATE_FORMAT = "Assets/AutoXR/SceneTemplates/{0}.scenetemplate";

    public const string EMPTY_SCENE_NAME = "AutoXREmptyScene";
    public const string GENERAL_TUTORIAL_SCENE_NAME = "AutoXRGeneralTutorialScene";
    public const string EXHIBITION_TUTORIAL_SCENE_NAME = "AutoXRExhibitionTutorialScene";
    public const string EXPERIMENTATION_TUTORIAL_SCENE_NAME = "AutoXRExperimentationTutorialScene";


    [MenuItem("AutoXR/Scenes.../Create New Empty Scene")]
    static void CreateEmptyScene() => LoadSceneTemplate(EMPTY_SCENE_NAME);

    [MenuItem("AutoXR/Scenes.../Create New General Tutorial Scene")]
    static void CreateGeneralTutorialScene() => LoadSceneTemplate(GENERAL_TUTORIAL_SCENE_NAME);

    [MenuItem("AutoXR/Scenes.../Create New Exhibition Tutorial Scene")]
    static void CreateExhibitionTutorialScene() => LoadSceneTemplate(EXHIBITION_TUTORIAL_SCENE_NAME);

    [MenuItem("AutoXR/Scenes.../Create New Experimentation Tutorial Scene")]
    static void CreateExperimentationTutorialScene() => LoadSceneTemplate(EXPERIMENTATION_TUTORIAL_SCENE_NAME);

    // Helper variable that stores the AutoXRRig wile a new scene gets created
    private static string _autoXRRigPrefabName = "";

    public static void LoadSceneTemplate(string templateName, string rigName = AutoXRCreationUtils.CUSTOM_AUTOXR_PREFAB_NAME)
    {
        string path = string.Format(AUTOXR_SCENE_TEMPLATE_FORMAT, templateName);
        SceneTemplateAsset templateAsset = AssetDatabase.LoadAssetAtPath<SceneTemplateAsset>(path);

        _autoXRRigPrefabName = rigName;
        EditorSceneManager.sceneOpened += OneShotAddAutoXRRigCallback;

        InstantiationResult result = SceneTemplateService.Instantiate(templateAsset, false);

        if (result != null && result.scene != null)
        {
            SceneManager.SetActiveScene(result.scene);
        }
    }

    private static void OneShotAddAutoXRRigCallback(UnityEngine.SceneManagement.Scene scene, UnityEditor.SceneManagement.OpenSceneMode mode)
    {
        GameObject rig = AutoXRCreationUtils.CreateAndPlaceGameObject(_autoXRRigPrefabName, null);
        if (rig == null)
        {
            Debug.LogWarning("There is currently no AutoXRRig in the scene. Please create one via the hierarchy.");
        }
        if (rig != null && rig.GetComponent<AutoXRRig>() == null)
        {
            UnityEngine.Object.Destroy(rig);
            Debug.LogError(string.Format("Could load AutoXR-prefab '{0}', but it has no AutoXRRig-Component! Please add an AutoXRRig via the hierarchy.", _autoXRRigPrefabName));
        }

        // Cleanup
        _autoXRRigPrefabName = "";
        EditorSceneManager.sceneOpened -= OneShotAddAutoXRRigCallback;
    }
}
