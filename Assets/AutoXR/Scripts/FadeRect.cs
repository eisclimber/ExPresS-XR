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
    

    // Start is called before the first frame update
    void Awake()
    {
        _fadeImage = GetComponent<Image>();
        if (_fadeImage != null)
        {
            _fadeImage.color = fadeColor;
        }
    }

    public void FadeToColor(bool instant = false)
    {
        _fadeDirection = 1.0f;
        if (instant)
        {
            fadeColor.a = 1.0f;
        }
    }

    private void FadeToClear(bool instant = false)
    {
        _fadeDirection = -1.0f;
        if (instant)
        {
            fadeColor.a = 0.0f;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            FadeToColor(Input.GetKey(KeyCode.LeftShift));
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            FadeToClear(Input.GetKey(KeyCode.LeftShift));
        }

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
}
