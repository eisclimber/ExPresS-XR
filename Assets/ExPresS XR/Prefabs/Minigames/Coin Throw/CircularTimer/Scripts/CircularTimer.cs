using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CircularTimer : MonoBehaviour
{
    public enum CountDirection { countUp, countDown }
    public enum FillDirection { fillUp, fillDown }
    public enum FillType { tick, smooth }
    public float duration;

    [System.Serializable]
    public class FillSettings
    {
        public Color color;
        public FillType fillType;
        public Image fillImage;
        public FillDirection fillDirection;
        public bool capEnabled;
        public Image headCapImage;
        public Image tailCapImage;
    }
    public FillSettings fillSettings;

    [System.Serializable]
    public class BackgroundSettings
    {
        public bool enabled;
        public Color color;
        public Image backgroundImage;
    }
    public BackgroundSettings backgroundSettings;
    
    [System.Serializable]
    public class TextSettings
    {
        public bool enabled;
        public bool millisecond;
        public Text text;
        public Color color;
        public CountDirection countType;
    }
    public TextSettings textSettings;

    public UnityEvent didFinishedTimerTime;

    public float CurrentTime { get; private set; }
    // float _AfterImageTime;

    bool isPaused = true;

    void Update()
    {
        if (!isPaused)
        {
            CurrentTime += Time.deltaTime;
            
            if (CurrentTime >= duration)
            {
                isPaused = true;
                CurrentTime = duration;
                didFinishedTimerTime.Invoke();
            }

            switch (fillSettings.fillDirection)
            {
                case FillDirection.fillDown:

                    if (fillSettings.fillType == FillType.smooth)
                    {
                        fillSettings.fillImage.fillAmount = CurrentTime / duration;
                    }
                    else if (fillSettings.fillType == FillType.tick)
                    {
                        fillSettings.fillImage.fillAmount = (float)System.Math.Round(CurrentTime / duration, 1);
                    }
                    break;
                case FillDirection.fillUp:
                    if (fillSettings.fillType == FillType.smooth)
                    {
                        fillSettings.fillImage.fillAmount = (duration - CurrentTime) / duration;
                    }
                    else if (fillSettings.fillType == FillType.tick)
                    {
                        fillSettings.fillImage.fillAmount = (float)System.Math.Round((duration - CurrentTime) / duration, 1);
                    }
                    break;
            }
            UpdateUI();
        }
    }

    void showCapImage(bool isShow)
    {
        fillSettings.headCapImage.enabled = isShow;
        fillSettings.tailCapImage.enabled = isShow;
    }

    public void UpdateUI()
    {
        fillSettings.fillImage.color = fillSettings.color;
        fillSettings.headCapImage.color = fillSettings.color;
        fillSettings.tailCapImage.color = fillSettings.color;

        textSettings.text.color = textSettings.color;

        if (backgroundSettings.enabled)
        {
            backgroundSettings.backgroundImage.gameObject.SetActive(true);
            backgroundSettings.backgroundImage.color = backgroundSettings.color;
        }
        else
        {
            backgroundSettings.backgroundImage.gameObject.SetActive(false);
        }

        if (fillSettings.capEnabled)
        {
            showCapImage(true);

            if (fillSettings.fillImage.fillAmount == 0f)
            {
                showCapImage(false);
            }
            else
            {
                showCapImage(true);
            }

            Vector3 capRotaionValue = Vector3.zero;
            capRotaionValue.z = 360 * (1 - fillSettings.fillImage.fillAmount);
            fillSettings.tailCapImage.rectTransform.localRotation = Quaternion.Euler(capRotaionValue);
        }
        else
        {
            showCapImage(false);
        }

        if (textSettings.enabled)
        {
            textSettings.text.gameObject.SetActive(true);

            float time = CurrentTime;

            switch (textSettings.countType)
            {
                case CountDirection.countUp:
                    if (textSettings.millisecond)
                    {
                        textSettings.text.text = time.ToString("F2");
                    }
                    else
                    {
                        textSettings.text.text = time.ToString("F0");
                    }
                    break;
                case CountDirection.countDown:
                    if (textSettings.millisecond)
                    {
                        textSettings.text.text = (duration - time).ToString("F2");
                    }
                    else
                    {
                        textSettings.text.text = (duration - time).ToString("F0");
                    }
                    break;
            }
        }
        else
        {
            textSettings.text.gameObject.SetActive(false);
        }
    }

    public void PauseTimer()
    {
        isPaused = true;
    }

    public void StartTimer()
    {
        isPaused = false;
    }

    public void StopTimer()
    {
        CurrentTime = 0;
        // AfterImageTime = 0;
        isPaused = true;
        ResetTimer();
    }

    void ResetTimer()
    {
        switch (fillSettings.fillDirection)
        {
            case FillDirection.fillDown:
                fillSettings.fillImage.fillAmount = 0;
                break;
            case FillDirection.fillUp:
                fillSettings.fillImage.fillAmount = 1;
                break;
        }
    }
}
