using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace ExPresSXR.Movement
{
    public class RigidMovementProvider : ActionBasedContinuousMoveProvider
    {
        public const float MIN_VELOCITY_THRESHOLD = 0.1f;

        // Because the one from the base class is not  the one used is not accessible...
        [field: SerializeField]
        public CharacterController characterController;

        [SerializeField]
        private Vector3 _velocity;

        [SerializeField]
        private float _lerpFactor = 0.03f;


        [ContextMenu("Test")]
        public void Test() => ApplyHorizontalImpulse(Vector3.back * 3.0f);

        public void ApplyHorizontalImpulse(Vector3 impulse)
        {
            Debug.Log("Vel applied: " + new Vector3(impulse.x, 0.0f, impulse.z));
            _velocity += new Vector3(impulse.x, 0.0f, impulse.z);
        }

        public void ApplyImpulse(Vector3 impulse) => _velocity += impulse;

        protected override Vector3 ComputeDesiredMove(Vector2 input)
        {
            Vector3 desiredMove = base.ComputeDesiredMove(input);
            UpdateVelocity(desiredMove);
            desiredMove += _velocity * Time.deltaTime;

            return desiredMove;
        }

        protected virtual void UpdateVelocity(Vector3 joystickInput)
        {
            _velocity = Vector3.Lerp(_velocity, Vector3.zero, _lerpFactor);
            _velocity.y = 0.0f;
            // Set _velocity to zero if it is almost there
            if (_velocity.sqrMagnitude <= MIN_VELOCITY_THRESHOLD)
            {
                _velocity = Vector3.zero;
            }
        }
    }
}