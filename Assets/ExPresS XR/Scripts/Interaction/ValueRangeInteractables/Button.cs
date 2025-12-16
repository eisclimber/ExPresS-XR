using System;
using ExPresSXR.Misc;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ExPresSXR.Interaction.ValueRangeInteractable
{
    /// <summary>
    /// Defines a button interactable.
    /// </summary>
    public class Button : ValueRangeInteractable<ButtonDescriptor, ButtonVisualizer, float>
    {
        /// <summary>
        /// If the button press is pressed normally or should toggle.
        /// </summary>
        [SerializeField]
        [Tooltip("If the button press is pressed normally or should toggle.")]
        private bool _toggleMode = false;
        public bool ToggleMode
        {
            get => _toggleMode;
            set
            {
                _toggleMode = value;
                OnToggleModeChanged.Invoke(_toggleMode);
            }
        }

        /// <summary>
        /// Current pressed state.
        /// </summary>
        [SerializeField]
        private bool _pressed;
        public bool Pressed
        {
            get => _pressed;
            set
            {
                _pressed = value;
                // Update the value in the descriptor to keep them in sync -> Value gets updated automatically
                ValueDescriptor.Pressed = _pressed;
            }
        }

        /// <summary>
        /// Accessor for the current value of the ValueDescriptor.
        /// Overwritten whether 
        /// </summary>
        /// <value>Value of the range.</value>
        [Tooltip("Accessor for the current value of the ValueDescriptor.")]
        public override float Value
        {
            get => ValueDescriptor.Value;
            set
            {
                ValueDescriptor.Value = value;
                UpdateValueVisualization();
                EmitOnValueChanged(Value, value);
            }
        }

        /// <summary>
        /// Max distance to to an interactor to be able to interact with the button.
        /// This is a hack for being able to determine if the button is hovered near or far.
        /// </summary>
        [SerializeField]
        [Tooltip("Max distance to to an interactor to be able to interact with the button. This is a hack for being able to determine if the button is hovered near or far.")]
        private float _maxInteractionDistance = 0.1f;
        public float MaxInteractionDistance
        {
            get => _maxInteractionDistance;
            set => _maxInteractionDistance = value;
        }

        /// <summary>
        /// Sound played when the button is pressed.
        /// </summary>
        [SerializeField]
        [Tooltip("Sound played when the button is pressed.")]
        protected AudioClip _pressedSound;

        /// <summary>
        /// Sound played when the button is toggled down.
        /// </summary>
        [SerializeField]
        [Tooltip("Sound played when the button is toggled down.")]
        protected AudioClip _releasedSound;

        /// <summary>
        /// Sound played when the button is toggled down.
        /// </summary>
        [SerializeField]
        [Tooltip("Sound played when the button is toggled down.")]
        protected AudioClip _toggledDownSound;

        /// <summary>
        /// Sound played when the button is toggled up.
        /// </summary>
        [SerializeField]
        [Tooltip("Sound played when the button is toggled up.")]
        protected AudioClip _toggledUpSound;

        /// <summary>
        /// Emitted when the button is pressed not in toggle mode.
        /// </summary>
        public UnityEvent OnPressed;

        /// <summary>
        /// Emitted when the button is released not in toggle mode.
        /// </summary>
        public UnityEvent OnReleased;


        /// <summary>
        /// Emitted when the button is pressed and toggled to the down position.
        /// </summary>
        public UnityEvent OnTogglePressed;

        /// <summary>
        /// Emitted when the button is released and toggled to the up position.
        /// </summary>
        public UnityEvent OnToggleReleased;


        /// <summary>
        /// Emitted when the toggle mode of the button has changed.
        /// </summary>
        public UnityEvent<bool> OnToggleModeChanged;

        // Helper value to allow repressing 
        private bool _canRepressToggle = true;


        protected override void OnEnable()
        {
            base.OnEnable();
            _canRepressToggle = true;
        }

        protected override void StartHover(HoverEnterEventArgs args)
        {
            base.StartHover(args);
            _valueVisualizer.RecordHoverStartHeight(args.interactableObject, args.interactorObject);
        }

        /// <summary>
        /// Function wrapper that wraps the function calls for the different toggle modes.
        /// </summary>
        protected virtual void UpdateValueVisualization()
        {
            if (_toggleMode)
            {
                _valueVisualizer.UpdateVisualizationWithToggle(Value, _pressed, this);
            }
            else
            {
                _valueVisualizer.UpdateVisualization(Value, this);
            }
        }


        protected override void UpdateValueWithHover() => Value = _valueVisualizer.GetVisualizedValue(this, _hoverInteractor);

        /// <summary>
        /// Disable grabbing the button (Don't update value when grabbed)
        /// </summary>
        protected override void UpdateValueWithGrab() { }

        protected override void AddValueDescriptorListeners()
        {
            base.AddValueDescriptorListeners();
            _valueDescriptor.OnPressed.AddListener(HandleButtonPressed);
            _valueDescriptor.OnReleased.AddListener(HandleButtonReleased);
        }

        protected override void RemoveValueDescriptorListeners()
        {
            base.RemoveValueDescriptorListeners();
            _valueDescriptor.OnPressed.RemoveListener(HandleButtonPressed);
            _valueDescriptor.OnReleased.RemoveListener(HandleButtonReleased);
        }

        /// <summary>
        /// Determines if the interactable can be selected by an IXRSelectInteractor.
        /// </summary>
        /// <param name="interactor">Interactor hovering the button.</param>
        /// <returns>If the interactor can hover.</returns>
        public override bool IsSelectableBy(IXRSelectInteractor interactor) => false; // Grab is not allowed

        /// <summary>
        /// Plays the `pressed` sound, if assigned.
        /// </summary>
        protected virtual void PlayPressedSound() => PlaySound(_pressedSound);

        /// <summary>
        /// Plays the `released` sound, if assigned.
        /// </summary>
        protected virtual void PlayReleasedSound() => PlaySound(_pressedSound);

        /// <summary>
        /// Function wrapper to emit the OnPressed-Event with the given value.
        /// Internally used to (dis-)connect the same events from the ValueDescriptor to make them more accessible.
        /// </summary>
        protected virtual void HandleButtonPressed()
        {
            PlayPressedSound();

            // Change value without setter as we assume the value is coming from the ValueDescriptor
            if (_toggleMode && _canRepressToggle)
            {
                _pressed = !_pressed;
                (_pressed ? OnTogglePressed : OnToggleReleased).Invoke();
            }
            else if (!_toggleMode)
            {
                _pressed = true;
                OnPressed.Invoke();
            }
        }

        /// <summary>
        /// Function wrapper to emit the OnReleased-Event with the given value.
        /// Internally used to (dis-)connect the same events from the ValueDescriptor to make them more accessible.
        /// </summary>
        protected virtual void HandleButtonReleased()
        {
            PlayReleasedSound();

            // Change value without setter as we assume the value is coming from the ValueDescriptor
            if (_toggleMode)
            {
                _canRepressToggle = true;
            }
            else
            {
                _pressed = true;
                OnReleased.Invoke();
            }
        }

        /// <inheritdoc />
        public override bool IsHoverableBy(IXRHoverInteractor interactor)
        {
            return base.IsHoverableBy(interactor) && IsInteractorInRange(interactor);
        }

        /// <summary>
        /// Helper function to determine if a n interactor is in range.
        /// This is used as a hack since we can not properly determine if a hover is from the near or far part of a NearFarInteractor.
        /// </summary>
        /// <param name="interactor">Interactor in question.</param>
        /// <returns>If the interactor is in range or not.</returns>
        protected virtual bool IsInteractorInRange(IXRHoverInteractor interactor)
        {
            return _maxInteractionDistance <= 0.0f || GetDistanceSqrToInteractor(interactor) <= Mathf.Pow(_maxInteractionDistance, 2.0f);   
        }

        /// <inheritdoc />
         public override void ResetValue()
        {
            _valueDescriptor.ResetValue();
            // We need to update the visualization with our custom logic
            UpdateValueVisualization();
            OnValueReset.Invoke();
        }

        /// <inheritdoc />
        public override void SetValueToMinValue()
        {
            Pressed = false;
            // We need to update visualization accordingly
            UpdateValueVisualization();
        }

        /// <inheritdoc />
        public override void SetValueToMaxValue()
        {
            Pressed = true;
            // We need to update visualization accordingly
            UpdateValueVisualization();
        }
    }

    /// <summary>
    /// Defines the visualization for a slider interactable along the x-axis.
    /// </summary>    
    [Serializable]
    public class ButtonVisualizer : ValueVisualizer<float>
    {
        [SerializeField]
        [Tooltip("The offset of the button in up position (value=0) along the y-axis.")]
        protected float _upPosition = 0.029f;

        [SerializeField]
        [Tooltip("The offset of the button in down position (value=1) along the y-axis.")]
        protected float _downPosition = 0.014f;

        [SerializeField]
        [Tooltip("The offset of the button in down position when toggling (value=1) along the y-axis.")]
        protected float _toggledDownPosition = 0.032f;

        [SerializeField]
        [Tooltip("How the press distance is calculated, either from the hover start or the interactors transform position.")]
        protected PositionReference _positionReference = PositionReference.HoverStart;

        [SerializeField]
        [Tooltip("The object that is visually grabbed and manipulated.")]
        protected Transform _buttonCap = null;

        private float _hoverStartHeight;

        /// <inheritdoc />
        public override float GetVisualizedValue(IXRInteractable interactable, IXRInteractor interactor)
        {
            Vector3 localPosition = GetInteractorLocalPosition(interactable, interactor) - GetHoverOffset();
            return Mathf.Clamp01((localPosition.y - _upPosition) / (_downPosition - _upPosition));
        }

        /// <inheritdoc />
        public override void UpdateVisualization(float value, IXRInteractable _)
        {
            if (_buttonCap == null)
            {
                Debug.LogWarning($"No reference to a button cap provided. Can not visualize the value '{value}' anything without it.");
                return;
            }

            Vector3 capPos = _buttonCap.localPosition;
            capPos.y = Mathf.Lerp(_upPosition, _downPosition, value);
            _buttonCap.localPosition = capPos;
        }

        /// <summary>
        /// Displays the pressed state of a button if in toggle mode.
        /// </summary>
        /// <param name="value">Value to be displayed.</param>
        /// <param name="toggledDown">Toggle state of the button.</param>
        /// <param name="interactable">Interactable to be manipulated.</param>
        public void UpdateVisualizationWithToggle(float value, bool toggledDown, IXRInteractable _)
        {
            if (_buttonCap == null)
            {
                Debug.LogWarning($"No reference to a button cap provided. Can not visualize the toggled value '{value}' anything without it.");
                return;
            }

            float upClampPos = toggledDown ? _toggledDownPosition : _upPosition;
            Vector3 capPos = _buttonCap.localPosition;
            capPos.y = Mathf.Lerp(upClampPos, _downPosition, value);
            _buttonCap.localPosition = capPos;
        }

        protected virtual Vector3 GetHoverOffset() => _positionReference == PositionReference.HoverStart ? new(0.0f, _hoverStartHeight, 0.0f) : Vector3.zero;

        public void RecordHoverStartHeight(IXRInteractable interactable, IXRInteractor interactor)
        {
            _hoverStartHeight = GetInteractorLocalPosition(interactable, interactor).y - _upPosition;
        }

        /// <inheritdoc />
        public override void DrawGizmos(Transform atTransform, float value)
        {
            Vector3 handleOffset = _buttonCap != null ? _buttonCap.localPosition : Vector3.zero;
            handleOffset.y = 0.0f;

            GizmoUtils.DrawMinMaxValueLine(
                new Vector3(0.0f, _upPosition, 0.0f) + handleOffset,
                new Vector3(0.0f, _downPosition, 0.0f) + handleOffset,
                value,
                Color.green,
                Color.red,
                Color.blue,
                Color.yellow,
                Vector3.up,
                atTransform,
                "Up",
                "Down"
            );
        }

        protected enum PositionReference
        {
            InteractorTransform,
            HoverStart
        }
    }
}