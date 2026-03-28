using System.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace ExPresSXR.UI
{
    /// <summary>
    /// A visual indicator for on screen collision that fades the screen corners.A visual indicator for on screen collision that fades the screen corners.A visual indicator for on screen collision that fades the screen corners.
    /// </summary>
    public class ScreenCollisionIndicator : MonoBehaviour
    {
        [Range(0f, 1f)]
        [SerializeField]
        [Tooltip("The strength of the indicator. Ranging from 0.0f (=invisible) to 1.0f(=completely visible).")]
        private float _strength;
        /// <summary>
        /// The strength of the indicator. Ranging from 0.0f (=invisible) to 1.0f(=completely visible).
        /// </summary>
        public float Strength
        {
            get => _strength;
            set
            {
                _strength = value;

                UpdateIndicator();
            }
        }

        [SerializeField]
        [Tooltip("The color the indicator is tinted.")]
        private Color _indicatorColor;
        /// <summary>
        /// The color the indicator is tinted.
        /// </summary>
        public Color IndicatorColor
        {
            get => _indicatorColor;
            set
            {
                _indicatorColor = value;

                UpdateIndicator();
            }
        }

        private Coroutine _fadeCoroutine;

        private Image _fadeImage;


        /// <summary>
        /// Fades the indicator in (i.e. makes it visible) over time. Values less or equal fade instant.
        /// </summary>
        /// <param name="duration">Duration of the fade in seconds.</param>
        public void FadeIn(float duration) => _fadeCoroutine = StartCoroutine(FadeCoroutine(1.0f, duration));

        /// <summary>
        /// Fades the indicator in (i.e. makes hides it) over time. Values less or equal fade instant.
        /// </summary>
        /// <param name="duration">Duration of the fade in seconds.</param>
        public void FadeOut(float duration) => _fadeCoroutine = StartCoroutine(FadeCoroutine(0.0f, duration));


        private IEnumerator FadeCoroutine(float toAlpha, float fadeDuration)
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = null;
            }

            if (fadeDuration <= 0.0f)
            {
                Strength = toAlpha;
            }
            else
            {
                float fromAlpha = _strength;
                float elapsedTime = 0.0f;
                while (elapsedTime < fadeDuration)
                {
                    elapsedTime += Time.deltaTime;
                    Strength = Mathf.Lerp(fromAlpha, toAlpha, elapsedTime / fadeDuration);
                    yield return null;
                }
            }
            _fadeCoroutine = null;
        }

        private void UpdateIndicator()
        {
            if (_fadeImage == null)
            {
                _fadeImage = GetComponent<Image>();
            }

            if (_fadeImage != null)
            {
                Color newColor = _indicatorColor;
                newColor.a = _strength;
                _fadeImage.color = newColor;
            }
        }


        private void OnValidate()
        {
            Strength = _strength;
            IndicatorColor = _indicatorColor;
        }
    }
}