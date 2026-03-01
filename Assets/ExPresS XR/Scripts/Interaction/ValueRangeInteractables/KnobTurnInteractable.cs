using System;
using ExPresSXR.Misc;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ExPresSXR.Interaction.ValueRangeInteractable
{
    /// <summary>
    /// Defines a turnable (door-) knob interactable.
    /// It features an option for turning using the wrist/controller rotation and by rotating around the up-axis.
    /// For a more complex turing behavior see: https://github.com/Unity-Technologies/XR-Interaction-Toolkit-Examples/blob/main/Assets/XRI_Examples/UI_3D/Scripts/XRKnob.cs
    /// </summary>
    public class KnobInteractable : ValueRangeInteractable<Float01Descriptor, TurnVisualizer, float>
    {
        /// <inheritdoc />
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

        [SerializeField]
        [Tooltip("How turning is performed.")]
        protected InteractorTurnType _turnType;
        /// <summary>
        /// How turning is performed.
        /// </summary>
        public InteractorTurnType TurnType
        {
            get => _turnType;
            set => _turnType = value;
        }

        [SerializeField]
        [Tooltip("If true, inverses the turn direction.")]
        private bool _flipTurnDirection;
        /// <summary>
        /// If true, inverses the turn direction.
        /// </summary>
        public bool FlipTurnDirection
        {
            get => _flipTurnDirection;
            set => _flipTurnDirection = value;
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
        /// <summary>
        /// Raw value visualized. Ensured to be in the range between 0.0f and 1.0f (inclusive).
        /// </summary>
        public float RawValue
        {
            get => _rawValue;
            set => _rawValue = Mathf.Clamp01(value);
        }

        private Vector3 _previousTurnForward = Vector3.zero;
        /// <summary>
        /// Forward direction of the previous update.
        /// </summary>
        public Vector3 PreviousTurnForward
        {
            get => _previousTurnForward;
            set => _previousTurnForward = value;
        }


        /// <inheritdoc />
        protected virtual Vector3 GetTurnForward(IXRInteractable interactable, IXRInteractor interactor)
        {

            Vector3 interactorForward = _turnType == InteractorTurnType.Forward
                                        ? interactor.GetAttachTransform(interactable).forward
                                        : GetInteractorDirection(interactable, interactor).normalized;

            return Vector3.ProjectOnPlane(interactable.transform.TransformDirection(interactorForward), interactable.transform.up);
        }

        /// <inheritdoc />
        public override float GetVisualizedValue(IXRInteractable interactable, IXRInteractor interactor)
        {
            Vector3 currentTurnForward = GetTurnForward(interactable, interactor);
            // Prevent initial grab -> previous turn forward is Vector3.zero
            if (_previousTurnForward == Vector3.zero)
            {
                _previousTurnForward = currentTurnForward;
            }
            float inverseFactor = _flipTurnDirection ? -1.0f : 1.0f;
            float turnAngleDiff = inverseFactor * Vector3.SignedAngle(_previousTurnForward, currentTurnForward, interactable.transform.up);

            float valueDelta = turnAngleDiff / AngleRange * _turnSpeed;
            _rawValue = Mathf.Clamp01(_rawValue + valueDelta);
            _previousTurnForward = currentTurnForward;
            return _rawValue;
        }

        /// <inheritdoc />
        public override void UpdateVisualization(float value, IXRInteractable interactable)
        {
            if (_pivot == null)
            {
                Debug.LogWarning($"No reference to a pivot provided. Can not visualize the value '{value}' anything without it.");
                return;
            }

            Vector3 handleRot = _pivot.localEulerAngles;
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
                atTransform,
                "0.0",
                "1.0",
                "{0:F1}"
            );
        }

        /// <summary>
        /// Determines how a turn is calculated. Either by turning the controller/wrist (Forward) or by rotating around the up-axis (Direction).
        /// </summary>
        public enum InteractorTurnType
        {
            /// <summary> Turning is determined from wrist movement. </summary>
            Forward,
            /// <summary> Turning is determined the direction from the interactable to the interactor. </summary>
            Direction
        }
    }
}