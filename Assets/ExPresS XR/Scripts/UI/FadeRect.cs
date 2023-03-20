using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;


namespace ExPresSXR.UI
{
    [RequireComponent(typeof(Image))]
    public class FadeRect : MonoBehaviour
    {
        public Color fadeColor = new(0.0f, 0.0f, 0.0f, 0.0f);

        public float fadeToColorTime = 0.5f;

        public float fadeToClearTime = 0.5f;

        [SerializeField]
        private Image _fadeImage;

        private FadeDirection _fadeDirection = FadeDirection.None;


        public UnityEvent OnFadeToColorCompleted;
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

        public void FadeToColor(bool instant = false)
        {
            _fadeDirection = FadeDirection.ToColor;

            if (instant)
            {
                fadeColor.a = 1.0f;
                UpdateFadeImage();
            }
        }

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
                    OnFadeToColorCompleted.Invoke();
                    _fadeDirection = FadeDirection.None;
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
                    OnFadeToClearCompleted.Invoke();
                    _fadeDirection = FadeDirection.None;
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