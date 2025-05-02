using System;
using ExPresSXR.Misc;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


namespace ExPresSXR.Interaction.ValueRangeInteractable
{
    /// <summary>
    /// Defines a turn interactable that also translates up and down.
    /// </summary>
    public class ScrewTurnInteractable : ValueRangeInteractable<Float01Descriptor, ScrewVisualizer, float> { }


    /// <summary>
    /// Defines the visualization for a turn interactable that also translates up and down.
    /// </summary>
    [Serializable]
    public class ScrewVisualizer : TurnVisualizer
    {
        [Space]

        [SerializeField]
        [Tooltip("The offset of the screw at value '0' along the y-axis.")]
        protected float _minPosition = -1.0f;

        [SerializeField]
        [Tooltip("The offset of the screw at value '1' along the y-axis.")]
        protected float _maxPosition = 1.0f;


        /// <inheritdoc />
        public override void UpdateVisualization(float value, IXRSelectInteractable interactable)
        {
            base.UpdateVisualization(value, interactable);

            // No need to add a warning message as it will be emitted by the parent
            if (_pivot != null)
            {
                Vector3 pivotPos = _pivot.transform.localPosition;
                pivotPos.y = Mathf.Lerp(_minPosition, _maxPosition, value);
                _pivot.transform.localPosition = pivotPos;
            }
        }

        /// <inheritdoc />
        public override void DrawGizmos(Transform atTransform, float value)
        {
            base.DrawGizmos(atTransform, value);

            GizmoUtils.DrawMinMaxValueLine(
                new Vector3(0.0f, _minPosition, 0.0f),
                new Vector3(0.0f, _maxPosition, 0.0f),
                value,
                Color.magenta,
                Color.cyan,
                Color.blue,
                Color.yellow,
                Vector3.right,
                atTransform
            );
        }
    }
}