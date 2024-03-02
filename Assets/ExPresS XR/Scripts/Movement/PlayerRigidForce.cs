using UnityEngine;

namespace ExPresSXR.Movement
{
    public class PlayerRigidForce : PlayerForceBase
    {
        [SerializeField]
        private bool _applyForce = true;
        public bool applyForce
        {
            get => _applyForce;
            set => _applyForce = value;
        }

        [SerializeField]
        private float _airLerpFactor = 0.05f;

        [SerializeField]
        private float _floorLerpFactor = 0.1f;


        public void Update()
        {
            if (_characterController != null && _applyForce && !forceTemporarilyDisabled)
            {
                float lerpFactor = _characterController.isGrounded ? _floorLerpFactor : _airLerpFactor;
                _currentVelocity = Vector3.Lerp(_currentVelocity, Vector3.zero, lerpFactor);
                _characterController.Move(_currentVelocity * Time.deltaTime);
            }
        }

        public void ApplyHorizontalImpulse(Vector3 impulse) => ApplyImpulse(new Vector3(impulse.x, 0.0f, impulse.z));

        public void ApplyImpulseUpperHalfSphere(Vector3 impulse) => ApplyImpulse(new Vector3(impulse.x, Mathf.Max(impulse.y, 0.0f), impulse.z));

        public void ApplyImpulse(Vector3 impulse) => _currentVelocity += impulse;
    }
}