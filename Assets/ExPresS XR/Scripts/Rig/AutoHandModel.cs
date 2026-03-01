using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

namespace ExPresSXR.Rig
{
    /// <summary>
    /// The Auto Hand Model detects and displays the currently used controller as well as providing an optional animated hand model.
    /// It is used to represent the hand/controller of the XR Rig.  
    /// 
    /// There are in total four versions, two for for the for each hand, one having collisions and one doesn't.
    /// </summary>
    public class AutoHandModel : MonoBehaviour
    {
        /// <summary>
        /// Determines which model is displayed.
        /// </summary>
        [SerializeField]
        private HandModelMode _HandModelMode;
        public HandModelMode HandModelMode
        {
            get => _HandModelMode;
            set
            {
                _HandModelMode = value;

                // Allows updating the model during runtime
                UpdateDisplayedModel();
            }
        }
        /// <summary>
        /// Characteristics of the controller to search for, which is used to get the correct controller for the correct hand.
        /// </summary>
        public InputDeviceCharacteristics _controllerCharacteristics;
        /// <summary>
        /// A list of models from which the correct model for the used controller when is chosen `HandModelMode` is set to `Controller` or `Both`. 
        /// If no model was found for the controller a generic model will be shown.
        /// </summary>
        public List<GameObject> _controllerModels;
        /// <summary>
        /// The model that is displayed and animated when `HandModelMode` is set to `Hand` or `Both`.
        /// </summary>
        public GameObject _handModel;

        /// <summary>
        /// A custom model that is shown when `HandModelMode` is set to `Custom`.
        /// </summary>
        public GameObject customModel;

        /// <summary>
        /// The current attach transform provided by the current model.
        /// </summary>
        public Transform CurrentAttach
        {
            get
            {
                if (_currentHandModel != null && (HandModelMode == HandModelMode.Hand || HandModelMode == HandModelMode.Both))
                {
                    Transform handAttach = _currentHandModel.transform.Find("Attach");
                    if (handAttach != null)
                    {
                        return handAttach;
                    }
                }
                else if (_currentControllerModel != null && HandModelMode == HandModelMode.Controller)
                {
                    Transform controllerAttach = _currentControllerModel.transform.Find("Attach");
                    if (controllerAttach != null)
                    {
                        return controllerAttach;
                    }
                }
                return transform;
            }
        }

        /// <summary>
        /// Completely disables collisions with the hand/controller models during runtime.
        /// Overwrites the functionality of `_collisionsEnabled`.
        /// </summary>
        [Tooltip("Completely disables collisions with the hand/controller models during runtime. Overwrites the functionality of _collisionsEnabled.")]
        [SerializeField]
        private bool _modelCollisionsEnabled;
        public bool ModelCollisionsEnabled
        {
            get => _modelCollisionsEnabled;
            set
            {
                _modelCollisionsEnabled = value;
                // Update collisions (Setting to true enables it automatically)
                CollisionsCurrentlyEnabled = true;
            }
        }

        /// <summary>
        /// Temporary en-/disables collisions if `_modelCollisionsEnabled` is true. Will be controlled by the HandController.
        /// To disable collisions completely use `_modelCollisionsEnabled` instead.
        /// </summary>
        [Tooltip("Temporary en-/disables collisions if _modelCollisionsEnabled is true. Will be controlled by the HandController. To disable collisions completely use _modelCollisionsEnabled instead.")]
        private bool _collisionsCurrentlyEnabled;
        public bool CollisionsCurrentlyEnabled
        {
            get => _collisionsCurrentlyEnabled;
            set
            {
                _collisionsCurrentlyEnabled = value;

                // Disable RigidBody
                if (TryGetComponent(out Rigidbody rb))
                {
                    rb.detectCollisions = _collisionsCurrentlyEnabled && _modelCollisionsEnabled;
                }

                // Disable Colliders
                foreach (Collider collider in gameObject.GetComponentsInChildren<Collider>())
                {
                    collider.enabled = _collisionsCurrentlyEnabled && _modelCollisionsEnabled;
                }
            }
        }


        private InputDevice _currentDevice;
        private GameObject _currentControllerModel;
        private GameObject _currentHandModel;

        /// <summary>
        /// Emitted when the auto hand models were loaded, meaning a controller was detected.
        /// </summary>
        public UnityEvent OnModelsLoaded;

        private void Update()
        {
            if (!_currentDevice.isValid)
            {
                UpdateDisplayedModel();
            }
        }

        private void UpdateDisplayedModel()
        {
            if (TryInitialize())
            {
                UpdateModelVisibility();
            }
        }


        private void UpdateModelVisibility()
        {
            bool showHand = HandModelMode == HandModelMode.Hand
                                || HandModelMode == HandModelMode.Both;

            // Also show controller for mode Custom
            bool showController = HandModelMode == HandModelMode.Controller
                                || HandModelMode == HandModelMode.Custom
                                || HandModelMode == HandModelMode.Both;

            _currentHandModel.SetActive(showHand);
            _currentControllerModel.SetActive(showController);
        }


        private bool TryInitialize()
        {
            List<InputDevice> devices = new();
            InputDevices.GetDevicesWithCharacteristics(_controllerCharacteristics, devices);

            if (devices.Count > 0)
            {
                _currentDevice = devices[0];

                LoadModels();

                // Ensures to Enable/Disable Collisions on currently loaded models
                ModelCollisionsEnabled = _modelCollisionsEnabled;
                return true;
            }
            return false;
        }

        private void LoadModels()
        {
            if (_currentDevice == null || !_currentDevice.isValid || !isActiveAndEnabled)
            {
                return;
            }

            // Load hand Model
            if (_currentHandModel != null)
            {
                Destroy(_currentHandModel);
            }
            _currentHandModel = Instantiate(_handModel, transform);

            // Load Controller Model
            if (_currentControllerModel != null)
            {
                Destroy(_currentControllerModel);
            }

            GameObject prefab = _controllerModels.Find(controller => controller.name.StartsWith(_currentDevice.name));
            if (HandModelMode == HandModelMode.Custom)
            {
                _currentControllerModel = customModel != null ? Instantiate(customModel, transform) : null;
            }
            else if (prefab != null)
            {
                _currentControllerModel = Instantiate(prefab, transform);
            }
            else
            {
                Debug.LogWarning($"No Model with name: '{_currentDevice.name}' found, using a generic model instead.");
                _currentControllerModel = Instantiate(_controllerModels[0], transform);
            }

            OnModelsLoaded.Invoke();
        }

        /// <summary>
        /// Sets the hand pointing pose enabled/disabled, if possible.
        /// </summary>
        /// <param name="pointing">If the hand should be pointing or not.</param>
        public void SetHandPointing(bool pointing)
        {
            if (_currentHandModel != null && _currentHandModel.TryGetComponent(out VirtualHandAnimator handAnimator))
            {
                handAnimator.SetPointing(pointing);
            }
        }
    }

    /// <summary>
    /// How the hands are represented.
    /// </summary>
    public enum HandModelMode
    {
        Controller, /// <summary> Displays the hands as controllers matching the headset. </summary>
        Hand, /// <summary> Uses an articulated hand for visualization. </summary>
        Both, /// <summary> Uses both hand and controller models. </summary>
        Custom, /// <summary> Uses a custom model. </summary>
        None /// No hand visualization. </summary>
    }
}