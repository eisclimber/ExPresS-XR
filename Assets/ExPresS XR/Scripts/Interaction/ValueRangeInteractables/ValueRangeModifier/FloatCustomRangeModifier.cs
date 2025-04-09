using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Interaction.ValueRangeInteractable.ValueModifier
{
    /// <summary>
    /// Allows scaling a normalized float value range between a customizable min and max value.
    /// </summary>
    public class FloatCustomRangeModifier : BaseValueRangeModifier<float, float>
    {
        [SerializeField]
        private float _minValue = 0.0f;

        [SerializeField]
        private float _maxValue = 100.0f;

        /// <summary>
        /// Modifies the normalized value from a value range interactable.
        /// </summary>
        /// <param name="value">Normalized value to be modified.</param>
        protected override float GetModifiedValue(float value) => Mathf.Lerp(value, _minValue, _maxValue);
    }
}