using UnityEngine;
using ExPresSXR.Rig;

namespace ExPresSXR.Misc
{
    /// <summary>
    /// Allows switching scenes with a rig.
    /// </summary>
    public class SceneSwitcher : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("If the scene should be switched with fade. A rig is required if switching with fade.")]
        private bool _useFade = true;
        /// <summary>
        /// If the scene should be switched with fade. A rig is required if switching with fade.
        /// </summary>
        public bool UseFade
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
        [Tooltip("A reference to the rig. Will prevent interactions after exiting and required for fading out.")]
        private ExPresSXRRig _rig;

        /// <summary>
        /// If enabled will try to find the current ExPresSXRRig. 
        /// As this operation is rather expensive, it is best to directly set the reference directly.
        /// </summary>
        [SerializeField]
        [Tooltip("If enabled will try to find the current ExPresSXRRig.\n"
                + "As this operation is rather expensive, it is best to directly set the reference directly.")]
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
        public void SwitchScene() => SwitchScene(_sceneIndex);

        /// <summary>
        /// Switch the scene as configured.
        /// </summary>
        public void SwitchScene(int scene)
        {
            // Disable interactions while exiting
            if (_rig != null)
            {
                _rig.InteractionOptions = InteractionOptions.Nothing;
            }

            if (_useFade)
            {
                RuntimeUtils.ChangeSceneWithFade(_rig, scene, false, null);
            }
            else
            {
                RuntimeUtils.SwitchSceneAsync(scene, null);
            }
        }
    }
}