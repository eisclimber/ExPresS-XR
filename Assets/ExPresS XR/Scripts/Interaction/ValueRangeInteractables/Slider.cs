using System;
using ExPresSXR.Misc;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace ExPresSXR.Interaction.ValueRangeInteractable
{
    /// <summary>
    /// Defines a slider interactable along the x-axis. Please avoid rotating the interactable!
    /// </summary>
    public class Slider : ValueRangeInteractable<Float01Descriptor, SliderVisualizer, float>
    {
        protected override void StartGrab(SelectEnterEventArgs args)
        {
            base.StartGrab(args);
            _valueVisualizer.SetHandleGrabOffsetWithInteraction(args.interactableObject, args.interactorObject);
        }
    }


    /// <summary>
    /// Defines the visualization for a slider interactable along the x-axis.
    /// </summary>    
    [Serializable]
    public class SliderVisualizer : ValueVisualizer<float>
    {
        [SerializeField]
        [Tooltip("The offset of the slider at value '0' along the x-axis.")]
        protected float _minPosition = -0.42f;

        [SerializeField]
        [Tooltip("The offset of the slider at value '1' along the x-axis.")]
        protected float _maxPosition = 0.42f;

        [Space]

        /// <summary>
        /// Keep the offset to the handle when grabbed. Use for large handles.
        /// </summary>
        [SerializeField]
        [Tooltip("Keep the offset to the handle when grabbed. Use for large handles.")]
        protected bool _useHandleGrabOffset;

        [SerializeField]
        [Tooltip("The object that is visually grabbed and manipulated.")]
        protected Transform _handle = null;


        private Vector3 _grabOffset;


        /// <inheritdoc />
        public override float GetVisualizedValue(IXRSelectInteractable interactable, IXRSelectInteractor interactor)
        {
            Vector3 localPosition = GetInteractorLocalPosition(interactable, interactor) - _grabOffset;
            return Mathf.Clamp01((localPosition.x - _minPosition) / (_maxPosition - _minPosition));
        }

        /// <inheritdoc />
        public override void UpdateVisualization(float value, IXRSelectInteractable interactable)
        {
            if (_handle == null)
            {
                Debug.LogWarning($"No reference to a handle provided. Can not visualize the value '{value}' anything without it.");
                return;
            }

            Vector3 handlePos = _handle.localPosition;
            handlePos.x = Mathf.Lerp(_minPosition, _maxPosition, value);
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
        public override void DrawGizmos(Transform atTransform, float value)
        {
            Vector3 handleOffset = _handle != null ? _handle.localPosition : Vector3.zero;
            handleOffset.x = 0.0f;

            GizmoUtils.DrawMinMaxValueLine(
                new Vector3(_minPosition, 0.0f, 0.0f) + handleOffset,
                new Vector3(_maxPosition, 0.0f, 0.0f) + handleOffset,
                value,
                Color.green,
                Color.red,
                Color.blue,
                Color.yellow,
                Vector3.up,
                atTransform
            );
        }
    }
}