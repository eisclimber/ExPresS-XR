using System;
using ExPresSXR.Misc;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


namespace ExPresSXR.Interaction.ValueRangeInteractable
{
    /// <summary>
    /// Defines a three dimensional slider interactable.
    /// </summary>
    public class Slider3D : ValueRangeInteractable<Vector3Descriptor, Slider3DVisualizer, Vector3>
    {
        protected override void StartGrab(SelectEnterEventArgs args)
        {
            base.StartGrab(args);
            _valueVisualizer.SetHandleGrabOffsetWithInteraction(args.interactableObject, args.interactorObject);
        }
    }


    /// <summary>
    /// Defines the visualization for a three dimensional slider interactable.
    /// </summary>
    [Serializable]
    public class Slider3DVisualizer : ValueVisualizer<Vector3>
    {
        /// <summary>
        /// The minimum offset of the slider at value '0'.
        /// </summary>
        [SerializeField]
        [Tooltip("The minimum offset of the slider at value '0'.")]
        private Vector3 _minPosition = new(-0.42f, -0.42f, -0.42f);

        /// <summary>
        /// The maximum of the slider at value '1'.
        /// </summary>
        [SerializeField]
        [Tooltip("The maximum of the slider at value '1'.")]
        private Vector3 _maxPosition = new(0.42f, 0.42f, 0.42f);

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

            float sliderXValue = Mathf.Clamp01((localPosition.x - _minPosition.x) / (_maxPosition.x - _minPosition.x));
            float sliderYValue = Mathf.Clamp01((localPosition.y - _minPosition.y) / (_maxPosition.y - _minPosition.y));
            float sliderZValue = Mathf.Clamp01((localPosition.z - _minPosition.z) / (_maxPosition.z - _minPosition.z));

            return new(sliderXValue, sliderYValue, sliderZValue);
        }

        /// <inheritdoc />
        public override void UpdateVisualization(Vector3 value, IXRSelectInteractable interactable)
        {
            if (_handle == null)
            {
                Debug.LogWarning($"No reference to a handle provided. Can not visualize the value '{value}' anything without it.");
                return;
            }

            Vector3 handlePos = _handle.localPosition;
            handlePos.x = Mathf.Lerp(_minPosition.x, _maxPosition.x, value.x);
            handlePos.y = Mathf.Lerp(_minPosition.y, _maxPosition.y, value.y);
            handlePos.z = Mathf.Lerp(_minPosition.z, _maxPosition.z, value.z);
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
        public override void DrawGizmos(Transform atTransform, Vector3 value)
        {
            GizmoUtils.DrawMinMaxLine(
                new Vector3(_minPosition.x, 0.0f, 0.0f),
                new Vector3(_maxPosition.x, 0.0f, 0.0f),
                Color.red,
                Color.green,
                Color.blue,
                Vector3.up,
                atTransform
            );

            GizmoUtils.DrawMinMaxLine(
                new Vector3(0.0f, _minPosition.y, 0.0f),
                new Vector3(0.0f, _maxPosition.y, 0.0f),
                Color.black,
                Color.white,
                Color.blue,
                Vector3.up,
                atTransform
            );

            GizmoUtils.DrawMinMaxLine(
                new Vector3(0.0f, 0.0f, _minPosition.z),
                new Vector3(0.0f, 0.0f, _maxPosition.z),
                Color.magenta,
                Color.cyan,
                Color.blue,
                Vector3.up,
                atTransform
            );

            Gizmos.matrix = atTransform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, _maxPosition - _minPosition);
        }
    }
}