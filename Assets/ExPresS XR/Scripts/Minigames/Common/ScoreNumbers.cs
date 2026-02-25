using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;


namespace ExPresSXR.Minigames.Common
{
    /// <summary>
    /// Utility for setting up text-based feedback objects when scoring points in a minigame.
    /// </summary>
    public class ScoreNumbers : MonoBehaviour
    {
        /// <summary>
        /// Text displaying the score.
        /// </summary>
        [SerializeField]
        [Tooltip("Text displaying the score.")]
        private TMP_Text _text;

        /// <summary>
        /// Text displaying an optional bonus score.
        /// </summary>
        [SerializeField]
        [Tooltip("Text displaying an optional bonus score.")]
        private TMP_Text _bonusText;

        [Space]

        /// <summary>
        /// Displays points with signs (+ or -).
        /// If disabled only negative values will be displayed with a `-`.
        /// </summary>
        [SerializeField]
        [Tooltip("Displays points with signs (+ or -). If disabled only negative values will be displayed with a `-`.")]
        private bool _showSigns;

        /// <summary>
        /// Hides visuals if the score is zero.
        /// </summary>
        [SerializeField]
        [Tooltip("Hides visuals if the score is zero.")]
        private bool _hideIfZero;

        /// <summary>
        /// Prefix for the score.
        /// </summary>
        [SerializeField]
        [Tooltip("Prefix for the score.")]
        private string _scorePrefix = "";

        /// <summary>
        /// Prefix for the bonus.
        /// </summary>
        [SerializeField]
        [Tooltip("Prefix for the bonus.")]
        private string _bonusPrefix = "";

        /// <summary>
        /// Text displayed if no bonus was achieved (=0).
        /// </summary>
        [SerializeField]
        [Tooltip("Text displayed if no bonus was achieved (=0).")]
        private string _noBonusText = "";

        /// <summary>
        /// If the text color should be randomized.
        /// </summary>
        [SerializeField]
        [Tooltip("If the text color should be randomized.")]
        private bool _randomColor = true;

        /// <summary>
        /// Saturation of the randomized text colors based based on the HSV color system.
        /// </summary>
        [SerializeField]
        [Tooltip("Saturation of the randomized text colors based based on the HSV color system.")]
        private float _randomColorSaturation = 0.9f;


        /// <summary>
        /// Value of the randomized text colors based on the HSV color system.
        /// </summary>
        [SerializeField]
        [Tooltip("Value of the randomized text colors based on the HSV color system.")]
        private float _randomColorValue = 0.9f;


        /// <summary>
        /// Score to be displayed.
        /// </summary>
        public int Score
        {
            set
            {
                if (_text != null)
                {
                    if (_hideIfZero && value == 0)
                    {
                        _text.text = "";
                    }
                    else
                    {
                        _text.text = _showSigns ? _scorePrefix + value.ToString("+#;-#;0") : value.ToString();
                    }
                }
                else
                {
                    Debug.LogError($"Could not display score '{value}', no Text to display set.");
                }
            }
        }

        /// <summary>
        /// BonusScore to be displayed.
        /// </summary>
        public int BonusScore
        {
            set
            {
                if (_bonusText != null)
                {
                    _bonusText.text = value > 0 ? _bonusPrefix + value.ToString() : _noBonusText;
                }
                else
                {
                    Debug.LogError($"Could not display bonus score '{value}', no Text to display set.");
                }
            }
        }

        private void OnEnable()
        {
            if (_randomColor)
            {
                _text.color = GetRandomHueColor();
                if (_bonusText != null)
                {
                    _bonusText.color = GetRandomHueColor();
                }
            }
        }

        /// <summary>
        /// Sets up the score with the provided data.
        /// </summary>
        /// <param name="score">Score to display.</param>
        /// <param name="bonus">Optional bonus to display.</param>
        /// <param name="scorePrefix">Optional score prefix to add.</param>
        /// <param name="bonusPrefix">Optional bonus score prefix to add.</param>
        /// <param name="noBonusText">Optional no bonus text display.</param>
        public void SetupScoreData(int score, int bonus = 0, string scorePrefix = "", string bonusPrefix = "", string noBonusText = "")
        {
            _scorePrefix = scorePrefix;
            _bonusPrefix = bonusPrefix;
            _noBonusText = noBonusText;
            // Set scores after setting the prefixes
            Score = score;
            BonusScore = bonus;
        }

        /// <summary>
        /// Randomizes the Hue of the text colors.
        /// </summary>
        /// <param name="bonusSameColor">If the score and bonus should have the same color.</param>
        public void RandomizeColorHue(bool bonusSameColor = false)
        {
            Color textColor = GetRandomHueColor();
            _text.color = textColor;
            if (_bonusText != null)
            {
                _bonusText.color = bonusSameColor ? textColor : GetRandomHueColor();
            }
        }

        /// <summary>
        /// Sets the color of the score.
        /// </summary>
        /// <param name="fontColor">Color to set.</param>
        public void SetScoreColor(Color fontColor)
        {
            _text.color = fontColor;
        }

        /// <summary>
        /// Sets the color of the bonus score.
        /// </summary>
        /// <param name="fontColor">Color to set.</param>

        public void SetBonusColor(Color fontColor)
        {
            _text.color = fontColor;
        }

        /// <summary>
        /// Sets the outline colors of both scores.
        /// </summary>
        /// <param name="fontColor">Color to set.</param>

        public void SetOutlineColor(Color outlineColor)
        {
            _text.outlineColor = outlineColor;
            if (_bonusText != null)
            {
                _bonusText.outlineColor = outlineColor;
            }
        }

        /// <summary>
        /// Destroys the GameObject for cleaning up the display.
        /// </summary>
        public void DestroySelf() => Destroy(gameObject);

        /// <summary>
        /// Creates a new random hue color with the configured saturation and value.
        /// </summary>
        /// <returns>Random color.</returns>
        public Color GetRandomHueColor() => Color.HSVToRGB(Random.Range(0.0f, 1.0f), _randomColorSaturation, _randomColorValue);
    }
}