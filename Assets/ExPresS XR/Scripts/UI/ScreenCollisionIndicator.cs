using System.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace ExPresSXR.UI
{
    public class ScreenCollisionIndicator : MonoBehaviour
    {
        /// <summary>
        /// The strength of the indicator. Ranging from 0.0f (=invisible) to 1.0f(=completely visible).
        /// </summary>
        [Range(0f, 1f)]
        [SerializeField]
        private float _strength;
        public float strength
        {
            get => _strength;
            set
            {
                _strength = value;

                UpdateIndicator();
            }
        }

        /// <summary>
        /// The color the indicator is tinted.
        /// </summary>
        [SerializeField]
        private Color _indicatorColor;
        public Color indicatorColor
        {
            get => _indicatorColor;
            set
            {
                _indicatorColor = value;

                UpdateIndicator();
            }
        }

        private Coroutine fadeCoroutine;

        private Image _fadeImage;


        /// <summary>
        /// Fades the indicator in (i.e. makes it visible) over time. Values less or equal fade instant.
        /// </summary>
        /// <param name="duration">Duration of the fade in seconds.</param>
        public void FadeIn(float duration) => fadeCoroutine = StartCoroutine(FadeCoroutine(1.0f, duration));

        /// <summary>
        /// Fades the indicator in (i.e. makes hides it) over time. Values less or equal fade instant.
        /// </summary>
        /// <param name="duration">Duration of the fade in seconds.</param>
        public void FadeOut(float duration) => fadeCoroutine = StartCoroutine(FadeCoroutine(0.0f, duration));


        private IEnumerator FadeCoroutine(float toAlpha, float fadeDuration)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            if (fadeDuration <= 0.0f)
            {
                strength = toAlpha;
            }
            else
            {
                float fromAlpha = _strength;
                float elapsedTime = 0.0f;
                while (elapsedTime < fadeDuration)
                {
                    elapsedTime += Time.deltaTime;
                    strength = Mathf.Lerp(fromAlpha, toAlpha, elapsedTime / fadeDuration);
                    yield return null;
                }
            }
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
            strength = _strength;
            indicatorColor = _indicatorColor;
        }
    }
}