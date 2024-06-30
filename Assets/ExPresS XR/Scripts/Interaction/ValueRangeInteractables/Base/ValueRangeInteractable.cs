using System;
using ExPresSXR.Experimentation.DataGathering;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace ExPresSXR.Interaction.ValueRangeInteractable
{
    /// <summary>
    /// An abstract interactable base class for selecting a value on a specific range with the option to snap a value.
    /// The value as well as the behavior is described by a ValueDescriptor so that this class can be used with different types of ranges for creating sliders, levers, joysticks and more.
    /// For creating a new range please refer to <see cref="ValueDescriptor"/> where you will also find some predefined ranges.
    /// 
    /// To create a new RangeInteractable create a new child class from this, providing both a value type and compatible ValueDescriptor.
    /// You can then proceed with implementing the function `UpdateValueWithGrab()` to reflect the value change when the interactable is selected.
    /// Make sure to set the value via 'Value = ...'.
    /// Visual changes should ideally be done via a function call in the setter for the `Value`-property to reflect changes not only during a grab but every time the value of the interactable changes.
    /// We advise you to also implement the `OnDrawGizmosSelected()`-function to indicate how your interactor behaves.
    /// Please refer to other interactors if you need an example.
    /// 
    /// Last but not least you can create a Custom Editor for your interactable, else the <see cref="ValueRangeInteractableEditor"/> will be used.
    /// </summary>
    public abstract class ValueRangeInteractable<T, U, V> : XRBaseInteractable, IRangeInteractorInternal where T : ValueDescriptor<V> where U : ValueVisualizer<V>
    {
        /// <summary>
        /// Describes the value and behavior of the manipulated value.
        /// </summary>
        [SerializeField]
        [Tooltip("Describes the value and behavior of the manipulated value.")]
        protected T _valueDescriptor;
        public T ValueDescriptor
        {
            get => _valueDescriptor;
        }

        /// <summary>
        /// Accessor for the current value of the ValueDescriptor.
        /// </summary>
        /// <value>Value of the range.</value>
        [Tooltip("Accessor for the current value of the ValueDescriptor.")]
        public virtual V Value
        {
            get => ValueDescriptor.Value;
            set
            {
                ValueDescriptor.Value = value;
                _valueVisualizer.UpdateVisualization(Value, this);
                EmitOnValueChanged(Value, value);
            }
        }

        /// <summary>
        /// Describes the value and behavior of the manipulated value.
        /// </summary>
        [SerializeField]
        [Tooltip("Describes how the value is displayed and modified during interactions.")]
        protected U _valueVisualizer;
        public U ValueVisualizer
        {
            get => _valueVisualizer;
        }

        /// <summary>
        /// If true, the joystick will snap to the upright position on release.
        /// </summary>
        [SerializeField]
        [Tooltip("If true, the joystick will snap to the upright position on release.")]
        private bool _zeroValueOnRelease = true;

        /// <summary>
        /// If only direct (i.e. grab) interactions are allowed. For this you'll need a child GameObject with a RigidBody with a collision.
        /// </summary>
        [SerializeField]
        [Tooltip("If only direct (i.e. grab) interactions are allowed. For this you'll need a child GameObject with a RigidBody with a collision.")]
        protected bool _requireDirectInteraction;


        /// <summary>
        /// Sound played when the interactable snaps to a position. Also acts as a default for minValueSound and maxValueSound if they are omitted.
        /// </summary>
        [SerializeField]
        [Tooltip("Sound played when the interactable snaps to a position. Also acts as a default for minValueSound and maxValueSound if they are omitted.")]
        protected AudioClip _snapSound;

        /// <summary>
        /// Sound played when the interactor snaps to the min value. If omitted, the snap sound will be played instead.
        /// </summary>
        [SerializeField]
        [Tooltip("Sound played when the interactor snaps to the min value. If omitted, the snap sound will be played instead.")]
        protected AudioClip _minValueSound;

        /// <summary>
        /// Sound played when the interactor snaps to the max value. If omitted the, snap sound will be played instead.
        /// </summary>
        [SerializeField]
        [Tooltip("Sound played when the interactor snaps to the max value. If omitted the, snap sound will be played instead.")]
        protected AudioClip _maxValueSound;

        /// <summary>
        /// Sound played when the value is moved without snapping.
        /// </summary>
        [SerializeField]
        [Tooltip("Sound played when the value is moved without snapping.")]
        protected AudioClip _moveSound;

        /// <summary>
        /// AudioPlayer used to play sounds that is used to play the provided AudioClips when the button is pressed or released.
        /// </summary>
        [SerializeField]
        protected AudioSource _defaultAudioPlayer;


        /// <summary>
        /// The interactor currently selecting with the interactable. Is null if nothing selects this interactable.
        /// </summary>
        protected IXRSelectInteractor _interactor;


        /// <summary>
        /// Emitted when the current value is a minimal value of the range.
        /// </summary>
        public UnityEvent<V> OnMinValue;

        /// <summary>
        /// Emitted when the current value is a maximal value of for the range.
        /// </summary>
        public UnityEvent<V> OnMaxValue;

        /// <summary>
        /// Emitted every time the current value is snapped to a new position.
        /// </summary>
        public UnityEvent<V> OnSnapped;

        /// <summary>
        /// Emitted if the value changes dynamically (i.e. when the interactor is moved). The first value is the new, the second the old value.
        /// </summary>
        public UnityEvent<V, V> OnValueChanged;

        /// <summary>
        /// Emitted if the value changes dynamically (i.e. when the interactor is moved). Returns a string for easier use with labels.
        /// </summary>
        public UnityEvent<string> OnValueChangedString;

        /// <summary>
        /// Emitted when the interactable is released, returning the currently selected value.
        /// </summary>
        public UnityEvent<V> OnValueSelected;

        /// <summary>
        /// Called when this component is enabled.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            selectEntered.AddListener(StartGrab);
            selectExited.AddListener(EndGrab);

            AddValueDescriptorListeners();
        }

        /// <summary>
        /// Called when this component is disabled.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            selectEntered.RemoveListener(StartGrab);
            selectExited.RemoveListener(EndGrab);

            RemoveValueDescriptorListeners();
        }

        /// <summary>
        /// Called when a grab starts.
        /// </summary>
        /// <param name="args">Context of this event.</param>
        protected virtual void StartGrab(SelectEnterEventArgs args)
        {
            _interactor = args.interactorObject;
        }

        /// <summary>
        /// Called when a grab ends.
        /// </summary>
        /// <param name="args">Context of this event.</param>
        protected virtual void EndGrab(SelectExitEventArgs args)
        {
            _interactor = null;

            OnValueSelected.Invoke(Value);

            // Reset after emitting the event to allow passing the selected value
            if (_zeroValueOnRelease)
            {
                _valueDescriptor.ResetValue();
                _valueVisualizer.UpdateVisualization(Value, this);
            }
        }

        /// <summary>
        /// Updates the interactable and allowing to implement custom functionality by calling the UpdateValueWithGrab if it is being grabbed.
        /// </summary>
        /// <param name="updatePhase">Phase of the update</param>
        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                if (isSelected)
                {
                    UpdateValueWithGrab();
                }
            }
        }

        /// <summary>
        /// Determines if the interactable can be hovered by an IXRHoverInteractor.
        /// </summary>
        /// <param name="interactor">Interactor hovering the button.</param>
        /// <returns>If the interactor can hover.</returns>
        public override bool IsHoverableBy(IXRHoverInteractor interactor)
        {
            return base.IsHoverableBy(interactor) && (!_requireDirectInteraction || interactor is XRDirectInteractor);
        }

        /// <summary>
        /// Determines if the interactable can be selected by an IXRHoverInteractor.
        /// </summary>
        /// <param name="interactor">Interactor hovering the button.</param>
        /// <returns>If the interactor can hover.</returns>
        public override bool IsSelectableBy(IXRSelectInteractor interactor)
        {
            return base.IsSelectableBy(interactor) && (!_requireDirectInteraction || interactor is XRDirectInteractor);
        }

        /// <summary>
        /// Adds listeners the ValueDescriptor events. Can be overwritten if your value range introduces more events.
        /// Automatically called during OnEnable().
        /// </summary>
        protected virtual void AddValueDescriptorListeners()
        {
            _valueDescriptor.OnMinValue.AddListener(EmitOnMinValue);
            _valueDescriptor.OnMaxValue.AddListener(EmitOnMaxValue);
            _valueDescriptor.OnSnapped.AddListener(EmitOnSnapped);
            _valueDescriptor.OnValueChanged.AddListener(EmitOnValueChanged);
        }

        /// <summary>
        /// Adds listeners the ValueDescriptor events. Can be overwritten if your value range introduces more events.
        /// Automatically called during OnEnable().
        /// </summary>
        protected virtual void RemoveValueDescriptorListeners()
        {
            _valueDescriptor.OnMinValue.RemoveListener(EmitOnMinValue);
            _valueDescriptor.OnMaxValue.RemoveListener(EmitOnMaxValue);
            _valueDescriptor.OnSnapped.RemoveListener(EmitOnSnapped);
            _valueDescriptor.OnValueChanged.RemoveListener(EmitOnValueChanged);
        }

#region Audio
        /// <summary>
        /// Function wrapper to emit the OnMinValue-Event with the given value.
        /// Internally used to (dis-)connect the same events from the ValueDescriptor to make them more accessible.
        /// </summary>
        /// <param name="v">Value to be passed with the event.</param>
        protected virtual void EmitOnMinValue(V v) => OnMinValue.Invoke(v);

        /// <summary>
        /// Function wrapper to emit the OnMaxValue-Event with the given value.
        /// Internally used to (dis-)connect the same events from the ValueDescriptor to make them more accessible.
        /// </summary>
        /// <param name="v">Value to be passed with the event.</param>
        protected virtual void EmitOnMaxValue(V v) => OnMaxValue.Invoke(v);

        /// <summary>
        /// Function wrapper to emit the OnSnapped-Event with the given value.
        /// Internally used to (dis-)connect the same events from the ValueDescriptor to make them more accessible.
        /// </summary>
        /// <param name="v">Value to be passed with the event.</param>
        protected virtual void EmitOnSnapped(V v) => OnSnapped.Invoke(v);

        /// <summary>
        /// Function wrapper to emit the OnValueChanged-Events with the given values.
        /// Internally used to (dis-)connect the same events from the ValueDescriptor to make them more accessible.
        /// </summary>
        /// <param name="newV">New value to be passed with the event.</param>
        /// <param name="oldV">Old value to be passed with the event.</param>
        protected virtual void EmitOnValueChanged(V newV, V oldV)
        {
            OnValueChanged.Invoke(oldV, newV);
            OnValueChangedString.Invoke(newV.ToString());
        }

        /// <summary>
        /// Automatically called when an interactor is trying to manipulate the interactor's value.
        /// Use this function update the interactor's Value based on your representation of the range.
        /// </summary>
        protected virtual void UpdateValueWithGrab() => Value = _valueVisualizer.GetVisualizedValue(this, _interactor);

        /// <summary>
        /// Plays the `minValueSound`, if assigned. If omitted, the snapSound is played.
        /// </summary>
        protected virtual void PlayMinValueSound() => PlaySound(_minValueSound != null ? _minValueSound : _snapSound);

        /// <summary>
        /// Plays the `maxValueSound`, if assigned. If omitted, the snapSound is played.
        /// </summary>
        protected virtual void PlayMaxValueSound() => PlaySound(_maxValueSound != null ? _maxValueSound : _snapSound);

        /// <summary>
        /// Plays the `snapSound`, if assigned.
        /// </summary>
        protected virtual void PlaySnapSound() => PlaySound(_snapSound);

        /// <summary>
        /// Plays the `moveSound`, if assigned.
        /// </summary>
        protected virtual void PlayMoveSound()
        {
            // Only play the sound, if it is not already playing to avoid spamming
            if (_defaultAudioPlayer != null && (_defaultAudioPlayer.clip != _moveSound || !_defaultAudioPlayer.isPlaying))
            {
                PlaySound(_moveSound);
            }
        }

        /// <summary>
        /// Plays the provided clip with the provided AudioPlayer.
        /// If the player is null, the `_defaultAudioPlayer` will be used.
        /// </summary>
        /// <param name="clip">Clip to be played.</param>
        /// <param name="player">AudioPlayer to play the clip.</param>
        protected virtual void PlaySound(AudioClip clip, AudioSource player = null)
        {
            if (player == null)
            {
                player = _defaultAudioPlayer;
            }

            if (clip != null)
            {
                player.clip = clip;
                player.Play();
            }
        }
#endregion

#region Internal
        /// <summary>
        /// Calls the gizmo representation of the selected visualizer.
        /// </summary>
        public void OnDrawGizmosSelected()
        {
            ValueVisualizer.DrawGizmos(transform, Value);
        }


        /// <inheritdoc />
        public virtual void InternalUpdateValue()
        {
            Value = Value;
        }
#endregion
    }

    /// <summary>
    /// We need this interface to call in the generic BaseComponent-Editor without knowing the exact generic type.
    /// </summary>
    public interface IRangeInteractorInternal
    {
        /// <summary>
        /// Update value to allow editing from the editor.
        /// </summary>
        public void InternalUpdateValue();
    }
}