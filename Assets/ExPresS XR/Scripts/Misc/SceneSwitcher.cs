using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using ExPresSXR.Rig;

namespace ExPresSXR.Misc
{
    public class SceneSwitcher : MonoBehaviour
    {
        /// <summary>
        /// If the scene should be switched with fade. A rig is required if switching with fade.
        /// </summary>
        [SerializeField]
        private bool _useFade = true;
        public bool useFade
        {
            get => _useFade;
        }

        /// <summary>
        /// The scene index to switch to.
        /// Must be added via the Build Settings.
        /// </summary>
        [SerializeField]
        [Tooltip("The scene index to switch to. Must be added via the Build Settings.")]
        private int _sceneIndex = 0;

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
        /// Switch the scene as configured.
        /// </summary>
        public void SwitchScene()
        {
            // Disable interactions while exiting
            if (_rig != null)
            {
                _rig.interactionOptions = 0;
            }

            if (_useFade)
            {
                RuntimeUtils.ChangeSceneWithFade(_rig, _sceneIndex, false, null);
            }
            else
            {
                RuntimeUtils.SwitchSceneAsync(_sceneIndex, null);
            }
        }
    }
}