using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;


namespace ExPresSXR.UI
{
    [RequireComponent(typeof(Image))]
    public class FadeRect : MonoBehaviour
    {
        /// <summary>
        /// The color to be faded to.
        /// Default is Transparent Black (`new(0.0f, 0.0f, 0.0f, 0.0f`).
        /// </summary>
        public Color fadeColor = new(0.0f, 0.0f, 0.0f, 0.0f);

        /// <summary>
        /// Duration in seconds of a fade to black.
        /// </summary>
        public float fadeToColorTime = 0.5f;

        /// <summary>
        /// Duration in seconds of a fade to transparent.
        /// </summary>
        public float fadeToClearTime = 0.5f;

        /// <summary>
        /// Reference to the image used for fading.
        /// </summary>
        [SerializeField]
        private Image _fadeImage;

        private FadeDirection _fadeDirection = FadeDirection.None;

        /// <summary>
        /// Emitted when a Fade to Color was completed.
        /// </summary>
        public UnityEvent OnFadeToColorCompleted;
        /// <summary>
        /// Emitted when a Fade to Clear was completed.
        /// </summary>
        public UnityEvent OnFadeToClearCompleted;


        // Screen visible
        public bool screenCompletelyVisible
        {
            get => fadeColor.a == 0.0f;
        }

        // Screen NOT visible
        public bool screenCompletelyHidden
        {
            get => fadeColor.a == 1.0f;
        }


        // Start is called before the first frame update
        private void Awake()
        {
            _fadeImage = GetComponent<Image>();
            UpdateFadeImage();
        }

        /// <summary>
        /// Starts a fade to color. 
        /// This will be done over the duration of `fadeToBlackTime` if `instant = false` or instantaneously otherwise.
        /// </summary>
        /// <param name="instant">If the fade should use `fadeToColorTime` or be instant.</param>
        public void FadeToColor(bool instant = false)
        {
            _fadeDirection = FadeDirection.ToColor;

            if (instant)
            {
                fadeColor.a = 1.0f;
                UpdateFadeImage();
            }
        }

        /// <summary>
        /// Starts a fade to clear.
        /// This will be done over the duration of `fadeToClearTime` if `instant = false` or instantaneously otherwise.
        /// </summary>
        /// <param name="instant">If the fade should use `fadeToClearTime` or be instant.</param>
        public void FadeToClear(bool instant = false)
        {
            _fadeDirection = FadeDirection.ToClear;

            if (instant)
            {
                fadeColor.a = 0.0f;
                UpdateFadeImage();
            }
        }

        private void Update()
        {
            float fadeDelta = Time.deltaTime / fadeToColorTime;

            if (_fadeDirection == FadeDirection.ToColor)
            {
                // Fade to Color
                float newFadeValue = Mathf.Clamp01(fadeColor.a + fadeDelta);

                // Alpha was below 1 and new value is 1.0f
                // => Fade to color completed
                if (fadeColor.a < 1.0f && newFadeValue == 1.0f)
                {
                    _fadeDirection = FadeDirection.None;
                    OnFadeToColorCompleted.Invoke();
                }

                fadeColor.a = newFadeValue;
                UpdateFadeImage();
            }
            else if (_fadeDirection == FadeDirection.ToClear)
            {
                // Fade to Clear
                float newFadeValue = Mathf.Clamp01(fadeColor.a - fadeDelta);

                // Alpha was below 1 and new value is 1.0f
                // => Fade to color completed
                if (fadeColor.a > 0.0f && newFadeValue == 0.0f)
                {
                    _fadeDirection = FadeDirection.None;                    
                    OnFadeToClearCompleted.Invoke();
                }

                fadeColor.a = newFadeValue;
                UpdateFadeImage();
            }
        }

        private void UpdateFadeImage()
        {
            if (_fadeImage == null)
            {
                _fadeImage = GetComponent<Image>();
            }

            if (_fadeImage != null)
            {
                _fadeImage.color = fadeColor;

#if UNITY_EDITOR
                // Instantaneously Update Editor Visuals
                EditorUtility.SetDirty(this);
#endif
            }
        }
    }

    public enum FadeDirection
    {
        None,
        ToColor,
        ToClear
    }
}