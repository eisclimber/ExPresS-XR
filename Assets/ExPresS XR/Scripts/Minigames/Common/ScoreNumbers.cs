using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;


namespace ExPresSXR.Minigames.Common
{
    public class ScoreNumbers : MonoBehaviour
    {

        [SerializeField]
        private TMP_Text _text;

        [SerializeField]
        private TMP_Text _bonusText;


        [SerializeField]
        private Animator _animator;

        [Space]

        [SerializeField]
        private bool _showSigns;

        [SerializeField]
        private bool _hideIfZero;

        [SerializeField]
        private string _scorePrefix = "";

        [SerializeField]
        private string _bonusPrefix = "";

        [SerializeField]
        private string _noBonusText = "";

        [SerializeField]
        private bool _randomColor = true;

        [SerializeField]
        private float _randomColorSaturation = 0.9f;

        [SerializeField]
        private float _randomColorValue = 0.9f;


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


        public void SetupScoreData(int score, int bonus = 0, string scorePrefix = "", string bonusPrefix = "", string noBonusText = "")
        {
            _scorePrefix = scorePrefix;
            _bonusPrefix = bonusPrefix;
            _noBonusText = noBonusText;
            // Set scores after setting the prefixes
            Score = score;
            BonusScore = bonus;
        }

        public void RandomizeColorHue(bool bonusSameColor = false)
        {
            Color textColor = GetRandomHueColor();
            _text.color = textColor;
            if (_bonusText != null)
            {
                _bonusText.color = bonusSameColor ? textColor : GetRandomHueColor();
            }
        }


        public void SetScoreColor(Color fontColor)
        {
            _text.color = fontColor;
        }

        public void SetBonusColor(Color fontColor)
        {
            _text.color = fontColor;
        }


        public void SetOutlineColor(Color outlineColor)
        {
            _text.outlineColor = outlineColor;
            if (_bonusText != null)
            {
                _bonusText.outlineColor = outlineColor;
            }
        }

        public void DestroySelf() => Destroy(gameObject);

        public Color GetRandomHueColor() => Color.HSVToRGB(Random.Range(0.0f, 1.0f), _randomColorSaturation, _randomColorValue);
    }
}