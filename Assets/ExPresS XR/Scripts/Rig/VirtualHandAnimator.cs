using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace ExPresSXR.Rig
{

    [RequireComponent(typeof(Animator))]
    public class VirtualHandAnimator : MonoBehaviour
    {
        enum Handiness
        {
            Left,
            Right
        }

        const InputDeviceCharacteristics CONTROLLER_BASE_CHARACTERISTICS = InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.HeldInHand;

        [SerializeField]
        private Handiness _handiness = Handiness.Right;

        [SerializeField]
        private string _triggerAnimatorName = "Trigger";
        
        [SerializeField]
        private string _gripAnimatorName = "Grip";

        [SerializeField]
        private string _pointAnimatorName = "Point";

        [Space]

        [SerializeField]
        [Tooltip("Input action references canceling the point mode.")]
        private UnityEngine.InputSystem.InputActionReference[] _pointCancelActions;

        [SerializeField]
        [Tooltip("")]
        private float _postCancelDowntime = 0.2f;


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

        public bool PointActive
        {
            get => _pointAreaCollisions > 0 && Time.time - _lastPointCancelTime > _postCancelDowntime;
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

        // General access
        public void SetAnimatorTrigger(string triggerName)
        {
            if (_animator != null)
            {
                _animator.SetTrigger(triggerName);
            }
        }

        public void SetAnimatorBool(string boolName, bool boolValue)
        {
            if (_animator != null)
            {
                _animator.SetBool(boolName, boolValue);
            }
        }

        public void SetAnimatorFloat(string floatName, float floatValue)
        {
            if (_animator != null)
            {
                _animator.SetFloat(floatName, floatValue);
            }
        }


        // Pointing

        public void ResetPointing() => PointAreaCollisions = 0; // It can happen that a hand does unregister completely on teleport so we call it manually.
        
        public void SetPointing(bool point) => SetAnimatorBool(_pointAnimatorName, point);
        

        // Grab and pinch
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
    }
}