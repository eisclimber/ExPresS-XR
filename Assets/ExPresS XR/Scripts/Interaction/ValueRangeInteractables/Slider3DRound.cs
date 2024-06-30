using System;
using ExPresSXR.Misc;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


namespace ExPresSXR.Interaction.ValueRangeInteractable
{
    /// <summary>
    /// Defines a three dimensional slider interactable with a spherical shape.
    /// </summary>
    public class Slider3DRound : ValueRangeInteractable<SphereDescriptor, Slider3DSphereVisualizer, Vector3>
    {
        protected override void StartGrab(SelectEnterEventArgs args)
        {
            base.StartGrab(args);
            _valueVisualizer.SetHandleGrabOffsetWithInteraction(args.interactableObject, args.interactorObject);
        }
    }

    /// <summary>
    /// Defines the visualization for a three dimensional slider interactable with a spherical shape.
    /// </summary>
    [Serializable]
    public class Slider3DSphereVisualizer : ValueVisualizer<Vector3>
    {
        /// <summary>
        /// The radius of the interactable area.
        /// </summary>
        [SerializeField]
        [Tooltip("The radius of the interactable area.")]
        private float _radius = 0.42f;

        [Space]

        /// <summary>
        /// Keep the offset to the handle when grabbed. Use for large handles.
        /// </summary>
        [SerializeField]
        [Tooltip("Keep the offset to the handle when grabbed. Use for large handles.")]
        protected bool _useHandleGrabOffset;

        /// <summary>
        /// The object that is visually grabbed and used to manipulate the space.
        /// </summary>
        [SerializeField]
        [Tooltip("The object that is visually grabbed and used to manipulate the space.")]
        private Transform _handle = null;

        private Vector3 _grabOffset;

        /// <inheritdoc />
        public override Vector3 GetVisualizedValue(IXRSelectInteractable interactable, IXRSelectInteractor interactor)
        {
            // Put anchor position into slider space
            Vector3 localPosition = GetInteractorLocalPosition(interactable, interactor) - _grabOffset;

            return localPosition / _radius;
        }

        /// <inheritdoc />
        public override void UpdateVisualization(Vector3 value, IXRSelectInteractable interactable)
        {
            if (_handle == null)
            {
                Debug.LogWarning($"No reference to a handle provided. Can not visualize the value '{value}' anything without it.");
                return;
            }
            _handle.localPosition = value * _radius;
        }

        public void SetHandleGrabOffsetWithInteraction(IXRSelectInteractable interactable, IXRSelectInteractor interactor)
        {
            _grabOffset = _useHandleGrabOffset ? GetInteractorLocalPosition(interactable, interactor) - _handle.localPosition : Vector3.zero;
        }

        /// <inheritdoc />
        public override void DrawGizmos(Transform atTransform, Vector3 value)
        {
            GizmoUtils.DrawMinMaxLine(
                new Vector3(-_radius, 0.0f, 0.0f),
                new Vector3(_radius, 0.0f, 0.0f),
                Color.red,
                Color.green,
                Color.blue,
                Vector3.up,
                atTransform
            );

            GizmoUtils.DrawMinMaxLine(
                new Vector3(0.0f, -_radius, 0.0f),
                new Vector3(0.0f, _radius, 0.0f),
                Color.magenta,
                Color.cyan,
                Color.blue,
                Vector3.up,
                atTransform
            );

            GizmoUtils.DrawMinMaxLine(
                new Vector3(0.0f, 0.0f, -_radius),
                new Vector3(0.0f, 0.0f, _radius),
                Color.magenta,
                Color.cyan,
                Color.blue,
                Vector3.up,
                atTransform
            );

            Gizmos.matrix = atTransform.localToWorldMatrix;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(Vector3.zero, _radius);
        }
    }
}