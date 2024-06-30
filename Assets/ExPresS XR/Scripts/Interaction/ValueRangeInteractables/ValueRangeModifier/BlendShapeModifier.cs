using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Interaction.ValueRangeInteractable.ValueModifier
{
    /// <summary>
    /// Allows scaling a normalized float value range between a min and max value.
    /// </summary>
    public class BlendShapeModifier : MonoBehaviour
    {
        [SerializeField]
        private int _blendShapeIdx = 0;


        /// <summary>
        /// Event emitted with the modified value.
        /// </summary>
        public UnityEvent<int, float> OnNewValue;

        /// <summary>
        /// Callback for the interactor events like OnValueChanged.
        /// </summary>
        /// <param name="value">Value to be modified and emitted.</param>
        /// <param name="_">Allows callback with two parameters.</param>
        public void EmitModifiedValue(float value, float _ = default) => OnNewValue.Invoke(_blendShapeIdx, value * 100.0f);
    }
}