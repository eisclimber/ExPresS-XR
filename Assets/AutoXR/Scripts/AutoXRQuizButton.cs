using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEditor;

public class AutoXRQuizButton : AutoXRBaseButton
{
    public bool correctChoice;
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
    }


    public void DisplayAnswer(string answerText, GameObject answerObject, bool correctChoice)
    {
        this.answerText = answerText;
        this.answerPrefab = answerObject;
        this.correctChoice = correctChoice;
    }


    private void NotifyChoice()
    {
        if (correctChoice || (!correctChoice && invertedFeedback))
        {
            OnPressedCorrect.Invoke();
        }
        else
        {
            OnPressedIncorrect.Invoke();
        }
    }
}
