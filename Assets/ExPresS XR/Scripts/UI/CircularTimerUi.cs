using UnityEngine;
using UnityEngine.UI;
using System;
using ExPresSXR.Misc;

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
        public override void ResetVisualization()
        {
            base.ResetVisualization();
            _fillSettings.ResetVisualization();
        }

        /// <summary>
        /// A helper class to hold the settings for the visualization of the circular timer.
        /// </summary>
        [Serializable]
        public class FillSettings
        {
            /// <summary>
            /// The direction in which the fill image fills up.
            /// </summary>
            [Tooltip("The direction in which the fill image fills up.")]
            public FillDirection FillDirection = FillDirection.Down;

            /// <summary>
            /// The type of progress visualization.
            /// </summary>
            [Tooltip("The type of progress visualization.")]
            public ProgressType FillType = ProgressType.Smooth;

            /// <summary>
            /// The tick frequency for the tick-based visualization.
            /// Will be interpreted as seconds if 'fillType' is set to 'TickTime'.
            /// If 'fillType' is set to 'TickNum' the value is interpreted as number of ticks.
            /// Has no effect if `fillType` is set to `Smooth` or set to a value equal or below zero.
            /// </summary>
            [Tooltip("The tick frequency for the tick-based visualization.\nOnly relevant if 'fillType' is set to 'Tick'."
                + "\nWill be interpreted as seconds if 'fillType' is set to 'TickTime'."
                + "\nIf 'fillType' is set to 'TickNum' the value is interpreted as number of ticks."
                + "\nHas no effect if `fillType` is set to `Smooth` or set to a value equal or below zero.")]
            public int tickFrequency = 0;

            [SerializeField]
            [Tooltip("If the caps shown at the ends of the progress meter to round the ends.")]
            private bool _capsEnabled;
            /// <summary>
            /// If the caps shown at the ends of the progress meter to round the ends.
            /// </summary>
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
            [Tooltip("Color of the fill meter and the caps.")]
            private Color _color = Color.white;
            /// <summary>
            /// Color of the fill meter and the caps.
            /// </summary>
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
            [Tooltip("The image used to visualize the fill of the timer. Should be of Image Type 'Filled' for correct visualization.")]
            private Image _fillImage;
            /// <summary>
            /// The image used to visualize the fill of the timer. Should be of Image Type 'Filled' for correct visualization.
            /// </summary>
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
            [Tooltip("The image used to visualize the start cap of the timer.")]
            private Image _startCapImage;
            /// <summary>
            /// The image used to visualize the start cap of the timer.
            /// </summary>
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
            [Tooltip("The image used to visualize the end cap of the timer.")]
            private Image _endCapImage;
            /// <summary>
            /// The image used to visualize the end cap of the timer.
            /// </summary>
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

            [SerializeField]
            [Tooltip("If the caps will be hidden when the timer is not running and the fill amount is 0.")]
            private bool _hideCapsIfNotRunning = true;
            /// <summary>
            /// If the caps will be hidden when the timer is not running and the fill amount is 0.
            /// </summary>
            public bool HideCapsIfNotRunning
            {
                get => _hideCapsIfNotRunning;
                set
                {
                    _hideCapsIfNotRunning = value;
                    UpdateCaps();
                }
            }

            /// <summary>
            /// Updates the colors set for the visualization.
            /// </summary>
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

            /// <summary>
            /// Updates the visibility of the caps.
            /// </summary>
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

            /// <summary>
            /// Updates the visualization of the timer according to the remaining time and overall wait time.
            /// </summary>
            /// <param name="remainingTime">Remaining time of the timer.</param>
            /// <param name="waitTime">Total wait time of the timer.</param>
            public void UpdateVisualization(float remainingTime, float waitTime)
            {
                UpdateCaps();

                if (_fillImage != null)
                {
                    float progressPct = CalculateProgressPct(remainingTime, waitTime);
                    _fillImage.fillAmount = FillDirection == FillDirection.Down ? progressPct : 1.0f - progressPct;
                }

                if (CapsEnabled)
                {
                    Vector3 capRotationValue = Vector3.zero;
                    capRotationValue.z = 360.0f * (1.0f - FillImage.fillAmount);
                    _endCapImage.rectTransform.localRotation = Quaternion.Euler(capRotationValue);
                }
            }

            /// <summary>
            /// Resets the visualization to the default state (fill amount 0).
            /// </summary>
            public void ResetVisualization()
            {
                _fillImage.fillAmount = FillDirection == FillDirection.Down ? 0 : 1;

                if (_hideCapsIfNotRunning)
                {
                    _startCapImage.enabled = false;
                    _endCapImage.enabled = false;
                }
            }

            private float CalculateProgressPct(float remainingTime, float waitTime)
            {
                float progressPct = remainingTime / waitTime;
                if (FillType == ProgressType.TickTime && tickFrequency > 0)
                {
                    int numTicks = Mathf.CeilToInt(waitTime / tickFrequency);
                    return RuntimeUtils.GetValue01Stepped(progressPct, numTicks);
                }
                else if (FillType == ProgressType.TickNum && tickFrequency > 0)
                {
                    return RuntimeUtils.GetValue01Stepped(progressPct, tickFrequency);
                }

                return progressPct;
            }
        }

        /// <summary>
        /// The way progress is visualized, either as counting up or counting down.
        /// </summary>
        public enum FillDirection
        {
            /// <summary>Progress is shown as counting up.</summary>
            Up,
            /// <summary>Progress is shown as counting down.</summary>
            Down
        }

        /// <summary>
        /// The type of progress visualization.
        /// </summary>
        public enum ProgressType
        {
            /// <summary>Progress will be visualized in discrete time-based steps.</summary>
            TickTime,
            /// <summary>Progress will be visualized in discrete value-based steps.</summary>
            TickNum,
            /// <summary>Progress will be visualized smoothly.</summary>
            Smooth
        }
    }
}