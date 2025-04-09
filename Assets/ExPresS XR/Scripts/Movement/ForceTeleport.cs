using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using ExPresSXR.Rig;

namespace ExPresSXR.Movement
{
    public class ForceTeleport : MonoBehaviour
    {
        [SerializeField]
        private ExPresSXRRig _rig;

        [SerializeField]
        private Transform _cameraTransform;

        [SerializeField]
        private Transform _defaultTarget;

        private CharacterController _playerController;

        private bool _pendingTeleport;
        private Vector3 _pendingTeleportPosition;
        private Quaternion _pendingTeleportRotation;


        public UnityEvent OnForceTeleport;
        public UnityEvent OnFullyFaded;


        private void Start()
        {
            _playerController = _rig.GetComponent<CharacterController>();
        }

        private void OnEnable()
        {
            _rig.fadeRect.OnFadeToColorCompleted.AddListener(OnFadeToColorCompleted);
        }


        private void OnDisable()
        {
            _rig.fadeRect.OnFadeToColorCompleted.RemoveListener(OnFadeToColorCompleted);
        }

        public void DefaultTeleportTo() => TeleportTo(_defaultTarget, false);
        public void DefaultTeleportToWithFade() => TeleportTo(_defaultTarget, true);

        public void TeleportTo(Transform target) => TeleportTo(target, false);
        public void TeleportToWithFade(Transform target) => TeleportTo(target, true);
        
        public void TeleportTo(Transform target, bool fade)
        {
            if (target != null)
            {
                TeleportTo(target.position, target.rotation, fade);
            }
            {
                TeleportTo(Vector3.zero, Quaternion.identity, fade);
            }
        } 
        
        public void TeleportTo(Vector3 targetPosition, Quaternion targetRotation, bool fade)
        {
            if (fade)
            {
                _pendingTeleport = true;
                _pendingTeleportPosition = targetPosition;
                _pendingTeleportRotation = targetRotation;
                _rig.FadeToColor();
            }
            else
            {
                // Just to be sure, skip any fades
                PerformTeleport(targetPosition, targetRotation);
            }
        }

        public void CancelTeleport()
        {
            if (_pendingTeleport)
            {
                _pendingTeleport = false;
                _rig.FadeToClear(true);
            }
        }

        private void PerformTeleport(Vector3 targetPosition, Quaternion targetRotation)
        {
            // We need to exclude all collisions as these block the movement during teleport
            LayerMask preLayers = _playerController.excludeLayers;
            _playerController.excludeLayers = -1;
            // Actually move/rotate the player
            _playerController.Move(targetPosition - _rig.transform.position);
            Quaternion cameraRotation = Quaternion.Euler(0.0f, _cameraTransform.localEulerAngles.y, 0.0f);
            _rig.transform.rotation = targetRotation * Quaternion.Inverse(cameraRotation);
            // Enable collisions and do other post teleport stuff
            _playerController.excludeLayers = preLayers;
            _pendingTeleport = false;
            OnForceTeleport.Invoke();
        }

        private void OnFadeToColorCompleted()
        {
            if (_pendingTeleport)
            {
                PerformTeleport(_pendingTeleportPosition, _pendingTeleportRotation);
                OnFullyFaded.Invoke();
                _rig.FadeToClear();
            }
        }
    }
}