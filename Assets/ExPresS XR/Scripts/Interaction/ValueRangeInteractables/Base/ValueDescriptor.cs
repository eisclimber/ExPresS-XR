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
        /// Accessor defining the default min value.
        /// There can be multiple min values, but this is the one used in the editor for setting the value.
        /// Make sure it evaluates to a valid min value according to <see cref="IsMinValue(V)"/>.
        /// </summary>
        public abstract V DefaultMinValue { get; }

        /// <summary>
        /// Accessor defining the default max value.
        /// There can be multiple max values, but this is the one used in the editor for setting the value.
        /// Make sure it evaluates to a valid max value according to <see cref="IsMaxValue(V)"/>.
        /// </summary>
        public abstract V DefaultMaxValue { get; }

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

            if (IsMinValue(Value))
            {
                OnMinValue.Invoke(Value);
            }
            else if (IsMaxValue(Value))
            {
                OnMaxValue.Invoke(Value);
            }

            if (IsValueSnappingEnabled())
            {
                OnSnapped.Invoke(Value);
            }

            OnValueChanged.Invoke(Value, oldValue);
        }


        /*
         *    Don't serialize these as they are passed through to the interactable's editor!
         */

        /// <summary>
        /// Emitted when the value is changed to a min value.
        /// 
        /// Be careful when using this event without snapping enabled as can get called multiple times while grabbing.
        /// A threshold value modified (i.e. <see cref="Float01ThresholdModifier"/>) might be more appropriate in these cases.
        /// </summary>
        [HideInInspector]
        public UnityEvent<V> OnMinValue;

        /// <summary>
        /// Emitted when the value is changed to a max value.
        /// 
        /// Be careful when using this event without snapping enabled as can get called multiple times while grabbing.
        /// A threshold value modified (i.e. <see cref="Float01ThresholdModifier"/>) might be more appropriate in these cases.
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
        [SerializeField]
        private V _value;
        /// <inheritdoc />
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
        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float _value;
        /// <summary>
        /// Inheriting directly from <see cref="ValueDescriptor"/> and redefining value here to add the Range-attribute.
        /// </summary>
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

        /// <inheritdoc />
        public override float DefaultMinValue => 0.0f;

        /// <inheritdoc />
        public override float DefaultMaxValue => 1.0f;

        [SerializeField]
        [Tooltip("Number of evenly spaced steps to snap the value to. Anything below 1 will deactivate snapping.")]
        private int _numSteps = 0;
        /// <summary>
        /// Number of evenly spaced steps to snap the value to. Anything below 1 will deactivate snapping.
        /// </summary>
        public int NumSteps
        {
            get => _numSteps;
            set => _numSteps = value;
        }

        [SerializeField]
        [Tooltip("If true, no snapping will be performed, even if snaps are configured. This can be used to have a smooth motion and only snap in certain situations like after a grab has been released.")]
        private bool _enforceSnap = true;
        /// <summary>
        /// If true, no snapping will be performed, even if snaps are configured. This can be used to have a smooth motion 
        /// and only snap in certain situations like after a grab has been released.
        /// </summary>
        public bool EnforceSnap
        {
            get => _enforceSnap;
            set => _enforceSnap = value;
        }

        [SerializeField]
        [Tooltip("If the button is pressed or not.")]
        private bool _pressed;
        /// <summary>
        /// If the button is pressed or not.
        /// </summary>
        public bool Pressed
        {
            get => _pressed;
            set => _pressed = value;
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
    /// Represents the internal press value of a button represented by a value between 0.0f (up-position) and 1.0f (down-position).
    /// Sllows customizing the press threshold and deadzone and repress timeouts, whilst also supporting an optional toggle mode.
    /// </summary>
    [Serializable]
    public class ButtonDescriptor : ValueDescriptor<float>
    {
        private bool _pressed;
        /// <summary>
        /// Bool wrapper for checking if the button is pressed (i.e. value = 1.0f) or not.
        /// </summary>
        public bool Pressed
        {
            get => _pressed;
            set
            {
                _pressed = value;
                Value = _pressed ? 1.0f : 0.0f;
            } 
        }

        [SerializeField]
        [ReadonlyInInspector]
        [Range(0.0f, 1.0f)]
        private float _value;
        /// <summary>
        /// Inheriting directly from <see cref="ValueDescriptor"/> and redefining value here to add the Range-attribute.
        /// This property is readonly in the inspector as the value is managed via the "pressed" function.
        /// </summary>
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

        [SerializeField]
        [Tooltip("The threshold beyond which the button is considered pressed.")]
        private float _pressThreshold = 0.65f;
        /// <summary>
        /// The threshold beyond which the button is considered pressed.
        /// </summary>
        public float PressThreshold
        {
            get => _pressThreshold;
            set => _pressThreshold = value;
        }

        [SerializeField]
        [Tooltip("Deadzone around the press threshold in BOTH directions to avoid rapid pressing/releasing.\n"
            + "If the deadzone expands beyond 0.0f or 1.0f, it will be clamped and the press/release events will be fired at exactly 0.0f or 1.0f.")]
        private float _pressDeadzone = 0.1f;
        /// <summary>
        /// Deadzone around the press threshold in BOTH directions to avoid rapid pressing/releasing.
        /// If the deadzone expands beyond 0.0f or 1.0f, it will be clamped and the press/release events will be fired at exactly 0.0f or 1.0f.
        /// </summary>
        public float PressDeadzone
        {
            get => _pressDeadzone;
            set => _pressDeadzone = value;
        }

        [SerializeField]
        [Tooltip("Time after which the button can be re-pressed after being pressed.")]
        private float _repressTimeout = 0.5f;
        /// <summary>
        /// Time after which the button can be re-pressed after being pressed.
        /// </summary>
        public float RepressTimeout
        {
            get => _repressTimeout;
            set => _repressTimeout = value;
        }

        [SerializeField]
        [ReadonlyInInspector ]
        [Tooltip("Internal time of the last press to handle the repress timeout.")]
        private float _lastPressTime = 0.0f;

        /// <inheritdoc />
        public override float DefaultMinValue => 0.0f;

        /// <inheritdoc />
        public override float DefaultMaxValue => 1.0f;



        /// <summary>
        /// Emitted if the button is in the pressed position and the action is considered a press.
        /// </summary>
        [HideInInspector]
        public UnityEvent OnPressed;

        /// <summary>
        /// Emitted if the button is in the pressed position and the action is considered a release.
        /// </summary>
        [HideInInspector]
        public UnityEvent OnReleased;

        /// <inheritdoc />
        protected override float ProcessNewValue(float newValue)
        {
            return Mathf.Clamp01(newValue);
        }

        /// <summary>
        /// Emits value change events based on the current <see cref="Value"/> and <see cref="oldValue"/>.
        /// Used internally to emit the respective events.
        /// </summary>
        /// <param name="oldValue">Previous value.</param>
        protected override void HandleValueChange(float oldValue)
        {
            base.HandleValueChange(oldValue);

            if (oldValue.Equals(Value))
            {
                // Nothing changed or no press allowed, so there is nothing to do...
                return;
            }

            // If we can repress the button (or if it hasn't been pressed yet)
            bool canRepress = (Time.time >= _lastPressTime + _repressTimeout) || _lastPressTime <= 0.0f;
            // Check if we are outside of the deadzone still allow pressing if the the deadzone extends outside the range [0.0f, 1.0f] via the OR-condition
            bool wantsPress = _value > _pressThreshold + _pressDeadzone || _value >= 1.0f;
            bool wantsRelease = _value < _pressThreshold - _pressDeadzone || _value <= 0.0f;

            if (!_pressed && canRepress && wantsPress)
            {
                _pressed = true;
                _lastPressTime = Time.time;
                OnPressed.Invoke();
            }
            else if (_pressed && wantsRelease)
            {
                _pressed = false;
                OnReleased.Invoke();
            }
        }

        /// <inheritdoc />
        public override bool IsMinValue(float value) => value <= 0.0f;

        /// <inheritdoc />
        public override bool IsMaxValue(float value) => value >= 1.0f;

        /// <inheritdoc />
        public override bool IsValueSnappingEnabled() => false; // No snapping for buttons... Toggling is handled elsewhere.
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

        [SerializeField]
        private bool _enforceSnap = true;
        /// <summary>
        /// If true, no snapping will be performed, even if snaps are configured. This can be used to have a smooth motion 
        /// and only snap in certain situations like after a grab has been released.
        /// </summary>
        public bool EnforceSnap
        {
            get => _enforceSnap;
            set => _enforceSnap = value;
        }

        /// <inheritdoc />
        public override Vector2 DefaultMinValue => new();

        /// <inheritdoc />
        public override Vector2 DefaultMaxValue => new(1.0f, 1.0f);

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
        [SerializeField]
        private Vector2 _rawDirection;
        /// <summary>
        /// The raw direction to point too.
        /// This value does not need to be normalized and the coordinates can be both positive and negative.
        /// </summary>
        public Vector2 RawDirection
        {
            get => _rawDirection;
            set
            {
                _rawDirection = value;
                SetValue(RawDirection, RawMagnitude);
            }
        }

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float _rawMagnitude;
        /// <summary>
        /// The actual magnitude of the Value.
        /// </summary>
        public float RawMagnitude
        {
            get => _rawMagnitude;
            set
            {
                _rawMagnitude = value;
                SetValue(RawDirection, RawMagnitude);
            }
        }

        [SerializeField]
        [ReadonlyInInspector]
        private Vector2 _value;
        /// <summary>
        /// Inheriting directly from <see cref="ValueDescriptor"/> and redefining value here to add the Range-attribute.
        /// </summary>
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

        /// <inheritdoc />
        public override Vector2 DefaultMinValue => new();

        /// <inheritdoc />
        public override Vector2 DefaultMaxValue => new(1.0f, 0.0f);

        [SerializeField]
        [Tooltip("Number of evenly spaced steps of the magnitude to snap the value to. Anything below 1 will deactivate snapping.")]
        private int _numSteps = 0;
        /// <summary>
        /// Number of evenly spaced steps of the magnitude to snap the value to. Anything below 1 will deactivate snapping.
        /// </summary>
        public int NumSteps
        {
            get => _numSteps;
            set => _numSteps = value;
        }

        [SerializeField]
        private bool _enforceSnap = true;
        /// <summary>
        /// If true, no snapping will be performed, even if snaps are configured. This can be used to have a smooth motion 
        /// and only snap in certain situations like after a grab has been released.
        /// </summary>
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
        /// <inheritdoc />
        public override Vector3 DefaultMinValue => new();

        /// <inheritdoc />
        public override Vector3 DefaultMaxValue => new(1.0f, 1.0f, 1.0f);

        [SerializeField]
        [Tooltip("Number of evenly spaced steps along the respective axis to snap the value to. Anything below 1 will deactivate snapping.")]
        private Vector3Int _numSteps;
        /// <summary>
        /// Number of evenly spaced steps along the respective axis to snap the value to. Anything below 1 will deactivate snapping.
        /// </summary>
        public Vector3Int NumSteps
        {
            get => _numSteps;
            set => _numSteps = value;
        }

        [SerializeField]
        private bool _enforceSnap = true;
        /// <summary>
        /// If true, no snapping will be performed, even if snaps are configured. This can be used to have a smooth motion 
        /// and only snap in certain situations like after a grab has been released.
        /// </summary>
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
        /// <inheritdoc />
        public override Vector3 DefaultMinValue => new();

        /// <inheritdoc />
        public override Vector3 DefaultMaxValue => new(1.0f, 0.0f, 0.0f);

        [SerializeField]
        [Tooltip("Number of evenly spaced steps of the magnitude to snap the value to. Anything below 1 will deactivate snapping.")]
        private int _numSteps = 0;
        /// <summary>
        /// Number of evenly spaced steps of the magnitude to snap the value to. Anything below 1 will deactivate snapping.
        /// </summary>
        public int NumSteps
        {
            get => _numSteps;
            set => _numSteps = value;
        }

        [SerializeField]
        private bool _enforceSnap = true;
        /// <summary>
        /// If true, no snapping will be performed, even if snaps are configured. This can be used to have a smooth motion 
        /// and only snap in certain situations like after a grab has been released.
        /// </summary>
        public bool EnforceSnap
        {
            get => _enforceSnap;
            set => _enforceSnap = value;
        }

        /// <inheritdoc />
        protected override Vector3 ProcessNewValue(Vector3 newValue)
        {
            Vector3 newDir = newValue.normalized;
            float newMagnitude = EnforceSnap && _numSteps > 0
                        ? RuntimeUtils.GetValue01Stepped(newValue.magnitude, _numSteps)
                        : Mathf.Clamp01(newValue.magnitude);
            return newDir * newMagnitude;
        }

        /// <inheritdoc />
        public override bool IsMinValue(Vector3 value) => value.magnitude <= 0.0f;

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
        public override Vector3 DefaultMinValue => new(-1.0f, 0.0f, 0.0f);

        /// <inheritdoc />
        public override Vector3 DefaultMaxValue => new(1.0f, 0.0f, 0.0f);

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