using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace ExPresSXR.Rig
{

    [RequireComponent(typeof(Animator))]
    public class SingleHandGripAnimator : MonoBehaviour
    {
        enum ControllerSide
        {
            Left,
            Right
        }

        const InputDeviceCharacteristics CONTROLLER_BASE_CHARACTERISTICS = InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.HeldInHand;

        [SerializeField]
        private ControllerSide _controllerHand = ControllerSide.Right;

        [SerializeField]
        private string _triggerPrefix;


        private string triggerAnimatorName
        {
            get => _triggerPrefix + "Trigger";
        }


        private string gripAnimatorName
        {
            get => _triggerPrefix + "Grip";
        }


        private InputDevice currentDevice;
        private Animator _animator;


        private void Update()
        {
            // Debug.Log(currentDevice.isValid);
            if (!currentDevice.isValid)
            {
                TryInitialize();
            }
            else
            {
                UpdateHandAnimation();
            }
        }


        private void UpdateHandAnimation()
        {
            if (currentDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
            {
                // Debug.Log("Found trigger value");
                _animator.SetFloat(triggerAnimatorName, triggerValue);
            }
            else
            {
                // Debug.Log("No trigger value");
                _animator.SetFloat(triggerAnimatorName, 0);
            }

            if (currentDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
            {
                // Debug.Log("Found grip value");
                _animator.SetFloat(gripAnimatorName, gripValue);
            }
            else
            {
                // Debug.Log("No trigger value");
                _animator.SetFloat(gripAnimatorName, 0);
            }
        }


        private void TryInitialize()
        {
            List<InputDevice> devices = new();

            InputDeviceCharacteristics handCharacteristics = _controllerHand == ControllerSide.Left ?
                    InputDeviceCharacteristics.Left : InputDeviceCharacteristics.Right;

            InputDeviceCharacteristics deviceCharacteristics = CONTROLLER_BASE_CHARACTERISTICS | handCharacteristics;

            InputDevices.GetDevicesWithCharacteristics(deviceCharacteristics, devices);

            if (devices.Count > 0)
            {
                currentDevice = devices[0];
                _animator = GetComponent<Animator>();
            }

            if (_animator == null)
            {
                Debug.LogError("No animator found, make sure an one was added.");
            }
            else
            {
                ValidateTriggers();
            }
        }

        private void ValidateTriggers()
        {
            bool foundTrigger = false;
            bool foundGrip = false;

            foreach (AnimatorControllerParameter param in _animator.parameters)
            {
                if (param.name == triggerAnimatorName && param.type == AnimatorControllerParameterType.Float)
                {
                    foundTrigger = true;
                }

                if (param.name == gripAnimatorName && param.type == AnimatorControllerParameterType.Float)
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
                Debug.LogError($"No float value was found in the animator with name '{triggerAnimatorName}' in GameObject '{gameObject.name}'.");
            }

            if (!foundGrip)
            {
                Debug.LogError($"No float value was found in the animator with name '{gripAnimatorName}' in GameObject '{gameObject.name}'.");
            }
        }
    }
}