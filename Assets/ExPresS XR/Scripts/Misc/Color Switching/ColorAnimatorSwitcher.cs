using UnityEngine;

namespace ExPresSXR.Misc.ColorSwitching
{
    public class ColorAnimatorSwitcher : MonoBehaviour
    {
        const string TRIGGER_PREFIX = "TrColor";

        [SerializeField]
        [Tooltip("The animator with the Animation Controller implementing the logic for switching materials.")]
        private Animator _animator;

        /// <summary>
        /// The MeshRenderer whose material will be manipulated.
        /// </summary>
        [SerializeField]
        [Tooltip("The MeshRenderer whose material will be manipulated.")]
        private MeshRenderer _meshRenderer;

        /// <summary>
        /// List of materials associated with the animation states in the Animator Controller.
        /// </summary>
        [SerializeField]
        [Tooltip("List of materials associated with the animation states in the Animator Controller.")]
        private MaterialStateMapping[] _colorStateMappings;

        private void Awake()
        {
            if (_animator == null && !TryGetComponent(out _animator))
            {
                Debug.LogError("ColorAnimationSwitcher requires an Animator component to function properly.");
            }

            if (_meshRenderer == null && !TryGetComponent(out _meshRenderer))
            {
                Debug.LogError("ColorAnimationSwitcher requires a MeshRenderer component to function properly.");
            }

            EvalaluateBindings();
        }

        private void ChangeMaterial()
        {
            if (_meshRenderer != null)
            {
                _meshRenderer.material = _colorStateMappings[_animator.GetCurrentAnimatorStateInfo(0).shortNameHash].Color;
            }
        }

        public void ChangeColorWithTrigger(int triggerIdx) => ChangeColorWithTrigger(TRIGGER_PREFIX + triggerIdx);

        public void ChangeColorWithTrigger(string triggerName)
        {
            if (_animator != null)
            {
                _animator.SetTrigger(triggerName);
            }
        }

        public void ChangeColorWithBool(string boolName, bool boolValue)
        {
            if (_animator != null)
            {
                _animator.SetBool(boolName, boolValue);
            }
        }

        public void ChangeColorWithFloat(string floatName, float floatValue)
        {
            if (_animator != null)
            {
                _animator.SetFloat(floatName, floatValue);
            }
        }

        private void EvalaluateBindings()
        {
            for (int i = 0; i < _colorStateMappings.Length; i++)
            {
                if (string.IsNullOrEmpty(_colorStateMappings[i].StateName))
                {
                    Debug.LogWarning($"ColorAnimatorSwitcher: State name for MaterialStateMapping at index {i} is null or empty.");
                }

                if (_colorStateMappings[i].Color == null)
                {
                    Debug.LogWarning($"ColorAnimatorSwitcher: No material set for MaterialStateMapping at index {i}.");
                }
            }
        }

        // A helper class for mapping animation states to colors.
        [System.Serializable]
        public class MaterialStateMapping
        {
            public string StateName;
            public Material Color;
            public int MaterialIdx;

            public MaterialStateMapping(string stateName, Material material)
            {
                StateName = stateName;
                Color = material;
            }
            public MaterialStateMapping(string stateName, Material material, int materialIdx)
            {
                StateName = stateName;
                Color = material;
                MaterialIdx = materialIdx;
            }
        }
    }
}