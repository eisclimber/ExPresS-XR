using ExPresSXR.Interaction.ButtonQuiz;
using UnityEngine;

namespace ExPresSXR.Misc.ColorSwitching
{
    [RequireComponent(typeof(Animator))]
    public class QuizButtonColorSwitcher : ColorAnimatorSwitcher
    {
        [SerializeField]
        [Tooltip("The QuizButton to which this ColorAnimatorSwitcher is linked.")]
        private QuizButton _quizButton;

        protected virtual void OnEnable()
        {
            RegisterButtonEvents();
        }

        protected virtual void OnDisable()
        {
            UnregisterButtonEvents();
        }

        private void RegisterButtonEvents()
        {
            if (_quizButton != null)
            {
                _quizButton.OnAnsweredCorrect.AddListener(HandleButtonAnsweredCorrect);
                _quizButton.OnAnsweredIncorrect.AddListener(HandleButtonAnsweredIncorrect);

                _quizButton.OnPressed.AddListener(HandleButtonPressed);
                _quizButton.OnReleased.AddListener(HandleButtonReleased);

                _quizButton.OnTogglePressed.AddListener(HandleButtonPressed);
                _quizButton.OnToggleReleased.AddListener(HandleButtonReleased);

                _quizButton.OnInputEnabled.AddListener(HandleButtonEnabled);
                _quizButton.OnInputDisabled.AddListener(HandleButtonDisabled);

                _quizButton.OnButtonPressReset.AddListener(HandleButtonPressReset);
            }
        }

        private void UnregisterButtonEvents()
        {
            if (_quizButton != null)
            {
                _quizButton.OnAnsweredCorrect.RemoveListener(HandleButtonAnsweredCorrect);
                _quizButton.OnAnsweredIncorrect.RemoveListener(HandleButtonAnsweredIncorrect);

                _quizButton.OnPressed.RemoveListener(HandleButtonPressed);
                _quizButton.OnReleased.RemoveListener(HandleButtonReleased);

                _quizButton.OnTogglePressed.RemoveListener(HandleButtonPressed);
                _quizButton.OnToggleReleased.RemoveListener(HandleButtonReleased);

                _quizButton.OnInputEnabled.RemoveListener(HandleButtonEnabled);
                _quizButton.OnInputDisabled.RemoveListener(HandleButtonDisabled);

                _quizButton.OnButtonPressReset.RemoveListener(HandleButtonPressReset);
            }
        }

        private void HandleButtonAnsweredCorrect()
        {
            ChangeColorWithTrigger("TrPressedCorrect");
        }

        private void HandleButtonAnsweredIncorrect()
        {
            ChangeColorWithTrigger("TrPressedIncorrect");
        }

        private void HandleButtonPressed()
        {
            ChangeColorWithBool("IsPressed", true);
        }

        private void HandleButtonReleased()
        {
            ChangeColorWithBool("IsPressed", false);
        }

        private void HandleButtonEnabled()
        {
            ChangeColorWithBool("IsDisabled", false);
        }

        private void HandleButtonDisabled()
        {
            ChangeColorWithBool("IsDisabled", true);
        }

        private void HandleButtonPressReset()
        {
            // We need to update the pressed state, as it won't get set automatically
            ChangeColorWithBool("IsPressed", false);
        }
    }
}