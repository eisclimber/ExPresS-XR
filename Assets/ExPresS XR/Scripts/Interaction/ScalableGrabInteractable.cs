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
                float factorDelta = value - _scaleFactor;

                _scaleFactor = GetClampedScaleFactor(value);

                ScaleChildren(factorDelta);
            }
        }

        private float GetClampedScaleFactor(float value)
        {
            float minValue = _minScaleFactor >= 0 ? _minScaleFactor : value;
            float maxValue = _maxScaleFactor >= 0 ? _maxScaleFactor : value;

            return Mathf.Clamp(value, minValue, maxValue);
        }


        private void ScaleChildren(float factorDelta)
        {
            foreach (Transform child in transform)
            {
                // Use the child's current scale with the delta => 1.0f + factorDelta
                child.transform.localScale *= 1.0f + factorDelta;
            }
        }
    }
}