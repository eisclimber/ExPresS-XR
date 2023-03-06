using UnityEngine;

public class IKLegs : MonoBehaviour
{
    private const float MAX_RAYCAST_DISTANCE = 2.5f;

    [SerializeField]
    private float _feetOffset;

    [SerializeField]
    private LayerMask layerMask;

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
            Physics.Raycast(footPosition + Vector3.up, Vector3.down, out RaycastHit hit, MAX_RAYCAST_DISTANCE, layerMask);
            animator.SetIKPositionWeight(foot, 1);
            animator.SetIKPosition(foot, hit.point + (Vector3.up * _feetOffset));
        }
    }
}