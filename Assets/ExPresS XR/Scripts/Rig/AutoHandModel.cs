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

        [Tooltip("Completely disables collisions with the hand/controller models during runtime. Overwrites the functionality of _collisionsEnabled.")]
        [SerializeField]
        private bool _modelCollisionsEnabled;
        public bool modelCollisionsEnabled
        {
            get => _modelCollisionsEnabled;
            set
            {
                _modelCollisionsEnabled = value;
                // Update collisions (Setting to true enables it automatically)
                collisionsEnabled = true;
            }
        }


        [Tooltip("Temporary en-/disables collisions if _modelCollisionsEnabled is true. Will be controlled by the HandController. To disable collisions completely use _modelCollisionsEnabled instead.")]
        private bool _collisionsEnabled;
        public bool collisionsEnabled
        {
            get => _collisionsEnabled;
            set
            {
                _collisionsEnabled = value;

                // // Disable RigidBody
                if (GetComponent<Rigidbody>() != null)
                {
                    GetComponent<Rigidbody>().detectCollisions = _collisionsEnabled && _modelCollisionsEnabled;
                }

                // Disable Colliders
                foreach (Collider collider in gameObject.GetComponentsInChildren<Collider>())
                {
                    collider.enabled = _collisionsEnabled && _modelCollisionsEnabled;
                }
            }
        }


        private void TryInitialize()
        {
            List<InputDevice> devices = new();
            InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

            if (devices.Count > 0)
            {
                currentDevice = devices[0];
                GameObject prefab = controllerModels.Find(controller => controller.name.StartsWith(currentDevice.name));

                if (handModelMode == HandModelMode.Custom)
                {
                    currentControllerModel = customModel != null ? Instantiate(customModel, transform) : null;
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

                // Ensures to Enable/Disable Collisions on currently loaded models
                modelCollisionsEnabled = _modelCollisionsEnabled;
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
                bool showHand = handModelMode == HandModelMode.Hand 
                                || handModelMode == HandModelMode.Both;
                
                // Also show controller for mode Custom
                bool showController = handModelMode == HandModelMode.Controller 
                                    || handModelMode == HandModelMode.Custom
                                    || handModelMode == HandModelMode.Both;
                
                currentHandModel.SetActive(showHand);
                currentControllerModel.SetActive(showController);
            }
        }
    }

    public enum HandModelMode
    {
        Controller,
        Hand,
        Both,
        Custom,
        None
    }
}