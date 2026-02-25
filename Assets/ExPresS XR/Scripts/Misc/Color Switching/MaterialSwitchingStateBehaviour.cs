using UnityEngine;

namespace ExPresSXR.Misc.ColorSwitching
{
    /// <summary>
    /// Represents a state behavior that can be added to s state in an animator to allow color switching.
    /// </summary>
    public class MaterialSwitchingStateBehaviour : StateMachineBehaviour
    {
        /// <summary>
        /// Material to switch to.
        /// </summary>
        [SerializeField]
        [Tooltip("Material to switch to.")]
        private Material _switchMaterial;
        public Material SwitchMaterial
        {
            get => _switchMaterial;
            set => _switchMaterial = value;
        }

        private ColorAnimatorSwitcher _colorSwitcher;

        /// <summary>
        /// < inheritdoc />
        /// </summary>
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