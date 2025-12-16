using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


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
        private bool showSigns;

        [SerializeField]
        private bool hideIfZero;

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
                    if (hideIfZero && value == 0)
                    {
                        _text.text = "";
                    }
                    else
                    {
                        _text.text = showSigns ? _scorePrefix + value.ToString("+#;-#;0") : value.ToString();
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


        public void SetupScore(int score, int bonus, string scorePrefix = "", string bonusPrefix = "", string noBonusText = "")
        {
            _scorePrefix = scorePrefix;
            _bonusPrefix = bonusPrefix;
            _noBonusText = noBonusText;
            // Set scores after setting the prefixes
            Score = score;
            BonusScore = bonus;
        }

        public void ShowRandomHueScore(int score)
        {
            _text.color = GetRandomHueColor();
            if (_bonusText != null)
            {
                _bonusText.color = GetRandomHueColor();
            }

            ShowScore(score);
        }


        public void ShowScore(int score, Color fontColor)
        {
            ShowScore(score);
            _text.color = fontColor;
        }


        public void ShowScore(int score, Color fontColor, Color outlineColor)
        {
            ShowScore(score);
            _text.color = fontColor;
            _text.outlineColor = outlineColor;
        }

        public void ShowScore(int score)
        {
            _text.text = score.ToString();
        }

        public void DestroySelf() => Destroy(gameObject);

        public Color GetRandomHueColor() => Color.HSVToRGB(Random.Range(0.0f, 1.0f), _randomColorSaturation, _randomColorValue);
    }
}