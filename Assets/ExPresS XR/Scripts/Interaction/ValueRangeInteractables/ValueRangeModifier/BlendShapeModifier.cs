using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Interaction.ValueRangeInteractable.ValueModifier
{
    /// <summary>
    /// Scales a normalized value on the range [0.0f - 100.0f] to be used as a blend shape.
    /// </summary>
    public class BlendShapeModifier : BaseValueRangeModifier<float, float>
    {
        protected const float BLEND_SHAPE_MIN_VALUE = 0.0f;
        protected const float BLEND_SHAPE_MAX_VALUE = 100.0f;

        [SerializeField]
        private int _blendShapeIdx = 0;

        [SerializeField]
        private bool _oneMinus = false;


        /// <summary>
        /// Event emitted with the modified value.
        /// </summary>
        public UnityEvent<int, float> OnNewBlendShapeValue;

        /// <summary>
        /// Modifies the normalized value from a value range interactable.
        /// </summary>
        /// <param name="value">Normalized value to be modified.</param>
        protected override float GetModifiedValue(float value) => (_oneMinus ? 1 - value : value) * BLEND_SHAPE_MAX_VALUE;


        /// <summary>
        /// Callback for the interactor events like OnValueChanged.
        /// </summary>
        /// <param name="value">Value to be modified and emitted.</param>
        /// <param name="_">Allows callback with two parameters.</param>
        public override void EmitModifiedValue(float value, float _ = default)
        {
            base.EmitModifiedValue(value);
            OnNewBlendShapeValue.Invoke(_blendShapeIdx, GetModifiedValue(value));
        }
    }
}