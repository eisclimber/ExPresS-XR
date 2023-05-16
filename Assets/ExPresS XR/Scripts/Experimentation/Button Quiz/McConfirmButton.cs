using UnityEngine;

namespace ExPresSXR.Experimentation
{
    public class McConfirmButton : QuizButton
    {
        // ExPresSXR.Experimentation.McConfirmButton, Assembly-CSharp

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
                OnAnsweredCorrect.Invoke();
            }
            else
            {
                OnAnsweredIncorrect.Invoke();
            }
        }
    }
}
