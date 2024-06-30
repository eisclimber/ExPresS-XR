using System;
using ExPresSXR.Misc;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


namespace ExPresSXR.Interaction.ValueRangeInteractable
{
    /// <summary>
    /// Defines a two dimensional slider interactable along the xy-plane with a circular shape.
    /// </summary>
    public class Slider2DRound : ValueRangeInteractable<CircularDescriptor, Slider2DRoundVisualizer, Vector2>
    {
        /// <inheritdoc />
        public override void InternalUpdateValue()
        {
            // Use the SetValue function as it is manipulated via direction and magnitude
            ValueDescriptor.SetValue(ValueDescriptor.RawDirection, ValueDescriptor.RawMagnitude);
            _valueVisualizer.UpdateVisualization(Value, this);
        }

        /// <inheritdoc />
        protected override void StartGrab(SelectEnterEventArgs args)
        {
            base.StartGrab(args);
            _valueVisualizer.SetHandleGrabOffsetWithInteraction(args.interactableObject, args.interactorObject);
        }
    }

    /// <summary>
    /// Defines the visualization for a two dimensional slider interactable along the xy-plane.
    /// </summary>
    [Serializable]
    public class Slider2DRoundVisualizer : ValueVisualizer<Vector2>
    {
        /// <summary>
        /// The radius of the interactable area.
        /// </summary>
        [SerializeField]
        [Tooltip("The radius of the interactable area.")]
        private float _radius = 0.48f;

        [Space]

        /// <summary>
        /// Keep the offset to the handle when grabbed. Use for large handles.
        /// </summary>
        [SerializeField]
        [Tooltip("Keep the offset to the handle when grabbed. Use for large handles.")]
        protected bool _useHandleGrabOffset;

        /// <summary>
        /// The object that is visually grabbed and used to manipulate the slider.
        /// </summary>
        [SerializeField]
        [Tooltip("The object that is visually grabbed and used to manipulate the slider.")]
        private Transform _handle = null;

        private Vector3 _grabOffset;

        /// <inheritdoc />
        public override Vector2 GetVisualizedValue(IXRSelectInteractable interactable, IXRSelectInteractor interactor)
        {
            // Put anchor position into slider space
            Vector3 localPosition = GetInteractorLocalPosition(interactable, interactor) - _grabOffset;

            float sliderXValue = Mathf.Clamp(localPosition.x / _radius, -1.0f, 1.0f);
            float sliderZValue = Mathf.Clamp(localPosition.z / _radius, -1.0f, 1.0f);

            return new(sliderXValue, sliderZValue);
        }

        /// <inheritdoc />
        public override void UpdateVisualization(Vector2 value, IXRSelectInteractable interactable)
        {
            if (_handle == null)
            {
                Debug.LogWarning($"No reference to a handle provided. Can not visualize the value '{value}' anything without it.");
                return;
            }

            Vector3 handlePos = _handle.localPosition;
            handlePos.x = value.x * _radius;
            handlePos.z = value.y * _radius;
            _handle.localPosition = handlePos;
        }

        /// <summary>
        /// Sets the grab offset with the given context of interactable and interactor.
        /// </summary>
        /// <param name="interactable">Interactable selected.</param>
        /// <param name="interactor">Interactor selecting.</param>
        public void SetHandleGrabOffsetWithInteraction(IXRSelectInteractable interactable, IXRSelectInteractor interactor)
        {
            _grabOffset = _useHandleGrabOffset ? GetInteractorLocalPosition(interactable, interactor) - _handle.localPosition : Vector3.zero;
        }

        /// <inheritdoc />
        public override void DrawGizmos(Transform atTransform, Vector2 value)
        {
            float handleYOffset = _handle != null ? _handle.localPosition.y : 0.0f;

            GizmoUtils.DrawMinMaxLine(
                new Vector3(-_radius, handleYOffset, 0.0f),
                new Vector3(_radius, handleYOffset, 0.0f),
                Color.red,
                Color.green,
                Color.blue,
                Vector3.up,
                atTransform
            );

            GizmoUtils.DrawMinMaxLine(
                new Vector3(0.0f, handleYOffset, -_radius),
                new Vector3(0.0f, handleYOffset, _radius),
                Color.magenta,
                Color.cyan,
                Color.blue,
                Vector3.up,
                atTransform
            );

            Handles.matrix = atTransform.localToWorldMatrix;
            Handles.color = Color.blue;
            Handles.DrawWireDisc(new Vector3(0.0f, handleYOffset, 0.0f), Vector3.up, _radius);
        }
    }
}