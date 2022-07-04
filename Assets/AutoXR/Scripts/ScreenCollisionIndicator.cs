using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenCollisionIndicator : MonoBehaviour
{

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
    

    public void FadeIn(float _duration) => fadeCoroutine = StartCoroutine(FadeCoroutine(1.0f, _duration));
    

    public void FadeOut(float _duration) => fadeCoroutine = StartCoroutine(FadeCoroutine(0.0f, _duration));
    

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


    private void OnValidate() {
        strength = _strength;
        indicatorColor = _indicatorColor;
    }
}
