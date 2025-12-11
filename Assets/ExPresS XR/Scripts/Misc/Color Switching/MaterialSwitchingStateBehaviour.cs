using UnityEngine;

namespace ExPresSXR.Misc.ColorSwitching
{
    public class MaterialSwitchingStateBehaviour : StateMachineBehaviour
    {
        [SerializeField]
        private Material _switchMaterial;
        public Material SwitchMaterial
        {
            get => _switchMaterial;
            set => _switchMaterial = value;
        }

        private ColorAnimatorSwitcher _colorSwitcher;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Debug.Log("Switching material to: " + _switchMaterial.name);
            if (_colorSwitcher == null && !animator.TryGetComponent(out _colorSwitcher))
            {
                Debug.LogError("MaterialSwitchingStateBehaviour requires a ColorAnimatorSwitcher component on the same GameObject as the Animator.", this);
                return;
            }

            _colorSwitcher.ChangeToMaterial(_switchMaterial);
        }
    }
}