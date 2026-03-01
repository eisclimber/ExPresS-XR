using UnityEngine;
using UnityEngine.Events;
using TMPro;
using ExPresSXR.Interaction.Interactors;
using ExPresSXR.Interaction.ValueRangeInteractable;


namespace ExPresSXR.Interaction.ButtonQuiz
{
    /// <summary>
    /// An expansion of `BaseButton` representing the Button that is used when answering a `TutorialButtonQuiz`.
    /// 
    /// It is able to display answer options in the form of text and GameObject that will hover above the press anchor.
    /// In order to display the GameObjects it is advised to create a Prefab.
    /// Also move them a bit up on the y-axis and if they should be made interactable add a `XRGrabInteractable` to it.
    /// 
    /// Most logic is restricted to non-Toggle-Mode as Toggle-Mode is used when the quiz is in MultipleChoice-Mode. If in MultipleChoice-Mode the button will be automatically set to Toggle-Mode and the feedback is handled via an extra (See `McConfirmButton`).
    /// 
    /// When pressed the events `OnPressedCorrect` and `OnPressedIncorrect` are invoked to notify if the button was pressed correctly or not.
    /// </summary>

    // ExPresSXR.Interaction.ButtonQuiz.QuizButton, Assembly-CSharp
    public class QuizButton : Button
    {
        /// <summary>
        /// If the question currently displayed is correct.
        /// </summary>
        [SerializeField]
        [Tooltip("If the question currently displayed is correct.")]
        private bool _correctChoice;
        public bool CorrectChoice
        {
            get => _correctChoice;
            set => _correctChoice = value;
        }

        /// <summary>
        /// If feedback should be given when pressing the button.
        /// </summary>
        [SerializeField]
        [Tooltip("If feedback should be given when pressing the button.")]
        public bool _feedbackDisabled;
        public bool FeedbackDisabled
        {
            get => _feedbackDisabled;
            set => _feedbackDisabled = value;
        }

        /// <summary>
        /// If the feedback should be inverted (if feedback is given).
        /// </summary>
        [SerializeField]
        [Tooltip("If the feedback should be inverted (if feedback is given).")]
        public bool _invertedFeedback;
        public bool InvertedFeedback
        {
            get => _invertedFeedback;
            set => _invertedFeedback = value;
        }

        /// <summary>
        /// The string displayed as the answer.
        /// </summary>
        [SerializeField]
        [Tooltip("The string displayed as the answer.")]
        private string _answerText;
        public string AnswerText
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
                                        + "`_feedbackTextLabel` configured. Please check your setup!");
                }
            }
        }

        /// <summary>
        /// The prefab that is attached to the `feedbackObjectSocket` as answer option.
        /// </summary>
        [SerializeField]
        [Tooltip("The prefab that is attached to the `feedbackObjectSocket` as answer option.")]
        private GameObject _answerObject;
        public GameObject AnswerObject
        {
            get => _answerObject;
            set
            {
                _answerObject = value;

                if (_feedbackObjectSocket != null)
                {
                    // The buttons should allow holding non-interactables so we need to be able to destroy non-selecting (i.e. no interactables)
                    _feedbackObjectSocket.DestroyIfNotSelecting = true;
                    _feedbackObjectSocket.AllowNonInteractables = true;
                    _feedbackObjectSocket.PutBackPrefab = _answerObject;
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
        [Tooltip("Socket to hold an answer object prefab.\nAdding a `XRGrabInteractable`-Component to the `_answerObject` will make it interactable."
                + "\nThis will instantiate a new GameObject, so prefabs are recommended as normal GameObjects will be duplicated.")]
        private PutBackSocketInteractor _feedbackObjectSocket;

        /// <summary>
        /// Reference to a `Text`-GameObject that is used to display the answer texts.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to a `Text`-GameObject that is used to display the answer texts.")]
        private TMP_Text _feedbackTextLabel;


        /// <summary>
        /// Used to not emit inputDisabled Events after an answer was given.
        /// </summary>
        [SerializeField]
        [Tooltip("Used to not emit inputDisabled Events after an answer was given.")]
        private bool _overrideInputDisabledEvents;
        public bool OverrideInputDisabledEvents
        {
            get => _overrideInputDisabledEvents;
            set => _overrideInputDisabledEvents = value;
        }

        // Sounds
        /// <summary>
        /// Sound played when the button pressed with a correct answer.
        /// </summary>
        [SerializeField]
        [Tooltip("Sound played when the button pressed with a correct answer.")]
        protected AudioClip _answeredCorrectSound;
        public AudioClip AnsweredCorrectSound
        {
            get => _answeredCorrectSound;
            set => _answeredCorrectSound = value;
        }

        /// <summary>
        /// Sound played when the button pressed with an incorrect answer.
        /// </summary>
        [SerializeField]
        [Tooltip("Sound played when the button pressed with an incorrect answer.")]
        protected AudioClip _answeredIncorrectSound;
        public AudioClip AnsweredIncorrectSound
        {
            get => _answeredIncorrectSound;
            set => _answeredIncorrectSound = value;
        }


        /// <summary>
        /// Audio Source used to play `answeredCorrectSound` and `answeredCorrectSound`.
        /// This prevents interferences with pressed/released sounds when answering.
        /// </summary>
        [SerializeField]
        [Tooltip("Audio Source used to play `answeredCorrectSound` and `answeredCorrectSound`.\nThis prevents interferences with pressed/released sounds when answering.")]
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

            if (AnswerText != null && AnswerText != "")
            {
                AnswerText = _answerText;
            }

            if (AnswerObject != null)
            {
                AnswerObject = _answerObject;
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
                InputDisabled = true;
            }

            AnswerText = answerText;
            AnswerObject = answerObject;
            CorrectChoice = correctChoice;

            RestartTriggerTimer();
        }

        /// <summary>
        /// Removes any information shown on this button.
        /// </summary>
        public void ClearAnswer()
        {
            AnswerText = "";
            CorrectChoice = false;
            if (AnswerObject != null)
            {
                AnswerObject = null;
            }

            if (_answerFeedbackAudioPlayer != null)
            {
                _answerFeedbackAudioPlayer.Stop();
            }

            ResetValue();
        }

        /// <summary>
        /// Emits events based on the feedback type to notify the user if the answer was correct.
        /// </summary>
        protected virtual void NotifyChoice()
        {
            if (!FeedbackDisabled && !ToggleMode)
            {
                // (not invertedFeedback and correct) or (inverted and not correct)
                (CorrectChoice != InvertedFeedback ? OnAnsweredCorrect : OnAnsweredIncorrect).Invoke();
            }
        }

        /// <summary>
        /// Used to find out if the button was toggled correct for a multiple choice quiz.
        /// </summary>
        /// <returns>If the button had the correct pressed-state.</returns>
        public bool GiveMultipleChoiceFeedback()
        {
            // Buttons must be in toggle mode for MC
            if (!ToggleMode)
            {
                return false;
            }

            bool correctlyToggled = Pressed == CorrectChoice;

            return correctlyToggled;
        }

        /// <summary>
        /// Plays the `answeredCorrectSound`, if assigned.
        /// </summary>
        public void PlayAnsweredCorrectSound() => PlaySound(AnsweredCorrectSound, _answerFeedbackAudioPlayer);

        /// <summary>
        /// Plays the `answeredIncorrectSound`, if assigned.
        /// </summary>
        public void PlayAnsweredIncorrectSound() => PlaySound(AnsweredIncorrectSound, _answerFeedbackAudioPlayer);
    }
}