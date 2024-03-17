using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace ExPresSXR.Interaction
{
    /// <summary>
    /// Acts as a wrapper for allowing XRGrabInteractables to be scaled while being held. 
    /// This will **not** scale the GameObject itself but all of it's children. Scaling will be applied to the children using their initial scale.
    /// </summary>
    public class ScalableGrabInteractable : XRGrabInteractable
    {
        /// <summary>
        /// Minimal scale possible, negative values are considered unbound.
        /// </summary>
        [SerializeField]
        private float _minScaleFactor = -1.0f;

        /// <summary>
        /// Maximal scale possible, negative values are considered unbound.
        /// </summary>
        [SerializeField]
        private float _maxScaleFactor = -1.0f;


        /// <summary>
        /// Override to the scale speed of the `ScalingRayInteractor` or `ScalingDirectInteractor`. 
        /// It is recommended to change the scale speed in the interactors themselves and use this override sparingly!
        /// </summary>
        [SerializeField]
        [Tooltip("Override to the scale speed of the `ScalingRayInteractor` or `ScalingDirectInteractor`. \n"
                + "It is recommended to change the scale speed in the interactors themselves and use this override sparingly!")]
        private float _scaleSpeedOverride = -1.0f;
        public float scaleSpeedOverride
        {
            get => _scaleSpeedOverride;
            set => _scaleSpeedOverride = value;
        }

        /// <summary>
        /// If the interactable should reset its scale when selected by a socket or retain the current scale.
        /// </summary>
        [SerializeField]
        private bool _resetScaleInSockets = true;


        /// <summary>
        /// If all children should be scaled or only those set as `scaledChildren` via the editor.
        /// </summary>
        [SerializeField]
        private bool _scaleAllChildren = true;
        public bool scaleAllChildren
        {
            get => _scaleAllChildren;
        }


        /// <summary>
        /// Children affected by scaling. Setting this value during runtime will use the current scales as initial scale.
        /// </summary>
        [Tooltip("Setting this value during runtime will use the current scales as initial scale.")]
        [SerializeField]
        private Transform[] _scaledChildren;
        public Transform[] scaledChildren
        {
            get => _scaledChildren;
            set
            {
                _scaledChildren = value;

                // Load scales
                _initialScales = new Vector3[_scaledChildren.Length];

                for (int i = 0; i < _scaledChildren.Length; i++)
                {
                    _initialScales[i] = _scaledChildren[i] != null ? _scaledChildren[i].localScale : Vector3.one;
                }
            }
        }


        /// <summary>
        /// The current scale to the children, relative to their initial scale.
        /// </summary>
        private float _scaleFactor = 1.0f;
        public float scaleFactor
        {
            get => _scaleFactor;
            set
            {
                _scaleFactor = GetClampedScaleFactor(value);

                ScaleChildren();
            }
        }


        /// <summary>
        /// The initial scales of the object in `_scaledChildren`. Only available at runtime.
        /// </summary>
        private Vector3[] _initialScales;
        public Vector3[] initialScales
        {
            get => _initialScales;
        }


        /// <summary>
        /// If the interactable has a speed scale override.
        /// </summary>
        public bool hasScaleSpeedOverride
        {
            get => _scaleSpeedOverride > 0.0f;
        }


        /// <summary>
        /// Performs general setup for the interactable, load the initial scales and connects the scale reset event.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            // Load initial scales
            LoadInitialScales();
            // Add Listener to reset scale
            selectEntered.AddListener(TryResetScaleInSockets);
        }


        /// <summary>
        /// Performs general teardown for the interactable and disconnects the scale reset event.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            // Remove Listener to reset scale
            selectEntered.RemoveListener(TryResetScaleInSockets);
        }


        /// <summary>
        /// Resets the scale of all (scaled) children to 1.0f
        /// </summary>
        public void ResetScale() => scaleFactor = 1.0f;


        private void LoadInitialScales()
        {
            List<Transform> children = new();
            if (_scaleAllChildren && transform.childCount > 0)
            {
                foreach (Transform t in transform)
                {
                    children.Add(t);
                }
            }
            else if (scaledChildren.Length > 0 && transform.childCount > 0)
            {
                children.AddRange(scaledChildren);
            }
            else
            {
                Debug.LogError("Scalable Grab interactable has no children to scale. Either enable '_scaleAllChildren' or add objects to the 'scaledChildren' array.");
            }

            scaledChildren = children.ToArray();
        }


        private void ScaleChildren()
        {
            for (int i = 0; i < _scaledChildren.Length; i++)
            {
                _scaledChildren[i].transform.localScale = _initialScales[i] * scaleFactor;
            }
        }


        private float GetClampedScaleFactor(float value)
        {
            float minValue = _minScaleFactor >= 0 ? _minScaleFactor : value;
            float maxValue = _maxScaleFactor >= 0 ? _maxScaleFactor : value;

            return Mathf.Clamp(value, minValue, maxValue);
        }

        private void TryResetScaleInSockets(SelectEnterEventArgs args)
        {
            if (_resetScaleInSockets && args.interactorObject is XRSocketInteractor)
            {
                ResetScale();
            }
        }
    }
}