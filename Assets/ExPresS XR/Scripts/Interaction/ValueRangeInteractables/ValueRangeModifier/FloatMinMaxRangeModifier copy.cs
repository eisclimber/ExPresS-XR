using UnityEngine;

namespace ExPresSXR.Interaction.ValueRangeInteractable.ValueModifier
{
    /// <summary>
    /// Allows scaling a normalized float value range between a min and max value.
    /// </summary>
    public class FloatMinMaxRangeModifier : BaseValueRangeModifier<float, float>
    {
        /// <summary>
        /// Min value of the scaled range.
        /// </summary>
        [SerializeField]
        [Tooltip("Min value of the scaled range.")]
        private float _minValue = 0.0f;

        /// <summary>
        /// Max value of the scaled range.
        /// </summary>
        [SerializeField]
        [Tooltip("Max value of the scaled range.")]
        private float _maxValue = 100.0f;

        /// <summary>
        /// Getter for the size of the range.
        /// </summary>
        public float RangeSize { get => _maxValue - _minValue; }


        /// <inheritdoc />
        protected override float GetModifiedValue(float value)
        {
            float unclampedValue = _minValue + value * RangeSize;
            return Mathf.Clamp(unclampedValue, _minValue, _maxValue);
        }
    }
}