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

                if (!_timer.running)
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
            if (_timer != null && _timer.running)
            {
                UpdateUI(_timer.remainingTime, _timer.waitTime);
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

        [Serializable]
        public class TextSettings
        {
            public const string DEFAULT_TIME_DISPLAY_FORMAT = "{0}";
            public bool TextEnabled = true;
            
            [Tooltip("A format string how the time is displayed. Any occurrence of '{0}' will be replaced with the time.")]
            public string TimeDisplayFormatter = DEFAULT_TIME_DISPLAY_FORMAT;
            public bool ShowMilliseconds;
            public string TimeoutText = "0";
            public CountDirection CountType = CountDirection.Down;

            [SerializeField]
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

            protected string TimeValueFormatter
            {
                get => ShowMilliseconds ? "F2" : "F0";
            }

            [SerializeField]
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

            public void UpdateColors()
            {
                if (_text != null)
                {
                    _text.color = Color;
                }
            }

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

            public void ResetVisualization()
            {
                Text.text = TimeoutText;
            }
        }

        // Enums
        public enum CountDirection
        {
            Up,
            Down
        }
    }
}