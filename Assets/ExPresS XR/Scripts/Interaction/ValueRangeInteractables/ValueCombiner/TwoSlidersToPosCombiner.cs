using ExPresSXR.Misc;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class is used to combine the values of two (float) slider
/// as there seems to be no easy way of adding multiple sliders as 
/// of determining which handle is currently selected. The result is
/// that both sliders always move.
/// Instead we use two float sliders and this script to combine them.
/// </summary>

namespace ExPresSXR.Interaction.ValueRangeInteractable.ValueCombiner
{
    public class TwoSlidersToPosCombiner : MonoBehaviour
    {
        /// <summary>
        /// Value that is manipulated by a slider each for the x- and z-axis.
        /// The value is a Vector3 to be used directly as position.
        /// </summary>
        [SerializeField]
        [Tooltip("Value that is manipulated by a slider each for the x- and z-axis. The value is a Vector3 to be used directly as position.")]
        private Vector3 _value;
        public Vector3 Value
        {
            get => _value;
            set
            {
                _value = value;
                OnNewPosition.Invoke(_value);
            }
        }

        /// <summary>
        /// Slider used to manipulate the x-axis.
        /// </summary>
        [SerializeField]
        [Tooltip("Slider used to manipulate the x-axis.")]
        private Slider _xSlider;

        /// <summary>
        /// Slider used to manipulate the z-axis.
        /// </summary>
        [SerializeField]
        [Tooltip("Slider used to manipulate the z-axis.")]
        private Slider _ySlider;

        /// <summary>
        /// The y-offset of the position returned.
        /// </summary>
        [SerializeField]
        [Tooltip("The y-offset of the position returned.")]
        private float _height = 0.0f;

        /// <summary>
        /// The minimum offset of the slider at value '0'.
        /// </summary>
        [SerializeField]
        [Tooltip("The minimum offset of the slider at value '0'.")]
        private Vector2 _minPosition = new(-0.42f, -0.42f);

        /// <summary>
        /// The maximum offset of the slider at value '1'.
        /// </summary>
        [SerializeField]
        [Tooltip("The maximum offset of the slider at value '1'.")]
        private Vector2 _maxPosition = new(0.42f, 0.42f);

        /// <summary>
        /// Event emitted when the value changes.
        /// </summary>
        public UnityEvent<Vector3> OnNewPosition;

        /// <summary>
        /// Adds listeners to the value change event of each slider and sets the initial value.
        /// </summary>
        private void OnEnable()
        {
            _xSlider.OnValueChanged.AddListener(HandleValueChanged);
            _ySlider.OnValueChanged.AddListener(HandleValueChanged);

            // Make sure he value is set to the value of the sliders
            // The parameters will be ignored anyway..
            HandleValueChanged(0.0f, 0.0f);
        }

        /// <summary>
        /// Removes the listeners to the value change event of each slider.
        /// </summary>
        private void OnDisable()
        {
            _xSlider.OnValueChanged.RemoveListener(HandleValueChanged);
            _ySlider.OnValueChanged.RemoveListener(HandleValueChanged);
        }

        /// <summary>
        /// Callback that updates the value based on the values of each slider.
        /// </summary>
        /// <param name="_newV">Ignored.</param>
        /// <param name="_oldV">Ignored.</param>
        private void HandleValueChanged(float _newV, float _oldV)
        {
            float xValue = Mathf.Lerp(_minPosition.x, _maxPosition.y, _xSlider.Value);
            float yValue = Mathf.Lerp(_minPosition.y, _maxPosition.y, _ySlider.Value);
            Value = new(xValue, _height, yValue);
        }

        /// <summary>
        /// Draws the slider area and min/max positions.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Vector3 boxSize = new(_maxPosition.x - _minPosition.x, 0.0f, _maxPosition.y - _minPosition.y);

            GizmoUtils.DrawMinMaxLine(
                new Vector3(_minPosition.x, 0.0f, 0.0f),
                new Vector3(_maxPosition.x, 0.0f, 0.0f),
                Color.red,
                Color.green,
                Color.blue,
                Vector3.up,
                transform
            );

            GizmoUtils.DrawMinMaxLine(
                new Vector3(0.0f, 0.0f, _minPosition.y),
                new Vector3(0.0f, 0.0f, _maxPosition.y),
                Color.magenta,
                Color.cyan,
                Color.blue,
                Vector3.up,
                transform
            );

            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(new Vector3(0.0f, 0.0f, 0.0f), boxSize);
        }
    }
}