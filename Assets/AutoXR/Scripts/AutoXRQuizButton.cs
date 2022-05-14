using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AutoXRQuizButton : AutoXRBaseButton
{
    public bool correctChoice = false;

    [SerializeField]
    private string _answerText;
    public string answerText
    {
        get => _answerText;
        set
        {
            _answerText = value;


        }
    }
    
    [SerializeField]
    private GameObject _answerObject;
    public GameObject answerObject
    {
        get => _answerObject;
        set
        {
            _answerObject = value;

            _answerObject.transform.SetParent(pushAnchor.transform);
        }
    }


    public UnityEvent OnPressedCorrect;
    public UnityEvent OnPressedIncorrect;


    public Text feedbackTextLabel;


    protected override void Awake()
    {
        base.Awake();
        
        OnReleased.AddListener(NotifyCorrectChoice);
    }

    private void NotifyCorrectChoice()
    {
        if (correctChoice)
        {
            OnPressedCorrect.Invoke();
        }
        else
        {
            OnPressedIncorrect.Invoke();
        }
    }
}
