using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Interaction.ValueRangeInteractable.ValueModifier
{
    /// <summary>
    /// Allows scaling a normalized float value range between a customizable min and max value.
    /// </summary>
    public class FloatCustomRangeModifier : BaseValueRangeModifier<float, float>
    {
        /// <summary>
        /// Minimum value of the range, mapped to 0.0f of the normalized value.
        /// </summary>
        [SerializeField]
        [Tooltip("Minimum value of the range, mapped to 0.0f of the normalized value.")]
        private float _minValue = 0.0f;

        /// <summary>
        /// Maximum value of the range, mapped to 1.0f of the normalized value.
        /// </summary>
        [SerializeField]
        [Tooltip("Maximum value of the range, mapped to 1.0f of the normalized value.")]
        private float _maxValue = 100.0f;

        /// <summary>
        /// Modifies the normalized value from a value range interactable.
        /// </summary>
        /// <param name="value">Normalized value to be modified.</param>
        protected override float GetModifiedValue(float value) => Mathf.Lerp(_minValue, _maxValue, value);
    }
}