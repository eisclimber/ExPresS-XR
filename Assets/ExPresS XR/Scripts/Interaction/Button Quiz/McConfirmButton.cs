using UnityEngine;

namespace ExPresSXR.Interaction.ButtonQuiz
{
    /// <summary>
    /// An expansion of `BaseButton` representing the Button that is used when confirming a answer `TutorialButtonQuiz` when in MultipleChoiceMode.
    /// </summary>
    // ExPresSXR.Interaction.ButtonQuiz.McConfirmButton, Assembly-CSharp
    public class McConfirmButton : QuizButton
    {
        [SerializeField]
        [Tooltip("An arrays of references to `QuizButtons` that are used to determine if a multiple choice answer was given correctly.")]
        private QuizButton[] _answerButtons;
        /// <summary>
        /// An arrays of references to `QuizButtons` that are used to determine if a multiple choice answer was given correctly.
        /// </summary>
        public QuizButton[] AnswerButtons
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

            if (allCorrect != InvertedFeedback)
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
