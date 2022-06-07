using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AutoXRMcConfirmButton : AutoXRQuizButton
{
    [SerializeField]
    private AutoXRQuizButton[] _answerButtons;
    public AutoXRQuizButton[] answerButtons
    {
        get => _answerButtons;
        set => _answerButtons = value;
    }


    protected override void Awake()
    {
        base.Awake();
    }

    protected override void NotifyChoice()
    {
        bool allCorrect = true;
        foreach (AutoXRQuizButton button in _answerButtons)
        {
            if (button != null)
            {
                allCorrect &= button.GiveMultipleChoiceFeedback();
            }
        }

        if (allCorrect)
        {
            OnPressedCorrect.Invoke();
        }
        else
        {
            OnPressedIncorrect.Invoke();
        }
    }

}
