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
        private int _menuSceneIndex = 0; // Default value should be the menu scene

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
        private bool _findRigIfMissing = true;


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
                _rig.interactionOptions = 0;
            }

            if (_exitType == ExitType.ToScene)
            {
                RuntimeUtils.ChangeSceneWithFade(_rig, _menuSceneIndex, false, null);
            }
            else if (_exitType == ExitType.ToScene)
            {
                RuntimeUtils.SwitchSceneAsync(_menuSceneIndex, null);
            }
            else
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                Application.Quit();
            }
        }


        public enum ExitType
        {
            QuitGame,
            ToScene,
            ToSceneNoFade
        }
    }
}