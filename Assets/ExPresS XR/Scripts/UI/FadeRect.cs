using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using System.Collections;


namespace ExPresSXR.UI
{
    [RequireComponent(typeof(Image))]
    public class FadeRect : MonoBehaviour
    {
        /// <summary>
        /// The color to be faded to.
        /// Default is Transparent Black (`new(0.0f, 0.0f, 0.0f, 0.0f`).
        /// </summary>
        private Color _fadeColor = new(0.0f, 0.0f, 0.0f, 0.0f);
        public Color FadeColor
        {
            get => _fadeColor;
            set => _fadeColor = value;
        }

        /// <summary>
        /// Duration in seconds of a fade to black.
        /// </summary>
        public float _defaultFadeToColorTime = 0.5f;
        public float DefaultFadeToColorTime
        {
            get => _defaultFadeToColorTime;
            set => _defaultFadeToColorTime = value;
        }

        /// <summary>
        /// Duration in seconds of a fade to transparent.
        /// </summary>
        public float _defaultFadeToClearTime = 0.5f;
        public float DefaultFadeToClearTime
        {
            get => _defaultFadeToClearTime;
            set => _defaultFadeToClearTime = value;
        }

        /// <summary>
        /// Reference to the image used for fading.
        /// </summary>
        [SerializeField]
        private Image _fadeImage;

        private Coroutine _fadeCoroutine;


        /// <summary>
        /// Emitted when any fade was completed (excluding instant ones).
        /// </summary>
        public UnityEvent OnFadeCompleted;

        /// <summary>
        /// Emitted when a fade to color was completed (excluding instant ones).
        /// </summary>
        public UnityEvent OnFadeToColorCompleted;

        /// <summary>
        /// Emitted when a fade to clear was completed (excluding instant ones).
        /// </summary>
        public UnityEvent OnFadeToClearCompleted;


        // Screen visible
        public bool ScreenCompletelyVisible
        {
            get => FadeColor.a == 0.0f;
        }

        // Screen NOT visible
        public bool ScreenCompletelyHidden
        {
            get => FadeColor.a == 1.0f;
        }

        private void Start()
        {
            if (_fadeImage == null && !TryGetComponent(out _fadeImage))
            {
                Debug.LogWarning("FadeRect has no _fadeImage set to fade.", this);
            }
            UpdateFadeImage();
        }

        /// <summary>
        /// Starts a fade to color with the default duration.
        /// </summary>
        [ContextMenu("Fade to Color over default time")]
        public void FadeToColor() => StartFadeCoroutine(1.0f, _defaultFadeToColorTime);

        /// <summary>
        /// Starts a fade to color with the specified duration.
        /// </summary>
        /// <param name="duration">Custom duration of the fade.</param>
        public void FadeToColorWithDuration(float duration) => StartFadeCoroutine(1.0f, duration);

        /// <summary>
        /// Starts a fade to color instantaneously. Active fades will be canceled.
        /// </summary>
        [ContextMenu("Fade to color instant")]
        public void FadeToColorInstant() => FadeColorInstant(1.0f);


        /// <summary>
        /// Starts a fade to color with the default duration.
        /// </summary>
        [ContextMenu("Fade to clear over default time")]
        public void FadeToClear() => StartFadeCoroutine(0.0f, _defaultFadeToClearTime);

        /// <summary>
        /// Starts a fade to clear with the specified duration.
        /// </summary>
        /// <param name="duration">Custom duration of the fade.</param>
        public void FadeToClearWithDuration(float duration) => StartFadeCoroutine(0.0f, duration);

        /// <summary>
        /// Starts a fade to clear instantaneously. Active fades will be canceled.
        /// </summary>
        [ContextMenu("Fade to clear instant")]
        public void FadeToClearInstant() => FadeColorInstant(0.0f);


        private void StartFadeCoroutine(float toAlpha, float duration = -1.0f)
        {
            StopFadeCoroutine();
            _fadeCoroutine = StartCoroutine(ChangeAlphaOverTime(toAlpha, duration));
        }

        private void StopFadeCoroutine()
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }
        }

        private void FadeColorInstant(float alpha)
        {
            // Stop any previous fade
            StopFadeCoroutine();
            // Change fade to fully opaque
            ChangeFadeRectAlpha(alpha);
        }


        private void ChangeFadeRectAlpha(float value)
        {
            _fadeColor.a = value;
            UpdateFadeImage();
        }

        private void UpdateFadeImage()
        {
            if (_fadeImage == null)
            {
                _fadeImage = GetComponent<Image>();
            }

            if (_fadeImage != null)
            {
                _fadeImage.color = FadeColor;

#if UNITY_EDITOR
                // Instantaneously update editor visuals
                EditorUtility.SetDirty(this);
#endif
            }
        }

        private IEnumerator ChangeAlphaOverTime(float toAlpha, float duration)
        {
            float elapsed = 0.0f;
            float startAlpha = FadeColor.a;

            while (elapsed < duration)
            {
                float progress = elapsed / duration;
                float alpha = Mathf.Lerp(startAlpha, toAlpha, progress);
                ChangeFadeRectAlpha(alpha);
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Ensure fully faded
            ChangeFadeRectAlpha(toAlpha);
            
            if (toAlpha == 0.0f)
            {
                OnFadeToClearCompleted.Invoke();
            }

            if (toAlpha == 1.0f)
            {
                OnFadeToColorCompleted.Invoke();
            }
            
            OnFadeCompleted.Invoke();
        }
    }
}