using System;
using ExPresSXR.Misc;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

namespace ExPresSXR.Interaction.ValueRangeInteractable
{
    /// <summary>
    /// Defines a turnable (door-) knob interactable.
    /// It features an option for turning using the wrist/controller rotation and by rotating around the up-axis.
    /// For a more complex turing behavior see: https://github.com/Unity-Technologies/XR-Interaction-Toolkit-Examples/blob/main/Assets/XRI_Examples/UI_3D/Scripts/XRKnob.cs
    /// </summary>
    public class KnobInteractable : ValueRangeInteractable<Float01Descriptor, TurnVisualizer, float>
    {
        protected override void StartGrab(SelectEnterEventArgs args)
        {
            base.StartGrab(args);

            // The visualizer calculates the value using a delta, 
            // so we'll need to set the starting delta to reflect the initial value and external changes.
            // ValueVisualizer.RawValue = Value;
            ValueVisualizer.PreviousTurnForward = Vector3.zero;
        }
    }

    /// <summary>
    /// Defines the visualization for a turnable (door-)knob interactable.
    /// </summary>
    [Serializable]
    public class TurnVisualizer : ValueVisualizer<float>
    {
        /// <summary>
        /// Minimum angle in degrees (can be multiple rotations).
        /// </summary>
        [SerializeField]
        [Tooltip("Minimum angle in degrees (can be multiple rotations).")]
        protected float _minAngle = 0.0f;

        /// <summary>
        /// Maximum angle in degrees (can be multiple rotations).
        /// </summary>
        [SerializeField]
        [Tooltip("Maximum angle in degrees (can be multiple rotations).")]
        protected float _maxAngle = 180.0f;

        /// <summary>
        /// Factor for faster/slower turns.
        /// </summary>
        [SerializeField]
        [Tooltip("Factor for faster/slower turns.")]
        protected float _turnSpeed = 1.0f;

        /// <summary>
        /// How turning is performed.
        /// </summary>
        [SerializeField]
        [Tooltip("How turning is performed.")]
        protected InteractorTurnType _turnType;
        public InteractorTurnType TurnType
        {
            get => _turnType;
            set => _turnType = value;
        }

        [Space]

        /// <summary>
        /// The object that is visually grabbed and manipulated.
        /// </summary>
        [SerializeField]
        [Tooltip("The object that is visually grabbed and manipulated.")]
        protected Transform _pivot = null;

        /// <summary>
        /// The angle between the minimal and maximal angle.
        /// </summary>
        /// <value>Angle between the minimal and maximal angle.</value>
        public float AngleRange
        {
            get => _maxAngle - _minAngle;
        }

        private float _rawValue;
        public float RawValue
        {
            get => _rawValue;
            set => _rawValue = Mathf.Clamp01(value);
        }

        public Vector3 _previousTurnForward = Vector3.zero;
        public Vector3 PreviousTurnForward
        {
            get => _previousTurnForward;
            set => _previousTurnForward = value;
        }

        /// <inheritdoc />
        protected virtual Vector3 GetTurnForward(IXRSelectInteractable interactable, IXRSelectInteractor interactor)
        {
            Vector3 interactorForward = _turnType == InteractorTurnType.Forward
                                        ? interactor.GetAttachTransform(interactable).forward
                                        : GetInteractorDirection(interactable, interactor).normalized;
            return Vector3.ProjectOnPlane(interactorForward, interactable.transform.up);
        }

        /// <inheritdoc />
        public override float GetVisualizedValue(IXRSelectInteractable interactable, IXRSelectInteractor interactor)
        {
            Vector3 currentTurnForward = GetTurnForward(interactable, interactor);
            // Prevent initial grab -> previous turn forward is Vector3.zero
            if (_previousTurnForward == Vector3.zero)
            {
                _previousTurnForward = currentTurnForward;
            }
            float turnAngleDiff = Vector3.SignedAngle(_previousTurnForward, currentTurnForward, interactable.transform.up);
            float valueDelta = turnAngleDiff / AngleRange * _turnSpeed;
            _rawValue = Mathf.Clamp01(_rawValue + valueDelta);
            _previousTurnForward = currentTurnForward;
            return _rawValue;
        }

        /// <inheritdoc />
        public override void UpdateVisualization(float value, IXRSelectInteractable interactable)
        {
            if (_pivot == null)
            {
                Debug.LogWarning($"No reference to a pivot provided. Can not visualize the value '{value}' anything without it.");
                return;
            }

            Vector3 handleRot = _pivot.eulerAngles;
            handleRot.y = Mathf.Lerp(_minAngle, _maxAngle, value);
            _pivot.localEulerAngles = handleRot;
        }

        /// <inheritdoc />
        public override void DrawGizmos(Transform atTransform, float value)
        {
            GizmoUtils.DrawMinMaxValueArc(
                _minAngle,
                _maxAngle,
                value,
                Color.red,
                Color.green,
                Color.blue,
                Color.yellow,
                Vector3.zero,
                Vector3.up,
                atTransform
            );
        }

        /// <summary>
        /// Determines how a turn is calculated. Either by turning the controller/wrist (Forward) or by rotating around the up-axis (Direction).
        /// </summary>
        public enum InteractorTurnType
        {
            Forward,
            Direction
        }
    }
}