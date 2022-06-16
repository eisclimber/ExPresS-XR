using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AutoXRQuizButton : AutoXRBaseButton
{
    public bool correctChoice;
    public bool feedbackDisabled;
    public bool invertedFeedback;

    [SerializeField]
    private string _answerText;
    public string answerText
    {
        get => _answerText;
        set
        {
            _answerText = value;

            Transform textTransform = pushAnchor.Find("Canvas/Text");
            if (textTransform.GetComponent<Text>() != null)
            {
                textTransform.GetComponent<Text>().text = _answerText;
            }
        }
    }
    
    [SerializeField]
    private GameObject _answerPrefab;
    public GameObject answerPrefab
    {
        get => _answerPrefab;
        set
        {
            if (value == null)
            {
                Destroy(_answerPrefab);
            }
            _answerPrefab = value;

            if (pushAnchor != null && _answerPrefab != null)
            {
                // Instantiate Object
                GameObject answerObjectInstance = Instantiate<GameObject>(_answerPrefab, pushAnchor.transform);
                _answerPrefab = answerObjectInstance;
            }
        }
    }

    public UnityEvent OnPressedCorrect;
    public UnityEvent OnPressedIncorrect;    

    public Text feedbackTextLabel;


    ///////////
    private long triggerStartTime = -1;

    // Can be used to measure the time since between any point in time and a button press
    // Will be automatically started when input is (re-)enabled
    public void RestartTriggerTimer()
    {
        triggerStartTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }

    public float GetTriggerTimerValue()
    {
        return System.DateTimeOffset.Now.ToUnixTimeMilliseconds() - triggerStartTime;
    }


    ////// 

    protected override void Awake()
    {
        base.Awake();
        
        if (answerText != null && answerText != "")
        {
            answerText = _answerText;
        }
        if (answerPrefab != null)
        {
            answerPrefab = _answerPrefab;
        }

        OnPressed.AddListener(NotifyChoice);
        
        triggerStartTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }


    public void DisplayAnswer(string answerText, GameObject answerObject, bool correctChoice)
    {
        ClearAnswer();

        this.answerText = answerText;
        this.answerPrefab = answerObject;
        this.correctChoice = correctChoice;

        RestartTriggerTimer();
    }


    public void ClearAnswer()
    {
        answerText = "";
        correctChoice = false;
        if (answerPrefab != null)
        {
            Destroy(answerPrefab);
            answerPrefab = null;
        }

        ResetButtonPress();
    }


    protected virtual void NotifyChoice()
    {
        if (!feedbackDisabled && !toggleMode)
        {
            // (no invertedFeedback and correct) or (inverted and not correct)
            if (correctChoice != invertedFeedback)
            {
                OnPressedCorrect.Invoke();
            }
            else
            {
                OnPressedIncorrect.Invoke();
            }
        }
    }

    // Used to get feedback and 
    public bool GiveMultipleChoiceFeedback()
    {
        // Buttons must be in toggle mode for MC
        if (!toggleMode)
        {
            return false;
        }

        bool correctlyToggled = (pressed == correctChoice);

        return correctlyToggled;
    }
}
