using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;


namespace ExPresSXR.UI
{
    [RequireComponent(typeof(Image))]
    public class FadeRect : MonoBehaviour
    {
        public Color fadeColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);

        public float fadeToColorTime = 0.5f;

        public float fadeToClearTime = 0.5f;

        [SerializeField]
        private Image _fadeImage;

        private float _fadeDirection = 0.0f;


        public UnityEvent OnFadeToColorCompleted;
        public UnityEvent OnFadeToCleanCompleted;


        // Screen NOT visible
        public bool screenCompletelyVisible
        {
            get => (fadeColor.a == 1.0f);
        }

        // Screen visible
        public bool screenCompletelyHidden
        {
            get => (fadeColor.a == 0.0f);
        }


        // Start is called before the first frame update
        private void Awake()
        {
            _fadeImage = GetComponent<Image>();
            UpdateFadeImage();
        }

        public void FadeToColor(bool instant = false)
        {
            _fadeDirection = 1.0f;

            if (instant)
            {
                fadeColor.a = 1.0f;
                UpdateFadeImage();
            }
        }

        public void FadeToClear(bool instant = false)
        {
            _fadeDirection = -1.0f;

            if (instant)
            {
                fadeColor.a = 0.0f;
                UpdateFadeImage();
            }
        }

        private void Update()
        {
            float newFadeValue = fadeColor.a;

            if (_fadeDirection < 0.0f)
            {
                // Fade to Color
                newFadeValue = Mathf.Max(0.0f, fadeColor.a - (Time.deltaTime / fadeToColorTime));

                if (fadeColor.a > 1.0f && newFadeValue == 1.0f)
                {
                    // Fade to Color completed
                    OnFadeToColorCompleted.Invoke();
                }
            }
            else if (_fadeDirection > 0.0f)
            {
                // Fade to Clear
                newFadeValue = Mathf.Min(1.0f, fadeColor.a + (Time.deltaTime / fadeToClearTime));

                if (fadeColor.a < 0.0f && newFadeValue == 0.0f)
                {
                    // Fade to Clear completed
                    OnFadeToColorCompleted.Invoke();
                }
            }


            else if (fadeColor.a > 0.0f && newFadeValue == 1.0f)

                fadeColor.a = newFadeValue;

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
                _fadeImage.color = fadeColor;

#if UNITY_EDITOR
                // Instantaneously Update Editor Visuals
                EditorUtility.SetDirty(this);
#endif
            }
        }
    }
}