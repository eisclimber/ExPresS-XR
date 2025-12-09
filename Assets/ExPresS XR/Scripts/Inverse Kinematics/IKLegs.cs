using UnityEngine;

public class IKLegs : MonoBehaviour
{
    private const float MAX_RAYCAST_DISTANCE = 2.5f;

    // Corresponds to default layer mask = Default
    private const int FLOOR_LAYER_MASK = 1;

    [Tooltip("The height offset added to the heel of the feet.")]
    [SerializeField]
    private float _feetOffset;

    [Tooltip("LayerMask to determine which Layers are considered ground for the feet.")]
    [SerializeField]
    private LayerMask _layerMask = FLOOR_LAYER_MASK;

    private Animator _animator;

    private void Start()
    {
        if (!TryGetComponent(out _animator))
        {
            Debug.Log("Did not find an animator for the IK Legs to animate.", this);
        }
    }

    private void OnAnimatorIK(int layerIdx)
    {
        AvatarIKGoal[] feet = new AvatarIKGoal[] { 
            AvatarIKGoal.LeftFoot, 
            AvatarIKGoal.RightFoot
        };

        foreach(AvatarIKGoal foot in feet)
        {
            Vector3 footPosition = _animator.GetIKPosition(foot);
            Physics.Raycast(footPosition + Vector3.up, Vector3.down, out RaycastHit hit, MAX_RAYCAST_DISTANCE, _layerMask);
            if (hit.collider != null)
            {
                _animator.SetIKPositionWeight(foot, 1);
                _animator.SetIKPosition(foot, hit.point + (Vector3.up * _feetOffset));
            }
        }
    }
}