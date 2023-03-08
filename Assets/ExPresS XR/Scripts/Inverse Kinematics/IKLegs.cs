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

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIdx)
    {
        AvatarIKGoal[] feet = new AvatarIKGoal[] { 
            AvatarIKGoal.LeftFoot, 
            AvatarIKGoal.RightFoot
        };

        foreach(AvatarIKGoal foot in feet)
        {
            Vector3 footPosition = animator.GetIKPosition(foot);
            Physics.Raycast(footPosition + Vector3.up, Vector3.down, out RaycastHit hit, MAX_RAYCAST_DISTANCE, _layerMask);
            if (hit.collider != null)
            {
                animator.SetIKPositionWeight(foot, 1);
                animator.SetIKPosition(foot, hit.point + (Vector3.up * _feetOffset));
            }
        }
    }
}