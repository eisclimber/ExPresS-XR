using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FadeRect : MonoBehaviour
{
    public Color fadeColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    
    public float fadeToBlackTime = 0.5f;

    public float fadeToClearTime = 0.5f;

    [SerializeField]
    private float _fadeDirection = 0.0f;

    [SerializeField]
    private Image _fadeImage;
    

    // Screen NOT visible
    public bool completelyVisible
    {
        get => (fadeColor.a == 1.0f);
    }

    // Screen visible
    public bool completelyHidden
    {
        get => (fadeColor.a == 0.0f);
    }


    // Start is called before the first frame update
    void Awake()
    {
        _fadeImage = GetComponent<Image>();
        UpdateFadeImage();
    }

    public void FadeToColor(bool instant = false)
    {
        if (!instant)
        {
            _fadeDirection = 1.0f;
        }
        else
        {
            fadeColor.a = 1.0f;
            UpdateFadeImage();
        }
    }

    public void FadeToClear(bool instant = false)
    {
        if (!instant)
        {
            _fadeDirection = -1.0f;
        }
        else
        {
            fadeColor.a = 0.0f;
            UpdateFadeImage();
        }
    }

    private void Update()
    {
        if (_fadeDirection < 0.0f)
        {
            // Fade to Black
            fadeColor.a = Mathf.Max(0.0f, fadeColor.a - (Time.deltaTime / fadeToBlackTime));
        }
        else if (_fadeDirection > 0.0f)
        {
            // Fade to Clear
            fadeColor.a = Mathf.Min(1.0f, fadeColor.a + (Time.deltaTime / fadeToClearTime));
        }

        if (_fadeImage != null)
        {
            _fadeImage.color = fadeColor;
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
        }
    }
}
