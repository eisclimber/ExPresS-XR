using System;
using ExPresSXR.Misc;
using UnityEngine;
using UnityEngine.Events;
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
        /// Accessor for the current value of the ValueDescriptor.
        /// </summary>
        /// <value>Value of the range.</value>
        [Tooltip("Accessor for the current value of the ValueDescriptor.")]
        public override float Value
        {
            get => ValueDescriptor.Value;
            set
            {
                ValueDescriptor.Value = value;
                // Override value visualization with 
                if (_valueDescriptor.ToggleMode)
                {
                    _valueVisualizer.UpdateVisualizationWithToggle(Value, _valueDescriptor.Pressed, this);
                }
                else
                {
                    _valueVisualizer.UpdateVisualization(Value, this);
                }
                EmitOnValueChanged(Value, value);
            }
        }


        /// <summary>
        /// If enabled, the button will refuse input and will stay in the up-position.
        /// </summary>
        [SerializeField]
        private bool _inputDisabled;
        public bool InputDisabled
        {
            get => _inputDisabled;
            set
            {
                _inputDisabled = value;
                (_inputDisabled ? OnInputDisabled : OnInputEnabled).Invoke();
            }
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
        /// Sound played when the button is pressed.
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
        /// Emitted when the button is pressed.
        /// </summary>
        public UnityEvent OnPressed;

        /// <summary>
        /// Emitted when the button is released.
        /// </summary>
        public UnityEvent OnReleased;


        /// <summary>
        /// Emitted when the toggle mode of the button has changed.
        /// </summary>
        public UnityEvent<bool> OnToggleModeChanged;

        /// <summary>
        /// Emitted when the button has been toggled up.
        /// </summary>
        public UnityEvent OnToggledUp;

        /// <summary>
        /// Emitted when the button has been toggled down.
        /// </summary>
        public UnityEvent OnToggledDown;


        /// <summary>
        /// Emitted when input gets disabled.
        /// </summary>
        public UnityEvent OnInputDisabled;

        /// <summary>
        /// Emitted when input gets enabled.
        /// </summary>
        public UnityEvent OnInputEnabled;


        protected virtual void Start()
        {
            InputDisabled = _inputDisabled;
        }

        protected override void UpdateValueWithHover() => Value = _valueVisualizer.GetVisualizedValue(this, _hoverInteractor);

        /// <summary>
        /// Disable grabbing the button (Don't update value when grabbed)
        /// </summary>
        protected override void UpdateValueWithGrab() {}

        protected override void AddValueDescriptorListeners()
        {
            base.AddValueDescriptorListeners();

            _valueDescriptor.OnPressed.AddListener(EmitPressed);
            _valueDescriptor.OnReleased.AddListener(EmitReleased);

            _valueDescriptor.OnToggledUp.AddListener(EmitToggledDown);
            _valueDescriptor.OnToggledDown.AddListener(EmitToggledUp);

            _valueDescriptor.OnToggleModeChanged.AddListener(HandleToggleModeChanged);
        }

        protected override void RemoveValueDescriptorListeners()
        {
            base.RemoveValueDescriptorListeners();
            _valueDescriptor.OnPressed.RemoveListener(EmitPressed);
            _valueDescriptor.OnReleased.RemoveListener(EmitReleased);

            _valueDescriptor.OnToggledUp.RemoveListener(EmitToggledDown);
            _valueDescriptor.OnToggledDown.RemoveListener(EmitToggledUp);

            _valueDescriptor.OnToggleModeChanged.RemoveListener(HandleToggleModeChanged);
        }

        /// <summary>
        /// Determines if the Button can be hovered or in this case pressed by an IXRHoverInteractor.
        /// </summary>
        /// <param name="interactor">Interactor hovering the button.</param>
        /// <returns>Wether or not the interactor can hover (i.e. press) the button</returns>
        public override bool IsHoverableBy(IXRHoverInteractor interactor)
        {
            return !InputDisabled && base.IsHoverableBy(interactor);
        }

        /// <summary>
        /// Determines if the interactable can be selected by an IXRSelectInteractor.
        /// </summary>
        /// <param name="interactor">Interactor hovering the button.</param>
        /// <returns>If the interactor can hover.</returns>
        public override bool IsSelectableBy(IXRSelectInteractor interactor)
        {
            return false; // Grab is not allowed
        }

        /// <summary>
        /// Plays the `pressed` sound, if assigned.
        /// </summary>
        protected virtual void PlayPressedSound() => PlaySound(_pressedSound);

        /// <summary>
        /// Plays the `released` sound, if assigned.
        /// </summary>
        protected virtual void PlayReleasedSound() => PlaySound(_pressedSound);

        /// <summary>
        /// Plays the `toggledDown` sound, if assigned.
        /// </summary>
        protected virtual void PlayToggledDownSound() => PlaySound(_toggledDownSound);

        /// <summary>
        /// Plays the `toggledDown` sound, if assigned.
        /// </summary>
        protected virtual void PlayToggledUpSound() => PlaySound(_toggledUpSound);

        /// <summary>
        /// Function wrapper to emit the OnPressed-Event with the given value.
        /// Internally used to (dis-)connect the same events from the ValueDescriptor to make them more accessible.
        /// </summary>
        /// <param name="v">Value to be passed with the event.</param>
        protected virtual void EmitPressed()
        {
            Debug.Log("Pressed");
            PlayPressedSound();
            OnPressed.Invoke();
        }

        /// <summary>
        /// Function wrapper to emit the OnReleased-Event with the given value.
        /// Internally used to (dis-)connect the same events from the ValueDescriptor to make them more accessible.
        /// </summary>
        /// <param name="v">Value to be passed with the event.</param>
        protected virtual void EmitReleased()
        {
            Debug.Log("Released");
            PlayReleasedSound();
            OnReleased.Invoke();
        }

        /// <summary>
        /// Function wrapper to emit the OnToggledDown-Event with the given value.
        /// Internally used to (dis-)connect the same events from the ValueDescriptor to make them more accessible.
        /// </summary>
        protected virtual void EmitToggledUp()
        {
            Debug.Log("ToggledDown");
            PlayPressedSound();
            OnPressed.Invoke();
        }

        /// <summary>
        /// Function wrapper to emit the OnToggledUp-Event with the given value.
        /// Internally used to (dis-)connect the same events from the ValueDescriptor to make them more accessible.
        /// </summary>
        protected virtual void EmitToggledDown()
        {
            Debug.Log("ToggledUp");
            PlayReleasedSound();
            OnReleased.Invoke();
        }

        /// <summary>
        /// Function wrapper to emit the OnToggledUp-Event with the given value.
        /// Internally used to (dis-)connect the same events from the ValueDescriptor to make them more accessible.
        /// </summary>
        protected virtual void HandleToggleModeChanged(bool toggleMode)
        {
            OnToggleModeChanged.Invoke(toggleMode);
        }
 
        /// <summary>
        /// Executed automatically when the input is disabled. Allows changing values in the inspector during runtime.
        /// </summary>
        protected virtual void OnValidate()
        {
            InputDisabled = _inputDisabled;
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
        [Tooltip("The object that is visually grabbed and manipulated.")]
        protected Transform _buttonCap = null;

        /// <inheritdoc />
        public override float GetVisualizedValue(IXRInteractable interactable, IXRInteractor interactor)
        {
            Vector3 localPosition = GetInteractorLocalPosition(interactable, interactor);
            return Mathf.Clamp01((localPosition.y - _upPosition) / (_downPosition - _upPosition));
        }

        /// <inheritdoc />
        public override void UpdateVisualization(float value, IXRInteractable interactable)
        {
            ChangeButtonCapPosition(value, _upPosition, _downPosition);
        }

        /// <summary>
        /// Handles 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="toggledDown"></param>
        /// <param name="interactable"></param>
        public void UpdateVisualizationWithToggle(float value, bool toggledDown, IXRInteractable interactable)
        {
            float toggledPos = toggledDown ? _toggledDownPosition : _upPosition;
            Debug.Log($"Toggled pos: {toggledPos}");
            ChangeButtonCapPosition(value, toggledPos, _downPosition);
        }

        private void ChangeButtonCapPosition(float value, float upPos, float downPos)
        {
            if (_buttonCap == null)
            {
                Debug.LogWarning($"No reference to a button cap provided. Can not visualize the value '{value}' anything without it.");
                return;
            }

            Vector3 capPos = _buttonCap.localPosition;
            capPos.y = Mathf.Lerp(upPos, downPos, value);
            _buttonCap.localPosition = capPos;
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
    }
}