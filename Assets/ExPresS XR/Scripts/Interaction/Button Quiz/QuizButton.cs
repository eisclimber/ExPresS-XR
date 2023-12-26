using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ExPresSXR.Interaction.ButtonQuiz
{
    // ExPresSXR.Interaction.ButtonQuiz.QuizButton, Assembly-CSharp

    public class QuizButton : BaseButton
    {
        public bool correctChoice;
        public bool feedbackDisabled;
        public bool invertedFeedback;

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

                if (_feedbackObjectSocket != null)
                {
                    _feedbackObjectSocket.putBackPrefab = _answerObject;
                }
            }
        }

        [SerializeField]
        private Text _feedbackTextLabel;


        [SerializeField]
        private PutBackSocketInteractor _feedbackObjectSocket;

        // Used to not emit inputDisabled Events after an answer was given
        private bool _overrideInputDisabledEvents;
        public bool overrideInputDisabledEvents
        {
            get => _overrideInputDisabledEvents;
            set => _overrideInputDisabledEvents = value;
        }

        // Sounds
        [Tooltip("Sound played when the button pressed with a correct answer.")]
        public AudioClip answeredCorrectSound;

        [Tooltip("Sound played when the button pressed with an incorrect answer.")]
        public AudioClip answeredIncorrectSound;


        [SerializeField]
        private AudioSource _answerFeedbackAudioPlayer;


        // Events
        public UnityEvent OnAnsweredCorrect;
        public UnityEvent OnAnsweredIncorrect;


        ///////////
        private long triggerStartTime = -1;

        // Can be used to measure the time since between any point in time and a button press
        // Will be automatically started when input is (re-)enabled
        public void RestartTriggerTimer()
        {
            triggerStartTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        public float GetTriggerTimerValue()
        {
            return System.DateTimeOffset.Now.ToUnixTimeMilliseconds() - triggerStartTime;
        }


        ////// 

        protected override void Awake()
        {
            base.Awake();

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

        public void DisplayAnswer(string answerText, GameObject answerObject, bool correctChoice)
        {
            ClearAnswer();

            if (answerText == "" && answerObject == null && !correctChoice)
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

        // Used to get feedback and 
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


        public void PlayAnsweredCorrectSound() => PlaySound(answeredCorrectSound, _answerFeedbackAudioPlayer);

        public void PlayAnsweredIncorrectSound() => PlaySound(answeredIncorrectSound, _answerFeedbackAudioPlayer);



        public override void InternalEmitInputDisabledEvents()
        {
            if (!_overrideInputDisabledEvents)
            {
                base.InternalEmitInputDisabledEvents();
            }
        }
    }
}