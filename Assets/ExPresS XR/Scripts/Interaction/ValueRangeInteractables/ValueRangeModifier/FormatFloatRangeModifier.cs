using UnityEngine;

namespace ExPresSXR.Interaction.ValueRangeInteractable.ValueModifier
{
    /// <summary>
    /// Allows converting a normalized float to a string whilst passing it to a format string.
    /// </summary>
    public class FormatFloatRangeModifier : BaseValueRangeModifier<float, string>
    {
        /// <summary>
        /// Format string applied to the value.
        /// See: https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings
        /// </summary>
        [SerializeField]
        [Tooltip("Format string applied to the value. See: https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings")]
        private string floatFormat = "F2";


        private void Awake()
        {
            GetModifiedValue(.6f);
        }

        /// <inheritdoc />
        protected override string GetModifiedValue(float value) => value.ToString(floatFormat);
    }
}