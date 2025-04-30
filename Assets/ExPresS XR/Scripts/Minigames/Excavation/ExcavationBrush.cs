using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.Excavation
{
    public class ExcavationBrush : MonoBehaviour
    {
        /// <summary>
        /// The size of the brush.
        /// </summary>
        [SerializeField]
        [Tooltip("The size of the brush.")]
        private float _brushSize = 10.0f;

        /// <summary>
        /// The color channel to be used to draw with this brush.
        /// </summary>
        [SerializeField]
        [Tooltip("The color channel to be used to draw with this brush.")]
        private ColorChannel.Channels _drawChannel = ColorChannel.Channels.R;

        /// <summary>
        /// Minimum distance to the target area to be able to draw.
        /// </summary>
        [SerializeField]
        [Tooltip("Minimum distance to the target area to be able to draw.")]
        private float _minDrawDistance = 0.0f;

        /// <summary>
        /// Maximum distance to the target area to be able to draw.
        /// </summary>
        [SerializeField]
        [Tooltip("Maximum distance to the target area to be able to draw.")]
        private float _maxDrawDistance = 0.09f;

        /// <summary>
        /// Radius of the sphere cast to detect the area.
        /// </summary>
        [SerializeField]
        [Tooltip("Radius of the sphere cast to detect the area.")]
        private float _brushSphereCastRadius = 0.04f;

        /// <summary>
        /// Maximum opacity at minimal distance.
        /// </summary
        [SerializeField]
        [Range(0.0f, 1.0f)]
        [Tooltip("Maximum opacity at minimal distance.")]
        private float _maxDrawStrength = 0.3f;

        private RaycastHit _hit;
        private bool _hasHit;
        private bool _hasExcavationAreaHit;

        // Events

        /// <summary>
        /// Emitted once brushing starts.
        /// </summary>
        public UnityEvent OnBrushingStarted;

        /// <summary>
        /// Emitted once brushing ends.
        /// </summary>
        public UnityEvent OnBrushingStopped;


        private void OnDisable()
        {
            // Brushing also stops when the brush gets disabled. Duh!
            _hasExcavationAreaHit = false;
            OnBrushingStopped.Invoke();
        }

        private void Update()
        {
            bool hadTargetHit = _hasExcavationAreaHit;
            Vector3 startPos = transform.position + transform.up * _minDrawDistance;
            float drawDist = _maxDrawDistance - _minDrawDistance;
            _hasHit = Physics.SphereCast(startPos, _brushSphereCastRadius, transform.up, out _hit, drawDist);
            ExcavationArea area = null; // Define the variable here to avoid liner errors
            _hasExcavationAreaHit = _hasHit && _hit.transform.TryGetComponent(out area);
 
            if (_hasExcavationAreaHit)
            {
                Vector2 excavatePos = _hit.textureCoord;
                float strength = GetExcavationStrength(_hit.distance);
                Color color = ColorChannel.GetColorWithChannelValue(_drawChannel, 1.0f, strength);
                area.ExcavateAt(excavatePos, color, _brushSize);
            }

            if (_hasExcavationAreaHit != hadTargetHit)
            {
                (_hasExcavationAreaHit ? OnBrushingStarted : OnBrushingStopped).Invoke();
            }
        }

        private float GetExcavationStrength(float rawDistance)
        {
            // Get distance pct between min and max distance
            float rawStrength = Mathf.InverseLerp(_minDrawDistance, _maxDrawDistance, rawDistance);
            // Clamp strength and invert
            return Mathf.Clamp(1.0f - rawStrength, 0.0f, _maxDrawStrength);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = _hasHit ? Color.green : Color.white;
            Vector3 startPoint = transform.position + transform.up * _minDrawDistance;
            Vector3 maxEndPoint = transform.position + transform.up * _maxDrawDistance;
            Gizmos.DrawLine(startPoint, maxEndPoint);
            if (_hasHit)
            {
                Gizmos.DrawWireSphere(_hit.point, _brushSphereCastRadius);
            }
            else
            {
                Gizmos.DrawWireSphere(maxEndPoint, _brushSphereCastRadius);
            }
        }
    }
}