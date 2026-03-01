using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace ExPresSXR.Rig
{
    /// <summary>
    /// Animates a virtual hand, controlling pinch(=trigger), grip and point.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class VirtualHandAnimator : MonoBehaviour
    {
        /// <summary>
        /// Base characteristics of a handheld controller.
        /// </summary>
        private const InputDeviceCharacteristics CONTROLLER_BASE_CHARACTERISTICS = InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.HeldInHand;

        /// <summary>
        /// Handiness of the controller.
        /// </summary>
        [SerializeField]
        private Handiness _handiness = Handiness.Right;

        /// <summary>
        /// Name of the `float` parameter controlling the hands `trigger` value.
        /// </summary>
        [SerializeField]
        [Tooltip("Name of the `float` parameter controlling the hands `trigger` value.")]
        private string _triggerAnimatorName = "Trigger";

        /// <summary>
        /// Name of the `float` parameter controlling the hands `grip` value.
        /// </summary>
        [SerializeField]
        [Tooltip("Name of the `bool` parameter controlling the hands `grip` value.")]
        private string _gripAnimatorName = "Grip";

        /// <summary>
        /// Name of the `float` parameter controlling the hands `point` value.
        /// </summary>
        [SerializeField]
        [Tooltip("Name of the `bool` parameter controlling the hands `point` value.")]
        private string _pointAnimatorName = "Point";

        [Space]

        /// <summary>
        /// Input action references canceling the point mode.
        /// </summary>
        [SerializeField]
        [Tooltip("Input action references canceling the point mode.")]
        private UnityEngine.InputSystem.InputActionReference[] _pointCancelActions;

        /// <summary>
        /// Downtime between starting and stopping pointing.
        /// </summary>
        [SerializeField]
        [Tooltip("Downtime between starting and stopping pointing.")]
        private float _pointCancelDowntime = 0.2f;


        /// <summary>
        /// Number of collisions detected to indicate pointing.
        /// </summary>
        private int _pointAreaCollisions;
        public int PointAreaCollisions
        {
            get => _pointAreaCollisions;
            set
            {
                _pointAreaCollisions = value;
                SetPointing(_pointAreaCollisions > 0);
            }
        }

        /// <summary>
        /// If pointing is active
        /// </summary>
        public bool PointActive
        {
            get => _pointAreaCollisions > 0 && Time.time - _lastPointCancelTime > _pointCancelDowntime;
        }

        private float _lastPointCancelTime;
        private InputDevice _currentDevice;
        private Animator _animator;


        private void OnEnable()
        {
            foreach (UnityEngine.InputSystem.InputActionReference actionRef in _pointCancelActions)
            {
                actionRef.action.performed += ResetPointAreaCollisions;
            }
        }

        private void OnDisable()
        {
            foreach (UnityEngine.InputSystem.InputActionReference actionRef in _pointCancelActions)
            {
                actionRef.action.performed -= ResetPointAreaCollisions;
            }
        }


        private void Update()
        {
            if (!_currentDevice.isValid)
            {
                TryInitialize();
            }
            else
            {
                UpdateHandAnimations();
            }
        }

        /// <summary>
        /// Sets a trigger in the animator.
        /// </summary>
        /// <param name="triggerName">Name of the trigger.</param>
        public void SetAnimatorTrigger(string triggerName)
        {
            if (_animator != null)
            {
                _animator.SetTrigger(triggerName);
            }
        }

        /// <summary>
        /// Sets a bool parameter in the animator.
        /// </summary>
        /// <param name="boolName">Name of the parameter.</param>
        /// <param name="boolValue">Value to set.</param>
        public void SetAnimatorBool(string boolName, bool boolValue)
        {
            if (_animator != null)
            {
                _animator.SetBool(boolName, boolValue);
            }
        }

        /// <summary>
        /// Sets a float parameter in the animator.
        /// </summary>
        /// <param name="floatName">Name of the parameter.</param>
        /// <param name="floatValue">Value to set.</param>
        public void SetAnimatorFloat(string floatName, float floatValue)
        {
            if (_animator != null)
            {
                _animator.SetFloat(floatName, floatValue);
            }
        }


        /// <summary>
        /// Resets the point area collisions and thus the pointing.
        /// </summary>
        public void ResetPointing() => PointAreaCollisions = 0; // It can happen that a hand does unregister completely on teleport so we call it manually.

        /// <summary>
        /// Manually sets the pointing parameter in the animator.
        /// </summary>
        /// <param name="point">If pointing or not.</param>
        public void SetPointing(bool point) => SetAnimatorBool(_pointAnimatorName, point);


        /// <summary>
        /// Updates the grab and pinch values in the animator.
        /// </summary>
        protected virtual void UpdateHandAnimations()
        {
            if (_currentDevice == null)
            {
                Debug.LogWarning($"Could not retrieve values from non-existent device.", this);
            }

            if (!_currentDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
            {
                Debug.Log("Failed to read trigger value from device.", this);
                triggerValue = 0.0f;
            }
            SetAnimatorFloat(_triggerAnimatorName, triggerValue);

            if (!_currentDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
            {
                Debug.Log("Failed to read grip value from device.", this);
                gripValue = 0.0f;
            }
            SetAnimatorFloat(_gripAnimatorName, gripValue);
        }


        private void TryInitialize()
        {
            List<InputDevice> devices = new();

            InputDeviceCharacteristics handCharacteristics = _handiness == Handiness.Left ?
                    InputDeviceCharacteristics.Left : InputDeviceCharacteristics.Right;

            InputDeviceCharacteristics deviceCharacteristics = CONTROLLER_BASE_CHARACTERISTICS | handCharacteristics;

            InputDevices.GetDevicesWithCharacteristics(deviceCharacteristics, devices);

            if (devices.Count > 0)
            {
                _currentDevice = devices[0];
                TryGetComponent(out _animator);

                if (_animator == null)
                {
                    Debug.LogError("No animator found, make sure an one was added.");
                }
                else
                {
                    ValidateTriggers();
                }
            }
        }

        private void ValidateTriggers()
        {
            bool foundTrigger = false;
            bool foundGrip = false;

            foreach (AnimatorControllerParameter param in _animator.parameters)
            {
                if (param.name == _triggerAnimatorName && param.type == AnimatorControllerParameterType.Float)
                {
                    foundTrigger = true;
                }

                if (param.name == _gripAnimatorName && param.type == AnimatorControllerParameterType.Float)
                {
                    foundGrip = true;
                }

                if (foundGrip && foundTrigger)
                {
                    return;
                }
            }

            if (!foundTrigger)
            {
                Debug.LogError($"No float value was found in the animator with name '{_triggerAnimatorName}' in GameObject '{gameObject.name}'.");
            }

            if (!foundGrip)
            {
                Debug.LogError($"No float value was found in the animator with name '{_gripAnimatorName}' in GameObject '{gameObject.name}'.");
            }
        }

        private void ResetPointAreaCollisions(UnityEngine.InputSystem.InputAction.CallbackContext _)
        {
            _lastPointCancelTime = Time.time;
            PointAreaCollisions = 0;
        }

        /// <summary>
        /// Handiness of the displayed hand.
        /// </summary>
        enum Handiness
        {
            Left, /// <summary> Refers to the left hand. </summary>
            Right /// <summary> Refers to the right hand. </summary>
        }

    }
}