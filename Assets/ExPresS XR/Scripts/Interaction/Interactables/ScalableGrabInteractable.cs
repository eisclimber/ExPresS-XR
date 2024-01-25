using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace ExPresSXR.Interaction
{
    public class ScalableGrabInteractable : XRGrabInteractable
    {
        [SerializeField]
        private float _minScaleFactor = -1.0f;

        [SerializeField]
        private float _maxScaleFactor = -1.0f;


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

        [SerializeField]
        private bool _scaleAllChildren = true;
        public bool scaleAllChildren
        {
            get => _scaleAllChildren;
        }


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

        [SerializeField]
        private bool _resetScaleInSockets;

        private Vector3[] _initialScales;

        protected override void OnEnable()
        {
            base.OnEnable();
            // Load initial scales
            LoadInitialScales();
            // Add Listener to reset scale
            selectEntered.AddListener(TryResetScaleInSockets);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            // Remove Listener to reset scale
            selectEntered.RemoveListener(TryResetScaleInSockets);
        }

        private void LoadInitialScales()
        {
            List<Transform> children = new();
            if (_scaleAllChildren)
            {
                foreach (Transform t in transform)
                {
                    children.Add(t);
                }
            }
            else if (scaledChildren.Length > 0)
            {
                foreach (Transform t in scaledChildren)
                {
                    children.Add(t);
                }
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

        /// <summary>
        /// Resets the scale of all (scaled) children to 1.0f
        /// </summary>
        public void ResetScale() => scaleFactor = 1.0f;

        private void TryResetScaleInSockets(SelectEnterEventArgs args)
        {
            if (_resetScaleInSockets && args.interactorObject is XRSocketInteractor)
            {
                ResetScale();
            }
        }
    }
}