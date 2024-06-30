using ExPresSXR.Misc;
using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Interaction.ValueRangeInteractable.ValueModifier
{
    /// <summary>
    /// Implements the basic behavior of modifying a ValueRange and emitting a new event with this value.
    /// All you need to do is to implement the `GetModifiedValue()` function and connect the event.
    /// </summary>
    /// <typeparam name="U">Original value type of the ValueRangeInteractable.</typeparam>
    /// <typeparam name="V">Type to be returned.</typeparam>
    public abstract class BaseValueRangeModifier<U, V> : MonoBehaviour
    {
        /// <summary>
        /// Event emitted with the modified value.
        /// </summary>
        public UnityEvent<V> OnNewValue;

        /// <summary>
        /// Callback for the interactor events like OnValueChanged.
        /// </summary>
        /// <param name="value">Value to be modified and emitted.</param>
        /// <param name="_">Allows callback with two parameters.</param>
        public void EmitModifiedValue(U value, U _ = default) => OnNewValue.Invoke(GetModifiedValue(value));

        /// <summary>
        /// Modifies the normalized value from a value range interactable.
        /// </summary>
        /// <param name="value">Normalized value to be modified.</param>
        protected abstract V GetModifiedValue(U value);
    }
}