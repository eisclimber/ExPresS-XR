using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ExPresSXR.Rig;
using ExPresSXR.Misc;


namespace ExPresSXR.UI.Menu
{
    public class MainMenuLogic : BasicMenuLogic
    {
        /// <summary>
        /// The scene index of your game scene.
        /// It must be added via the Build Settings and should be greater than 0 (index 0 is the startup scene which should be the menu scene)
        /// </summary>
        [SerializeField]
        [Tooltip("The scene index of your game scene. It must be added via the Build Settings and should be greater than 0 (index 0 is the startup scene which should be the menu scene)")]
        private int _gameSceneIndex = 1; // Default value should be the game scene

        /// <summary>
        /// If enabled, will change to the new scene with fade. A reference to the rig is required if set to true.
        /// </summary>
        [SerializeField]
        [Tooltip("If enabled, will change to the new scene with fade. A reference to the rig is required if set to true.")]
        private bool _changeWithFade = true;

        /// <summary>
        /// Reference to the ExPresSXRRig used for fading. Required if changing with fade.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the ExPresSXRRig used for fading. Required if changing with fade.")]
        private ExPresSXRRig _rig;


        private void Start() {
            if (_changeWithFade && _rig == null)
            {
                Debug.LogError("Change to game was configured to be with fade but no rig to fade was provided.", this);
            }
        }


        /// <summary>
        /// Changes to the game scene as configured with optional fade.
        /// </summary>
        public void ChangeToGameScene()
        {
            if (_changeWithFade)
            {
                RuntimeUtils.ChangeSceneWithFade(_rig, _gameSceneIndex, false, null);
            }
            else
            {
                RuntimeUtils.SwitchSceneAsync(_gameSceneIndex, null);
            }
        }
    }
}