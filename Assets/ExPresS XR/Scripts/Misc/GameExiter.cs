using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using ExPresSXR.Rig;

namespace ExPresSXR.Misc
{
    public class GameExiter : MonoBehaviour
    {
        [SerializeField]
        private ExitType _exitType = ExitType.QuitGame;
        public ExitType exitType
        {
            get => _exitType;
        }

        [SerializeField]
        [Tooltip("The scene index of your menu scene. It must be added via the Build Settings and should usually be 0.")]
        private int _menuSceneIndex = 0; // Default value should be the menu scene

        [SerializeField]
        private ExPresSXRRig rig;

        [SerializeField]
        private bool findRigIfMissing = true;


        private Coroutine loadSceneCoroutine;


        private void Start()
        {
            if (rig == null && findRigIfMissing)
            {
                rig = FindFirstObjectByType<ExPresSXRRig>();
            }
        }

        public void StartGameEnd()
        {
            if (rig != null)
            {
                rig.FadeToColor();
                // Disable interactions while the game is loading
                rig.interactionOptions = 0;

                rig.fadeRect.OnFadeToColorCompleted.AddListener(QuitGame);
            }
            else
            {
                QuitGame();
            }
        }

        private void QuitGame()
        {
            if (_exitType == ExitType.ToScene)
            {
                ChangeToMenuScene();
            }
            else
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                Application.Quit();
            }
        }

        private void ChangeToMenuScene()
        {
            if (loadSceneCoroutine == null)
            {
                // Only start if not already loading a scene
                loadSceneCoroutine = StartCoroutine(ChangeToMenuSceneAsync());
            }
        }

        private IEnumerator ChangeToMenuSceneAsync()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_menuSceneIndex, LoadSceneMode.Single);

            // Wait for the scene to load
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }


        public enum ExitType
        {
            QuitGame,
            ToScene
        }
    }
}