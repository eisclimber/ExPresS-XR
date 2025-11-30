using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ExPresSXR.Interaction.ValueRangeInteractable
{
    /// <summary>
    /// Defines a button interactable.
    /// </summary>
    public class Button : ValueRangeInteractable<Float01Descriptor, ButtonVisualizer, float>
    {
        protected override void UpdateValueWithHover() => Value = _valueVisualizer.GetVisualizedValue(this, _selectInteractor);

        // Disable grabbing the button
        protected override void UpdateValueWithGrab() {}
    }

    /// <summary>
    /// Defines the visualization for a slider interactable along the x-axis.
    /// </summary>    
    [Serializable]
    public class ButtonVisualizer : ValueVisualizer<float>
    {
        [SerializeField]
        [Tooltip("The offset of the slider at value '0' along the x-axis.")]
        protected float _minPosition = 0.0f;

        [SerializeField]
        [Tooltip("The offset of the slider at value '1' along the x-axis.")]
        protected float _maxPosition = 0.42f;

        [SerializeField]
        [Tooltip("The object that is visually grabbed and manipulated.")]
        protected Transform _buttonCap = null;


        /// <inheritdoc />
        public override float GetVisualizedValue(IXRSelectInteractable interactable, IXRSelectInteractor interactor)
        {
            Vector3 localPosition = GetInteractorLocalPosition(interactable, interactor);
            return Mathf.Clamp01((localPosition.y - _minPosition) / (_maxPosition - _minPosition));
        }

        /// <inheritdoc />
        public override void UpdateVisualization(float value, IXRSelectInteractable interactable)
        {
            // if (_handle == null)
            // {
            //     Debug.LogWarning($"No reference to a handle provided. Can not visualize the value '{value}' anything without it.");
            //     return;
            // }

            // Vector3 handlePos = _handle.localPosition;
            // handlePos.x = Mathf.Lerp(_minPosition, _maxPosition, value);
            // _handle.localPosition = handlePos;
        }

        // private float GetLocalYPosition(Vector3 position)
        // {
        //     Vector3 localPosition = transform.root.InverseTransformPoint(position);
        //     return localPosition.y;
        // }

        // private void SetLocalYPosition(float position)
        // {
        //     Vector3 newPosition = pushAnchor.localPosition;
        //     newPosition.y = Mathf.Clamp(position, _yMin, _yMax);
        //     pushAnchor.localPosition = newPosition;
        // }

        /// <inheritdoc />
        public override void DrawGizmos(Transform atTransform, float value)
        {
            // Vector3 handleOffset = _handle != null ? _handle.localPosition : Vector3.zero;
            // handleOffset.x = 0.0f;

            // GizmoUtils.DrawMinMaxValueLine(
            //     new Vector3(_minPosition, 0.0f, 0.0f) + handleOffset,
            //     new Vector3(_maxPosition, 0.0f, 0.0f) + handleOffset,
            //     value,
            //     Color.green,
            //     Color.red,
            //     Color.blue,
            //     Color.yellow,
            //     Vector3.up,
            //     atTransform
            // );
        }
    }
}