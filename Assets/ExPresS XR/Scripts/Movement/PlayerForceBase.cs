using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ExPresSXR.Movement
{
    /// <summary>
    /// A base class that can be used to apply a force on the player, while providing an solution for the use with teleportation movement.
    /// Derived classes must implement the movement itself by calling `_characterController.Move()` during `Update()`
    /// but only apply the force only `_forceTemporarilyDisabled`
    /// </summary>
    public abstract class PlayerForceBase : MonoBehaviour
    {
        /// <summary>
        /// InputActions that temporarily disable the force for the specified duration.
        /// You want to set it to the InputsActions used for teleportation.
        /// </summary>
        [SerializeField]
        [Tooltip("InputActions that temporarily disable the force for the specified duration. You want to set it to the InputsActions used for teleportation.")]
        private InputActionReference[] _inputActionPausingForce;


        /// <summary>
        /// Duration for how long the force will be temporarily disabled.
        /// </summary>
        [SerializeField]
        private float _forcePauseDuration = 0.1f;

        /// <summary>
        /// CharacterController to be manipulated.
        /// </summary>
        [SerializeField]
        protected CharacterController _characterController;

        /// <summary>
        /// The current velocity applied.
        /// </summary>
        protected Vector3 _currentVelocity;
        public Vector3 currentVelocity
        {
            get => _currentVelocity;
            protected set => _currentVelocity = value;
        }

        /// <summary>
        /// Will be true if the force is temporarily disabled after a input action was performed.
        /// Do not apply the force in these cases to allow teleport movement.
        /// </summary>
        private bool _forceTemporarilyDisabled;
        protected bool forceTemporarilyDisabled
        {
            get => _forceTemporarilyDisabled;
        }


        private Coroutine _forcePausedCoroutine;


        /// <summary>
        /// Adds a callback that temporary pauses the force.
        /// </summary>
        protected virtual void OnEnable()
        {
            foreach (InputActionReference actionRef in _inputActionPausingForce)
            {
                actionRef.action.canceled += PauseForce;
            }
        }

        /// <summary>
        /// Removes callback that temporary pause the force.
        /// </summary>
        protected virtual void OnDisable()
        {
            foreach (InputActionReference actionRef in _inputActionPausingForce)
            {
                actionRef.action.canceled -= PauseForce;
            }
        }

        /// <summary>
        /// Sets the current velocity to (0, 0, 0).
        /// </summary>
        public virtual void ClearVelocity() => _currentVelocity = Vector3.zero;


        /// <summary>
        /// Callback that pauses the the force for the specified time.
        /// </summary>
        /// <param name="_">InputAction Callback Context</param>
        protected virtual void PauseForce(InputAction.CallbackContext _)
        {
            if (_forcePausedCoroutine != null)
            {
                StopCoroutine(_forcePausedCoroutine);
            }
            _forcePausedCoroutine = StartCoroutine(PauseForceCoroutine());
        }

        /// <summary>
        /// Coroutine function that pauses the force temporarily.
        /// </summary>
        /// <returns>IEnumerator of the Coroutine.</returns>
        protected virtual IEnumerator PauseForceCoroutine()
        {
            _forceTemporarilyDisabled = true;
            yield return new WaitForSeconds(_forcePauseDuration);
            _forceTemporarilyDisabled = false;
            _forcePausedCoroutine = null;
        }
    }
}