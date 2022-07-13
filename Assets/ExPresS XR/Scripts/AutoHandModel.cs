using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace ExPresSXR.Rig
{
    public class AutoHandModel : MonoBehaviour
    {
        public HandModelMode handModelMode;
        public InputDeviceCharacteristics controllerCharacteristics;
        public List<GameObject> controllerModels;
        public GameObject handModel;
        public GameObject customModel;

        private InputDevice currentDevice;
        private GameObject currentControllerModel;
        private GameObject currentHandModel;
        private Animator handAnimator;

        public Transform currentAttach
        {
            get
            {
                if (currentHandModel != null && (handModelMode == HandModelMode.Hand || handModelMode == HandModelMode.Both))
                {
                    Transform handAttach = currentHandModel.transform.Find("Attach");
                    if (handAttach != null)
                    {
                        return handAttach;
                    }
                }
                else if (currentControllerModel != null && handModelMode == HandModelMode.Controller)
                {
                    Transform controllerAttach = currentControllerModel.transform.Find("Attach");
                    if (controllerAttach != null)
                    {
                        return controllerAttach;
                    }
                }
                return transform;
            }
        }

        [SerializeField]
        private bool _collisionsEnabled;
        public bool collisionsEnabled
        {
            get => _collisionsEnabled;
            set
            {
                _collisionsEnabled = value;

                // Disable Rigid Body
                if (GetComponent<Rigidbody>() != null)
                {
                    GetComponent<Rigidbody>().detectCollisions = _collisionsEnabled;
                }

                // Disable Colliders
                foreach (Collider collider in gameObject.GetComponentsInChildren<Collider>())
                {
                    collider.enabled = _collisionsEnabled;
                }
            }
        }


        private void TryInitialize()
        {
            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

            if (devices.Count > 0)
            {
                currentDevice = devices[0];
                GameObject prefab = controllerModels.Find(controller => controller.name.StartsWith(currentDevice.name));

                if (handModelMode == HandModelMode.Custom)
                {
                    currentControllerModel = (customModel != null ? Instantiate(customModel, transform) : null);
                }
                else if (prefab != null)
                {
                    currentControllerModel = Instantiate(prefab, transform);
                }
                else
                {
                    Debug.LogWarning("No Model with name: '" + currentDevice.name + "' found, using a generic model instead.");
                    currentControllerModel = Instantiate(controllerModels[0], transform);
                }

                currentHandModel = Instantiate(handModel, transform);

                handAnimator = currentHandModel.GetComponent<Animator>();
            }
        }


        private void UpdateHandAnimation()
        {
            if (currentDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
            {
                handAnimator.SetFloat("Trigger", triggerValue);
            }
            else
            {
                handAnimator.SetFloat("Trigger", 0);
            }
            if (currentDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
            {
                handAnimator.SetFloat("Grip", gripValue);
            }
            else
            {
                handAnimator.SetFloat("Grip", 0);
            }
        }


        // Update is called once per frame
        private void Update()
        {
            if (!currentDevice.isValid)
            {
                TryInitialize();
            }
            else
            {
                if (handModelMode == HandModelMode.Both)
                {
                    currentHandModel.SetActive(true);
                    currentControllerModel.SetActive(true);
                    UpdateHandAnimation();
                }
                else if (handModelMode == HandModelMode.Hand)
                {
                    currentHandModel.SetActive(true);
                    currentControllerModel.SetActive(false);
                    UpdateHandAnimation();
                }
                else
                {
                    // Mode either Controller or Custom
                    currentHandModel.SetActive(false);
                    if (currentControllerModel)
                    {
                        currentControllerModel.SetActive(true);
                    }
                }
            }
        }
    }

    public enum HandModelMode
    {
        Controller,
        Hand,
        Both,
        Custom
    }
}