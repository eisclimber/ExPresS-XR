using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using ExPresSXR.Rig;

namespace ExPresSXR.Misc
{
    /// <summary>
    /// Allows exiting the game in both the built version of the game and the Unity Editor.
    /// </summary>
    public class GameExiter : MonoBehaviour
    {
        [SerializeField]
        private bool _useFade = true;
        /// <summary>
        /// If the games should be exited with fade. A rig is required if exiting with fade.
        /// </summary>
        public bool UseFade
        {
            get => _useFade;
        }

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
        public void ExitGame()
        {
            // Disable interactions while exiting
            if (_rig != null)
            {
                _rig.InteractionOptions = InteractionOptions.Nothing;
            }

            if (_useFade)
            {
                _rig.FadeRect.OnFadeToColorCompleted.AddListener(PerformGameExit);
                _rig.FadeToColor();
            }
            else
            {
                PerformGameExit();
            }
        }

        private void PerformGameExit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}