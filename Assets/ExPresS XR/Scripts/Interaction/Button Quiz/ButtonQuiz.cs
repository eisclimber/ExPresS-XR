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


namespace ExPresSXR.Interaction.ButtonQuiz
{
    public class ButtonQuiz : MonoBehaviour
    {
        public const int MIN_QUESTIONS = 1;
        public const int NUM_ANSWERS = 4;
        public const float DISPLAY_OBJECTS_SPACING = 0.5f;
        public const float DEFAULT_FEEDBACK_DURATION = 3.0f;

        public const string DEFAULT_QUIZ_COMPLETED_TEXT = "Quiz Completed";

        public const string FULL_QUIZ_CSV_HEADER = "isQuizActive" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING 
                                                + "currentQuestionNumber" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING 
                                                + "currentQuestionIdx" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING 
                                                + "answerPressTime" + CsvUtility.DEFAULT_COLUMN_SEPARATOR_STRING 
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

        private int _nextQuestionIdx;

        private ButtonQuizQuestion _currentQuestion;


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


        // Events
        public UnityEvent OnQuizStarted;
        public UnityEvent OnAnswerGiven;
        public UnityEvent OnQuizCompleted;


        // Export
        private long _quizStartTime;
        public long quizStartTime
        {
            get => _quizStartTime;
        }


        private string _latestChosenAnswersIdxs;
        public string latestChosenAnswersIdxs
        {
            get => _latestChosenAnswersIdxs;
        }

        private float _latestAnswerPressTime;
        public float latestAnswerPressTime
        {
            get => _latestAnswerPressTime;
        }

        private string _displayedFeedbackText;
        public string latestFeedbackText
        {
            get => _displayedFeedbackText;
        }
        private GameObject[] _displayedFeedbackObjects;
        public GameObject[] latestFeedbackObjects
        {
            get => _displayedFeedbackObjects;
        }

        private VideoClip _displayedFeedbackVideo;
        public VideoClip latestFeedbackVideo
        {
            get => _displayedFeedbackVideo;
        }


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
                SetButtonsDisabled(false);
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
            _nextQuestionIdx = 0;

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
                _quizStartTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
                Setup(_config, buttons, mcConfirmButton, displayText, displayAnchor, displayPlayer, displayVideoImage, afterQuizMenu);
                DisplayNextQuestion();
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
            if (_nextQuestionIdx >= _numQuestions)
            {
                StopQuiz();
            }
            else
            {
                _currentQuestion = _questions[_questionPermutation[_nextQuestionIdx]];

                SetButtonsDisabled(false);

                for (int i = 0; i < buttons.Length; i++)
                {
                    if (buttons[i] != null)
                    {
                        string answerText = i < _currentQuestion.answerTexts.Length ? _currentQuestion.answerTexts[i] : "";
                        GameObject answerGo = i < _currentQuestion.answerObjects.Length ? _currentQuestion.answerObjects[i] : null;
                        bool answerCorrect = i < _currentQuestion.correctAnswers.Length && _currentQuestion.correctAnswers[i];

                        // Always display (also empty) answers on the button
                        buttons[i].DisplayAnswer(answerText, answerGo, answerCorrect);
                    }
                }

                bool showTextQuestion = _currentQuestion.questionText != null;
                bool showObjectQuestion = _currentQuestion.questionObject != null;
                bool showVideoQuestion = _currentQuestion.questionVideo != null;
                bool showStreamedVideoQuestion = !showVideoQuestion && !string.IsNullOrEmpty(_currentQuestion.questionVideoUrl);

                SetQuizDisplayEnabled(showTextQuestion, showObjectQuestion, showVideoQuestion || showStreamedVideoQuestion);

                if (displayText != null && showTextQuestion)
                {
                    displayText.text = _currentQuestion.questionText;
                }

                if (displayAnchor != null && showObjectQuestion)
                {
                    Instantiate(_currentQuestion.questionObject, displayAnchor.transform);
                }

                if (displayPlayer != null)
                {
                    if (showVideoQuestion)
                    {
                        displayPlayer.source = VideoSource.VideoClip;
                        displayPlayer.clip = _currentQuestion.questionVideo;
                        displayPlayer.Play();
                    }
                    else if (showStreamedVideoQuestion)
                    {
                        displayPlayer.source = VideoSource.Url;
                        displayPlayer.url = QuizUtility.MakeStreamingAssetsVideoPath(_currentQuestion.questionVideoUrl);
                        displayPlayer.Play();
                    }
                }

                _nextQuestionIdx++;
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
                                            || (showIfAvailable && _currentQuestion.feedbackText != null)
                                            || showAnyAnswerType
                                            || _config.feedbackPrefixEnabled);
            bool showObjectFeedback = displayAnchor != null
                                        && showFeedback
                                        && (_config.feedbackType == FeedbackType.Object
                                            || (showAnswerType && config.answerType == AnswerType.Object)
                                            || (showIfAvailable && _currentQuestion.feedbackObject != null)
                                            || showAnyAnswerType);
            bool showVideoFeedback = displayPlayer != null
                                        && showFeedback
                                        && (showIfAvailable || _config.feedbackType == FeedbackType.Video)
                                        && _currentQuestion.feedbackVideo != null;
            bool showVideoFeedbackUrl = !showVideoFeedback
                                        && displayPlayer != null
                                        && showFeedback
                                        && (showIfAvailable || _config.feedbackType == FeedbackType.Video)
                                        && !string.IsNullOrEmpty(_currentQuestion.feedbackVideoUrl);

            SetQuizDisplayEnabled(showTextFeedback, showObjectFeedback, showVideoFeedback || showVideoFeedbackUrl);

            if (showTextFeedback)
            {
                string res = config.usedFeedbackPrefix;
                displayText.text = res + _displayedFeedbackText;
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
                displayPlayer.clip = _currentQuestion.feedbackVideo;
                displayPlayer.Play();
                displayPlayer.prepareCompleted += OnVideoPlayerPrepareComplete;
                displayPlayer.loopPointReached += OnFeedbackVideoCompleted;
            }
            else if (showVideoFeedbackUrl)
            {
                displayPlayer.source = VideoSource.Url;
                displayPlayer.url = QuizUtility.MakeStreamingAssetsVideoPath(_currentQuestion.feedbackVideoUrl);
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
                _latestChosenAnswersIdxs = CsvUtility.ArrayToString(pressedButtonIdxs.ToArray());
                _latestAnswerPressTime = mcConfirmButton.GetTriggerTimerValue();
            }
            else
            {
                _latestChosenAnswersIdxs = pressedButtonIdxs.Count > 0 ? pressedButtonIdxs[0].ToString() : "-1";
                _latestAnswerPressTime = pressedButtons.Count > 0 ? pressedButtons[0].GetTriggerTimerValue() : -1.0f;
            }

            _displayedFeedbackText = _currentQuestion?.GetFeedbackText(config) ?? "";
            _displayedFeedbackObjects = _currentQuestion.GetFeedbackGameObjects(config) ?? new GameObject[0];
            _displayedFeedbackVideo = _currentQuestion.GetFeedbackVideo(config);
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

        public float GetQuizUnixStartTime() => quizUndergoing ? quizStartTime : -1.0f;

        public float GetCurrentQuizUnixTimeMillisecondsDuration() => quizUndergoing ? (quizStartTime - System.DateTimeOffset.Now.ToUnixTimeMilliseconds()) : -1.0f;

        public int GetCurrentQuestionIdx() => _currentQuestion?.itemIdx ?? -1;

        public string GetCurrentQuestionCsvExportValue() => _currentQuestion?.GetCsvExportValues() ?? ButtonQuizQuestion.emptyCsvExportValues;

        public int GetCurrentQuestionNumber() => quizUndergoing ? _nextQuestionIdx : -1;

        public bool IsQuizActive() => quizUndergoing;


        // After Events
        public float GetAnswerPressTime() => _latestAnswerPressTime;

        /// <summary>
        /// Returns a string representing the of the latest chosen answer
        /// </summary>
        /// <returns>Either a single integer of the answer or a csv escaped array of values. </returns>
        public string GetChosenAnswersString() => _latestChosenAnswersIdxs;

        public string GetDisplayedFeedbackText() => _displayedFeedbackText;

        public string GetDisplayedFeedbackObjects()
        {
            List<string> _feedbackObjectNames = new();
            if (_displayedFeedbackObjects != null)
            {
                foreach (GameObject go in _displayedFeedbackObjects)
                {
                    if (go != null)
                    {
                        _feedbackObjectNames.Add(go.name);
                    }
                }
            }
            return CsvUtility.ArrayToString(_feedbackObjectNames.ToArray());
        }

        public string GetDisplayedFeedbackVideo() => _displayedFeedbackVideo != null ? _displayedFeedbackVideo.name : "";

        // Misc
        public string GetQuestionPermutationAsCsvString() => CsvUtility.ArrayToString(_questionPermutation);

        public string GetConfigCsvExportValues()
        {
            // Use EXPORT_CSV_COLUMN_STRING for header
            return config != null ? config.GetCsvExportValues() : ButtonQuizConfig.emptyCsvExportValues;
        }

        public string GetAllQuestionsCsvExportValues()
        {
            // Use EXPORT_CSV_COLUMN_STRING for header
            return config != null ? config.GetAllQuestionsCsvExportValues() : "";
        }

        public string GetFullQuizCsvValues() => CsvUtility.JoinAsCsv(
            new object[] {
                IsQuizActive(),
                GetCurrentQuestionNumber(),
                GetCurrentQuestionIdx(),
                GetAnswerPressTime(),
                GetChosenAnswersString(),
                GetDisplayedFeedbackText(),
                GetDisplayedFeedbackObjects(),
                GetDisplayedFeedbackVideo(),
                GetCurrentQuestionCsvExportValue(),
                GetConfigCsvExportValues()
            });
    }
}