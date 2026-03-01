using UnityEngine;

namespace ExPresSXR.Misc.ColorSwitching
{
    /// <summary>
    /// Represents a state behavior that can be added to s state in an animator to allow color switching.
    /// </summary>
    public class MaterialSwitchingStateBehaviour : StateMachineBehaviour
    {
        [SerializeField]
        [Tooltip("Material to switch to.")]
        private Material _switchMaterial;
        /// <summary>
        /// Material to switch to.
        /// </summary>
        public Material SwitchMaterial
        {
            get => _switchMaterial;
            set => _switchMaterial = value;
        }

        private ColorAnimatorSwitcher _colorSwitcher;

        /// <summary>
        /// Called when the state is entered.
        /// Tries to retrieve the `ColorAnimationSwitcher`-Component from the Animators GameObject and use it to change the material.
        /// </summary>
        /// <param name="animator">Animator associated with this state machine.</param>
        /// <param name="stateInfo">Runtime information on the state of the animator.</param>
        /// <param name="layerIndex">LayerIndex of the state.</param>
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_colorSwitcher == null && !animator.TryGetComponent(out _colorSwitcher))
            {
                Debug.LogError("MaterialSwitchingStateBehaviour requires a ColorAnimatorSwitcher component on the same GameObject as the Animator.", this);
                return;
            }

            _colorSwitcher.ChangeToMaterial(_switchMaterial);
        }
    }
}