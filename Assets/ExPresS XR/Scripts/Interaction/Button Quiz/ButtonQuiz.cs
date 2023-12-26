using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using TMPro;
using ExPresSXR.Misc;
using ExPresSXR.Experimentation.DataGathering;
using System;


namespace ExPresSXR.Interaction.ButtonQuiz
{
    public class ButtonQuiz : MonoBehaviour
    {
        public const int MIN_QUESTIONS = 1;
        public const int NUM_ANSWERS = 4;
        public const float DISPLAY_OBJECTS_SPACING = 0.5f;
        public const float DEFAULT_FEEDBACK_DURATION = 3.0f;

        public const string DEFAULT_QUIZ_COMPLETED_TEXT = "Quiz Completed";

        public const string FULL_QUIZ_CSV_HEADER = "quizUndergoing" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                + "quizPlaythroughNumber" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                + "currentQuestionNumber" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                + "currentQuestionIdx" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                + "answerPressTime" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                + "answerPermutation" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                + "chosenAnswerString" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                + "displayedFeedbackText" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                + "displayedFeedbackObjects" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                + "displayedFeedbackVideo" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                + ButtonQuizQuestion.QUESTION_CSV_HEADER_STRING + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING
                                                + ButtonQuizConfig.CONFIG_CSV_HEADER_STRING;


        [SerializeField]
        private ButtonQuizConfig _config;
        public ButtonQuizConfig config
        {
            get => _config;
            set
            {
                _config = value;

                if (_config != null)
                {
                    _questions = _config.questions;
                    _numQuestions = _config.questions.Length;
                }
            }
        }

        [SerializeField]
        private bool _quizUndergoing;
        public bool quizUndergoing
        {
            get => _quizUndergoing;
            private set
            {
                _quizUndergoing = value;
#if UNITY_EDITOR
                // Force the editor to update and display the correct buttons
                EditorUtility.SetDirty(this);
#endif
            }
        }


        [SerializeField]
        private float _feedbackDuration = DEFAULT_FEEDBACK_DURATION;

        private ButtonQuizQuestion[] _questions;

        private int _numQuestions;

        private int[] _questionPermutation;

        public  int currentQuestionIdx { get; private set; }

        public ButtonQuizQuestion currentQuestion { get; private set; }


        [SerializeField]
        private bool _startOnAwake = true;


        [SerializeField]
        private bool _showQuizCompletedText = true;

        [SerializeField]
        private bool _canRestartFromAfterQuizDialog = true;



        public QuizButton[] buttons = new QuizButton[NUM_ANSWERS];

        public McConfirmButton mcConfirmButton;


        public TMP_Text displayText; // TMPro.TMP_Text, Unity.TextMeshPro

        public GameObject displayAnchor;

        public VideoPlayer displayPlayer;

        public RawImage displayVideoImage;

        public Canvas afterQuizMenu;


        // Feedback
        private string _displayedFeedbackText;

        public GameObject[] _displayedFeedbackObjects;

        public VideoClip _displayedFeedbackVideo;



        // Events
        public UnityEvent OnQuizStarted;
        public UnityEvent OnAnswerGiven;
        public UnityEvent OnQuizCompleted;


        // Export (Preserve their type)
        public long quizStartTime { get; private set; }

        public int quizPlaythroughNumber { get; private set; }


        public bool latestAnswerCorrect { get; private set; }

        public bool[] latestAnswerChosen { get; private set; }

        public string latestAnswerChosenText { get => CsvUtility.ArrayToString(latestAnswerChosen ?? new bool[0]); }

        public int[] latestAnswerPermutation { get; private set; }

        public string latestAnswerPermutationText {get => CsvUtility.ArrayToString(latestAnswerPermutation ?? new int[0]); }

        public int latestAskOrderIdx { get; private set; }
        
        public int latestQuestionIdx { get; private set; }

        public float latestAnswerPressTime { get; private set; }


        // Coroutines
        private Coroutine _feedbackWaitCoroutine = null;


        private void Awake()
        {
            if (!SetupValidator.IsSetupValid(_config, this))
            {
                // Warn about invalid quiz directly (even when not starting)
                Debug.LogWarning("Quiz Config not set or is invalid.");
            }

            if (_startOnAwake)
            {
                StartQuiz();
            }
            else
            {
                SetButtonsDisabled(true);
                ClearVideoDisplay();
            }

            // Always disable AfterQuizMenu
            if (afterQuizMenu != null)
            {
                afterQuizMenu.enabled = false;
            }
        }



        // Setup
        public bool Setup(ButtonQuizConfig config, QuizButton[] buttons, McConfirmButton mcConfirmButton,
                                TMP_Text displayText, GameObject displayAnchor, VideoPlayer displayPlayer,
                                RawImage displayVideoImage, Canvas afterQuizMenu)
        {
            // Set values
            this.config = config;

            this.buttons = buttons;
            this.mcConfirmButton = mcConfirmButton;
            this.displayText = displayText;
            this.displayAnchor = displayAnchor;
            this.displayPlayer = displayPlayer;
            this.displayVideoImage = displayVideoImage;
            this.afterQuizMenu = afterQuizMenu;

            _questions = _config.questions;
            _numQuestions = _config.questions.Length;

            // Create Question Permutation
            _questionPermutation = QuizUtility.GenerateIdentityArray(_numQuestions);
            if (config.questionOrdering == QuestionOrdering.Randomize)
            {
                _questionPermutation = QuizUtility.Shuffle(_questionPermutation);
            }
            currentQuestionIdx = -1; // Quiz not started yet, Will be incremented to >= 0 during DisplayNextQuestion

            // Connect Events
            bool isMultipleChoice = config.quizMode == QuizMode.MultipleChoice;
            bool invertedFeedback = config.feedbackMode == FeedbackMode.AlwaysWrong;
            bool feedbackDisabled = config.feedbackMode == FeedbackMode.None || config.feedbackMode == FeedbackMode.Random;

            if (mcConfirmButton != null && isMultipleChoice)
            {
                mcConfirmButton.toggleMode = false;
                mcConfirmButton.feedbackDisabled = feedbackDisabled;
                mcConfirmButton.invertedFeedback = invertedFeedback;
                mcConfirmButton.answerButtons = buttons;
                // Remove ShowFeedback Callback (if exists)
                mcConfirmButton.OnPressed.RemoveListener(ShowFeedback);
                // Add ShowFeedback callback
                mcConfirmButton.OnPressed.AddListener(ShowFeedback);
            }

            foreach (QuizButton button in buttons)
            {
                if (button != null)
                {
                    button.toggleMode = isMultipleChoice;
                    button.feedbackDisabled = feedbackDisabled;
                    button.invertedFeedback = invertedFeedback;

                    if (!isMultipleChoice)
                    {
                        // Remove ShowFeedback Callback (if exists)
                        button.OnPressed.RemoveListener(ShowFeedback);
                        // Add ShowFeedback callback
                        button.OnPressed.AddListener(ShowFeedback);
                    }
                }
            }

            if (afterQuizMenu != null)
            {
                afterQuizMenu.enabled = false;
            }

            return SetupValidator.IsSetupValid(config, this);
        }

        public void StartQuiz()
        {
            if (!SetupValidator.IsSetupValid(_config, this))
            {
                Debug.LogError("Cannot start Quiz. Quiz Config not set or invalid.");
            }
            else
            {
                quizUndergoing = true;
                quizStartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                quizPlaythroughNumber++;
                Setup(_config, buttons, mcConfirmButton, displayText, displayAnchor, displayPlayer, displayVideoImage, afterQuizMenu);
                DisplayNextQuestion();
                OnQuizStarted.Invoke();
            }
        }

        public void StopQuiz()
        {
            OnQuizCompleted.Invoke();

            ClearAnswers();

            SetButtonsDisabled(true);

            quizUndergoing = false;

            ShowAfterQuizMenu();
        }


        // Runtime logic
        private void DisplayNextQuestion()
        {
            if (currentQuestionIdx > _numQuestions)
            {
                StopQuiz();
            }
            else
            {
                currentQuestionIdx++;
                currentQuestion = _questions[_questionPermutation[currentQuestionIdx]];

                SetButtonsDisabled(false);

                latestAnswerPermutation = QuizUtility.GetAnswerPermutation(config, currentQuestion);

                for (int i = 0; i < latestAnswerPermutation.Length; i++)
                {
                    int answerIdx = latestAnswerPermutation[i];

                    if (buttons[i] != null)
                    {
                        bool answerTextPossible = answerIdx >= 0 && answerIdx < currentQuestion.answerTexts.Length;
                        bool answerObjectPossible = answerIdx >= 0 && answerIdx < currentQuestion.answerObjects.Length;
                        bool answerCorrectPossible = answerIdx >= 0 && answerIdx < currentQuestion.correctAnswers.Length;

                        string answerText = answerTextPossible ? currentQuestion.answerTexts[answerIdx] : "";
                        GameObject answerGo = answerObjectPossible ? currentQuestion.answerObjects[answerIdx] : null;
                        bool answerCorrect = answerCorrectPossible && currentQuestion.correctAnswers[answerIdx];

                        // Always display (also empty) answers on the button
                        buttons[i].DisplayAnswer(answerText, answerGo, answerCorrect);
                    }
                }

                bool showTextQuestion = currentQuestion.questionText != null;
                bool showObjectQuestion = currentQuestion.questionObject != null;
                bool showVideoQuestion = currentQuestion.questionVideo != null;
                bool showStreamedVideoQuestion = !showVideoQuestion && !string.IsNullOrEmpty(currentQuestion.questionVideoUrl);

                SetQuizDisplayEnabled(showTextQuestion, showObjectQuestion, showVideoQuestion || showStreamedVideoQuestion);

                if (displayText != null && showTextQuestion)
                {
                    displayText.text = currentQuestion.questionText;
                }

                if (displayAnchor != null && showObjectQuestion)
                {
                    Instantiate(currentQuestion.questionObject, displayAnchor.transform);
                }

                if (displayPlayer != null)
                {
                    if (showVideoQuestion)
                    {
                        displayPlayer.source = VideoSource.VideoClip;
                        displayPlayer.clip = currentQuestion.questionVideo;
                        displayPlayer.Play();
                    }
                    else if (showStreamedVideoQuestion)
                    {
                        displayPlayer.source = VideoSource.Url;
                        displayPlayer.url = QuizUtility.MakeStreamingAssetsVideoPath(currentQuestion.questionVideoUrl);
                        displayPlayer.Play();
                    }
                }
            }
        }

        private void ShowFeedback()
        {
            UpdateAnswerExportValues();

            OnAnswerGiven.Invoke();

            // If no feedback was given the feedback is already completed
            if (config.feedbackMode == FeedbackMode.None)
            {
                OnFeedbackCompleted();
                return;
            }

            SetButtonsDisabled(true, true);

            bool showFeedback = _config.feedbackMode != FeedbackMode.None;
            bool showIfAvailable = _config.feedbackType == FeedbackType.DifferingTypes;
            bool showAnswerType = config.feedbackType == FeedbackType.ShowAnswers;
            bool showAnyAnswerType = showAnswerType && (config.answerType == AnswerType.DifferingTypes);

            bool showTextFeedback = displayText != null
                                        && showFeedback
                                        && (_config.feedbackType == FeedbackType.Text
                                            || (showAnswerType && config.answerType == AnswerType.Text)
                                            || (showIfAvailable && currentQuestion.feedbackText != null)
                                            || showAnyAnswerType
                                            || _config.feedbackPrefixEnabled);
            bool showObjectFeedback = displayAnchor != null
                                        && showFeedback
                                        && (_config.feedbackType == FeedbackType.Object
                                            || (showAnswerType && config.answerType == AnswerType.Object)
                                            || (showIfAvailable && currentQuestion.feedbackObject != null)
                                            || showAnyAnswerType);
            bool showVideoFeedback = displayPlayer != null
                                        && showFeedback
                                        && (showIfAvailable || _config.feedbackType == FeedbackType.Video)
                                        && currentQuestion.feedbackVideo != null;
            bool showVideoFeedbackUrl = !showVideoFeedback
                                        && displayPlayer != null
                                        && showFeedback
                                        && (showIfAvailable || _config.feedbackType == FeedbackType.Video)
                                        && !string.IsNullOrEmpty(currentQuestion.feedbackVideoUrl);

            SetQuizDisplayEnabled(showTextFeedback, showObjectFeedback, showVideoFeedback || showVideoFeedbackUrl);

            if (showTextFeedback)
            {
                displayText.text = config.usedFeedbackPrefix + _displayedFeedbackText;
            }

            if (showObjectFeedback)
            {
                float xOffset = DISPLAY_OBJECTS_SPACING * (_displayedFeedbackObjects.Length - 1) / 2.0f;

                foreach (Transform child in displayAnchor.transform)
                {
                    Destroy(child.gameObject);
                }

                for (int i = 0; i < _displayedFeedbackObjects.Length; i++)
                {
                    if (_displayedFeedbackObjects[i] != null)
                    {
                        GameObject go = Instantiate(_displayedFeedbackObjects[i], displayAnchor.transform);
                        go.transform.localPosition = new Vector3((DISPLAY_OBJECTS_SPACING * i) - xOffset, 0, 0);
                    }
                }
            }

            if (showVideoFeedback)
            {
                displayPlayer.source = VideoSource.VideoClip;
                displayPlayer.clip = currentQuestion.feedbackVideo;
                displayPlayer.Play();
                displayPlayer.prepareCompleted += OnVideoPlayerPrepareComplete;
                displayPlayer.loopPointReached += OnFeedbackVideoCompleted;
            }
            else if (showVideoFeedbackUrl)
            {
                displayPlayer.source = VideoSource.Url;
                displayPlayer.url = QuizUtility.MakeStreamingAssetsVideoPath(currentQuestion.feedbackVideoUrl);
                displayPlayer.Play();
                displayPlayer.prepareCompleted += OnVideoPlayerPrepareComplete;
                displayPlayer.loopPointReached += OnFeedbackVideoCompleted;
            }

            // Only wait for completion if no video was provided
            _feedbackWaitCoroutine = StartCoroutine(WaitForFeedbackCompletion());
        }

        private void OnFeedbackCompleted()
        {
            if (displayPlayer != null)
            {
                displayPlayer.loopPointReached -= OnFeedbackVideoCompleted;
            }

            if (displayAnchor != null)
            {
                foreach (Transform child in displayAnchor.transform)
                {
                    Destroy(child.gameObject);
                }
            }

            if (displayText != null)
            {
                displayText.text = "";
            }

            DisplayNextQuestion();
        }


        private void SetButtonsDisabled(bool disabled, bool overrideEvents = false)
        {
            foreach (QuizButton button in buttons)
            {
                if (button != null)
                {
                    button.overrideInputDisabledEvents = overrideEvents;
                    button.inputDisabled = disabled;
                }
            }

            if (mcConfirmButton != null)
            {
                mcConfirmButton.inputDisabled = disabled;
            }
        }

        private void ClearAnswers()
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i] != null)
                {
                    buttons[i].ClearAnswer();
                }
            }
        }


        private void ClearVideoDisplay()
        {
            if (displayPlayer != null && displayPlayer.targetTexture != null)
            {
                displayPlayer.targetTexture.Release();
            }
        }


        // Validation
        private void SetQuizDisplayEnabled(bool enableDisplayText, bool enableDisplayAnchor, bool enabledVideoPlayer)
        {
            if (displayText != null)
            {
                displayText.gameObject.SetActive(enableDisplayText);
            }

            if (displayAnchor != null)
            {
                displayAnchor.SetActive(enableDisplayAnchor);
            }

            if (displayPlayer != null)
            {
                displayPlayer.gameObject.SetActive(enabledVideoPlayer);
            }

            if (displayVideoImage != null)
            {
                displayVideoImage.gameObject.SetActive(enabledVideoPlayer);
            }
        }

        private void ShowAfterQuizMenu()
        {
            if (displayText != null && _showQuizCompletedText)
            {
                displayText.text = DEFAULT_QUIZ_COMPLETED_TEXT;
            }

            if (afterQuizMenu != null)
            {
                afterQuizMenu.enabled = true;

                Transform restartTransform = RuntimeUtils.RecursiveFindChild(afterQuizMenu.transform, "Restart Button");
                Transform closeTransform = RuntimeUtils.RecursiveFindChild(afterQuizMenu.transform, "Close Button");

                Button restartButton = restartTransform != null ? restartTransform.GetComponent<Button>() : null;
                Button closeButton = closeTransform != null ? closeTransform.GetComponent<Button>() : null;

                if (restartButton != null)
                {
                    restartTransform.gameObject.SetActive(_canRestartFromAfterQuizDialog);
                    restartButton.onClick.RemoveListener(StartQuiz);
                    restartButton.onClick.AddListener(StartQuiz);
                }

                if (closeButton != null)
                {
                    closeButton.onClick.RemoveListener(CloseAfterQuizMenu);
                    closeButton.onClick.AddListener(CloseAfterQuizMenu);
                }
            }
        }

        // Update After Event Export Values
        private void UpdateAnswerExportValues()
        {
            bool isMC = config.quizMode == QuizMode.MultipleChoice;
            List<int> pressedButtonIdxs = new();
            List<QuizButton> pressedButtons = new();
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i] != null && buttons[i].pressed)
                {
                    pressedButtonIdxs.Add(i);
                    pressedButtons.Add(buttons[i]);
                }
            }

            if (isMC)
            {
                // latestChosenAnswersIdxs = CsvUtility.ArrayToString(pressedButtonIdxs.ToArray());
                // latestAnswerPressTime = mcConfirmButton.GetTriggerTimerValue();
            }
            else
            {
                // latestChosenAnswersIdxs = pressedButtonIdxs.Count > 0 ? pressedButtonIdxs[0].ToString() : "-1";
                // latestAnswerPressTime = pressedButtons.Count > 0 ? pressedButtons[0].GetTriggerTimerValue() : -1.0f;
            }

            // displayedFeedbackText = currentQuestion?.GetFeedbackText(config) ?? "";
            // displayedFeedbackObjects = currentQuestion.GetFeedbackGameObjects(config) ?? new GameObject[0];
            // displayedFeedbackVideo = currentQuestion.GetFeedbackVideo(config);
        }

        // Coroutines & Actions
        private IEnumerator WaitForFeedbackCompletion()
        {
            yield return new WaitForSeconds(_feedbackDuration);
            OnFeedbackCompleted();
            _feedbackWaitCoroutine = null;
        }

        private void OnVideoPlayerPrepareComplete(VideoPlayer player)
        {
            // Stop feedback coroutine if it is to short and wait for the video completion instead
            float clipLength = player.frameCount / player.frameRate;
            if (clipLength >= _feedbackDuration && _feedbackWaitCoroutine != null)
            {
                StopCoroutine(_feedbackWaitCoroutine);
                _feedbackWaitCoroutine = null;
            }
            player.prepareCompleted -= OnVideoPlayerPrepareComplete;
        }

        private void OnFeedbackVideoCompleted(VideoPlayer evt) => OnFeedbackCompleted();

        private void CloseAfterQuizMenu()
        {
            if (afterQuizMenu != null)
            {
                afterQuizMenu.enabled = false;
            }
        }


        ///////////////////////////////////////////////
        //               Export Values               //
        ///////////////////////////////////////////////

        // Continuous Polling

        public int GetQuizPlaythroughNumber() => quizPlaythroughNumber;

        public float GetQuizUnixStartTime() => quizUndergoing ? quizStartTime : -1.0f;

        public float GetCurrentQuizUnixTimeMillisecondsDuration() => quizUndergoing ? (quizStartTime - DateTimeOffset.Now.ToUnixTimeMilliseconds()) : -1.0f;

        public int GetCurrentQuestionIdx() => currentQuestion?.itemIdx ?? -1;

        public string GetCurrentQuestionCsvExportValue() => currentQuestion?.GetCsvExportValues() ?? ButtonQuizQuestion.emptyCsvExportValues;

        public int GetCurrentQuestionNumber() => quizUndergoing ? currentQuestionIdx : -1;


        // After Events

        /// <summary>
        /// Returns a string representing the of the latest chosen answer.
        /// </summary>
        /// <returns>string representation of the answer array</returns>
        public string GetLatestAnswerChosen() => CsvUtility.ArrayToString(latestAnswerChosen);


        // public string GetDisplayedFeedbackText() => displayedFeedbackText;

        // public string GetDisplayedFeedbackObjects()
        // {
        //     List<string> _feedbackObjectNames = new();
        //     if (displayedFeedbackObjects != null)
        //     {
        //         foreach (GameObject go in displayedFeedbackObjects)
        //         {
        //             if (go != null)
        //             {
        //                 _feedbackObjectNames.Add(go.name);
        //             }
        //         }
        //     }
        //     return CsvUtility.ArrayToString(_feedbackObjectNames.ToArray());
        // }

        // public string GetDisplayedFeedbackVideo() => displayedFeedbackVideo != null ? displayedFeedbackVideo.name : "";

        // Misc
        public string GetQuestionPermutationAsCsvString() => CsvUtility.ArrayToString(_questionPermutation);

        // Use EXPORT_CSV_COLUMN_STRING for header
        public string GetConfigCsvExportValues() => config?.GetCsvExportValues() ?? ButtonQuizConfig.emptyCsvExportValues;

        // Use EXPORT_CSV_COLUMN_STRING for header
        public string GetAllQuestionsCsvExportValues() => config?.GetAllQuestionsCsvExportValues() ?? "";
        

        public string GetFullQuizCsvValues() => CsvUtility.JoinAsCsv(
            new object[] {
                quizUndergoing,
                quizPlaythroughNumber,

                latestAnswerChosenText,
                latestAnswerCorrect,
                latestAnswerPressTime,
                latestAskOrderIdx,
                latestQuestionIdx,
                latestAnswerPermutation,

                // latestAnswersPermutation,
                // latestChosenAnswers,
                // displayedFeedbackText,
                // displayedFeedbackObjects,
                // displayedFeedbackVideo,
                // GetCurrentQuestionCsvExportValue(),
                // GetConfigCsvExportValues()
            });
    }
}

/*
// Timing
unix_time
unity_time
delta_time



// General
isQuizUndergoing -> If the quiz is currently undergoing
quizPlaythroughNumber -> How many times the quiz has been played (since the last app start)

// Latest Question
answerWasCorrect -> If the answer was completely correct
answerChosen -> Which button(s) were pressed as array
answerPressTime -> Press time for submitting an answer (choosing one button or pressing the mcConfirm)
askOrderIdx -> Order of the question during quiz
questionIdx -> Index of question in the config quiz
answerPermutation -> Permutation in which the answer options were presented (can be used with MC to determine which answer was incorrectly chosen)

// Current Question Config
questionIdx
questionVideo
questionObject
questionText
answerObject0
answerObject1
answerObject2
answerObject3
answerText0
answerText1
answerText2
answerText3
correctAnswers0
correctAnswers1
correctAnswers2
correctAnswers3
feedbackVideo
feedbackObject
feedbackText
// Config
quizMode
questionOrdering
answersAmount
answersOrdering
questionType
answerType
feedbackMode
feedbackType
*/