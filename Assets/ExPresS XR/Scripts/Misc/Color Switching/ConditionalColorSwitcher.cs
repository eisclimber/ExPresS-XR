using UnityEngine;

namespace ExPresSXR.Misc.ColorSwitching
{
    /// <summary>
    /// Provides the option to switch materials of a renderer.
    /// 
    /// It is highly recommended to use a ColorAnimationSwitcher instead.
    /// </summary>
    public class ConditionalColorSwitcher : ColorSwitcher
    {
        [SerializeField]
        [Tooltip("Setting this bool controls if 'SwitchMaterialWithBool()' switches to the original or alternative material.")]
        private bool _switchToOriginal;
        /// <summary>
        /// Setting this bool controls if 'SwitchMaterialWithBool()' switches to the original or alternative material.
        /// </summary>
        public bool SwitchToOriginal
        {
            get => _switchToOriginal;
            set => _switchToOriginal = value;
        }

        /// <summary>
        /// Switches materials to either the original or alternative material according to 'SwitchToOriginal'.
        /// </summary>()
        public void SwitchMaterialWithBool()
        {
            if (_switchToOriginal)
            {
                ActivateOriginalMaterial();
            }
            else
            {
                ActivateAlternativeMaterial();
            }
        }
    }
}