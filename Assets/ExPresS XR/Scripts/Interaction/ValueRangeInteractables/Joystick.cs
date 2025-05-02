using System;
using ExPresSXR.Misc;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


namespace ExPresSXR.Interaction.ValueRangeInteractable
{
    /// <summary>
    /// Defines a joystick interactable.
    /// </summary>
    public class Joystick : ValueRangeInteractable<CircularDescriptor, JoystickVisualizer, Vector2>
    {
        /// <inheritdoc />
        public override void InternalUpdateValue()
        {
            // Use the SetValue function as it is manipulated via direction and magnitude
            ValueDescriptor.SetValue(ValueDescriptor.RawDirection, ValueDescriptor.RawMagnitude);
            _valueVisualizer.UpdateVisualization(Value, this);
        }
    }

    /// <summary>
    /// Defines the visualization for a joystick interactable.
    /// </summary>
    [Serializable]
    public class JoystickVisualizer : ValueVisualizer<Vector2>
    {
        /// <summary>
        /// Maximal angle to lean the joystick in any direction from the center.
        /// </summary>
        [SerializeField]
        [Range(0.0f, 90.0f)]
        [Tooltip("Maximal angle to lean the joystick in any direction from the center.")]
        private float _angleRange = 42.0f;
        public float AngleRange
        {
            get => _angleRange;
            set => _angleRange = value;
        }

        [Space]

        [SerializeField]
        [Tooltip("Pivot for the handle of the lever. The handle itself should be a RigidBody with a collision to work properly with grabbing.")]
        private Transform _pivot;

        /// <inheritdoc />
        public override Vector2 GetVisualizedValue(IXRSelectInteractable interactable, IXRSelectInteractor interactor)
        {
            return GetJoystickAngleNormalized(interactable, interactor);
        }

        /// <inheritdoc />
        public override void UpdateVisualization(Vector2 value, IXRSelectInteractable interactable)
        {
            if (_pivot == null)
            {
                Debug.LogWarning($"No reference to a pivot provided. Can not visualize the value '{value}' anything without it.");
                return;
            }

            float xRotation = value.x * AngleRange;
            float zRotation = value.y * AngleRange;
            _pivot.localRotation = Quaternion.Euler(xRotation, 0.0f, zRotation);
        }

        /// <summary>
        /// Returns the angle of the lever normalized on the available angle range.
        /// The valid values for each coordinate are in the range of [0.0f, 1.0f], but the returned value is not clamped. 
        /// </summary>
        /// <returns>Lever angle normalized.</returns>
        protected Vector2 GetJoystickAngleNormalized(IXRSelectInteractable interactable, IXRSelectInteractor interactor)
        {
            Vector3 interactorDirection = GetInteractorDirection(interactable, interactor);
            float leverXAngle = Mathf.Atan2(interactorDirection.z, interactorDirection.y) * Mathf.Rad2Deg;
            float leverZAngle = -Mathf.Atan2(interactorDirection.x, interactorDirection.y) * Mathf.Rad2Deg;
            float xRotation = leverXAngle / AngleRange;
            float zRotation = leverZAngle / AngleRange;
            return new(xRotation, zRotation);
        }

        /// <inheritdoc />
        public override void DrawGizmos(Transform atTransform, Vector2 value)
        {
            Vector3 localPivotPos = _pivot != null ? _pivot.localPosition : Vector3.zero;

            GizmoUtils.DrawMinMaxRotationSpan(
                Quaternion.Euler(-AngleRange, 0.0f, 0.0f),
                Quaternion.Euler(AngleRange, 0.0f, 0.0f),
                Color.red,
                Color.green,
                Color.blue,
                localPivotPos,
                Vector3.up,
                Vector3.right,
                atTransform
            );

            GizmoUtils.DrawMinMaxRotationSpan(
                Quaternion.Euler(0.0f, 0.0f, -AngleRange),
                Quaternion.Euler(0.0f, 0.0f, AngleRange),
                Color.magenta,
                Color.cyan,
                Color.blue,
                localPivotPos,
                Vector3.up,
                Vector3.forward,
                atTransform
            );

            float xRotation = value.x * AngleRange;
            float zRotation = value.y * AngleRange;

            Quaternion valueRotation = Quaternion.Euler(xRotation, 0.0f, zRotation);
            Vector3 angleValuePoint = localPivotPos + valueRotation * Vector3.up * GizmoUtils.ROTATION_SPAN_VALUE_LENGTH;

            Gizmos.matrix = atTransform.localToWorldMatrix;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(localPivotPos, angleValuePoint);
        }

        /// <inheritdoc />
        protected override Vector3 GetPivotOffset(IXRSelectInteractable interactable) => _pivot != null ? _pivot.position : base.GetPivotOffset(interactable);
    }
}