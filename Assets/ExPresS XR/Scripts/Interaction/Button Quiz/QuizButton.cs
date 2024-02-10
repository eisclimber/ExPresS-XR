using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace ExPresSXR.Interaction.ButtonQuiz
{
    // ExPresSXR.Interaction.ButtonQuiz.QuizButton, Assembly-CSharp
    public class QuizButton : BaseButton
    {
        /// <summary>
        /// If the question currently displayed is correct.
        /// </summary>
        public bool correctChoice;

        /// <summary>
        /// If feedback should be given when pressing the button.
        /// </summary>
        public bool feedbackDisabled;

        /// <summary>
        /// If the feedback should be inverted (if feedback is given).
        /// </summary>
        public bool invertedFeedback;

        /// <summary>
        /// The string displayed as the answer.
        /// </summary>
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
                else if (!string.IsNullOrEmpty(_answerText))
                {
                    Debug.LogWarning("An AnswerText was provided for a QuizButton but it does not have a "
                                        + "`_feedbackTextLabel` configured. Please check your setup!", this);
                }
            }
        }

        /// <summary>
        /// The prefab that is attached to the `feedbackObjectSocket` as answer option.
        /// </summary>
        [SerializeField]
        private GameObject _answerObject;
        public GameObject answerObject
        {
            get => _answerObject;
            set
            {
                _answerObject = value;

                if (_feedbackObjectSocket != null)
                {
                    _feedbackObjectSocket.putBackPrefab = _answerObject;
                }
                else if (_answerObject != null)
                {
                    Debug.LogWarning("An AnswerObject was provided for a QuizButton but it does not have a "
                                        + "`_feedbackObjectSocket` configured. Please check your setup!", this);
                }
            }
        }

        /// <summary>
        /// Socket to hold an answer object prefab.
        /// Adding a `XRGrabInteractable`-Component to the `_answerObject` will make it interactable.
        /// This will instantiate a new GameObject, so prefabs are recommended as normal GameObjects will be duplicated. 
        /// </summary>
        [SerializeField]
        private PutBackSocketInteractor _feedbackObjectSocket;

        /// <summary>
        /// Reference to a `Text`-GameObject that is used to display the answer texts.
        /// </summary>
        [SerializeField]
        private TMP_Text _feedbackTextLabel;


        /// <summary>
        /// Used to not emit inputDisabled Events after an answer was given.
        /// </summary>
        private bool _overrideInputDisabledEvents;
        public bool overrideInputDisabledEvents
        {
            get => _overrideInputDisabledEvents;
            set => _overrideInputDisabledEvents = value;
        }

        // Sounds
        /// <summary>
        /// Sound played when the button pressed with a correct answer.
        /// </summary>
        [Tooltip("Sound played when the button pressed with a correct answer.")]
        public AudioClip answeredCorrectSound;

        /// <summary>
        /// Sound played when the button pressed with an incorrect answer.
        /// </summary>
        [Tooltip("Sound played when the button pressed with an incorrect answer.")]
        public AudioClip answeredIncorrectSound;


        /// <summary>
        /// Audio Source used to play `answeredCorrectSound` and `answeredCorrectSound`.
        /// This prevents interferences with pressed/released sounds when answering.
        /// </summary>
        [SerializeField]
        private AudioSource _answerFeedbackAudioPlayer;


        // Events
        /// <summary>
        /// Invoked via `NotifyChoice()` when the button was pressed (in non-ToggleMode) and the answer was correct.
        /// </summary>
        public UnityEvent OnAnsweredCorrect;

        /// <summary>
        /// Invoked via `NotifyChoice()` when the button was pressed (in non-ToggleMode) and the answer was not correct.
        /// </summary>
        public UnityEvent OnAnsweredIncorrect;


        ///////////
        private long triggerStartTime = -1;

        
        /// <summary>
        /// Resets the timer measuring the time until the button was pressed to give an answer.
        /// Will be automatically called when displaying a new answer (`DisplayAnswer()` is called).
        /// </summary>
        public void RestartTriggerTimer() => triggerStartTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();

        /// <summary>
        /// The time in milliseconds since the last reset of the trigger timer.
        /// </summary>
        /// <returns>The time since displaying the answer.</returns>
        public float GetTriggerTimerValue() => System.DateTimeOffset.Now.ToUnixTimeMilliseconds() - triggerStartTime;


        /// <summary>
        /// Connects additional events.
        /// </summary>
        protected override void Start()
        {
            base.Start();

            if (answerText != null && answerText != "")
            {
                answerText = _answerText;
            }

            if (answerObject != null)
            {
                answerObject = _answerObject;
            }

            if (_answerFeedbackAudioPlayer == null)
            {
                Debug.Log("No Answer Feedback Audio Player specified. No extra sound will be played when answering questions.");
            }

            OnPressed.AddListener(NotifyChoice);
            OnAnsweredCorrect.AddListener(PlayAnsweredCorrectSound);
            OnAnsweredIncorrect.AddListener(PlayAnsweredIncorrectSound);

            triggerStartTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }


        /// <summary>
        /// Displays an answer using the provided information and removes the old one.
        ///  If no information is provided will disable the button. This may only happen 
        /// during differing-answers-multiple-choice-quizzes.
        /// </summary>
        /// <param name="answerText">Text to be displayed.</param>
        /// <param name="answerObject">Prefab to be displayed.</param>
        /// <param name="correctChoice">Wether or not the button is correct.</param>
        public void DisplayAnswer(string answerText, GameObject answerObject, bool correctChoice)
        {
            ClearAnswer();

            if (string.IsNullOrEmpty(answerText) && answerObject == null && !correctChoice)
            {
                // May occur only during differing-answers-multiple-choice-quizzes
                // Disable Button as it is not used or part of the answer (=> answerCorrect)
                inputDisabled = true;
            }

            this.answerText = answerText;
            this.answerObject = answerObject;
            this.correctChoice = correctChoice;

            RestartTriggerTimer();
        }

        /// <summary>
        /// Removes any information shown on this button.
        /// </summary>
        public void ClearAnswer()
        {
            answerText = "";
            correctChoice = false;
            if (answerObject != null)
            {
                answerObject = null;
            }

            if (_answerFeedbackAudioPlayer != null)
            {
                _answerFeedbackAudioPlayer.Stop();
            }

            ResetButtonPress();
        }

        /// <summary>
        /// Emits events based on the feedback type to notify the user if the answer was correct.
        /// </summary>
        protected virtual void NotifyChoice()
        {
            if (!feedbackDisabled && !toggleMode)
            {
                // (not invertedFeedback and correct) or (inverted and not correct)
                if (correctChoice != invertedFeedback)
                {
                    OnAnsweredCorrect.Invoke();
                }
                else
                {
                    OnAnsweredIncorrect.Invoke();
                }
            }
        }

        /// <summary>
        /// Used to find out if the button was toggled correct for a multiple choice quiz.
        /// </summary>
        /// <returns>If the button had the correct pressed-state.</returns>
        public bool GiveMultipleChoiceFeedback()
        {
            // Buttons must be in toggle mode for MC
            if (!toggleMode)
            {
                return false;
            }

            bool correctlyToggled = pressed == correctChoice;

            return correctlyToggled;
        }

        /// <summary>
        /// Plays the `answeredCorrectSound`, if assigned.
        /// </summary>
        public void PlayAnsweredCorrectSound() => PlaySound(answeredCorrectSound, _answerFeedbackAudioPlayer);

        /// <summary>
        /// Plays the `answeredIncorrectSound`, if assigned.
        /// </summary>
        public void PlayAnsweredIncorrectSound() => PlaySound(answeredIncorrectSound, _answerFeedbackAudioPlayer);

        /// <summary>
        /// Calls the base function only if not disabled via `_overrideInputDisabledEvents`.
        /// </summary>
        public override void InternalEmitInputDisabledEvents()
        {
            if (!_overrideInputDisabledEvents)
            {
                base.InternalEmitInputDisabledEvents();
            }
        }
    }
}