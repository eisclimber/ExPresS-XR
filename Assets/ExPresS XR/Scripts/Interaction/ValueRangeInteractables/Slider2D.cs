using System;
using ExPresSXR.Misc;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


namespace ExPresSXR.Interaction.ValueRangeInteractable
{
    /// <summary>
    /// Defines a two dimensional slider interactable along the xy-plane.
    /// </summary>
    public class Slider2D : ValueRangeInteractable<Vector2Descriptor, Slider2DVisualizer, Vector2>
    {
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
    public class Slider2DVisualizer : ValueVisualizer<Vector2>
    {
        [SerializeField]
        [Tooltip("The minimum offset of the slider at value '0'.")]
        private Vector2 _minPosition = new(-0.42f, -0.42f);

        [SerializeField]
        [Tooltip("The maximum of the slider at value '1'.")]
        private Vector2 _maxPosition = new(0.42f, 0.42f);

        [Space]

        /// <summary>
        /// Keep the offset to the handle when grabbed. Use for large handles.
        /// </summary>
        [SerializeField]
        [Tooltip("Keep the offset to the handle when grabbed. Use for large handles.")]
        protected bool _useHandleGrabOffset;

        [SerializeField]
        [Tooltip("The object that is visually grabbed and used to manipulate the xz-plane.")]
        private Transform _handle = null;

        private Vector3 _grabOffset;

        /// <inheritdoc />
        public override Vector2 GetVisualizedValue(IXRSelectInteractable interactable, IXRSelectInteractor interactor)
        {
            // Put anchor position into slider space
            Vector3 localPosition = GetInteractorLocalPosition(interactable, interactor) - _grabOffset;

            float sliderXValue = Mathf.Clamp01((localPosition.x - _minPosition.x) / (_maxPosition.x - _minPosition.x));
            float sliderZValue = Mathf.Clamp01((localPosition.z - _minPosition.y) / (_maxPosition.y - _minPosition.y));

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
            handlePos.x = Mathf.Lerp(_minPosition.x, _maxPosition.x, value.x);
            handlePos.z = Mathf.Lerp(_minPosition.y, _maxPosition.y, value.y);
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
            Vector3 boxSize = new(_maxPosition.x - _minPosition.x, 0.0f, _maxPosition.y - _minPosition.y);

            GizmoUtils.DrawMinMaxLine(
                new Vector3(_minPosition.x, handleYOffset, 0.0f),
                new Vector3(_maxPosition.x, handleYOffset, 0.0f),
                Color.red,
                Color.green,
                Color.blue,
                Vector3.up,
                atTransform
            );

            GizmoUtils.DrawMinMaxLine(
                new Vector3(0.0f, handleYOffset, _minPosition.y),
                new Vector3(0.0f, handleYOffset, _maxPosition.y),
                Color.magenta,
                Color.cyan,
                Color.blue,
                Vector3.up,
                atTransform
            );

            Gizmos.matrix = atTransform.localToWorldMatrix;
            Gizmos.DrawWireCube(new Vector3(0.0f, handleYOffset, 0.0f), boxSize);
        }
    }
}