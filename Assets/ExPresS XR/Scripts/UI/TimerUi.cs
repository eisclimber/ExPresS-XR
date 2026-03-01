using System;
using ExPresSXR.Misc.Timing;
using TMPro;
using UnityEngine;


namespace ExPresSXR.UI
{
    /// <summary>
    /// Visualizes an ExPresSXR Timer as a filling circle.
    /// </summary>
    public class TimerUi : MonoBehaviour
    {
        /// <summary>
        /// Reference to the timer to visualize.
        /// </summary>
        [SerializeField]
        protected Timer _timer;

        /// <summary>
        /// Object that holds the settings for the text.
        /// </summary>
        [SerializeField]
        protected TextSettings _textSettings;

        /// <summary>
        /// Tries connecting signals of the timer.
        /// </summary>
        protected virtual void OnEnable()
        {
            if (_timer == null && !TryGetComponent(out _timer))
            {
                Debug.LogError("No Timer was provided. Cannot display a non-existent timer.", this);
            }
            else
            {
                _timer.OnTimeout.AddListener(HandleTimeout);

                if (!_timer.Running)
                {
                    ResetVisualization();
                }
            }
        }

        /// <summary>
        /// Tries disconnecting to signals of the timer.
        /// </summary>
        protected virtual void OnDisable()
        {
            if (_timer != null)
            {
                _timer.OnTimeout.RemoveListener(HandleTimeout);
            }
        }

        protected virtual void Update()
        {
            if (_timer != null && _timer.Running)
            {
                UpdateUI(_timer.RemainingTime, _timer.WaitTime);
            }
        }

        protected virtual void UpdateUI(float remainingTime, float waitTime)
        {
            _textSettings.UpdateVisualization(remainingTime, waitTime);
        }


        // Event Listeners
        protected virtual void HandleTimeout() => ResetVisualization();

        /// <summary>
        /// Resets the visualization.
        /// </summary>
        public virtual void ResetVisualization()
        {
            _textSettings.ResetVisualization();
        }

        /// <summary>
        /// A helper class to hold the settings for the visualization text of a timer.
        /// </summary>
        [Serializable]
        public class TextSettings
        {
            /// <summary>
            /// Default format for displaying the remaining time. Uses `string.Format`. Any occurrence of '{0}' will be replaced with the time.
            /// </summary>
            public const string DEFAULT_TIME_DISPLAY_FORMAT = "{0}";

            /// <summary>
            /// If the text should be shown.
            /// </summary>
            public bool TextEnabled = true;

            /// <summary>
            /// A format string how the time is displayed. Uses `string.Format`. Any occurrence of '{0}' will be replaced with the time.
            /// </summary>
            [Tooltip("A format string how the time is displayed. Uses `string.Format`. Any occurrence of '{0}' will be replaced with the time.")]
            public string TimeDisplayFormatter = DEFAULT_TIME_DISPLAY_FORMAT;

            /// <summary>
            /// If only seconds or also milliseconds should be displayed.
            /// </summary>
            public bool ShowMilliseconds;

            /// <summary>
            /// Text displayed when the timer times out or isn't running.
            /// </summary>
            public string TimeoutText = "0";

            /// <summary>
            /// How the remaining time is displayed (i.e. count up or down).
            /// </summary>
            public CountDirection CountType = CountDirection.Down;

            /// <summary>
            /// Color of the text.
            /// </summary>
            [SerializeField]
            [Tooltip("Color of the text.")]
            protected Color _color = Color.white;
            public Color Color
            {
                get => _color;
                set
                {
                    _color = value;
                    UpdateColors();
                }
            }

            /// <summary>
            /// Format string for displaying time as float or not.
            /// </summary>
            protected string TimeValueFormatter
            {
                get => ShowMilliseconds ? "F2" : "F0";
            }

            /// <summary>
            /// Text to display the time.
            /// </summary>
            [SerializeField]
            [Tooltip("Text to display the time.")]
            protected TMP_Text _text;
            public TMP_Text Text
            {
                get => _text;
                set
                {
                    _text = value;
                    UpdateColors();
                }
            }

            /// <summary>
            /// Updates the text color.
            /// </summary>
            public void UpdateColors()
            {
                if (_text != null)
                {
                    _text.color = Color;
                }
            }

            /// <summary>
            /// Updates the visualization of the time.
            /// </summary>
            /// <param name="remainingTime">Remaining time of the timer.</param>
            /// <param name="waitTime">Total wait time of the timer.</param>
            public void UpdateVisualization(float remainingTime, float waitTime)
            {
                if (Text == null)
                {
                    // No text -> Nothing to update
                    return;
                }

                UpdateColors();

                // Update Text visibility
                Text.gameObject.SetActive(TextEnabled);

                if (!TextEnabled)
                {
                    // Text disabled -> nothing to do
                    return;
                }

                float time = CountType == CountDirection.Up ? waitTime - remainingTime : remainingTime;

                string timeValue = time.ToString(TimeValueFormatter);
                Text.text = string.Format(TimeDisplayFormatter, timeValue);
            }

            /// <summary>
            /// Resets the visualization.
            /// </summary>
            public void ResetVisualization()
            {
                Text.text = TimeoutText;
            }
        }

        /// <summary>
        /// How the text indicates the remaining time. Either counting up or down.
        /// </summary>
        public enum CountDirection
        {
            Up, /// <summary> Text is counting up from 0. </summary>
            Down /// <summary> Text is counting down to 0. </summary>
        }
    }
}