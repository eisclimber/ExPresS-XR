using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;


namespace ExPresSXR.Editor
{
    public class CreationUtils
    {
        private const string EXPRESS_XR_PREFABS_PATH = "Assets/ExPresS XR/Prefabs/";
        public const string EXPRESS_XR_PREFAB_FORMAT = EXPRESS_XR_PREFABS_PATH + "{0}.prefab";
        public const string TELEPORT_EXPRESS_XR_RIG_PREFAB_NAME = "ExPresS XR Rigs/ExPresS XR Rig - Teleport";
        public const string CONTINUOUS_MOVE_EXPRESS_XR_RIG_PREFAB_NAME = "ExPresS XR Rigs/ExPresS XR Rig - Continuous Move";
        public const string HEAD_GAZE_EXPRESS_XR_RIG_PREFAB_NAME = "ExPresS XR Rigs/ExPresS XR Rig - Head Gaze";
        public const string HEAD_GAZE_TELEPORT_EXPRESS_XR_RIG_PREFAB_NAME = "ExPresS XR Rigs/ExPresS XR Rig - Head Gaze Teleport";
        public const string CUSTOM_EXPRESS_XR_RIG_PREFAB_NAME = "ExPresS XR Rigs/ExPresS XR Rig - Custom";
        public const string QUIZ_BUTTON_SQUARE_PREFAB_NAME = "Buttons/Quiz Buttons/Quiz Button Square";
        public const string MC_CONFIRM_BUTTON_SQUARE_PREFAB_NAME = "ExPresS XR Buttons/Quiz Buttons/Multiple Choice Confirm Button Square";
        public const string AFTER_QUIZ_DIALOG_PATH_NAME = "Misc/After Quiz Dialog";



        /// <summary>
        /// Creates an <see cref="GameObject"> from a given prefab from a menu command and adds it under the current selection
        /// </summary>
        /// <param name="menuCommand"><see cref="MenuCommand"> that requested the creation.</param>
        /// <param name="name">The name of the prefab to be instantiated.</param>
        /// <returns> A Reference the object that was created or <see langword="null"/> if the prefab was 
        /// not found.</returns>
        public static GameObject InstantiatePrefabAtContextTransform(MenuCommand menuCommand, string prefabName)
        {
            Transform parent = CreationUtils.GetContextTransform(menuCommand);
            return CreationUtils.InstantiateAndPlacePrefab(prefabName, parent);
        }


        /// <summary>
        /// Creates an <see cref="GameObject"> from a given prefab and adds it under the current selection
        /// </summary>
        /// <param name="name">The name of the prefab to be instantiated.</param>
        /// <param name="path">The object passed to custom menu item functions to operate on.</param>
        /// <returns> A Reference the object that was created or <see langword="null"/> if the prefab was 
        /// not found.</returns>
        public static GameObject InstantiateAndPlacePrefab(string name, Transform parent = null)
        {
            string path = MakeExPresSXRPrefabPath(name);
            UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab != null)
            {
                GameObject go = (GameObject)GameObject.Instantiate(prefab, parent);

                if (parent == null)
                {
                    Transform goTransform = go.transform;
                    SceneView view = SceneView.lastActiveSceneView;
                    if (view != null)
                        view.MoveToView(goTransform);
                    else
                        goTransform.position = Vector3.zero;

                    StageUtility.PlaceGameObjectInCurrentStage(go);
                }
                GameObjectUtility.EnsureUniqueNameForSibling(go);

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
        /// Creates an path that *MAY* be a path to an ExPresSXR Prefab by using the EXPRESS_XR_PREFAB_FORMAT.
        /// </summary>
        /// <param name="name">The name (or subpath) to an prefab.</param>
        /// <returns>Returns the formatted <see cref="string"/>.</returns>
        public static string MakeExPresSXRPrefabPath(string name)
        {
            if (name.StartsWith(EXPRESS_XR_PREFABS_PATH))
            {
                Debug.LogWarning("Do not add the ExPresSXRPrefabPath to the name. We're accounting for that already.");
                name = name.Substring(EXPRESS_XR_PREFABS_PATH.Length);
            }
            if (name.EndsWith(".prefab"))
            {
                Debug.LogWarning("Do not add the suffix '.prefab' to the name. We're accounting for that already.");
                name = name.Substring(0, name.Length - ".prefab".Length);
            }
            return String.Format(EXPRESS_XR_PREFAB_FORMAT, name);
        }


        public static string customXRRigPath
        {
            get => MakeExPresSXRPrefabPath(CUSTOM_EXPRESS_XR_RIG_PREFAB_NAME);
        }

        /// <summary>
        /// Gets the <see cref="Transform"/> associated with the <see cref="MenuCommand.context"/>.
        /// </summary>
        /// <param name="menuCommand">The object passed to custom menu item functions to operate on.</param>
        /// <returns>Returns the <see cref="Transform"/> of the object that is the target of a menu command,
        /// or <see langword="null"/> if there is no context.</returns>
        public static Transform GetContextTransform(MenuCommand menuCommand)
        {
            var context = menuCommand.context as GameObject;
            return context?.transform;
        }
    }
}