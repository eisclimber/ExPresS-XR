using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

// Modified version of CaseyHofland's code (thanks!!): https://forum.unity.com/threads/possible-bug-unable-to-parse-file-scene-unity-parser-failure-on-localizestringevent.1168706/
namespace ExPresSXR.Editor.Utility
{
    public static class MissingScriptDetector
    {
        [MenuItem("ExPresS XR/Tools.../Find Missing Scripts", false, 6)]
        private static void FindMissingScripts()
        {
            int numMissing = 0;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                var gameObjects = scene.GetRootGameObjects();

                foreach (var gameObject in gameObjects)
                {
                    numMissing = FindInGO(gameObject, numMissing);
                }
            }
            if (numMissing <= 0)
            {
                Debug.Log("No missing Scrips found.");
            }
        }

        private static int FindInGO(GameObject gameObject, int numMissing)
        {
            Component[] components = gameObject.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    var name = gameObject.name;
                    var transform = gameObject.transform;
                    while (transform.parent != null)
                    {
                        name = transform.parent.name + "/" + name;
                        transform = transform.parent;
                    }
                    Debug.LogWarning($"{name} has an empty script attached in position: {i}", gameObject);
                    numMissing++;
                }
            }

            // Now recurse through each child GO (if there are any):
            foreach (Transform childTransform in gameObject.transform)
            {
                numMissing = FindInGO(childTransform.gameObject, numMissing);
            }
            return numMissing;
        }
    }
}