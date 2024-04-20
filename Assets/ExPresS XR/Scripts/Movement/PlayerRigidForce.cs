using UnityEngine;

namespace ExPresSXR.Movement
{
    /// <summary>
    /// Applies a impulses on the player while still allowing teleportation, climbing, other movement. The applied impulse will be reduced over time.
    /// </summary>
    public class PlayerRigidForce : PlayerForceBase
    {
        /// <summary>
        /// Whether or not the force should be applied.
        /// </summary>
        [SerializeField]
        private bool _applyForce = true;
        public bool applyForce
        {
            get => _applyForce;
            set => _applyForce = value;
        }

        /// <summary>
        /// How much the player slows down when in the air.
        /// </summary>
        [SerializeField]
        private float _airLerpFactor = 0.05f;

        /// <summary>
        /// How much the player slows down when on the ground.
        /// </summary>
        [SerializeField]
        private float _floorLerpFactor = 0.1f;

        private void Update()
        {
            if (_characterController != null && _applyForce && !forceTemporarilyDisabled)
            {
                float lerpFactor = _characterController.isGrounded ? _floorLerpFactor : _airLerpFactor;
                _currentVelocity = Vector3.Lerp(_currentVelocity, Vector3.zero, lerpFactor);
                _characterController.Move(_currentVelocity * Time.deltaTime);
            }
        }

        /// <summary>
        /// Applies a horizontal impulse.
        /// </summary>
        /// <param name="impulse">Impulse to be applied.</param>
        public void ApplyImpulse(Vector3 impulse) => _currentVelocity += impulse;

        /// <summary>
        /// Applies a horizontal impulse, ignoring vertical forces.
        /// </summary>
        /// <param name="impulse">Impulse to be applied.</param>
        public void ApplyHorizontalImpulse(Vector3 impulse) => ApplyImpulse(new Vector3(impulse.x, 0.0f, impulse.z));

        /// <summary>
        /// Applies a horizontal impulse, ignoring vertical forces if it is negative.
        /// </summary>
        /// <param name="impulse">Impulse to be applied.</param>
        public void ApplyImpulseUpperHalfSphere(Vector3 impulse) => ApplyImpulse(new Vector3(impulse.x, Mathf.Max(impulse.y, 0.0f), impulse.z));
    }
}