using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ExPresSXR.Rig;


namespace ExPresSXR.UI.Menu
{
    public class MainMenuLogic : BasicMenuLogic
    {
        [SerializeField]
        [Tooltip("The scene index of your game scene. It must be added via the Build Settings and should be greater than 0 (index 0 is the startup scene which should be the menu scene)")]
        private int _gameSceneIndex = 1; // Default value should be the game scene

        [SerializeField]
        private ExPresSXRRig rig;

        [SerializeField]
        private bool findRigIfMissing = true;


        private Coroutine loadSceneCoroutine;


        private void Start() {
            if (rig == null && findRigIfMissing)
            {
                rig = FindFirstObjectByType<ExPresSXRRig>();
            }
        }


        public void StartGame()
        {
            if (rig != null)
            {
                rig.FadeToColor();
                // Disable interactions while the game is loading
                rig.interactionOptions = 0;

                rig.fadeRect.OnFadeToColorCompleted.AddListener(ChangeToGameScene);
            }
            else
            {
                ChangeToGameScene();
            }
        }


        private void ChangeToGameScene()
        {
            // Only start if not already loading a scene
            loadSceneCoroutine ??= StartCoroutine(ChangeToGameSceneAsync());
        }

        private IEnumerator ChangeToGameSceneAsync()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_gameSceneIndex, LoadSceneMode.Single);

            // Wait for the scene to load
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
    }
}