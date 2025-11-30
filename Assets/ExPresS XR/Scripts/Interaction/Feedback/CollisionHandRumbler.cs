using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

namespace ExPresSXR.Interaction.Feedback
{
    /// <summary>
    /// Rumbles the given controllers when the Rigidbody of their model collides with this object. 
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class CollisionHandRumbler : MonoBehaviour
    {
        [SerializeField]
        private RumbleDescription _rumble;
        public RumbleDescription Rumble
        {
            get => _rumble;
            set => _rumble = value;
        }

        [Space]

        [SerializeField]
        private HapticImpulsePlayer _leftHapticPlayer;
        public HapticImpulsePlayer LeftHapticPlayer
        {
            get => _leftHapticPlayer;
            set => _leftHapticPlayer = value;
        }

        [SerializeField]
        private HapticImpulsePlayer _rightHapticPlayer;
        public HapticImpulsePlayer RightXrController
        {
            get => _rightHapticPlayer;
            set => _rightHapticPlayer = value;
        }

        private int _leftCollisions;
        private int _rightCollisions;

        private void Awake()
        {
            if (_leftHapticPlayer == null && _rightHapticPlayer == null)
            {
                Debug.LogWarning("No XR Controller was provided. This Component will have no effect.");
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (IsControllerTransformMatch(other, _leftHapticPlayer))
            {
                if (_leftCollisions <= 0)
                {
                    RumbleUtility.PerformRumble(_rumble, _leftHapticPlayer);
                }
                _leftCollisions++;
            }

            if (IsControllerTransformMatch(other, _rightHapticPlayer))
            {
                if (_rightCollisions <= 0)
                {
                    RumbleUtility.PerformRumble(_rumble, _rightHapticPlayer);
                }
                _rightCollisions++;
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (IsControllerTransformMatch(other, _leftHapticPlayer))
            {
                _leftCollisions = Mathf.Max(_leftCollisions - 1, 0);
            }

            if (IsControllerTransformMatch(other, _rightHapticPlayer))
            {
                _rightCollisions = Mathf.Max(_rightCollisions - 1, 0);
            }
        }


        private bool IsControllerTransformMatch(Collision col, HapticImpulsePlayer compareHaptics)
        {
            HapticImpulsePlayer colHaptics = RumbleUtility.FindHapticsOfTransform(col.rigidbody.transform);
            return colHaptics != null && colHaptics == compareHaptics;
        }
    }
}