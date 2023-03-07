using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace ExPresSXR.Rig {
    
    [RequireComponent(typeof(Animator))]
    public class SingleHandGripAnimator : MonoBehaviour
    {
        enum ControllerSide {
            Left,
            Right
        }

        const InputDeviceCharacteristics CONTROLLER_BASE_CHARACTERISTICS = InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.HeldInHand;
        
        [SerializeField] 
        private ControllerSide _controllerHand = ControllerSide.Right;

        [SerializeField]
        private string _triggerPrefix;

        private InputDevice currentDevice;
        private Animator animator;

        private void Update()
        {
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
                animator.SetFloat(_triggerPrefix + "Trigger", triggerValue);
            }
            else
            {
                animator.SetFloat(_triggerPrefix + "Trigger", 0);
            }

            if (currentDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
            {
                animator.SetFloat(_triggerPrefix + "Grip", gripValue);
            }
            else
            {
                animator.SetFloat(_triggerPrefix + "Grip", 0);
            }
        }


        private void TryInitialize()
        {
            List<InputDevice> devices = new();

            InputDeviceCharacteristics handCharacteristics =  _controllerHand == ControllerSide.Left ?
                    InputDeviceCharacteristics.Left : InputDeviceCharacteristics.Right;

            InputDeviceCharacteristics deviceCharacteristics = CONTROLLER_BASE_CHARACTERISTICS | handCharacteristics;

            InputDevices.GetDevicesWithCharacteristics(deviceCharacteristics, devices);

            if (devices.Count > 0)
            {
                currentDevice = devices[0];
                animator = GetComponent<Animator>();
            }
        }
    }
}