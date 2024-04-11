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
        /// <summary>
        /// How the game is exited. QuitGame quits the game completely 
        /// while the other two switch to another (menu-)scene with optional fade.
        /// </summary>
        [SerializeField]
        private ExitType _exitType = ExitType.QuitGame;
        public ExitType exitType
        {
            get => _exitType;
        }

        /// <summary>
        /// The scene index of your menu scene.
        /// It must be added via the Build Settings and should usually be 0.
        /// </summary>
        [SerializeField]
        [Tooltip("The scene index of your menu scene. It must be added via the Build Settings and should usually be 0.")]
        private int _menuSceneIdx = 0; // Default value should be the menu scene

        /// <summary>
        /// A reference to the rig. Will prevent interactions after exiting and required for fading out.
        /// </summary>
        [SerializeField]
        private ExPresSXRRig _rig;

        /// <summary>
        /// If enabled will try to find the current ExPresSXRRig. 
        /// As this operation is rather expensive, it is best to directly set the reference directly.
        /// </summary>
        [SerializeField]
        private bool _findRigIfMissing;


        private void Start()
        {
            if (_rig == null && _findRigIfMissing)
            {
                _rig = FindFirstObjectByType<ExPresSXRRig>();
            }
        }

        /// <summary>
        /// Exits the game as configured.
        /// </summary>
        public void QuitGame()
        {
            // Disable interactions while exiting
            if (_rig != null)
            {
                _rig.interactionOptions = InteractionOptions.Nothing;
            }

            if (_exitType == ExitType.QuitGame)
            {
                _rig.fadeRect.OnFadeToColorCompleted.AddListener(ExitGame);
                _rig.FadeToColor();
            }
            else if (_exitType == ExitType.ToScene)
            {
                RuntimeUtils.ChangeSceneWithFade(_rig, _menuSceneIdx, false, null);
            }
            else if (_exitType == ExitType.ToSceneNoFade)
            {
                RuntimeUtils.SwitchSceneAsync(_menuSceneIdx, null);
            }
            else
            {
                ExitGame();
            }
        }

        private void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }


        public enum ExitType
        {
            QuitGame,
            QuitGameNoFade,
            ToScene,
            ToSceneNoFade
        }
    }
}