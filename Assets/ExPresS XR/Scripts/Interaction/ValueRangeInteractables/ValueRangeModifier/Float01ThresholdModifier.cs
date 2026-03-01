using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Interaction.ValueRangeInteractable.ValueModifier
{
    /// <summary>
    /// Allows checking if a normalized float is above/below a threshold
    /// </summary>
    public class Float01ThresholdModifier : BaseValueRangeModifier<float, bool>
    {
        /// <summary>
        /// Threshold for the normalized value. If the value is above/below this threshold, the modifier will emit true, otherwise false.
        /// </summary>
        [SerializeField]
        [Range(0.0f, 1.0f)]
        [Tooltip("Threshold for the normalized value. If the value is above/below this threshold, the modifier will emit true, otherwise false.")]
        private float _threshold = 0.95f;

        /// <summary>
        /// If enabled will inverse the range, meaning '>= 0.95f' will become a '<= 0.05f'.
        /// </summary>
        [SerializeField]
        [Tooltip("If enabled will inverse the range, meaning '>= 0.95f' will become a '<= 0.05f'.")]
        private bool _oneMinus = false;

        /// <summary>
        /// Emitted when the value is above the threshold.
        /// </summary>
        public UnityEvent OnAboveThreshold;

        /// <summary>
        /// Emitted when the value is below the threshold.
        /// </summary>
        public UnityEvent OnBelowThreshold;


        /// <summary>
        /// Modifies the normalized value from a value range interactable.
        /// </summary>
        /// <param name="value">Normalized value to be modified.</param>
        protected override bool GetModifiedValue(float value) => _oneMinus ?  value <= 1.0f - _threshold : value >= _threshold;

        /// <summary>
        /// Callback for the interactor events like OnValueChanged.
        /// </summary>
        /// <param name="value">Value to be modified and emitted.</param>
        /// <param name="_">Allows callback with two parameters.</param>
        public override void EmitModifiedValue(float value, float _ = default)
        {
            base.EmitModifiedValue(value);
            bool modifiedValue = GetModifiedValue(value);
            (modifiedValue ? OnAboveThreshold : OnBelowThreshold).Invoke();
        }
    }
}