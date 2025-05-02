using UnityEngine;
using UnityEngine.Events;
using ExPresSXR.Rig;

namespace ExPresSXR.Movement
{
    public class ForceTeleport : MonoBehaviour
    {
        /// <summary>
        /// Rig to be teleported.
        /// </summary>
        [SerializeField]
        [Tooltip("Rig to be teleported.")]
        private ExPresSXRRig _rig;

        /// <summary>
        /// Position of the camera, to correct the rotation.
        /// </summary>
        [SerializeField]
        private Transform _cameraTransform;
    	
        /// <summary>
        /// Default target for teleporting without having to provide a transform.
        /// </summary>
        [SerializeField]
        [Tooltip("Default target for teleporting without having to provide a transform.")]
        private Transform _defaultTarget;

        private CharacterController _playerController;

        private bool _pendingTeleport;
        private Vector3 _pendingTeleportPosition;
        private Quaternion _pendingTeleportRotation;

        //  Events

        /// <summary>
        /// Emitted if a teleport is requested. This will also be the moment a teleport without fade is performed.
        /// </summary>
        public UnityEvent OnForceTeleport;

        /// <summary>
        /// Emitted once the player's vision is fully faded and the teleport with fade is happening.
        /// </summary>
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

        /// <summary>
        /// Teleports the player to the default target without fade.
        /// </summary>
        public void DefaultTeleportTo() => TeleportTo(_defaultTarget, false);

        /// <summary>
        /// Teleports the player to the default target with fade.
        /// </summary>
        public void DefaultTeleportToWithFade() => TeleportTo(_defaultTarget, true);

        /// <summary>
        /// Teleports the player to the target without fade.
        /// </summary>
        /// <param name="target">Teleport position and rotation.</param>
        public void TeleportTo(Transform target) => TeleportTo(target, false);

        /// <summary>
        /// Teleports the player to the target with fade.
        /// </summary>
        /// <param name="target">Teleport position and rotation.</param>
        public void TeleportToWithFade(Transform target) => TeleportTo(target, true);
        
        /// <summary>
        /// Teleports the player to the target with optional fade.
        /// </summary>
        /// <param name="target">Teleport position and rotation.</param>
        /// <param name="fade">With or without fade.</param>
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
        
        /// <summary>
        /// Teleports the player to the given position and rotation with optional fade.
        /// </summary>
        /// <param name="targetPosition">Target position.</param>
        /// <param name="targetRotation">Target rotation.</param>
        /// <param name="fade">With or without fade.</param>
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

        /// <summary>
        /// Cancels a teleport. Only effective for teleports with fade.
        /// </summary>
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