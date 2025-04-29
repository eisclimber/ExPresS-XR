using UnityEngine;
using UnityEngine.UI;
using System;

namespace ExPresSXR.UI
{
    /// <summary>
    /// Visualizes an ExPresSXR Timer as a filling circle.
    /// </summary>
    public class CircularTimerUi : TimerUi
    {
        /// <summary>
        /// Object that holds the settings for for visualization.
        /// </summary>
        [SerializeField]
        private FillSettings _fillSettings;

        /// <summary>
        /// Updates the visualization of the timer.
        /// </summary>
        /// <param name="remainingTime">Remaining time of the timer.</param>
        /// <param name="waitTime">Overall wait time of the timer.</param>
        protected override void UpdateUI(float remainingTime, float waitTime)
        {
            base.UpdateUI(remainingTime, waitTime);
            _fillSettings.UpdateVisualization(remainingTime, waitTime);
        }

        /// <summary>
        /// Resets the visualization.
        /// </summary>
        public override void ResetVisualization() => _fillSettings.ResetVisualization();

        // Helper classes
        [Serializable]
        public class FillSettings
        {
            public FillDirection fillDirection = FillDirection.Down;
            public ProgressType fillType = ProgressType.Smooth;

            [SerializeField]
            private bool _capsEnabled;
            public bool CapsEnabled
            {
                get => _capsEnabled;
                set
                {
                    _capsEnabled = value;
                    UpdateCaps();
                }
            }


            [SerializeField]
            private Color _color = Color.white;
            public Color Color
            {
                get => _color;
                set
                {
                    _color = value;
                    UpdateColors();
                }
            }

            [SerializeField]
            private Image _fillImage;
            public Image FillImage
            {
                get => _fillImage;
                set
                {
                    _fillImage = value;
                    UpdateColors();
                }
            }

            [SerializeField]
            private Image _startCapImage;
            public Image StartCapImage
            {
                get => _startCapImage;
                set
                {
                    _startCapImage = value;
                    UpdateColors();
                    UpdateCaps();
                }
            }


            [SerializeField]
            private Image _endCapImage;
            public Image EndCapImage
            {
                get => _endCapImage;
                set
                {
                    _endCapImage = value;
                    UpdateColors();
                    UpdateCaps();
                }
            }


            public void UpdateColors()
            {
                if (FillImage != null)
                {
                    FillImage.color = Color;
                }
                if (_startCapImage != null)
                {
                    _startCapImage.color = Color;
                }
                if (_endCapImage != null)
                {
                    _endCapImage.color = Color;
                }
            }

            public void UpdateCaps()
            {
                if (_startCapImage != null)
                {
                    _startCapImage.enabled = CapsEnabled && (_fillImage == null || _fillImage.fillAmount > 0.0f);
                }
                if (_endCapImage != null)
                {
                    _endCapImage.enabled = CapsEnabled && (_fillImage == null || _fillImage.fillAmount > 0.0f);
                }
            }

            public void UpdateVisualization(float remainingTime, float waitTime)
            {
                UpdateCaps();

                if (_fillImage != null)
                {
                    float progressPct = Mathf.Clamp01(remainingTime / waitTime);
                    _fillImage.fillAmount = fillDirection == FillDirection.Down ? progressPct : 1.0f - progressPct;
                }

                if (CapsEnabled)
                {
                    Vector3 capRotationValue = Vector3.zero;
                    capRotationValue.z = 360.0f * (1.0f - FillImage.fillAmount);
                    _endCapImage.rectTransform.localRotation = Quaternion.Euler(capRotationValue);
                }
            }

            public void ResetVisualization()
            {
                _fillImage.fillAmount = fillDirection == FillDirection.Down ? 0 : 1;
            }
        }

        // Enums
        public enum FillDirection
        {
            Up,
            Down
        }

        public enum ProgressType
        {
            Tick,
            Smooth
        }
    }
}