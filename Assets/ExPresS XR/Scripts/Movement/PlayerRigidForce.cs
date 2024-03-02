using UnityEngine;
using UnityEngine.Localization.SmartFormat.Utilities;

namespace ExPresSXR.Movement
{
    public class PlayerRigidForce : PlayerForceBase
    {
        [SerializeField]
        private bool _applyForce;
        public bool applyForce
        {
            get => _applyForce;
            set => _applyForce = value;
        }

        [SerializeField]
        private float _airLerpFactor = 0.1f;

        [SerializeField]
        private float _floorLerpFactor = 0.4f;


        public void Update()
        {
            if (_characterController != null && _applyForce && !forceTemporarilyDisabled)
            {
                float lerpFactor = _characterController.isGrounded ? _floorLerpFactor : _airLerpFactor;
                _currentVelocity = Vector3.Lerp(_currentVelocity, Vector3.zero, lerpFactor);
                _characterController.Move(_currentVelocity * Time.deltaTime);
            }
        }

        [ContextMenu("Apply Test Force")]
        public void Test() => ApplyHorizontalImpulse(Vector3.back * 10.0f);

        public void ApplyHorizontalImpulse(Vector3 impulse) => ApplyImpulse(new Vector3(impulse.x, 0.0f, impulse.z));

        public void ApplyImpulseUpperHalfSphere(Vector3 impulse) => ApplyImpulse(new Vector3(impulse.x, Mathf.Max(impulse.y, 0.0f), impulse.z));

        public void ApplyImpulse(Vector3 impulse) => _currentVelocity += impulse;
    }
}