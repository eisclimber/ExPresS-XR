using ExPresSXR.Misc;
using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.TargetArea
{
    public class BreakableModel : MonoBehaviour
    {
        /// <summary>
        /// Assumed default variable name of the damage shader. 
        /// </summary>
        private const string DEFAULT_SHADER_VARIABLE_NAME = "_DamagePct";

        /// <summary>
        /// Maximum possible damage. Make sure that the value can be reached with the your setup of TargetAreas.
        /// </summary>
        [SerializeField]
        [Tooltip("Maximum possible damage.")]
        private int _maxDamage = 3;
        public int MaxDamage
        {
            get => _maxDamage;
            set => _maxDamage = value;
        }

        [ReadonlyInInspector]
        [SerializeField]
        [Tooltip("Current damage.")]
        private int _currentDamage;
        public int CurrentDamage
        {
            get => _currentDamage;
            set
            {
                _currentDamage = Mathf.Clamp(value, 0, _maxDamage);
                bool firstDamage = _currentDamage == 1;
                bool lastDamage = _currentDamage == _maxDamage;

                UpdateVisuals();

                // Emit damage events
                if (firstDamage)
                {
                    OnFirstDamage.Invoke();
                }
                else if (lastDamage)
                {
                    OnLastDamage.Invoke();
                }
                OnDamage.Invoke(_currentDamage);

                if (lastDamage && _deactivateOnMaxDamage)
                {
                    // Disable the object after emitting events to avoid them getting lost
                    gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Automatically deactivates the GameObject upon reaching max damage.
        /// </summary>
        [SerializeField]
        [Tooltip("Automatically deactivates the GameObject upon reaching max damage.")]
        private bool _deactivateOnMaxDamage = true;

        [Space]

        /// <summary>
        /// Models displayed for different stage of damaging (including fully in tact). Model damaging will be ignored if empty.
        /// </summary>
        [SerializeField]
        [Tooltip("Models displayed for different stage of damaging (including fully in tact). Model damaging will be ignored if empty.")]
        private GameObject[] _damageModels;

        [Space]

        /// <summary>
        /// Renderer providing the damage material. If none is set, texture damaging will be ignored.
        /// </summary>
        [SerializeField]
        [Tooltip("Renderer providing the damage material. If none is set, texture damaging will be ignored.")]
        private Renderer _damageRenderer;

        /// <summary>
        /// Variable name of the shader to be manipulated.
        /// </summary>
        [SerializeField]
        [Tooltip("Variable name of the shader to be manipulated.")]
        private string _shaderVariableName = DEFAULT_SHADER_VARIABLE_NAME;

        /// <summary>
        /// Values to be shown per damage. If empty or values are missing, the values will be linear spaced on an interval between zero and one.
        /// If the last value is greater than one, all following values will be set to this value instead.
        /// </summary>
        [SerializeField]
        [Tooltip("Values to be shown per damage. If empty or values are missing, the values will be linear spaced on an interval between zero and one. "
            + "If the last value is greater than one, all following values will be set to this value instead.")]
        private float[] _shaderVariableValues;

        // Events

        /// <summary>
        /// Emitted when damaged the first time.
        /// </summary>
        public UnityEvent OnFirstDamage;

        /// <summary>
        /// Emitted when damaged the last time (i.e. destroyed).
        /// </summary>
        public UnityEvent OnLastDamage;

        /// <summary>
        /// Emitted with the current damage when damaged.
        /// </summary>
        public UnityEvent<int> OnDamage;

        /// <summary>
        /// Increases the damage by one.
        /// </summary>
        [ContextMenu("Increase Damage")]
        public void IncreaseDamage() => CurrentDamage++;

        /// <summary>
        /// Decreases the damage by one.
        /// </summary>
        [ContextMenu("Decrease Damage")]
        public void DecreaseDamage() => CurrentDamage--;

        /// <summary>
        /// Disables the visuals of all models after a given period of time.
        /// </summary>
        public void DisableModelVisualsDelayed(float delay) => Invoke(nameof(DisableVisuals), delay);

        /// <summary>
        /// Disables the visuals of all models.
        /// </summary>
        public void DisableVisuals()
        {
            foreach (GameObject damageModel in _damageModels)
            {
                damageModel.SetActive(false);
            }
        }

        private void UpdateVisuals()
        {
            UpdateShaderVisuals();
            UpdateModelVisuals();
        }

        private void UpdateModelVisuals()
        {
            for (int i = 0; i < _damageModels.Length; i++)
            {
                if (_damageModels[i] != null)
                {
                    _damageModels[i].SetActive(i == _currentDamage);
                }
            }
        }

        private void UpdateShaderVisuals()
        {
            if (_damageRenderer != null)
            {
                _damageRenderer.material.SetFloat(_shaderVariableName, GetShaderValueForDamage());
            }
        }

        private float GetShaderValueForDamage()
        {
            float linearValue = _maxDamage != 0 ? _currentDamage / (float)_maxDamage : 0.0f;
            if (_shaderVariableValues.Length <= 0)
            {
                // No custom values provided -> return linear scaled value
                return linearValue;
            }
            else if (_currentDamage >= 0 && _currentDamage < _shaderVariableValues.Length)
            {
                // Value provided -> return it
                return _shaderVariableValues[_currentDamage];
            }
            // Values provided but no all -> return 
            float maxValue = Mathf.Max(_shaderVariableValues);
            bool isNormalized = maxValue >= 0.0f && maxValue <= 1.0f;
            return isNormalized ? linearValue : _shaderVariableValues[^1];
        }
    }
}