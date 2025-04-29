using UnityEngine;

namespace ExPresSXR.Interaction.ValueRangeInteractable.ValueModifier
{
    /// <summary>
    /// Allows scaling a normalized float value range between a min and max value.
    /// </summary>
    public class PingPongBlendShapeModifier : BlendShapeModifier
    {
        /// <summary>
        /// How many times the value changes direction.
        /// </summary>
        [SerializeField]
        [Tooltip("How many times the value changes direction.")]
        private float _numPings = 2.0f;

        /// <summary>
        /// Modifies the normalized value from a value range interactable.
        /// </summary>
        /// <param name="value">Normalized value to be modified.</param>
        protected override float GetModifiedValue(float value)
        {
            float baseValue = base.GetModifiedValue(value) * _numPings;
            int quotient = (int) (baseValue / BLEND_SHAPE_MAX_VALUE);
            float remainder = (int) (baseValue % BLEND_SHAPE_MAX_VALUE);
            float directedValue = (quotient % 2 == 0) ? remainder : BLEND_SHAPE_MAX_VALUE - remainder;
            return directedValue;
        }
    }
}