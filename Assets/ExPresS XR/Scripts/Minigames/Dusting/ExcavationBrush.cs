using ExPResSXR.Minigames.ExcavationGame;
using UnityEngine;
using UnityEngine.Events;

public class ExcavationBrush : MonoBehaviour
{
    // [SerializeField]
    // private float _brushSize = 0.1f;

    [SerializeField]
    private ColorChannel.Channels _drawChannel = ColorChannel.Channels.R;


    [SerializeField]
    [Tooltip("Min distance to the target area, must be larger than the ")]
    private float _minDrawDistance = 0.0f;

    [SerializeField]
    [Tooltip("Max distance to the target area. After which nothing will be drawn.")]
    private float _maxDrawDistance = 0.09f;

    [SerializeField]
    [Tooltip("Radius of the sphere cast to detect the area.")]
    private float _brushSphereCastRadius = 0.04f;


    [SerializeField]
    [Range(0.0f, 1.0f)]
    [Tooltip("Maximum opacity at minimal distance")]
    private float _maxDrawStrength = 0.3f;

    [SerializeField]
    private ExcavationArea _area;


    private RaycastHit _hit;
    private bool _hasHit;
    private bool _hasTargetHit;


    public UnityEvent OnBrushingStarted;
    public UnityEvent OnBrushingStopped;


    private void OnDisable()
    {
        // Brushing also stops when the brush gets disabled. Duh!
        _hasTargetHit = false;
        OnBrushingStopped.Invoke();
    }

    private void Update()
    {
        bool hadTargetHit = _hasTargetHit;
        _hasHit = Physics.SphereCast(transform.position, _brushSphereCastRadius, -transform.right, out _hit, _maxDrawDistance);
        _hasTargetHit = _hasHit && _hit.transform == _area.transform;

        if (_hasTargetHit)
        {
            Vector2 excavatePos = _hit.textureCoord;
            float strength = GetExcavationStrength(_hit.distance);
            Color color = ColorChannel.GetColorWithChannelValue(_drawChannel, 1.0f, strength);

            _area.ExcavateAt(excavatePos, color);
        }

        if (_hasTargetHit != hadTargetHit)
        {
            (_hasHit ? OnBrushingStarted : OnBrushingStopped).Invoke();
        }
    }

    private float GetExcavationStrength(float rawDistance)
    {
        // Get distance pct between min and max distance
        float rawStrength = Mathf.InverseLerp(_minDrawDistance, _maxDrawDistance, rawDistance);
        // Clamp strength and inverse
        return Mathf.Clamp(1.0f - rawStrength, 0.0f, _maxDrawStrength);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = _hasHit ? Color.green : Color.white;
        Vector3 maxEndPoint = transform.position - transform.right * _maxDrawDistance;
        Gizmos.DrawLine(transform.position, maxEndPoint);
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
