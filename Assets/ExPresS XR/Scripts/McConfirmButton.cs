using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class McConfirmButton : QuizButton
{
    [SerializeField]
    private QuizButton[] _answerButtons;
    public QuizButton[] answerButtons
    {
        get => _answerButtons;
        set => _answerButtons = value;
    }

    protected override void NotifyChoice()
    {
        bool allCorrect = true;
        foreach (QuizButton button in _answerButtons)
        {
            if (button != null)
            {
                allCorrect &= button.GiveMultipleChoiceFeedback();
            }
        }

        if (allCorrect != invertedFeedback)
        {
            OnPressedCorrect.Invoke();
        }
        else
        {
            OnPressedIncorrect.Invoke();
        }
    }

}
