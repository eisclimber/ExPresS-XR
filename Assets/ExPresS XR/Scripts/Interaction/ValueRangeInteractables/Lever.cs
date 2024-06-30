using System;
using ExPresSXR.Misc;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace ExPresSXR.Interaction.ValueRangeInteractable
{
    /// <summary>
    /// Defines a lever interactable that rotates around back and forth (around the x-axis).
    /// </summary>
    public class Lever : ValueRangeInteractable<Float01Descriptor, LeverVisualizer, float> { }


    /// <summary>
    /// Defines the visualization for a lever interactable that rotates around back and forth (around the x-axis).
    /// </summary>
    [Serializable]
    public class LeverVisualizer : ValueVisualizer<float>
    {
        /// <summary>
        /// The minimum angle in degrees to rotate along the x-axis.
        /// </summary>
        [SerializeField]
        [Tooltip("The minimum angle in degrees to rotate along the x-axis.")]
        private float _minAngle = -60;

        /// <summary>
        /// The maximum angle in degrees to rotate along the x-axis.
        /// </summary>
        [SerializeField]
        [Tooltip("The maximum angle in degrees to rotate along the x-axis.")]
        private float _maxAngle = 60;

        [Space]

        /// <summary>
        /// Pivot for the handle of the lever. The handle itself should be a RigidBody with a collision to work properly with grabbing.
        /// </summary>
        [SerializeField]
        [Tooltip("Pivot for the handle of the lever. The handle itself should be a RigidBody with a collision to work properly with grabbing.")]
        private Transform _pivot;

        /// <summary>
        /// The angle between the minimal and maximal angle.
        /// </summary>
        /// <value>Angle between the minimal and maximal angle.</value>
        public float AngleRange
        {
            get => _maxAngle - _minAngle;
        }

        /// <inheritdoc />
        public override float GetVisualizedValue(IXRSelectInteractable interactable, IXRSelectInteractor interactor)
        {
            Vector3 interactorDirection = GetInteractorDirection(interactable, interactor);
            interactorDirection.x = 0.0f; // Ignore x coordinate
            float leverAngle = Mathf.Atan2(interactorDirection.z, interactorDirection.y) * Mathf.Rad2Deg;
            return (leverAngle - _minAngle) / AngleRange;
        }

        /// <inheritdoc />
        public override void UpdateVisualization(float value, IXRSelectInteractable interactable)
        {
            if (_pivot == null)
            {
                Debug.LogWarning($"No reference to a pivot provided. Can not visualize the value '{value}' anything without it.");
                return;
            }
            // Rotate around the (positive) x-axis
            _pivot.localRotation = Quaternion.Euler(_minAngle + value * AngleRange, 0.0f, 0.0f);
        }

        /// <inheritdoc />
        public override void DrawGizmos(Transform atTransform, float value)
        {
            Vector3 localPivotPos = _pivot != null ? _pivot.localPosition : Vector3.zero;

            GizmoUtils.DrawMinMaxValueRotationSpan(
                Quaternion.Euler(_minAngle, 0.0f, 0.0f),
                Quaternion.Euler(_maxAngle, 0.0f, 0.0f),
                value,
                Color.red,
                Color.green,
                Color.blue,
                Color.yellow,
                localPivotPos,
                Vector3.up,
                Vector3.right,
                atTransform
            );
        }

        /// <inheritdoc />
        protected override Vector3 GetPivotOffset(IXRSelectInteractable interactable) => _pivot != null ? _pivot.position : base.GetPivotOffset(interactable);
    }

}