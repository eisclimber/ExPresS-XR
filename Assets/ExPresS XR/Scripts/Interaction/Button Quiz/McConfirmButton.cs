using UnityEngine;

namespace ExPresSXR.Interaction.ButtonQuiz
{
    // ExPresSXR.Interaction.ButtonQuiz.McConfirmButton, Assembly-CSharp
    public class McConfirmButton : QuizButton
    {
        /// <summary>
        /// An arrays of references to `QuizButtons` that are used to determine if a multiple choice answer was given correctly.
        /// </summary>
        [SerializeField]
        private QuizButton[] _answerButtons;
        public QuizButton[] answerButtons
        {
            get => _answerButtons;
            set => _answerButtons = value;
        }

        /// <summary>
        /// Checks if the buttons from `answerButtons` are toggled correctly and gives visual feedback based on if the answer was correct or incorrect.
        /// </summary>
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
