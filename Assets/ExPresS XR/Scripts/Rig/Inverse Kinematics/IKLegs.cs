using UnityEngine;

namespace ExPresSXR.Experimentation.EyeTracking
{
    /// <summary>
    /// Simple IK controller to place the bodies feet on the floor.
    /// </summary>
    public class IKLegs : MonoBehaviour
    {
        /// <summary>
        /// The maximum length to cast the raycast (= max body size).
        /// </summary>
        private const float MAX_RAYCAST_DISTANCE = 2.5f;

        /// <summary>
        /// Corresponds to default layer mask (= Default).
        /// </summary>
        private const int FLOOR_LAYER_MASK = 1;

        /// <summary>
        /// The height offset added to the heel of the feet.
        /// </summary>
        [SerializeField]
        [Tooltip("The height offset added to the heel of the feet.")]
        private float _feetOffset;

        /// <summary>
        /// LayerMask to determine which Layers are considered ground for the feet.
        /// </summary>
        [SerializeField]
        [Tooltip("LayerMask to determine which Layers are considered ground for the feet.")]
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

            foreach (AvatarIKGoal foot in feet)
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
}