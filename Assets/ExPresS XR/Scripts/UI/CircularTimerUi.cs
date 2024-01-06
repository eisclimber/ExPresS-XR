using UnityEngine;
using UnityEngine.UI;
using ExPresSXR.Misc.Timing;
using System;
using TMPro;


namespace ExPresSXR.UI
{
    public class CircularTimerUi : MonoBehaviour
    {
        [SerializeField]
        private Timer _timer;

        [SerializeField]
        private FillSettings _fillSettings;

        [SerializeField]
        private TextSettings _textSettings;


        private void OnEnable()
        {
            if (_timer == null && !TryGetComponent(out _timer))
            {
                Debug.LogError("No Timer was provided for the CircularTimerUi. Cannot display a non-existent timer.", this);
            }
            else
            {
                _timer.OnTimeout.AddListener(HandleTimeout);
            }
        }

        private void OnDisable()
        {
            if (_timer != null)
            {
                _timer.OnTimeout.RemoveListener(HandleTimeout);
            }
        }

        private void Update()
        {
            if (_timer != null && _timer.running)
            {
                UpdateUI(_timer.remainingTime, _timer.waitTime);
            }
        }

        private void UpdateUI(float remainingTime, float waitTime)
        {
            _fillSettings.UpdateVisualization(remainingTime, waitTime);
            _textSettings.UpdateVisualization(remainingTime, waitTime);
        }


        // Event Listeners

        private void HandleTimeout() => ResetVisualization();

        public void ResetVisualization() => _fillSettings.ResetVisualization();

        // Helper classes
        [Serializable]
        public class FillSettings
        {
            public FillDirection fillDirection = FillDirection.Down;
            public ProgressType fillType = ProgressType.Smooth;

            [SerializeField]
            private bool _capsEnabled;
            public bool capsEnabled
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
            public Color color
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
            public Image fillImage
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
            public Image startCapImage
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
            public Image endCapImage
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
                if (fillImage != null)
                {
                    fillImage.color = color;
                }
                if (_startCapImage != null)
                {
                    _startCapImage.color = color;
                }
                if (_endCapImage != null)
                {
                    _endCapImage.color = color;
                }
            }

            public void UpdateCaps()
            {
                if (_startCapImage != null)
                {
                    _startCapImage.enabled = capsEnabled && (_fillImage == null || _fillImage.fillAmount > 0.0f);
                }
                if (_endCapImage != null)
                {
                    _endCapImage.enabled = capsEnabled && (_fillImage == null || _fillImage.fillAmount > 0.0f);
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

                if (capsEnabled)
                {
                    Vector3 capRotationValue = Vector3.zero;
                    capRotationValue.z = 360.0f * (1.0f - fillImage.fillAmount);
                    _endCapImage.rectTransform.localRotation = Quaternion.Euler(capRotationValue);
                }
            }

            public void ResetVisualization()
            {
                _fillImage.fillAmount = fillDirection == FillDirection.Down ? 0 : 1;
            }
        }

        [Serializable]
        public class TextSettings
        {
            public bool textEnabled = true;
            public bool showMilliseconds;
            public CountDirection countType = CountDirection.Down;

            [SerializeField]
            private Color _color = Color.white;
            public Color color
            {
                get => _color;
                set
                {
                    _color = value;
                    UpdateColors();
                }
            }

            private string timeFormatter { get => showMilliseconds ? "F2" : "F0"; }

            [SerializeField]
            private TMP_Text _text;
            public TMP_Text text
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
                    _text.color = color;
                }
            }

            public void UpdateVisualization(float remainingTime, float waitTime)
            {
                if (text == null)
                {
                    // No text -> Nothing to update
                    return;
                }

                UpdateColors();

                // Update Text visibility
                text.gameObject.SetActive(textEnabled);

                if (!textEnabled)
                {
                    // Text disabled -> nothing to do
                    return;
                }

                float time = countType == CountDirection.Up
                                ? waitTime - remainingTime
                                : remainingTime;

                text.text = time.ToString(timeFormatter);
            }
        }

        // Enums
        public enum CountDirection
        {
            Up,
            Down
        }

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