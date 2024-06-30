using System;
using ExPresSXR.Misc;
using UnityEngine;
using UnityEngine.Events;


namespace ExPresSXR.Interaction.ValueRangeInteractable
{
    /// <summary>
    /// This class is the base implementation of a value range to define the behavior of interpolating a value.
    /// This is currently used to hold the information needed for ExPresS XR#s ValueRangeInteractables, such as Levers, sliders, ...
    /// 
    /// You can of course create your own range by inheriting from <see cref="BaseValueDescriptor"/> (NOT <see cref="ValueDescriptor"/>!).
    /// You'll need to implement the `ProcessNewValue()` function to handle and return new values as you like.
    /// To define the bounds of your range also implement `IsMinValue()` and `IsMinValue()` to properly trigger the respective events.
    /// Keep in mind, that a range may have multiple min/max values.
    /// Make sure to add a `[System.Serializable]` attribute to your ValueDescriptor-class.
    /// 
    /// In case you want or to add attributes to your serialized value-field (like <see cref="Float01Range"/>)
    /// you'll need to  inherit from <see cref="ValueDescriptor"/>. If you to that, you will need to implement the behavior
    ///  of the `Value` property yourself, setting your value to the return value of `ProcessNewValue()` and calling `HandleValueChange()`
    ///  with the **old** value. It is usually sufficient to call the 
    /// </summary>
    /// <typeparam name="V">Class used to interpolate between.</typeparam>
    [Serializable]
    public abstract class ValueDescriptor<V>
    {
        /// <summary>
        /// Property for getting and setting the value. If you do *not* need to add custom attributes (like [Range(..)]) to your value field,
        /// you can use <see cref="BaseValueDescriptor"/>, which has the property already set up.
        /// Otherwise you'll need to implement it yourself, making sure to call the same functions as in <see cref="BaseValueDescriptor"/>,
        /// to ensure a correct behavior.
        /// </summary>
        public abstract V Value { get; set; }

        /// <summary>
        /// Use this function to handle new value (e.g. clamping or snapping) your value and setting it to the value property: 'Value = ...'
        /// </summary>
        /// <param name="newValue">new value trying to be set to the </param>
        protected abstract V ProcessNewValue(V newValue);


        /// <summary>
        /// Checks if the current value is considered minimal.
        /// </summary>
        /// <returns>If the current value is min.</returns>
        public bool IsMinValue() => IsMinValue(Value);

        /// <summary>
        /// Checks if the provided value is considered minimal.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>If the current value is min.</returns>
        public abstract bool IsMinValue(V value);


        /// <summary>
        /// Checks if the current value is considered minimal.
        /// </summary>
        /// <returns>If the current value is min.</returns>
        public bool IsMaxValue() => IsMaxValue(Value);

        /// <summary>
        /// Checks if the provided value is considered maximal.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>If the current value is max.</returns>
        public abstract bool IsMaxValue(V value);


        /// <summary>
        /// Resets the value to it's default.
        /// </summary>
        public virtual void ResetValue() => Value = default;

        /// <summary>
        /// Allows checking if snapping is enabled and is used internally to emit the correct events.
        /// This function must be implemented as snapping must be implemented individually. If no snapping will be performed, simply return false.
        /// </summary>
        /// <returns>If snapping is enabled.</returns>
        public abstract bool IsValueSnappingEnabled();


        /// <summary>
        /// Emits value change events based on the current <see cref="Value"/> and <see cref="oldValue"/>.
        /// Used internally to emit the respective events.
        /// </summary>
        /// <param name="oldValue">Previous value.</param>
        protected virtual void HandleValueChange(V oldValue)
        {
            if (oldValue.Equals(Value))
            {
                // Nothing changed, so there is nothing to do...
                return;
            }

            if (IsValueSnappingEnabled())
            {
                if (IsMinValue(Value))
                {
                    OnMinValue.Invoke(Value);
                }
                else if (IsMaxValue(Value))
                {
                    OnMaxValue.Invoke(Value);
                }

                OnSnapped.Invoke(Value);
            }

            OnValueChanged.Invoke(Value, oldValue);
        }


        /*
         *    Don't serialize these as they are passed through to the interactable's editor!
         */

        /// <summary>
        /// Emitted when a min value is set.
        /// </summary>
        [HideInInspector]
        public UnityEvent<V> OnMinValue;

        /// <summary>
        /// Emitted when a max value is set.
        /// </summary>
        [HideInInspector]
        public UnityEvent<V> OnMaxValue;

        /// <summary>
        /// Emitted when a value is snapped.
        /// </summary>
        [HideInInspector]
        public UnityEvent<V> OnSnapped;

        /// <summary>
        /// Emitted when a value changed. Provides the new and old value respectively.
        /// </summary>
        [HideInInspector]
        public UnityEvent<V, V> OnValueChanged;
    }



    /// <inheritdoc />
    [Serializable]
    public abstract class BaseValueDescriptor<V> : ValueDescriptor<V>
    {
        /// <inheritdoc />
        [SerializeField]
        private V _value;
        public override V Value
        {
            get => _value;
            set
            {
                V oldValue = _value;
                _value = ProcessNewValue(value);
                HandleValueChange(oldValue);
            }
        }
    }


    /// <summary>
    /// A range to interpolate a float between 0.0f and 1.0f (both inclusive), whilst supporting snapping.
    /// </summary>
    [Serializable]
    public class Float01Descriptor : ValueDescriptor<float>
    {
        /// <summary>
        /// Inheriting directly from <see cref="ValueDescriptor"/> and redefining value here to add the Range-attribute.
        /// </summary>
        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float _value;
        public override float Value
        {
            get => _value;
            set
            {
                float oldValue = _value;
                _value = ProcessNewValue(value);
                HandleValueChange(oldValue);
            }
        }

        /// <summary>
        /// Number of evenly spaced steps to snap the value to. Anything below 1 will deactivate snapping.
        /// </summary>
        [SerializeField]
        [Tooltip("Number of evenly spaced steps to snap the value to. Anything below 1 will deactivate snapping.")]
        protected int _numSteps = 0;

        /// <summary>
        /// If true, no snapping will be performed, even if snaps are configured. This can be used to have a smooth motion 
        /// and only snap in certain situations like after a grab has been released.
        /// </summary>
        [SerializeField]
        [Tooltip("If true, no snapping will be performed, even if snaps are configured. This can be used to have a smooth motion and only snap in certain situations like after a grab has been released.")]
        private bool _enforceSnap = true;
        public bool EnforceSnap
        {
            get => _enforceSnap;
            set => _enforceSnap = value;
        }

        /// <inheritdoc />
        protected override float ProcessNewValue(float newValue)
        {
            // No snapping (possible/needed) => Return value clamped
            return EnforceSnap && _numSteps > 0
                    ? RuntimeUtils.GetValue01Stepped(newValue, _numSteps)
                    : Mathf.Clamp01(newValue);
        }

        /// <inheritdoc />
        public override bool IsMinValue(float value) => value <= 0.0f;

        /// <inheritdoc />
        public override bool IsMaxValue(float value) => value >= 1.0f;

        /// <inheritdoc />
        public override bool IsValueSnappingEnabled() => _numSteps > 0;
    }


    /// <summary>
    /// A range to interpolate each individually coordinate a Vector2 between 0.0f and 1.0f (both inclusive), whilst supporting snapping.
    /// </summary>
    [Serializable]
    public class Vector2Descriptor : BaseValueDescriptor<Vector2>
    {
        /// <summary>
        /// Number of evenly spaced steps along the respective axis to snap the value to. Anything below 1 will deactivate snapping.
        /// </summary>
        [SerializeField]
        [Tooltip("Number of evenly spaced steps along the respective axis to snap the value to. Anything below 1 will deactivate snapping.")]
        protected Vector2Int _numSteps;

        /// <summary>
        /// If true, no snapping will be performed, even if snaps are configured. This can be used to have a smooth motion 
        /// and only snap in certain situations like after a grab has been released.
        /// </summary>
        [SerializeField]
        private bool _enforceSnap = true;
        public bool EnforceSnap
        {
            get => _enforceSnap;
            set => _enforceSnap = value;
        }

        /// <inheritdoc />
        protected override Vector2 ProcessNewValue(Vector2 newValue)
        {
            // No snapping (possible/needed) => Return value clamped
            float x = EnforceSnap && _numSteps.x > 0
                        ? RuntimeUtils.GetValue01Stepped(newValue.x, _numSteps.x)
                        : Mathf.Clamp01(newValue.x);
            float y = EnforceSnap && _numSteps.y > 0
                        ? RuntimeUtils.GetValue01Stepped(newValue.y, _numSteps.y)
                            : Mathf.Clamp01(newValue.y);

            return new(x, y);
        }

        /// <inheritdoc />
        public override bool IsMinValue(Vector2 value) => value.x <= 0.0f || value.y <= 0.0f;

        /// <inheritdoc />
        public override bool IsMaxValue(Vector2 value) => value.x >= 1.0f || value.x >= 1.0f;

        /// <inheritdoc />
        public override bool IsValueSnappingEnabled() => _numSteps.sqrMagnitude > 0.0f; // Use sqrMagnitude as it is faster
    }


    /// <summary>
    /// A range to interpolate the magnitude of a Vector2 between -1.0f and 1.0f (both inclusive), whilst supporting snapping.
    /// </summary>
    [Serializable]
    public class CircularDescriptor : ValueDescriptor<Vector2>
    {
        /// <summary>
        /// The raw direction to point too.
        /// This value does not need to be normalized and the coordinates can be both positive and negative.
        /// </summary>
        [SerializeField]
        private Vector2 _rawDirection;
        public Vector2 RawDirection
        {
            get => _rawDirection;
            set
            {
                _rawDirection = value;
                SetValue(RawDirection, RawMagnitude);
            }
        }

        /// <summary>
        /// The actual magnitude of the Value.
        /// </summary>
        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float _rawMagnitude;
        public float RawMagnitude
        {
            get => _rawMagnitude;
            set
            {
                _rawMagnitude = value;
                SetValue(RawDirection, RawMagnitude);
            }
        }

        /// <summary>
        /// Inheriting directly from <see cref="ValueDescriptor"/> and redefining value here to add the Range-attribute.
        /// </summary>
        [SerializeField]
        [ReadonlyInInspector]
        private Vector2 _value;
        public override Vector2 Value
        {
            get => _value;
            set
            {
                Vector2 oldValue = _value;
                _value = ProcessNewValue(value);
                _rawDirection = _value.normalized;
                _rawMagnitude = _value.magnitude;
                HandleValueChange(oldValue);
            }
        }

        /// <summary>
        /// Number of evenly spaced steps of the magnitude to snap the value to. Anything below 1 will deactivate snapping.
        /// </summary>
        [SerializeField]
        [Tooltip("Number of evenly spaced steps of the magnitude to snap the value to. Anything below 1 will deactivate snapping.")]
        protected int _numSteps = 0;

        /// <summary>
        /// If true, no snapping will be performed, even if snaps are configured. This can be used to have a smooth motion 
        /// and only snap in certain situations like after a grab has been released.
        /// </summary>
        [SerializeField]
        private bool _enforceSnap = true;
        public bool EnforceSnap
        {
            get => _enforceSnap;
            set => _enforceSnap = value;
        }

        /// <inheritdoc />
        protected override Vector2 ProcessNewValue(Vector2 newValue)
        {
            Vector2 newDir = newValue.normalized;
            float newMagnitude = EnforceSnap && _numSteps > 0
                                ? RuntimeUtils.GetValue01Stepped(newValue.magnitude, _numSteps)
                                : Mathf.Clamp01(newValue.magnitude);
            return newDir * newMagnitude;
        }

        /// <inheritdoc />
        public override bool IsMinValue(Vector2 value) => value.magnitude <= 0.0f;

        /// <inheritdoc />
        public override bool IsMaxValue(Vector2 value) => value.magnitude >= 1.0f;

        /// <inheritdoc />
        public override bool IsValueSnappingEnabled() => _numSteps > 0;

        /// <summary>
        /// Sets the value based on the direction and magnitude provided.
        /// </summary>
        /// <param name="direction">Direction to point too.</param>
        /// <param name="magnitude">Magnitude of the value.</param>
        public virtual void SetValue(Vector2 direction, float magnitude)
        {
            Vector2 oldValue = _value;
            Vector2 newValue = ProcessNewValue(direction.normalized * magnitude);
            _value = ProcessNewValue(newValue);
            HandleValueChange(oldValue);
        }
    }

    /// <summary>
    /// A range to interpolate each individually coordinate a Vector3 between 0.0f and 1.0f (both inclusive), whilst supporting snapping.
    /// </summary>
    [Serializable]
    public class Vector3Descriptor : BaseValueDescriptor<Vector3>
    {
        /// <summary>
        /// Number of evenly spaced steps along the respective axis to snap the value to. Anything below 1 will deactivate snapping.
        /// </summary>
        [SerializeField]
        [Tooltip("Number of evenly spaced steps along the respective axis to snap the value to. Anything below 1 will deactivate snapping.")]
        protected Vector3Int _numSteps;

        /// <summary>
        /// If true, no snapping will be performed, even if snaps are configured. This can be used to have a smooth motion 
        /// and only snap in certain situations like after a grab has been released.
        /// </summary>
        [SerializeField]
        private bool _enforceSnap = true;
        public bool EnforceSnap
        {
            get => _enforceSnap;
            set => _enforceSnap = value;
        }

        /// <inheritdoc />
        protected override Vector3 ProcessNewValue(Vector3 newValue)
        {
            float x = EnforceSnap && _numSteps.x > 0
                        ? RuntimeUtils.GetValue01Stepped(newValue.x, _numSteps.x)
                        : Mathf.Clamp01(newValue.x);
            float y = EnforceSnap && _numSteps.y > 0
                        ? RuntimeUtils.GetValue01Stepped(newValue.y, _numSteps.y)
                            : Mathf.Clamp01(newValue.y);
            float z = EnforceSnap && _numSteps.z > 0
                        ? RuntimeUtils.GetValue01Stepped(newValue.z, _numSteps.z)
                        : Mathf.Clamp01(newValue.z);

            return new(x, y, z);
        }

        /// <inheritdoc />
        public override bool IsMinValue(Vector3 value) => value.x <= 0.0f || value.y <= 0.0f || value.z <= 0.0f;

        /// <inheritdoc />
        public override bool IsMaxValue(Vector3 value) => value.x >= 1.0f || value.x >= 1.0f || value.z >= 1.0f;

        /// <inheritdoc />
        public override bool IsValueSnappingEnabled() => _numSteps.sqrMagnitude > 0.0f; // Use sqrMagnitude as it is faster
    }


    /// <summary>
    /// A range to interpolate the magnitude of a Vector3 between 0.0f and 1.0f (both inclusive), whilst supporting snapping.
    /// </summary>
    [Serializable]
    public class SphereDescriptor : BaseValueDescriptor<Vector3>
    {
        /// <summary>
        /// Number of evenly spaced steps of the magnitude to snap the value to. Anything below 1 will deactivate snapping.
        /// </summary>
        [SerializeField]
        [Tooltip("Number of evenly spaced steps of the magnitude to snap the value to. Anything below 1 will deactivate snapping.")]
        protected int _numSteps = 0;

        /// <summary>
        /// If true, no snapping will be performed, even if snaps are configured. This can be used to have a smooth motion 
        /// and only snap in certain situations like after a grab has been released.
        /// </summary>
        [SerializeField]
        private bool _enforceSnap = true;
        public bool EnforceSnap
        {
            get => _enforceSnap;
            set => _enforceSnap = value;
        }

        /// <inheritdoc />
        protected override Vector3 ProcessNewValue(Vector3 newValue)
        {
            Debug.Log(newValue + " x " + newValue.magnitude);
            Vector3 newDir = newValue.normalized;
            float newMagnitude = EnforceSnap && _numSteps > 0
                        ? RuntimeUtils.GetValue01Stepped(newValue.magnitude, _numSteps)
                        : Mathf.Clamp01(newValue.magnitude);
            return newDir * newMagnitude;
        }

        /// <inheritdoc />
        public override bool IsMinValue(Vector3 value) => value.magnitude <= -1.0f;

        /// <inheritdoc />
        public override bool IsMaxValue(Vector3 value) => value.magnitude >= 1.0f;

        /// <inheritdoc />
        public override bool IsValueSnappingEnabled() => _numSteps > 0;
    }

    /// <summary>
    /// A range to interpolate a direction Vector3 having a magnitude of 1.0f, whilst supporting snapping.
    /// </summary>
    [Serializable]
    public class DirectionDescriptor : BaseValueDescriptor<Vector3>
    {
        /// <inheritdoc />
        protected override Vector3 ProcessNewValue(Vector3 newValue) => newValue.normalized;

        /// <inheritdoc />
        public override bool IsMinValue(Vector3 value) => value.x <= -1.0f || value.y <= -1.0f || value.z <= -1.0f;

        /// <inheritdoc />
        public override bool IsMaxValue(Vector3 value) => value.x >= 1.0f || value.x >= 1.0f || value.z >= 1.0f;

        /// <inheritdoc />
        public override bool IsValueSnappingEnabled() => false; // No snapping!
    }
}