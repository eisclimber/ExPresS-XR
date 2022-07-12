using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class QuizButton : BaseButton
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

            if (_feedbackTextLabel != null)
            {
                _feedbackTextLabel.text = _answerText;
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
            _answerPrefab = value;
            
            if (_feedbackObjectSocket != null)
            {
                _feedbackObjectSocket.putBackPrefab = _answerPrefab;
            }
        }
    }

    [SerializeField]
    private Text _feedbackTextLabel;


    [SerializeField]
    private PutBackSocketInteractor _feedbackObjectSocket;
    public PutBackSocketInteractor feedbackObjectSocket
    {
        get => _feedbackObjectSocket;
        set
        {
            _feedbackObjectSocket = value;
        }
    }


    public UnityEvent OnPressedCorrect;
    public UnityEvent OnPressedIncorrect;    


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
