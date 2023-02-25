using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using ExPresSXR.Rig;

[RequireComponent(typeof(Animator))]
public class VRGripAnimator : MonoBehaviour
{
    enum ControllerSide {
        Left,
        Right
    }

    const InputDeviceCharacteristics CONTROLLER_BASE_CHARACTERISTICS = InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.HeldInHand;
    
    [SerializeField] 
    ControllerSide controllerHand = ControllerSide.Right;

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
            animator.SetFloat("Trigger", triggerValue);
        }
        else
        {
            animator.SetFloat("Trigger", 0);
        }

        if (currentDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            animator.SetFloat("Grip", gripValue);
        }
        else
        {
            animator.SetFloat("Grip", 0);
        }
    }


    private void TryInitialize()
    {
        List<InputDevice> devices = new();

        InputDeviceCharacteristics handCharacteristics =  controllerHand == ControllerSide.Left ?
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
